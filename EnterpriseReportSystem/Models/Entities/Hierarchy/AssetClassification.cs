using EnterpriseReportSystem.Models.Entities.Hierarchy;

namespace EnterpriseReportSystem.Models.Entities;

public class AssetClassification
{
    public int Id { get; set; } // Цифрово уникално ID
    public string Name { get; set; } = string.Empty; // напр. Ил дьо Франс, John Deere, Тип 500

    // ВЪНШНИЯТ КЛЮЧ: Свързва спецификацията с конкретната подгрупа (Ниво 3)
    public string AssetSubGroupCode { get; set; } = string.Empty;
    public AssetSubGroup AssetSubGroup { get; set; } = null!;
}
