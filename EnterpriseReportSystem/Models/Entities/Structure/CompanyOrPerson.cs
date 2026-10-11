namespace EnterpriseReportSystem.Models.Entities.Structure;

public class CompanyOrPerson
{
    public int Id { get; set; } // Числов първичен ключ (Identity)
    public string ClientCode { get; set; } = string.Empty; // Твоят уникален код
    public string Name { get; set; } = string.Empty; // Име на фирмата или човека
    public string TaxIdentifier { get; set; } = string.Empty; // ЕГН или Булстат за одит

    // Административно седалище по Търговски регистър / Лична карта
    public int RegisteredTownId { get; set; }
    public TownOrVillage RegisteredTown { get; set; } = null!;

    // Релация: Един субект може да притежава много физически ферми
    public ICollection<Facility> Facilities { get; set; } = new List<Facility>();
}
