using System.ComponentModel.DataAnnotations;

namespace EnterpriseReportSystem.Models.Entities.Structure;

public class SystemUser
{
    public int Id { get; set; }

    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;

    // НОВИ ПОТРЕБИТЕЛСКИ НАСТРОЙКИ: Системата ще помни избора на всеки човек
    [Required]
    public string LayoutType { get; set; } = "DASHBOARD"; // Падащо меню в UI: DASHBOARD, WORD, EXCEL

    public bool IsMenuExpanded { get; set; } = true; // Чекбокс в UI: true = Разгънато, false = Свито

    public int CompanyId { get; set; }
    public CompanyOrPerson Company { get; set; } = null!;
}
