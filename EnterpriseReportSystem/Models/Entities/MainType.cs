using System.Collections.Generic;

namespace EnterpriseReportSystem.Models.Entities;

public class MainType
{
    public int Id { get; set; } // ЧИСЛОВ ПЪРВИЧЕН КЛЮЧ (Identity)
    public string Code { get; set; } = string.Empty; // напр. "ANIMAL", "PRODUCT"
    public string Name { get; set; } = string.Empty; // напр. "Животно (ДПЖ/ЕПЖ)", "Продукт / Машина"

    // Релация: Един главен тип съдържа много основни групи
    public ICollection<AssetGroup> AssetGroups { get; set; } = new List<AssetGroup>();
}
