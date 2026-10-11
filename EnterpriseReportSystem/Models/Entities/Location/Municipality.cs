namespace EnterpriseReportSystem.Models.Entities.Location;

public class Municipality
{
    public int Id { get; set; } // Числов първичен ключ (Identity)
    public string Name { get; set; } = string.Empty; // напр. Община Марица

    // Външен ключ към Областта (Ниво 1)
    public int RegionId { get; set; }
    public Region Region { get; set; } = null!;

    // Релация: Една община съдържа много градове/села
    public ICollection<TownOrVillage> TownsOrVillages { get; set; } = new List<TownOrVillage>();
}
