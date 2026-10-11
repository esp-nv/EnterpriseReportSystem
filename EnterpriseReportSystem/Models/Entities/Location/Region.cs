using System.Collections.Generic;

namespace EnterpriseReportSystem.Models.Entities;

public class Region
{
    public int Id { get; set; } // Числов първичен ключ (Identity)
    public string Name { get; set; } = string.Empty; // напр. Област Пловдив

    // Релация: Една област съдържа много общини
    public ICollection<Municipality> Municipalities { get; set; } = new List<Municipality>();
}
