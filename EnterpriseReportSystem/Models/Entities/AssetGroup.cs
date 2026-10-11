using System.Collections.Generic;

namespace EnterpriseReportSystem.Models.Entities;

public class AssetGroup
{
    public int Id { get; set; } // ЧИСЛОВ ПЪРВИЧЕН КЛЮЧ (Identity)
    public string Code { get; set; } = string.Empty; // напр. "DPJ"
    public string Name { get; set; } = string.Empty; // напр. "Дребни преживни"

    // ВЪНШЕН КЛЮЧ КЪМ НИВО 1 (Свързва се с числовия Id)
    public int MainTypeId { get; set; }
    public MainType MainType { get; set; } = null!;

    // Релация: Една група има много подгрупи
    public ICollection<AssetSubGroup> AssetSubGroups { get; set; } = new List<AssetSubGroup>();
}
