using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using EnterpriseReportSystem.Data;
using EnterpriseReportSystem.Models.Entities;
using EnterpriseReportSystem.Models.ViewModels;
using EnterpriseReportSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseReportSystem.Services.Implementations;

public class ReportService : IReportService
{
    private readonly ApplicationDbContext _context;

    // Връзката с SQL Server се инжектира автоматично през Dependency Injection
    public ReportService(ApplicationDbContext context)
    {
        _context = context;
    }

    // 1. ГЕНЕРИРАНЕ НА ОБОРОТКА НА БАЗА НАЛИЧНИТЕ ТРАНСАКЦИИ ОТ SQL
    public List<ITrackableAsset> GenerateTurnoverReport(List<AssetTransaction> transactions, List<ITrackableAsset> currentRegistry)
    {
        var result = new List<ITrackableAsset>();

        foreach (var asset in currentRegistry)
        {
            var assetTransactions = _context.AssetTransactions
                .Where(t => t.AssetId == asset.Id && !t.IsDeleted)
                .ToList();

            asset.InputCount = assetTransactions
                .Where(t => (int)t.Operation.Direction == 1) // Income = 1 (Раждане, покупка)
                .Sum(t => t.Quantity);

            asset.OutputCount = assetTransactions
                .Where(t => (int)t.Operation.Direction == 2) // Expense = 2 (Продажба, смърт, брак)
                .Sum(t => t.Quantity);

            result.Add(asset);
        }

        return result;
    }

    // 2. ФИЛТРИРАНЕ НА ДАННИТЕ ПО КРИТЕРИИ
    public List<ITrackableAsset> FilterAssets(List<ITrackableAsset> assets, string municipality, string town, string groupCode)
    {
        var query = assets.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(municipality))
        {
            query = query.Where(x => x.Facility.Equals(municipality, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(groupCode))
        {
            query = query.Where(x => x.Group.Code == groupCode);
        }

        return query.ToList();
    }

    // 3. ИНТЕЛЕГЕНТНИЯТ ПАРСЕР (ВЕЧЕ Е НА ПРАВИЛНОТО МЯСТО ВЪТРЕ В КЛАСА)
    public FileProcessSummary ProcessSmartImport(Stream fileStream, SmartImportViewModel config)
    {
        var summary = new FileProcessSummary();
        int currentLine = 0;

        // Определяне на Главния и Вътрешния разделител въз основа на избора "CUSTOM"
        string mainDel = config.MainDelimiter == "CUSTOM" ? config.CustomMainDelimiter : config.MainDelimiter;
        string innerDel = config.InnerDelimiter == "CUSTOM" ? config.CustomInnerDelimiter : config.InnerDelimiter;

        // Взимаме активните обекти от SQL регистъра за вътрешното търсене и съпоставка
        var dbAssets = _context.AssetTransactions
            .Include(t => t.Operation)
            .Where(t => !t.IsDeleted)
            .ToList();

        using (var reader = new StreamReader(fileStream))
        {
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                currentLine++;
                summary.TotalLinesRead = currentLine;

                if (string.IsNullOrWhiteSpace(line))
                {
                    summary.Errors.Add(new ValidationError { LineNumber = currentLine, RawContent = "[Празен ред]", Reason = "Празен ред", Details = "Редът е напълно празен." });
                    continue;
                }

                // РАЗДЕЛЯНЕ НИВО 1: По главния разделител
                var mainColumns = line.Split(new[] { mainDel }, StringSplitOptions.None);

                // Валидация за минимален брой изисквани колони на база индексите
                int maxRequiredIndex = new[] { config.AssetIdColumnIndex, config.ChipColumnIndex, config.DateColumnIndex, config.QuantityColumnIndex }.Max();
                if (mainColumns.Length <= maxRequiredIndex)
                {
                    summary.Errors.Add(new ValidationError { LineNumber = currentLine, RawContent = line, Reason = "Липсваща колона", Details = $"Файлът съдържа {mainColumns.Length} колони, а се изисква индекс {maxRequiredIndex}." });
                    continue;
                }

                // Извличаме суровите блокове по индексите от Column Mapping
                string rawAssetBlock = mainColumns[config.AssetIdColumnIndex].Trim();
                string rawQtyStr = mainColumns[config.QuantityColumnIndex].Trim();

                string detectedAssetId = string.Empty;
                string detectedChip = string.Empty;

                // РАЗДЕЛЯНЕ НИВО 2: По вътрешния разделител за слепени/разменени данни
                var pieces = rawAssetBlock.Split(new[] { innerDel }, StringSplitOptions.RemoveEmptyEntries)
                                          .Select(p => p.Trim())
                                          .ToList();

                // Динамично разпознаване по потребителска сигнатура
                foreach (var piece in pieces)
                {
                    // Проверка за Идентификатор (Марка/Баркод)
                    bool matchesPrefix = string.IsNullOrEmpty(config.AssetIdSignature) || piece.StartsWith(config.AssetIdSignature, StringComparison.OrdinalIgnoreCase);
                    bool matchesLength = !config.AssetIdRequiredLength.HasValue || piece.Length == config.AssetIdRequiredLength.Value;

                    if (matchesPrefix && matchesLength && string.IsNullOrEmpty(detectedAssetId))
                    {
                        detectedAssetId = piece;
                        continue;
                    }

                    // Проверка за Електронен Чип
                    bool matchesChipPrefix = string.IsNullOrEmpty(config.ChipSignature) || piece.StartsWith(config.ChipSignature, StringComparison.OrdinalIgnoreCase);
                    bool matchesChipLength = !config.ChipRequiredLength.HasValue || piece.Length == config.ChipRequiredLength.Value;

                    if (matchesChipPrefix && matchesChipLength && string.IsNullOrEmpty(detectedChip))
                    {
                        detectedChip = piece;
                    }
                }

                // Валидация на резултатите от потребителските правила
                if (config.TrackAssetId && string.IsNullOrEmpty(detectedAssetId))
                {
                    summary.Errors.Add(new ValidationError { LineNumber = currentLine, RawContent = line, Reason = "Непозната сигнатура", Details = "Никое парче текст не отговаря на префикса или дължината за Идентификатор." });
                    continue;
                }

                if (config.TrackQuantity && (!int.TryParse(rawQtyStr, out int qty) || qty < 0))
                {
                    summary.Errors.Add(new ValidationError { LineNumber = currentLine, RawContent = line, Reason = "Грешно количество", Details = $"Стойността '{rawQtyStr}' не е положително число." });
                    continue;
                }

                // Ако редът е преминал успешно всички проверки
                summary.SuccessfulLines++;
            }
        }

        return summary;
    }
}
