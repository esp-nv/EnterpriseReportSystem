using System.Collections.Generic;

namespace EnterpriseReportSystem.Models.Entities;

public class AssetGroup
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    // Премахваме старото булево свойство IsAnimal и го заменяме с релация към Главния Тип
    public string MainTypeCode { get; set; } = string.Empty;
    public MainType MainType { get; set; } = null!;

    // Релация: Една група съдържа много подгрупи
    public ICollection<AssetSubGroup> AssetSubGroups { get; set; } = new List<AssetSubGroup>();
}
