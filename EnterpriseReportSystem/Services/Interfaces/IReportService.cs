using System.Collections.Generic;
using EnterpriseReportSystem.Models.Entities;

namespace EnterpriseReportSystem.Services.Interfaces;

public interface IReportService
{
    // 1. Изчислява и генерира обобщената количествена ведомост (Оборотка) на база хронологията на трансакциите
    List<ITrackableAsset> GenerateTurnoverReport(List<AssetTransaction> transactions, List<ITrackableAsset> currentRegistry);

    // 2. Филтрира активите по комплексните критерии (период, община, населено място, група)
    List<ITrackableAsset> FilterAssets(List<ITrackableAsset> assets, string municipality, string town, string groupCode);
}
