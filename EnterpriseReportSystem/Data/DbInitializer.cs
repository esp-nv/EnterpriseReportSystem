using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseReportSystem.Data;

public static class DbInitializer
{
    public static string Seed(ApplicationDbContext context)
    {
        // 1. Подсигуряваме физическото съществуване на празната SQL база данни
        context.Database.EnsureCreated();

        // Откриваме правилния път до папка InputFiles в зависимост от работната среда
        string? baseDir = AppDomain.CurrentDomain.BaseDirectory;
        string rootPath = Path.Combine(baseDir ?? string.Empty, "InputFiles");

        if (!Directory.Exists(rootPath))
        {
            rootPath = Path.Combine(Directory.GetCurrentDirectory(), "InputFiles");
        }

        if (!Directory.Exists(rootPath))
        {
            return "⚠️ [Грешка]: Основната папка InputFiles липсва в директорията на уеб сървъра.";
        }

        // =========================================================================
        // 2. ЖЕЛЯЗЕН СЧЕТОВОДЕН И РЕЛАЦИОНЕН РЕД НА ЗАРЕЖДАНЕ НА ДОМЕЙН ПАПКИТЕ
        // =========================================================================
        string[] domainFolders = new string[] { "Ledger", "Location", "Hierarchy", "Structure" };

        try
        {
            foreach (var folder in domainFolders)
            {
                string folderPath = Path.Combine(rootPath, folder);
                if (!Directory.Exists(folderPath)) continue;

                // Четем всички JSON компоненти в папката, сортирани по азбучен ред спрямо техния префикс (1_, 2_)
                var jsonFiles = Directory.GetFiles(folderPath, "*.json").OrderBy(f => f).ToList();

                foreach (var file in jsonFiles)
                {
                    string fileName = Path.GetFileNameWithoutExtension(file);

                    int underscoreIndex = fileName.IndexOf('_');
                    if (underscoreIndex == -1) continue;

                    // Извличаме точното име на SQL таблицата (напр. "Regions", "SystemMenus")
                    string tableName = fileName.Substring(underscoreIndex + 1);

                    // Чрез C# Reflection намираме съответния DbSet вътре в контекста на базата
                    PropertyInfo? dbSetProperty = context.GetType().GetProperties()
                        .FirstOrDefault(p => p.Name == tableName && p.PropertyType.IsGenericType &&
                                             (p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>) ||
                                              p.PropertyType.GetGenericTypeDefinition() == typeof(IQueryable<>)));

                    if (dbSetProperty == null) continue;

                    // Извличаме типа на самата Entity класа безопасно
                    var genericArgs = dbSetProperty.PropertyType.GetGenericArguments();
                    if (genericArgs == null || genericArgs.Length == 0) continue;
                    Type entityType = genericArgs[0];

                    // Проверяваме динамично дали в тази SQL таблица вече има записани данни
                    IQueryable? queryableTable = dbSetProperty.GetValue(context) as IQueryable;
                    if (queryableTable == null) continue;

                    bool hasData = false;
                    foreach (var _ in queryableTable) { hasData = true; break; }

                    // Ако в таблицата в SQL вече има данни, я прескачаме, за да не дублираме мастър редовете
                    if (hasData) continue;

                    // Разчитаме съдържанието на съответния JSON файл
                    string jsonText = File.ReadAllText(file);

                    // Динамично конструираме десериализиращия списъчен тип List<EntityType>
                    Type listType = typeof(List<>).MakeGenericType(entityType);
                    object? deserializedList = JsonSerializer.Deserialize(jsonText, listType);

                    if (deserializedList == null) continue;

                    object? dbSetInstance = dbSetProperty.GetValue(context);
                    if (dbSetInstance == null) continue;

                    // Откриваме AddRange метода на DbSet обекта
                    MethodInfo? addRangeMethod = dbSetInstance.GetType().GetMethod("AddRange", new Type[] { typeof(IEnumerable<>).MakeGenericType(entityType) })
                                              ?? dbSetInstance.GetType().GetMethod("AddRange", new Type[] { typeof(object) });

                    if (addRangeMethod == null) continue;

                    // АВТОМАТИЧНА СИСТЕМНА ЗАЩИТА
                    bool requiresIdentityInsert = true;

                    if (requiresIdentityInsert)
                    {
                        context.Database.OpenConnection();

                        // Изключваме предупреждението за SQL инжекция само за този системен ред
#pragma warning disable EF1002
                        context.Database.ExecuteSqlRaw($"SET IDENTITY_INSERT {tableName} ON");
#pragma warning restore EF1002
                    }

                    // Подаваме правилно конструиран обектен масив към Invoke метода на Рефлекшъна
                    addRangeMethod.Invoke(dbSetInstance, new object[] { deserializedList });
                    context.SaveChanges();

                    if (requiresIdentityInsert)
                    {
#pragma warning disable EF1002
                        context.Database.ExecuteSqlRaw($"SET IDENTITY_INSERT {tableName} OFF");
#pragma warning restore EF1002

                        context.Database.CloseConnection();
                    }

                }
            }

            return "SUCCESS"; // Всички папки са налети автоматично без нито един ред твърд код!
        }
        catch (Exception ex)
        {
            return $"⚠️ [Критична грешка при Рефлекшън наливане]: Детайли: {ex.InnerException?.Message ?? ex.Message}";
        }
    }
}
