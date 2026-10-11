using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EnterpriseReportSystem.Data;
using EnterpriseReportSystem.Models.Entities;
using EnterpriseReportSystem.Models.Entities.Ledger;
using EnterpriseReportSystem.Models.ViewModels;
using EnterpriseReportSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseReportSystem.Services.Implementations;

public class ReportService : IReportService
{
    private readonly ApplicationDbContext _context;

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
            int targetAssetIntId = int.TryParse(asset.Id, out int parsedId) ? parsedId : 1;

            var assetTransactions = _context.AssetTransactions
                .Include(t => t.Operation)
                .Where(t => t.AssetId == targetAssetIntId && !t.IsDeleted)
                .ToList();

            asset.InputCount = assetTransactions
                .Where(t => (int)t.Operation.Direction == 1)
                .Sum(t => t.Quantity);

            asset.OutputCount = assetTransactions
                .Where(t => (int)t.Operation.Direction == 2)
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

    // 3. ИНТЕЛЕГЕНТНИЯТ ПАРСЕР (КОРИГИРАН ЗА ЧИСЛОВИ КЛЮЧОВЕ)
    public FileProcessSummary ProcessSmartImport(Stream fileStream, SmartImportViewModel config)
    {
        var summary = new FileProcessSummary();
        int currentLine = 0;

        string mainDel = config.MainDelimiter == "CUSTOM" ? config.CustomMainDelimiter : config.MainDelimiter;
        string innerDel = config.InnerDelimiter == "CUSTOM" ? config.CustomInnerDelimiter : config.InnerDelimiter;

        // КОРЕКЦИЯ 1: Речникът пази числови ИД-та на активите
        var fileData = new Dictionary<int, int>();

        var transactionsFromDb = _context.AssetTransactions
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

                var mainColumns = line.Split(new[] { mainDel }, StringSplitOptions.None);

                int maxRequiredIndex = new[] { config.AssetIdColumnIndex, config.ChipColumnIndex, config.DateColumnIndex, config.QuantityColumnIndex }.Max();
                if (mainColumns.Length <= maxRequiredIndex)
                {
                    summary.Errors.Add(new ValidationError { LineNumber = currentLine, RawContent = line, Reason = "Липасваща колона", Details = $"Файлът съдържа {mainColumns.Length} колони." });
                    continue;
                }

                string rawAssetBlock = mainColumns[config.AssetIdColumnIndex].Trim();
                string rawQtyStr = mainColumns[config.QuantityColumnIndex].Trim();

                string detectedAssetIdStr = string.Empty;
                string detectedChip = string.Empty;

                var pieces = rawAssetBlock.Split(new[] { innerDel }, StringSplitOptions.RemoveEmptyEntries)
                                          .Select(p => p.Trim())
                                          .ToList();

                foreach (var piece in pieces)
                {
                    bool matchesPrefix = string.IsNullOrEmpty(config.AssetIdSignature) || piece.StartsWith(config.AssetIdSignature, StringComparison.OrdinalIgnoreCase);
                    bool matchesLength = !config.AssetIdRequiredLength.HasValue || piece.Length == config.AssetIdRequiredLength.Value;

                    if (matchesPrefix && matchesLength && string.IsNullOrEmpty(detectedAssetIdStr))
                    {
                        detectedAssetIdStr = piece;
                        continue;
                    }

                    bool matchesChipPrefix = string.IsNullOrEmpty(config.ChipSignature) || piece.StartsWith(config.ChipSignature, StringComparison.OrdinalIgnoreCase);
                    bool matchesChipLength = !config.ChipRequiredLength.HasValue || piece.Length == config.ChipRequiredLength.Value;

                    if (matchesChipPrefix && matchesChipLength && string.IsNullOrEmpty(detectedChip))
                    {
                        detectedChip = piece;
                    }
                }

                if (config.TrackAssetId && string.IsNullOrEmpty(detectedAssetIdStr))
                {
                    summary.Errors.Add(new ValidationError { LineNumber = currentLine, RawContent = line, Reason = "Непозната сигнатура", Details = "Липсва Идентификатор." });
                    continue;
                }

                int qty = 1;
                if (config.TrackQuantity && (!int.TryParse(rawQtyStr, out qty) || qty < 0))
                {
                    summary.Errors.Add(new ValidationError { LineNumber = currentLine, RawContent = line, Reason = "Грешно количество", Details = $"Стойността '{rawQtyStr}' не е число." });
                    continue;
                }

                // КОРЕКЦИЯ 2: Парсваме текстовия код от файла към число преди запис в речника
                if (!string.IsNullOrEmpty(detectedAssetIdStr) && int.TryParse(detectedAssetIdStr, out int parsedAssetId))
                {
                    fileData[parsedAssetId] = qty;
                }
                summary.SuccessfulLines++;
            }
        }

        // Пресмятаме текущите баланси от SQL
        var systemCounts = transactionsFromDb
           .GroupBy(t => t.AssetId)
           .ToDictionary(
               g => g.Key,
               g => g.Sum(t => (int)t.Operation.Direction == 1 ? t.Quantity : ((int)t.Operation.Direction == 2 ? -t.Quantity : 0))
           );

        // Съпоставяме данните (int търси int - без грешки)
        foreach (var sysAsset in systemCounts)
        {
            var diffLine = new DiffReportLine
            {
                Id = sysAsset.Key.ToString(), // Чист каст към стринг за модела на отчета
                Name = $"Регистриран актив в SQL Server база данни",
                SystemCount = sysAsset.Value
            };

            if (fileData.TryGetValue(sysAsset.Key, out int fileQty))
            {
                diffLine.FileCount = fileQty;
                diffLine.Status = diffLine.Difference == 0 ? "Съвпада" : "Разлика в бройката";
                fileData.Remove(sysAsset.Key);
            }
            else
            {
                diffLine.FileCount = 0;
                diffLine.Status = "Липсва във файла";
            }

            summary.DiffLines.Add(diffLine);
        }

        // Обработка на непознати активи
        foreach (var fileAsset in fileData)
        {
            summary.DiffLines.Add(new DiffReportLine
            {
                Id = fileAsset.Key.ToString(),
                Name = "⚠️ НЕПОЗНАТ ОБЕКТ (Липсва официален запис в SQL!)",
                SystemCount = 0,
                FileCount = fileAsset.Value,
                Status = "Липсва във файла"
            });
        }

        return summary;
    }

    // =========================================================================
    // ТВОЯТ ВАЛУТЕН ДВИГАТЕЛ
    // =========================================================================
    public async Task<decimal> GetExchangeRateAsync(int fromCurrencyId, int toCurrencyId, DateTime transactionDate)
    {
        if (fromCurrencyId == toCurrencyId) return 1.00000m;

        var fixedRate = await _context.ExchangeRateHistories
            .Where(r => r.FromCurrencyId == fromCurrencyId && r.ToCurrencyId == toCurrencyId && r.IsFixed)
            .Select(r => r.Rate)
            .FirstOrDefaultAsync();

        if (fixedRate > 0) return fixedRate;

        var floatingRate = await _context.ExchangeRateHistories
            .Where(r => r.FromCurrencyId == fromCurrencyId
                     && r.ToCurrencyId == toCurrencyId
                     && !r.IsFixed
                     && r.RateDate.Date <= transactionDate.Date)
            .OrderByDescending(r => r.RateDate)
            .Select(r => r.Rate)
            .FirstOrDefaultAsync();

        return floatingRate > 0 ? floatingRate : 1.00000m;
    }

    public async Task<decimal> CalculateAssetValueInBgnAsync(int transactionId)
    {
        var transaction = await _context.AssetTransactions
            .Include(t => t.Operation)
            .FirstOrDefaultAsync(t => t.Id == transactionId);

        if (transaction == null) return 0.00000m;

        int bgnCurrencyId = 1;
        int foreignCurrencyId = 2;

        decimal rate = await GetExchangeRateAsync(foreignCurrencyId, bgnCurrencyId, transaction.TransactionDate);
        return transaction.Quantity * 1.00m * rate;
    }
    public async Task CreateTransactionAsync(int assetId, int quantity, int? fromFacilityId, int? toFacilityId, DateTime transactionDate, string docRef, string notes, int opId)
    {
        var transaction = new AssetTransaction
        {
            AssetId = assetId,
            Quantity = quantity,
            FromFacilityId = fromFacilityId,
            ToFacilityId = toFacilityId,
            TransactionDate = transactionDate,
            DocumentReference = string.IsNullOrEmpty(docRef) ? "Първоначално завеждане" : docRef.Trim(),
            Notes = string.IsNullOrEmpty(notes) ? "Заприходяване на нов оперативен актив" : notes.Trim(),
            OperationId = opId,
            IsDeleted = false,
            DeletedOn = null
        };
        _context.AssetTransactions.Add(transaction);
        await _context.SaveChangesAsync();
        return true;
    }
}