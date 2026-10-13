using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EnterpriseReportSystem.Common.Constants; // Добавяме достъп до константите

namespace EnterpriseReportSystem.Models.Entities.Structure;

public class SystemUserTheme
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int SystemUserId { get; set; }

    [ForeignKey("SystemUserId")]
    public SystemUser? SystemUser { get; set; }

    [Required]
    public int SystemUIElementId { get; set; }

    [ForeignKey("SystemUIElementId")]
    public SystemUIElement? SystemUIElement { get; set; }

    [Required]
    [StringLength(ValidationConstants.UIElementKeyMaxLength)] // Използваме 50
    public string ElementContext { get; set; } = string.Empty;

    [Required]
    [StringLength(ValidationConstants.ThemeBootstrapClassMaxLength)] // Използваме 100 от предния път
    public string BootstrapClass { get; set; } = string.Empty;

    [Required]
    [StringLength(ValidationConstants.UIElementDescMaxLength)] // Използваме 200 за твоето описание
    public string Description { get; set; } = string.Empty;
}
