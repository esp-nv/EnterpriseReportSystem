namespace EnterpriseReportSystem.Models.Entities.Location; // Само един чист и точен namespace път

public class TownOrVillage
{
    public int Id { get; set; } // Числов първичен ключ (Identity)
    public string Name { get; set; } = string.Empty; // напр. с. Труд, гр. Пловдив
    public string PostalCode { get; set; } = string.Empty; // напр. 4199

    // Външен ключ към Общината (Ниво 2)
    public int MunicipalityId { get; set; }
    public Municipality Municipality { get; set; } = null!;
}
