namespace EnterpriseReportSystem.Models.Entities;

public class AssetGroup
{
    public string Code { get; set; } = string.Empty; // напр. "DPJ_UNDER_6M", "DPJ_ADULT", "EPJ_ADULT"
    public string Name { get; set; } = string.Empty; // напр. "ДПЖ - Телета до 6м.", "ДПЖ - Крави"
    public bool IsAnimal { get; set; } // true за ДПЖ/ЕПЖ, false за складов продукт (фураж)
}
