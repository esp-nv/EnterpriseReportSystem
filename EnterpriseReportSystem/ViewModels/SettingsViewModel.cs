using System.ComponentModel.DataAnnotations;
using EnterpriseReportSystem.Models.Entities.Structure;
using EnterpriseReportSystem.Common.Enum;

namespace EnterpriseReportSystem.ViewModels;

public class SettingsViewModel : IValidatableObject
{
    public int UserId { get; set; }

    // ДОБАВЕНО: Задължителен идентификатор на компанията, за да се отпуши изгледът
    [Required]
    public int CompanyId { get; set; }

    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;

    [Required]
    public AppLayoutType LayoutType { get; set; } = AppLayoutType.Dashboard;

    [Required]
    public bool IsMenuExpanded { get; set; } = true;

    [Required]
    public bool IsCustomPaletteActive { get; set; }

    [Required]
    public int ThemeId { get; set; }

    [Required]
    public bool IsDarkUserPreference { get; set; }

    // Контейнер за филтрирания списък с теми (точно като Courses в модела ти от GitHub)
    public IEnumerable<GlobalTheme> AvailableThemes { get; set; } = new List<GlobalTheme>();

    // ЖЕЛЯЗНА БЕКЕНД КРОС-ВАЛИДАЦИЯ (Бронирано съответствие)
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        // 1. Защита при избран режим "Персонализирана тема"
        if (IsCustomPaletteActive && ThemeId == 1)
        {
            yield return new ValidationResult(
                "⚠️ Моля, изберете конкретен външен стилов файл от падащото меню или превключете на 'Фабричен Bootstrap'.",
                new[] { nameof(ThemeId) });
        }

        // 2. Защита при избран режим "Фабричен Bootstrap"
        if (!IsCustomPaletteActive && ThemeId != 1)
        {
            yield return new ValidationResult(
                "⚠️ При фабричен Bootstrap изглед не може да има прикачен външен CSS файл.",
                new[] { nameof(ThemeId) });
        }
    }
}
