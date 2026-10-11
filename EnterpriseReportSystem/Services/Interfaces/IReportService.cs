using System.Collections.Generic;
using System.IO;
using EnterpriseReportSystem.Models.Entities;
using EnterpriseReportSystem.Models.Entities.Ledger;
using EnterpriseReportSystem.Models.ViewModels;

namespace EnterpriseReportSystem.Services.Interfaces;

public interface IReportService
{
    // 1. Изчислява и генерира обобщената количествена ведомост (Оборотка) на база хронологията на трансакциите
    List<ITrackableAsset> GenerateTurnoverReport(List<AssetTransaction> transactions, List<ITrackableAsset> currentRegistry);

    // 2. Филтрира активите по комплексните критерии (период, община, населено място, група)
    List<ITrackableAsset> FilterAssets(List<ITrackableAsset> assets, string municipality, string town, string groupCode);

    // 3. НОВИЯТ ДОГОВОР: Интелигентен одит и съпоставка на външни файлове по потребителски правила
    FileProcessSummary ProcessSmartImport(Stream fileStream, SmartImportViewModel config);

    //4. Валута 
    Task<decimal> GetExchangeRateAsync(int fromCurrencyId, int toCurrencyId, DateTime transactionDate);
    Task<decimal> CalculateAssetValueInBgnAsync(int transactionId);

}
