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
        // 1. Подсигуряваме съществуването на празната SQL база данни
        context.Database.EnsureCreated();

        string rootPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "InputFiles");
        if (!Directory.Exists(rootPath))
        {
            rootPath = Path.Combine(Directory.GetCurrentDirectory(), "InputFiles");
        }

        if (!Directory.Exists(rootPath))
        {
            return "⚠️ [Грешка]: Основната папка InputFiles липсва в директорията на уеб сървъра.";
        }

        // 2. ВСЕКИ ЕДИН ДОМЕЙН Е В ПЪЛНА ХОМОГЕННОСТ (МАСИВ ОТ ПАПКИ ЕДНО КЪМ ЕДНО С ENTITIES)
        string domainFolders = { "Location", "Hierarchy", "Ledger", "Structure" };

        try
        {
            foreach (var folder in domainFolders)
            {
                string folderPath = Path.Combine(rootPath, folder);
                if (!Directory.Exists(folderPath)) continue;

                // Четем JSON файловете, подредени по префикс за Foreign Key защита
                var jsonFiles = Directory.GetFiles(folderPath, "*.json").OrderBy(f => f).ToList();

                foreach (var file in jsonFiles)
                {
                    string fileName = Path.GetFileNameWithoutExtension(file);

                    int underscoreIndex = fileName.IndexOf('_');
                    if (underscoreIndex == -1) continue;

                    string tableName = fileName.Substring(underscoreIndex + 1); // Екстрактва точно "SystemMenus", "Currencies" и т.н.

                    // Чрез Рефлекшън намираме съответния DbSet в контекста
                    PropertyInfo dbSetProperty = context.GetType().GetProperties()
                        .FirstOrDefault(p => p.Name == tableName && p.PropertyType.IsGenericType &&
                                             (p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>) ||
                                              p.PropertyType.GetGenericTypeDefinition() == typeof(IQueryable<>)));

                    if (dbSetProperty == null) continue;

                    Type entityType = dbSetProperty.PropertyType.GetGenericArguments()[0];

                    // Проверяваме дали в съответната SQL таблица вече има заведени данни
                    IQueryable queryableTable = (IQueryable)dbSetProperty.GetValue(context);
                    bool hasData = false;
                    foreach (var _ in queryableTable) { hasData = true; break; }

                    if (hasData) continue;

                    string jsonText = File.ReadAllText(file);

                    Type listType = typeof(List<>).MakeGenericType(entityType);
                    object deserializedList = JsonSerializer.Deserialize(jsonText, listType);

                    if (deserializedList == null) continue;

                    object dbSetInstance = dbSetProperty.GetValue(context);
                    MethodInfo addRangeMethod = dbSetInstance.GetType().GetMethod("AddRange", new[] { typeof(IEnumerable<>).MakeGenericType(entityType) })
                                              ?? dbSetInstance.GetType().GetMethod("AddRange", new[] { typeof(object) });

                    // АВТОМАТИЧНА СИСТЕМНА ЗАЩИТА: Отваряме Identity Insert само за таблиците с твърди Id релации
                    bool requiresIdentityInsert = tableName == "SystemMenus" ||
                                                  tableName == "SystemMenuTranslations" ||
                                                  tableName == "Currencies" ||
                                                  tableName == "OperationTypes" ||
                                                  tableName == "AssetAttributes";

                    if (requiresIdentityInsert)
                    {
                        context.Database.OpenConnection();
                        context.Database.ExecuteSqlRaw($"SET IDENTITY_INSERT {tableName} ON");
                    }

                    addRangeMethod.Invoke(dbSetInstance, new[] { deserializedList });
                    context.SaveChanges();

                    if (requiresIdentityInsert)
                    {
                        context.Database.ExecuteSqlRaw($"SET IDENTITY_INSERT {tableName} OFF");
                        context.Database.CloseConnection();
                    }
                }
            }

            return "SUCCESS";
        }
        catch (Exception ex)
        {
            return $"⚠️ [Критична грешка при Рефлекшън наливане]: Детайли: {ex.InnerException?.Message ?? ex.Message}";
        }
    }
}
