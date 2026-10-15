using EnterpriseReportSystem.Common.Constants;
using EnterpriseReportSystem.Common.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnterpriseReportSystem.Models.Entities.Structure
{
    [Table("GlobalThemes")]
    public class GlobalTheme
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(ValidationConstants.ThemeNameMaxLength)]
        public string ThemeName { get; set; } = string.Empty; // Приятелско име: "Корпоративно синьо"

        [Required]
        [StringLength(ValidationConstants.FileNameMaxLength)]
        public string FileName { get; set; } = string.Empty; // Име на файла: "corporate_blue.css"

        [StringLength(ValidationConstants.FileSizeMaxLength)]
        public string FileSize { get; set; } = "0 KB"; // Мета-данни за размер

        /// <summary>
        /// Контрастен капацитет на темата: 1 = Светла, 2 = Тъмна, 3 = Универсална (И двете)
        /// </summary>
        [Required]
        public ThemeSupportType SupportType { get; set; } = ThemeSupportType.Universal;
    }

}
}
