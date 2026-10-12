using System.ComponentModel.DataAnnotations;

namespace EnterpriseReportSystem.Models.Entities.Structure;

public class SystemUser
{
    public int Id { get; set; }

    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty; // Пазим я свободно за тестови цели

    // Връзка към фирмата, в която работи потребителят
    public int CompanyId { get; set; }
    public CompanyOrPerson Company { get; set; } = null!;
}
