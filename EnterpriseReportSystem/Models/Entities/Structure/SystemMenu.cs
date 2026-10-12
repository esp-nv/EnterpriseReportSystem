using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace EnterpriseReportSystem.Models.Entities.Structure;

public class SystemMenu
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    public string Notes { get; set; } = string.Empty;

    // Пазим само иконата/класа (напр. "bi bi-box-seam"), за да има визуална красота
    public string CustomCssClass { get; set; } = string.Empty;

    public int? ParentMenuId { get; set; }
    public SystemMenu? ParentMenu { get; set; }

    public List<SystemMenu> ChildMenus { get; set; } = new();
}
