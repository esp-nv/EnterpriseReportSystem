using System.ComponentModel.DataAnnotations;

namespace EnterpriseReportSystem.Models.Entities.Ledger;

public class AssetAttributeValue
{
    //Реалните стойности за конкретния актив

    public int Id { get; set; }

    // Свързваме със счетоводната трансакция в Журнала
    public int TransactionId { get; set; }
    public AssetTransaction Transaction { get; set; } = null!;

    // Свързваме с дефиницията на характеристиката
    public int AssetAttributeId { get; set; }
    public AssetAttribute AssetAttribute { get; set; } = null!;

    [Required]
    public string Value { get; set; } = string.Empty; // "Женско", "ДВГ-987654", "ПЛ-2026"
}
