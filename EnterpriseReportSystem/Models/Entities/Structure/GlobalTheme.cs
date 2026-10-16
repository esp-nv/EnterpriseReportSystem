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
        public string FileName { get; set; } = string.Empty; // Име на файла: "corporate_blue.css" (празно за фабрична)

        [Required]
        [StringLength(50)]
        public string FileSize { get; set; } = "0 KB";

        /// <summary>
        /// БИТ-ФИЛТЪР: false (0) = Светла тема, true (1) = Тъмна тема
        /// </summary>
        [Required]
        [Column("SupportType")]
        public bool SupportType { get; set; } = true; // По подразбиране е Тъмна (1)
    }
}
