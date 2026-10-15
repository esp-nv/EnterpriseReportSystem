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
        [StringLength(100)]
        public string ThemeName { get; set; } = string.Empty; // Приятелско име: "Корпоративно синьо"

        [Required]
        [StringLength(150)]
        public string FileName { get; set; } = string.Empty; // Име на файла: "corporate_blue.css"

        [StringLength(50)]
        public string FileSize { get; set; } = "0 KB"; // Мета-данни за размер

        [Required]
        public bool IsDark { get; set; } // Флаг: true за Тъмна, false за Светла тема
    }
}
