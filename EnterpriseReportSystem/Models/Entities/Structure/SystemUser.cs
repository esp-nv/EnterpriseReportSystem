using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EnterpriseReportSystem.Common.Constants;
using EnterpriseReportSystem.Common.Enum;

namespace EnterpriseReportSystem.Models.Entities.Structure
{
    /// <summary>
    /// ОФИЦИАЛНА ХОЛДИНГОВА МАТРИЦА ЗА ТИПОВЕТЕ ОФОРМЛЕНИЕ НА НАВИГАЦИЯТА
    /// Елиминира напълно магическите числа и записва леки флагове (1, 2, 3) в SQL Server.
    /// </summary>
   

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

        /// <summary>
        /// Системен тип на оформлението, защитен чрез системния enum AppLayoutType
        /// </summary>
        [Required]
        public AppLayoutType LayoutType { get; set; } = AppLayoutType.Dashboard;

        /// <summary>
        /// КОНТРОЛ НА ВАРИАНТИТЕ ЗА ИЗБРАНОТО ОФОРМЛЕНИЕ:
        /// За LayoutType = Navbar (3):
        /// - false = Вариант 1 (Тънка хоризонтална лента с класически падащи списъци)
        /// - true  = Вариант 2 (Масивно Мега Меню с разгърната решетка на колони)
        /// </summary>
        [Required]
        public bool IsMenuExpanded { get; set; } = true;

        [Required]
        [StringLength(ValidationConstants.ThemeTypeMaxLength)]
        public string ThemeType { get; set; } = "LIGHT";

        public bool IsGlobalAdmin { get; set; } = false;

        [StringLength(200)]
        public string WishListMenuIds { get; set; } = string.Empty;

        [Required]
        public int CompanyId { get; set; }

        [ForeignKey("CompanyId")]
        public virtual CompanyOrPerson Company { get; set; } = null!;

        [Required]
        public bool IsCustomPaletteActive { get; set; } = false;

       
        public int? ThemeId { get; set; }

        [ForeignKey("ThemeId")]
        public virtual GlobalTheme? GlobalTheme { get; set; }

        [Required]
        public bool IsDarkUserPreference { get; set; } = true;
    }
}
