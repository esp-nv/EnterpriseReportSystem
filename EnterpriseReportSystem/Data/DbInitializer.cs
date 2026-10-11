using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Collections.Generic;
using EnterpriseReportSystem.Models.Entities;
using EnterpriseReportSystem.Models.Entities.Ledger;

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
