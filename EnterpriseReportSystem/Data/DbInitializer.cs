using EnterpriseReportSystem.Models.Entities;
using EnterpriseReportSystem.Models.Entities.Ledger;
using EnterpriseReportSystem.Models.Entities.Structure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace EnterpriseReportSystem.Data;

public static class DbInitializer
{
    // Променяме метода да връща string при проблем, за да го логнем в уеб сървъра
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

                // 4. ДИНАМИЧНО ЗАРЕЖДАНЕ НА СИСТЕМНИТЕ МЕНЮТА ОТ JSON
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
                            context.SystemMenus.AddRange(menus);
                            context.SaveChanges();
                        }
                    }
                }

                // 5. ДИНАМИЧНО ЗАРЕЖДАНЕ НА СИСТЕМНИТЕ ПОТРЕБИТЕЛИ
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
                            // Тъй като базата в момента е празна от към фирми, подсигуряваме тестов запис на фирма, за да не гръмне релацията
                            if (!context.CompaniesOrPersons.Any())
                            {
                                var defaultCompany = new CompanyOrPerson { ClientCode = "HOLD_01", Name = "Централен Агро Холдинг ЕООД", TaxIdentifier = "BG123456789", Address = "Главен Стопански Двор", RegisteredTownId = 1 };
                                context.CompaniesOrPersons.Add(defaultCompany);
                                context.SaveChanges();
                            }

                            // Напасваме потребителите към заведената фирма и пренасяме правата им
                            var firstCompany = context.CompaniesOrPersons.First();
                            foreach (var u in users)
                            {
                                if (!u.IsGlobalAdmin)
                                {
                                    u.CompanyId = firstCompany.Id; // Обикновените потребители се заключват към фирма 1
                                }
                                else
                                {
                                    u.CompanyId = firstCompany.Id; // Администраторът също има базова фирма, но глобални права
                                }
                            }


                            context.SystemUsers.AddRange(users);
                            context.SaveChanges();
                        }
                    }
                }

            }

            return "SUCCESS"; // Всичко е заредено перфектно
        }
        catch (JsonException jsonEx)
        {
            // ДРУЖЕЛЮБНО УЕБ СЪОБЩЕНИЕ ПРИ ГРЕШКА В JSON СИНТАКСИСА
            return $"⚠️ [Проблем с конфигурацията]: Открита е синтактична грешка в базовия файл 'system_base_data.json'. Моля, проверете за забравени запетаи или кавички. Подробности: {jsonEx.Message}";
        }
        catch (Exception ex)
        {
            // ОБЩА УЕБ ЗАЩИТА ПРИ БЛОКИРАН ДОСТЪП ДО ФАЙЛА
            return $"❌ [Системна грешка]: Базовият инициализатор не успя да прочете файла в InputFiles. Причина: {ex.Message}";
        }
    }
}
