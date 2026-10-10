using EnterpriseReportSystem.Services.Interfaces;
using System;

namespace EnterpriseReportSystem.Models.Entities;

public abstract class BaseTrackableItem : ITrackableAsset, ISoftDeletable
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public AssetGroup Group { get; set; } = new();

    public string Municipality { get; set; } = string.Empty;
    public string TownOrVillage { get; set; } = string.Empty;
    public string Facility { get; set; } = string.Empty;

    public int InitialCount { get; set; }
    public int InputCount { get; set; }
    public int OutputCount { get; set; }

    public int FinalCount => this.InitialCount + this.InputCount - this.OutputCount;

    // Имплементация на Soft Delete
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedOn { get; set; }
}
