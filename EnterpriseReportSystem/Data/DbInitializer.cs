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
    public static void Seed(ApplicationDbContext context)
    {
        context.Database.EnsureCreated();

        // Намираме физическия път до JSON файла в папка InputFiles
        string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "InputFiles", "system_base_data.json");

        // Защита: Ако проектът се стартира в тестов режим, проверяваме и алтернативния корен на паката
        if (!File.Exists(filePath))
        {
            filePath = Path.Combine(Directory.GetCurrentDirectory(), "InputFiles", "system_base_data.json");
        }

        if (!File.Exists(filePath)) return; // Ако файлът липсва, спираме без срив

        try
        {
            string jsonContent = File.ReadAllText(filePath);
            using var doc = JsonDocument.Parse(jsonContent);
            var root = doc.RootElement;

            // 1. ДИНАМИЧНО ЧЕТЕНЕ НА ВАЛУТИТЕ
            if (!context.Currencies.Any() && root.TryGetProperty("Currencies", out var currenciesElement))
            {
                var currencies = JsonSerializer.Deserialize<List<Currency>>(currenciesElement.GetRawText());
                if (currencies != null) context.Currencies.AddRange(currencies);
                context.SaveChanges();
            }

            // 2. ДИНАМИЧНО ЧЕТЕНЕ НА ОПЕРАЦИИТЕ
            if (!context.OperationTypes.Any() && root.TryGetProperty("OperationTypes", out var operationsElement))
            {
                var operations = JsonSerializer.Deserialize<List<OperationType>>(operationsElement.GetRawText());
                if (operations != null) context.OperationTypes.AddRange(operations);
                context.SaveChanges();
            }

            // 3. ДИНАМИЧНО ЧЕТЕНЕ НА ХАРАКТЕРИСТИКИТЕ
            if (!context.AssetAttributes.Any() && root.TryGetProperty("AssetAttributes", out var attributesElement))
            {
                var attributes = JsonSerializer.Deserialize<List<AssetAttribute>>(attributesElement.GetRawText());
                if (attributes != null) context.AssetAttributes.AddRange(attributes);
                context.SaveChanges();
            }
        }
        catch (Exception)
        {
            // Сигурна счетоводна защита срещу повредени JSON файлове
        }
    }
}
