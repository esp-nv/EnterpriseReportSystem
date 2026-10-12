using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using EnterpriseReportSystem.Models.Entities;
using EnterpriseReportSystem.Models.Entities.Ledger;
using EnterpriseReportSystem.Models.Entities.Structure;

namespace EnterpriseReportSystem.Data;

public static class DbInitializer
{
    public static string Seed(ApplicationDbContext context)
    {
        // 1. Подсигуряваме физическото съществуване на базата данни
        context.Database.EnsureCreated();

        // 2. ПЪРВОНАЧАЛНО ЗАРЕЖДАНЕ НА ГЛОБАЛНИТЕ СИСТЕМНИ ДАННИ (Глава на счетоводството)
        string rootPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "InputFiles");
        if (!Directory.Exists(rootPath))
        {
            rootPath = Path.Combine(Directory.GetCurrentDirectory(), "InputFiles");
        }

        if (!Directory.Exists(rootPath))
        {
            return "⚠️ [Грешка]: Папка InputFiles липсва в директорията на проекта.";
        }

        // Зареждане на Валути и Операции от system_base_data.json
        string baseDataPath = Path.Combine(rootPath, "system_base_data.json");
        if (File.Exists(baseDataPath))
        {
            try
            {
                string jsonContent = File.ReadAllText(baseDataPath);
                using var doc = JsonDocument.Parse(jsonContent);
                var root = doc.RootElement;

                if (!context.Currencies.Any() && root.TryGetProperty("Currencies", out var currenciesElement))
                {
                    var currencies = JsonSerializer.Deserialize<List<Currency>>(currenciesElement.GetRawText());
                    if (currencies != null) context.Currencies.AddRange(currencies);
                    context.SaveChanges();
                }

                if (!context.OperationTypes.Any() && root.TryGetProperty("OperationTypes", out var operationsElement))
                {
                    var operations = JsonSerializer.Deserialize<List<OperationType>>(operationsElement.GetRawText());
                    if (operations != null) context.OperationTypes.AddRange(operations);
                    context.SaveChanges();
                }

                if (!context.AssetAttributes.Any() && root.TryGetProperty("AssetAttributes", out var attributesElement))
                {
                    var attributes = JsonSerializer.Deserialize<List<AssetAttribute>>(attributesElement.GetRawText());
                    if (attributes != null) context.AssetAttributes.AddRange(attributes);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                return $"❌ [Грешка базов JSON]: {ex.Message}";
            }
        }

        // =========================================================================
        // 3. МАГИЯТА НА РЕФЛЕКШЪНА: ДИНАМИЧНО НАЛИВАНЕ НА ДОМЕЙН ПАПКИТЕ ПО ИМЕНА НА ТАБЛИЦИ
        // =========================================================================
        string[] domainFolders = { "Location", "Structure", "Hierarchy" };

        try
        {
            foreach (var folder in domainFolders)
            {
                string folderPath = Path.Combine(rootPath, folder);
                if (!Directory.Exists(folderPath)) continue;

                // Взимаме всички JSON файлове, подредени по азбучен ред (благодарение на цифрения ни префикс)
                var jsonFiles = Directory.GetFiles(folderPath, "*.json").OrderBy(f => f).ToList();

                foreach (var file in jsonFiles)
                {
                    string fileName = Path.GetFileNameWithoutExtension(file); // напр. "1_Regions"

                    // Извличаме точното име на SQL таблицата, като премахваме цифрата отпред
                    int underscoreIndex = fileName.IndexOf('_');
                    if (underscoreIndex == -1) continue;

                    string tableName = fileName.Substring(underscoreIndex + 1); // Получаваме "Regions"

                    // Чрез C# Reflection намираме съответното DbSet свойство в нашия ApplicationDbContext
                    PropertyInfo dbSetProperty = context.GetType().GetProperties()
                        .FirstOrDefault(p => p.Name == tableName && p.PropertyType.IsGenericType &&
                                             (p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>) ||
                                              p.PropertyType.GetGenericTypeDefinition() == typeof(IQueryable<>)));

                    if (dbSetProperty == null) continue;

                    // Намираме типа на самата Entity класа (напр. EnterpriseReportSystem.Models.Entities.Location.Region)
                    Type entityType = dbSetProperty.PropertyType.GetGenericArguments()[0];

                    // Извикваме динамично метода .Any() за съответната таблица, за да проверим дали е празна
                    IQueryable queryableTable = (IQueryable)dbSetProperty.GetValue(context);
                    bool hasData = false;
                    foreach (var _ in queryableTable) { hasData = true; break; }

                    // Ако в таблицата в SQL вече има данни, я прескачаме, за да не дублираме
                    if (hasData) continue;

                    // Разчитаме съдържанието на JSON файла
                    string jsonText = File.ReadAllText(file);

                    // Динамично конструираме десериализиращия списъчен тип List<EntityType>
                    Type listType = typeof(List<>).MakeGenericType(entityType);
                    object deserializedList = JsonSerializer.Deserialize(jsonText, listType);

                    if (deserializedList == null) continue;

                    // Извикваме скрито съответния DbSet и добавяме масива от обекти наведнъж
                    object dbSetInstance = dbSetProperty.GetValue(context);
                    MethodInfo addRangeMethod = dbSetInstance.GetType().GetMethod("AddRange", new[] { typeof(IEnumerable<>).MakeGenericType(entityType) })
                                              ?? dbSetInstance.GetType().GetMethod("AddRange", new[] { typeof(object[]) });

                    // СЛУЖЕБНА МАГИЯ: Автоматично отваряне на IDENTITY_INSERT за таблици със специфични каскадни релации (напр. Менюта)
                    bool requiresIdentityInsert = tableName == "SystemMenus" || tableName == "SystemMenuTranslations";

                    if (requiresIdentityInsert)
                    {
                        context.Database.OpenConnection();
                        context.Database.ExecuteSqlRaw($"SET IDENTITY_INSERT {tableName} ON");
                    }

                    addRangeMethod.Invoke(dbSetInstance, new object[] { deserializedList });
                    context.SaveChanges();

                    if (requiresIdentityInsert)
                    {
                        context.Database.ExecuteSqlRaw($"SET IDENTITY_INSERT {tableName} OFF");
                        context.Database.CloseConnection();
                    }
                }
            }

            // Първично зареждане на менютата и преводите от коренната папка, ако таблиците са чисти
            SeedSystemMenusAndTranslations(context, rootPath);

            return "SUCCESS";
        }
        catch (Exception ex)
        {
            return $"⚠️ [Системна грешка при Рефлекшън наливане]: Неуспешно четене на мастър структурите. Детайли: {ex.InnerException?.Message ?? ex.Message}";
        }
    }

    private static void SeedSystemMenusAndTranslations(ApplicationDbContext context, string rootPath)
    {
        if (!context.SystemMenus.Any())
        {
            string menuPath = Path.Combine(rootPath, "system_menu.json");
            if (File.Exists(menuPath))
            {
                string menuJson = File.ReadAllText(menuPath);
                var menus = JsonSerializer.Deserialize<List<SystemMenu>>(menuJson);
                if (menus != null)
                {
                    context.Database.OpenConnection();
                    context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT SystemMenus ON");
                    context.SystemMenus.AddRange(menus);
                    context.SaveChanges();
                    context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT SystemMenus OFF");
                    context.Database.CloseConnection();
                }
            }
        }

        if (!context.SystemMenuTranslations.Any())
        {
            string transPath = Path.Combine(rootPath, "system_menu_translations.json");
            if (File.Exists(transPath))
            {
                string transJson = File.ReadAllText(transPath);
                var translations = JsonSerializer.Deserialize<List<SystemMenuTranslation>>(transJson);
                if (translations != null)
                {
                    context.SystemMenuTranslations.AddRange(translations);
                    context.SaveChanges();
                }
            }
        }
    }
}
