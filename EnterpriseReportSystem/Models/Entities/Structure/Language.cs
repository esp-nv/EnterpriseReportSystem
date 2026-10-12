using System.ComponentModel.DataAnnotations;

namespace EnterpriseReportSystem.Models.Entities.Structure;

public class Language
{
    public int Id { get; set; }

    [Required]
    public string Code { get; set; } = string.Empty; // напр. "bg", "en", "de"

    [Required]
    public string Name { get; set; } = string.Empty; // напр. "Български", "English"
}
