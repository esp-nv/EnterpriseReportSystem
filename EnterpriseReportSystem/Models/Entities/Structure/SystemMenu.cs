using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace EnterpriseReportSystem.Models.Entities.Structure;

public class SystemMenu
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    public string Notes { get; set; } = string.Empty;

    // Счетоводна йерархична релация: Сочи към главното родителско меню
    public int? ParentMenuId { get; set; }
    public SystemMenu? ParentMenu { get; set; }

    // Колекция от всички подчинени подменюта (Ниво 2)
    public List<SystemMenu> ChildMenus { get; set; } = new();
}
