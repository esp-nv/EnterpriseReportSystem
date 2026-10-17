using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using EnterpriseReportSystem.Models.Entities.Structure;
using EnterpriseReportSystem.Common.Enum;
using EnterpriseReportSystem.Common.Constants; // Включваме константите

namespace EnterpriseReportSystem.ViewModels;

public class SettingsViewModel : IValidatableObject
{
    public int UserId { get; set; }

    [Required(ErrorMessage = "⚠️ Потребителското име е задължително.")]
    [StringLength(ValidationConstants.UsernameMaxLength, MinimumLength = ValidationConstants.UsernameMinLength,
        ErrorMessage = "⚠️ Името трябва да бъде между {2} и {1} символа.")]
    public string Username { get; set; } = string.Empty;

    public bool IsGlobalAdmin { get; set; }

    [Required]
    public int CompanyId { get; set; }

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

    public IEnumerable<GlobalTheme> AvailableThemes { get; set; } = new List<GlobalTheme>();

    [DataType(DataType.Password)]
    public string? CurrentPassword { get; set; }

    [DataType(DataType.Password)]
    public string? NewPassword { get; set; }

    [DataType(DataType.Password)]
    public string? ConfirmNewPassword { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        // 1. Защита за визуалните теми
        if (IsCustomPaletteActive && ThemeId == 1)
        {
            yield return new ValidationResult(
                "⚠️ Моля, изберете конкретен външен стилов файл или преминете към 'Фабричен Bootstrap'.",
                new[] { nameof(ThemeId) });
        }

        if (!IsCustomPaletteActive && ThemeId != 1)
        {
            yield return new ValidationResult(
                "⚠️ При фабричен Bootstrap изглед не може да има прикачен външен CSS файл.",
                new[] { nameof(ThemeId) });
        }

        // 2. Защита при смяна на парола (БЕЗ МАГИЧЕСКИ ЧИСЛА)
        bool hasCurrent = !string.IsNullOrWhiteSpace(CurrentPassword);
        bool hasNew = !string.IsNullOrWhiteSpace(NewPassword);
        bool hasConfirm = !string.IsNullOrWhiteSpace(ConfirmNewPassword);

        if (hasCurrent || hasNew || hasConfirm)
        {
            if (!hasCurrent)
            {
                yield return new ValidationResult("⚠️ Трябва да въведете текущата си парола, за да я промените.", new[] { nameof(CurrentPassword) });
            }

            if (!hasNew)
            {
                yield return new ValidationResult("⚠️ Моля, въведете новата си парола.", new[] { nameof(NewPassword) });
            }

            if (!hasConfirm)
            {
                yield return new ValidationResult("⚠️ Моля, повторете новата си парола.", new[] { nameof(ConfirmNewPassword) });
            }

            if (hasNew && hasConfirm && NewPassword != ConfirmNewPassword)
            {
                yield return new ValidationResult("⚠️ Новата парола и повторението не съвпадат.", new[] { nameof(ConfirmNewPassword) });
            }

            if (hasNew)
            {
                if (NewPassword!.Length < ValidationConstants.PasswordMinLength)
                {
                    yield return new ValidationResult($"⚠️ Новата парола трябва да бъде дълга поне {ValidationConstants.PasswordMinLength} символа.", new[] { nameof(NewPassword) });
                }
                else
                {
                    var hasUpper = new Regex(ValidationConstants.PasswordUpperRegex);
                    var hasLower = new Regex(ValidationConstants.PasswordLowerRegex);
                    var hasDigit = new Regex(ValidationConstants.PasswordDigitRegex);

                    // ДОБАВЕНО: Зареждаме новия регулярен израз за специален символ
                    var hasSpecial = new Regex(ValidationConstants.PasswordSpecialRegex);

                    // Разширяваме проверката с новото твърдо изискване
                    if (!hasUpper.IsMatch(NewPassword) ||
                        !hasLower.IsMatch(NewPassword) ||
                        !hasDigit.IsMatch(NewPassword) ||
                        !hasSpecial.IsMatch(NewPassword))
                    {
                        yield return new ValidationResult("⚠️ Паролата трябва да съдържа поне една главна буква, една малка буква, една цифра и поне един специален символ (напр. @, #, $, %).", new[] { nameof(NewPassword) });
                    }
                }
            }

        }
    }
}
