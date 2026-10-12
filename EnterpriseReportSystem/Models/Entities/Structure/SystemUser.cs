using System.ComponentModel.DataAnnotations;

namespace EnterpriseReportSystem.Models.Entities.Structure;

public class SystemUser
{
    public int Id { get; set; }

    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;

    [Required]
    public string LayoutType { get; set; } = "DASHBOARD"; // DASHBOARD, WORD, EXCEL

    public bool IsMenuExpanded { get; set; } = true; // true = Разгънато, false = Свито

    // ФИНАЛНИЯТ ЩРИХ: Системата ще помни и визуалната тема на потребителя
    [Required]
    public string ThemeType { get; set; } = "LIGHT"; // LIGHT, DARK, PHOTOSHOP

    // НОВИЯТ АДМИНИСТРАТОРСКИ ФЛАГ: Дава глобален достъп над целия холдинг
    public bool IsGlobalAdmin { get; set; } = false;

    public int CompanyId { get; set; }
    public CompanyOrPerson Company { get; set; } = null!;
}
