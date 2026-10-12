using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace EnterpriseReportSystem.Models.Entities.Structure;

public class SystemMenu
{
    public int Id { get; set; }

    // Имената и бележките са премахнати оттук — отиват в SystemMenuTranslations за многоезичност!

    // НОВО: Динамичен бърз клавиш (напр. "alt+n", "ctrl+s"), конфигуриран от потребителя
    public string CustomShortcut { get; set; } = string.Empty;

    // Пазим само иконата/класа (напр. "bi bi-box-seam"), за да има визуална красота
    public string CustomCssClass { get; set; } = string.Empty;

    public int? ParentMenuId { get; set; }
    public SystemMenu? ParentMenu { get; set; }

    public List<SystemMenu> ChildMenus { get; set; } = new();
}
