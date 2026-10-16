using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EnterpriseReportSystem.Common.Constants;
using EnterpriseReportSystem.Common.Enum;

namespace EnterpriseReportSystem.Models.Entities.Structure
{
    [Table("SystemUsers")]
    public class SystemUser
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(ValidationConstants.UsernameMaxLength)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(ValidationConstants.PasswordMaxLength)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public AppLayoutType LayoutType { get; set; } = AppLayoutType.Dashboard; // Използва чистия enum

        [Required]
        public bool IsMenuExpanded { get; set; } = true;

        public bool IsGlobalAdmin { get; set; } = false;

        [StringLength(200)]
        public string WishListMenuIds { get; set; } = string.Empty;

        [Required]
        public int CompanyId { get; set; }

        [ForeignKey("CompanyId")]
        public virtual CompanyOrPerson Company { get; set; } = null!;

        [Required]
        public bool IsCustomPaletteActive { get; set; } = false; // Контролира се от радио бутоните

        public int? ThemeId { get; set; } // Външен ключ към новата каталог-таблица

        [ForeignKey("ThemeId")]
        public virtual GlobalTheme? GlobalTheme { get; set; }

        [Required]
        public bool IsDarkUserPreference { get; set; } = true; // БИТ-ФЛАГ: true (1) = Тъмна, false (0) = Светла
    }
}
