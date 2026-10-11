using System;
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
            // Вътре в GenerateTurnoverReport метода на ReportService.cs:
            var assetTransactions = _context.AssetTransactions
                .Include(t => t.Operation)
                .Include(t => t.ToGroup)
                    .ThenInclude(g => g.MainType) // Зареждаме Главния тип динамично от SQL
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

        // Временна памет за успешно прочетените данни от външния източник
        var fileData = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        // Взимаме активните трансакции от SQL регистъра за количествено засичане през Entity Framework
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

                // РАЗДЕЛЯНЕ НИВО 1: По главния разделител (Твоят чист масив)
                var mainColumns = line.Split(new[] { mainDel }, StringSplitOptions.None);

                // Валидация за минимален брой изисквани колони на база индексите
                int maxRequiredIndex = new[] { config.AssetIdColumnIndex, config.ChipColumnIndex, config.DateColumnIndex, config.QuantityColumnIndex }.Max();
                if (mainColumns.Length <= maxRequiredIndex)
                {
                    summary.Errors.Add(new ValidationError { LineNumber = currentLine, RawContent = line, Reason = "Липсваща колона", Details = $"Файлът съдържа {mainColumns.Length} колони, а се изисква индекс {maxRequiredIndex}." });
                    continue;
                }

                // Извличаме суровите блокове по потребителския Column Mapping
                string rawAssetBlock = mainColumns[config.AssetIdColumnIndex].Trim();
                string rawQtyStr = mainColumns[config.QuantityColumnIndex].Trim();

                string detectedAssetId = string.Empty;
                string detectedChip = string.Empty;

                // РАЗДЕЛЯНЕ НИВО 2: По вътрешния разделител за слепени/разменени данни (Твоят чист масив)
                var pieces = rawAssetBlock.Split(new[] { innerDel }, StringSplitOptions.RemoveEmptyEntries)
                                          .Select(p => p.Trim())
                                          .ToList();

                // Динамично разпознаване по потребителска сигнатура (Пълна свобода)
                foreach (var piece in pieces)
                {
                    bool matchesPrefix = string.IsNullOrEmpty(config.AssetIdSignature) || piece.StartsWith(config.AssetIdSignature, StringComparison.OrdinalIgnoreCase);
                    bool matchesLength = !config.AssetIdRequiredLength.HasValue || piece.Length == config.AssetIdRequiredLength.Value;

                    if (matchesPrefix && matchesLength && string.IsNullOrEmpty(detectedAssetId))
                    {
                        detectedAssetId = piece;
                        continue;
                    }

                    bool matchesChipPrefix = string.IsNullOrEmpty(config.ChipSignature) || piece.StartsWith(config.ChipSignature, StringComparison.OrdinalIgnoreCase);
                    bool matchesChipLength = !config.ChipRequiredLength.HasValue || piece.Length == config.ChipRequiredLength.Value;

                    if (matchesChipPrefix && matchesChipLength && string.IsNullOrEmpty(detectedChip))
                    {
                        detectedChip = piece;
                    }
                }

                // Прилагане на потребителските чекбокс правила за филтрация
                if (config.TrackAssetId && string.IsNullOrEmpty(detectedAssetId))
                {
                    summary.Errors.Add(new ValidationError { LineNumber = currentLine, RawContent = line, Reason = "Непозната сигнатура", Details = "Никое парче текст не отговаря на изискванията за Идентификатор." });
                    continue;
                }

                int qty = 1; // По подразбиране
                if (config.TrackQuantity && (!int.TryParse(rawQtyStr, out qty) || qty < 0))
                {
                    summary.Errors.Add(new ValidationError { LineNumber = currentLine, RawContent = line, Reason = "Грешно количество", Details = $"Стойността '{rawQtyStr}' не е валидно число." });
                    continue;
                }

                // Записваме пречистените данни
                if (!string.IsNullOrEmpty(detectedAssetId))
                {
                    fileData[detectedAssetId] = qty;
                }
                summary.SuccessfulLines++;
            }
        }

        // 2. ИНТЕЛЕГЕНТНО ЗАСИЧАНЕ С РЕАЛНИТЕ БРОЙКИ ОТ SQL SERVER ПРЕЗ LINQ
        // Пресмятаме текущите крайни баланси за абсолютно всеки заведен обект в SQL
        var systemCounts = transactionsFromDb
            .GroupBy(t => t.AssetId)
            .ToDictionary(
                g => g.Key,
                g => g.Sum(t => (int)t.Operation.Direction == 1 ? t.Quantity : ((int)t.Operation.Direction == 2 ? -t.Quantity : 0)),
                StringComparer.OrdinalIgnoreCase
            );

        // Съпоставяме официалния регистър от базата данни с качения външен одит
        foreach (var sysAsset in systemCounts)
        {
            var diffLine = new DiffReportLine
            {
                Id = sysAsset.Key,
                Name = $"Регистриран актив в SQL Server база данни",
                SystemCount = sysAsset.Value
            };

            if (fileData.TryGetValue(sysAsset.Key, out int fileQty))
            {
                diffLine.FileCount = fileQty;
                diffLine.Status = diffLine.Difference == 0 ? "Съвпада" : "Разлика в бройката";
                fileData.Remove(sysAsset.Key); // Премахваме го, за да останат само непознатите
            }
            else
            {
                diffLine.FileCount = 0;
                diffLine.Status = "Липсва във файла";
            }

            summary.DiffLines.Add(diffLine);
        }

        // Обработка на обекти, които ГИ ИМА във външния файл, но ГИ НЯМА в SQL регистъра ни
        foreach (var fileAsset in fileData)
        {
            summary.DiffLines.Add(new DiffReportLine
            {
                Id = fileAsset.Key,
                Name = "⚠️ НЕПОЗНАТ ОБЕКТ (Липсва официален запис в SQL!)",
                SystemCount = 0,
                FileCount = fileAsset.Value,
                Status = "Липсва във файла" // Ще се оцвети в червено/тъмно на екрана
            });
        }

        return summary;
    }

    // =========================================================================
    // ТВОЯТ ВАЛУТЕН ДВИГАТЕЛ: Автоматично намиране на курс по диапазон (<= на датата)
    // =========================================================================
    public async Task<decimal> GetExchangeRateAsync(int fromCurrencyId, int toCurrencyId, DateTime transactionDate)
    {
        // 1. Ако валутите съвпадат, курсът винаги е 1.00000
        if (fromCurrencyId == toCurrencyId) return 1.00000m;

        // 2. Проверка за Фиксиран курс в базата данни (независим от датата)
        var fixedRate = await _context.ExchangeRateHistories
            .Where(r => r.FromCurrencyId == fromCurrencyId && r.ToCurrencyId == toCurrencyId && r.IsFixed)
            .Select(r => r.Rate)
            .FirstOrDefaultAsync();

        if (fixedRate > 0) return fixedRate;

        // 3. ТВОЯТА ФОРМУЛА: Плаващ курс по диапазон (<= на датата на трансакцията)
        var floatingRate = await _context.ExchangeRateHistories
            .Where(r => r.FromCurrencyId == fromCurrencyId
                     && r.ToCurrencyId == toCurrencyId
                     && !r.IsFixed
                     && r.RateDate.Date <= transactionDate.Date)
            .OrderByDescending(r => r.RateDate) // Взима най-близкия последно въведен курс назад във времето
            .Select(r => r.Rate)
            .FirstOrDefaultAsync();

        // Ако няма въведен курс за диапазона, връщаме 1.00000 като счетоводна защита срещу зануляване
        return floatingRate > 0 ? floatingRate : 1.00000m;
    }

    // Автоматично преизчисляване на левовата себестойност на трансакция
    public async Task<decimal> CalculateAssetValueInBgnAsync(int transactionId)
    {
        var transaction = await _context.AssetTransactions
            .Include(t => t.Operation)
            .FirstOrDefaultAsync(t => t.Id == transactionId);

        if (transaction == null) return 0.00000m;

        // Приемаме ID = 1 за основна национална валута (BGN) в SQL Server
        int bgnCurrencyId = 1;

        // Взимаме валутата, посочена в трансакцията (Тук в реалния модел ще добавим CurrencyId)
        int foreignCurrencyId = 2; // Временно заложено за USD/EUR тест

        // Извикваме валутния двигател по диапазон на датата на трансакцията
        decimal rate = await GetExchangeRateAsync(foreignCurrencyId, bgnCurrencyId, transaction.TransactionDate);

        // Връщаме крайната левова себестойност: Количество х Единична цена във валута х Намерения курс
        return transaction.Quantity * 1.00m * rate;
    }

    // ТВОЯТ СЕРВИЗЕН МЕТОД ЗА ЗАПИС ПО МОДЕЛА ОТ STUDENT MANAGEMENT
    public async Task<bool> CreateTransactionAsync(int assetId, int quantity, int? fromFacilityId, int? toFacilityId, DateTime transactionDate, string docRef, string notes, int opId)
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
