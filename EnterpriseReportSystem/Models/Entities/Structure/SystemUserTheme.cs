using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnterpriseReportSystem.Models.Entities.Structure;

public class SystemUserTheme
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int SystemUserId { get; set; }

    // Релационна връзка: Всеки стил принадлежи строго на един потребител
    [ForeignKey("SystemUserId")]
    public SystemUser? SystemUser { get; set; }

    [Required]
    [StringLength(50)]
    public string ElementContext { get; set; } = string.Empty; // "navbar", "sidebar", "text", "badge", "active_menu"

    [Required]
    [StringLength(100)]
    public string BootstrapClass { get; set; } = string.Empty; // "text-bg-dark", "bg-primary", "text-warning"
}
