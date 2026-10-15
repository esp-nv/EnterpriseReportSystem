using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EnterpriseReportSystem.Common.Constants;

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

        [Required]
        public bool IsDark { get; set; } // Флаг: true за Тъмна, false за Светла тема
    }
}
