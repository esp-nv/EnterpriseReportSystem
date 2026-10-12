using System.ComponentModel.DataAnnotations;

namespace EnterpriseReportSystem.Models.Entities.Structure;

public class SystemMenuTranslation
{
    public int Id { get; set; }

    // Връзка към менюто, което превеждаме
    public int SystemMenuId { get; set; }
    public SystemMenu SystemMenu { get; set; } = null!;

    // Връзка към езика на превода
    public int LanguageId { get; set; }
    public Language Language { get; set; } = null!;

    [Required]
    public string TranslatedName { get; set; } = string.Empty; // Преведеното име на менюто

    public string TranslatedNotes { get; set; } = string.Empty; // Преведената подсказка (Tooltip)
}
