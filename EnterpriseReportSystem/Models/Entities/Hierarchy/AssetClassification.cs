using System.ComponentModel.DataAnnotations;

namespace EnterpriseReportSystem.Models.Entities.Hierarchy;

public class AssetSpecification
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty; // Име на породата (напр. "Херефорд") или модела ("John Deere 6R")

    // Връзка към Подгрупата (Ниво 3), за да знаем тази порода/модел за кое животно/машина се отнася
    public int AssetSubGroupId { get; set; }
    public AssetSubGroup AssetSubGroup { get; set; } = null!;
}
