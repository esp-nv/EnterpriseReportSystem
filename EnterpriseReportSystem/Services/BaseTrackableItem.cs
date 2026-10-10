using EnterpriseReportSystem.Services.Interfaces;

namespace EnterpriseReportSystem.Models.Entities;

public abstract class BaseTrackableItem : ITrackableAsset
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public AssetGroup Group { get; set; } = new();

    // Географска и счетоводна локация
    public string Municipality { get; set; } = string.Empty;
    public string TownOrVillage { get; set; } = string.Empty;
    public string Facility { get; set; } = string.Empty;

    // Счетоводни бройки за количествената ведомост
    public int InitialCount { get; set; }
    public int InputCount { get; set; }
    public int OutputCount { get; set; }

    // Изчисляваме крайното салдо автоматично на база счетоводното правило
    public int FinalCount => this.InitialCount + this.InputCount - this.OutputCount;
}
