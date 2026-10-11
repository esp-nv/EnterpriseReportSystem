using System.Collections.Generic;

namespace EnterpriseReportSystem.Models.Entities;

public class AssetSubGroup
{
    public int Id { get; set; } // ЧИСЛОВ ПЪРВИЧЕН КЛЮЧ (Identity)
    public string Code { get; set; } = string.Empty; // напр. "OVCE_MAYKI"
    public string Name { get; set; } = string.Empty; // напр. "Овце майки"

    // ТВОЕТО ИЗИСКВАНЕ: Мерна единица за пълна свобода на активите
    public string MeasurementUnit { get; set; } = string.Empty; // напр. бр., кг, л.

    // ВЪНШЕН КЛЮЧ КЪМ НИВО 2 (Свързва се с числовия Id)
    public int AssetGroupId { get; set; }
    public AssetGroup AssetGroup { get; set; } = null!;
}
