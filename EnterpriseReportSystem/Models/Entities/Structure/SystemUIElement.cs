using System.ComponentModel.DataAnnotations;
using EnterpriseReportSystem.Common.Constants; // Добавяме достъп до константите

namespace EnterpriseReportSystem.Models.Entities.Structure;

public class SystemUIElement
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(ValidationConstants.UIElementKeyMaxLength)] // Използваме 50
    public string ElementKey { get; set; } = string.Empty;

    [Required]
    [StringLength(ValidationConstants.UIElementNameMaxLength)] // Използваме 100
    public string DisplayName { get; set; } = string.Empty;
}
