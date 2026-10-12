using EnterpriseReportSystem.Models.Entities;
using EnterpriseReportSystem.Models.Entities.Ledger;
using EnterpriseReportSystem.Models.Entities.Structure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace EnterpriseReportSystem.Data;

public static class DbInitializer
{
    public static string Seed(ApplicationDbContext context)
    {
        context.Database.EnsureCreated();

        string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "InputFiles", "system_base_data.json");

        if (!File.Exists(filePath))
        {
            filePath = Path.Combine(Directory.GetCurrentDirectory(), "InputFiles", "system_base_data.json");
        }

        if (!File.Exists(filePath))
        {
            return "⚠️ [Внимание]: Базовият конфигурационен файл 'system_base_data.json' липсва в папка InputFiles. Системата стартира без начални данни.";
        }

        try
        {
            string jsonContent = File.ReadAllText(filePath);
            using var doc = JsonDocument.Parse(jsonContent);
            var root = doc.RootElement;

            // 1. ДИНАМИЧНО ЗАРЕЖДАНЕ НА ВАЛУТИТЕ
            if (!context.Currencies.Any() && root.TryGetProperty("Currencies", out var currenciesElement))
            {
                var currencies = JsonSerializer.Deserialize<List<Currency>>(currenciesElement.GetRawText());
                if (currencies != null) context.Currencies.AddRange(currencies);
                context.SaveChanges();
            }

            // 2. ДИНАМИЧНО ЗАРЕЖДАНЕ НА ОПЕРАЦИИТЕ
            if (!context.OperationTypes.Any() && root.TryGetProperty("OperationTypes", out var operationsElement))
            {
                var operations = JsonSerializer.Deserialize<List<OperationType>>(operationsElement.GetRawText());
                if (operations != null) context.OperationTypes.AddRange(operations);
                context.SaveChanges();
            }

            // 3. ДИНАМИЧНО ЗАРЕЖДАНЕ НА ХАРАКТЕРИСТИКИТЕ
            if (!context.AssetAttributes.Any() && root.TryGetProperty("AssetAttributes", out var attributesElement))
            {
                var attributes = JsonSerializer.Deserialize<List<AssetAttribute>>(attributesElement.GetRawText());
                if (attributes != null) context.AssetAttributes.AddRange(attributes);
                context.SaveChanges();
            }

            // 4. ДИНАМИЧНО ЗАРЕЖДАНЕ НА СИСТЕМНИТЕ МЕНЮТА ОТ JSON (С IDENTITY_INSERT ЗАЩИТА)
            if (!context.SystemMenus.Any())
            {
                string menuPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "InputFiles", "system_menu.json");
                if (!File.Exists(menuPath)) menuPath = Path.Combine(Directory.GetCurrentDirectory(), "InputFiles", "system_menu.json");

                if (File.Exists(menuPath))
                {
                    string menuJson = File.ReadAllText(menuPath);
                    var menus = JsonSerializer.Deserialize<List<SystemMenu>>(menuJson);
                    if (menus != null)
                    {
                        // СЛУЖЕБНА МАГИЯ: Включваме IDENTITY_INSERT, за да ни пусне SQL Server с твърдите Id номера!
                        context.Database.OpenConnection();
                        try
                        {
                            context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT SystemMenus ON");
                            context.SystemMenus.AddRange(menus);
                            context.SaveChanges();
                            context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT SystemMenus OFF");
                        }
                        finally
                        {
                            context.Database.CloseConnection();
                        }
                    }
                }
            }



            // 5. ДИНАМИЧНО ЗАРЕЖДАНЕ НА СИСТЕМНИТЕ ПОТРЕБИТЕЛИ (ИЗВЕСТЕН НА ВЪНШНО НИВО)
            if (!context.SystemUsers.Any())
            {
                string usersPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "InputFiles", "system_users.json");
                if (!File.Exists(usersPath)) usersPath = Path.Combine(Directory.GetCurrentDirectory(), "InputFiles", "system_users.json");

                if (File.Exists(usersPath))
                {
                    string usersJson = File.ReadAllText(usersPath);
                    var users = JsonSerializer.Deserialize<List<SystemUser>>(usersJson);
                    if (users != null)
                    {
                        if (!context.CompaniesOrPersons.Any())
                        {
                            var defaultCompany = new CompanyOrPerson { ClientCode = "HOLD_01", Name = "Централен Агро Холдинг ЕООД", TaxIdentifier = "BG123456789", Address = "Главен Стопански Двор", RegisteredTownId = 1 };
                            context.CompaniesOrPersons.Add(defaultCompany);
                            context.SaveChanges();
                        }

                        var firstCompany = context.CompaniesOrPersons.First();
                        foreach (var u in users)
                        {
                            u.CompanyId = firstCompany.Id;

                            // ЖЕЛЯЗНА ЗАЩИТА: Превръщаме в малки букви, за да съвпаднат 100% с CSS файловете
                            u.ThemeType = u.ThemeType.Trim().ToLower(); // "photoshop", "light", "dark"
                            u.LayoutType = u.LayoutType.Trim().ToUpper(); // "WORD", "DASHBOARD"
                        }




                        context.SystemUsers.AddRange(users);
                        context.SaveChanges();
                    }
                }
            }

            // 6. ДИНАМИЧНО ЗАРЕЖДАНЕ НА ЕЗИЦИТЕ
            if (!context.Languages.Any())
            {
                context.Languages.AddRange(
                    new Language { Id = 1, Code = "bg", Name = "Български" },
                    new Language { Id = 2, Code = "en", Name = "English" }
                );
                context.SaveChanges();
            }

            // 7. ДИНАМИЧНО ЗАРЕЖДАНЕ НА МНОГОЕЗИЧНИТЕ ПРЕВОДИ ЗА МЕНЮТАТА
            if (!context.SystemMenuTranslations.Any())
            {
                string transPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "InputFiles", "system_menu_translations.json");
                if (!File.Exists(transPath)) transPath = Path.Combine(Directory.GetCurrentDirectory(), "InputFiles", "system_menu_translations.json");

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

            return "SUCCESS";
        }
        catch (JsonException jsonEx)
        {
            return $"⚠️ [Проблем с конфигурацията]: Открита е синтактична грешка в базовия файл 'system_base_data.json'. Подробности: {jsonEx.Message}";
        }
        catch (Exception ex)
        {
            return $"❌ [Системна грешка]: Базовият инициализатор не успя да прочете файла в InputFiles. Причина: {ex.Message}";
        }
    }
}
