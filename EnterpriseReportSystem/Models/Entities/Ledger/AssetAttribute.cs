using System.ComponentModel.DataAnnotations;
using EnterpriseReportSystem.Models.Entities.Hierarchy;

namespace EnterpriseReportSystem.Models.Entities.Ledger;

public class AssetAttribute
{

    // Регистър на динамичните белези
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty; // "Пол", "Номер на двигател", "Пътен лист №"

    // 1 = За самия Актив (зависи от Категорията), 2 = За Трансакцията (зависи от Операцията)
    public int ContextType { get; set; }

    // Външен числителен ключ, който се обвързва динамично
    public int TargetId { get; set; }
}
