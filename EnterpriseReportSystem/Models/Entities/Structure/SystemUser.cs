using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnterpriseReportSystem.Models.Entities.Structure
{
    [Table("SystemUsers")]
    public class SystemUser
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// ОПТИМИЗИРАН ТИП НА ОФОРМЛЕНИЕТО (ПАЗИ ЧИСЛО В БАЗАТА):
        /// 1 = DASHBOARD (Страничен вертикален контролен панел)
        /// 2 = RIBBON (Хоризонтална широка лента тип MS Excel / Word)
        /// 3 = NAVBAR (Хоризонтална лента най-отгоре с два варианта на визуализация)
        /// </summary>
        [Required]
        public int LayoutType { get; set; } = 1; // По подразбиране е 1 (DASHBOARD)

        /// <summary>
        /// КОНТРОЛ НА ВАРИАНТИТЕ ЗА ИЗБРАНОТО ОФОРМЛЕНИЕ:
        /// За LayoutType = 3 (NAVBAR):
        /// - false = Вариант 1 (Тънка хоризонтална лента с класически падащи списъци)
        /// - true  = Вариант 2 (Масивно Мега Меню с разгърната решетка на колони)
        /// </summary>
        [Required]
        public bool IsMenuExpanded { get; set; } = true;

        [Required]
        [StringLength(50)]
        public string ThemeType { get; set; } = "LIGHT"; // LIGHT, DARK, CUSTOM

        public bool IsGlobalAdmin { get; set; } = false;

        public string WishListMenuIds { get; set; } = string.Empty;

        [Required]
        public int CompanyId { get; set; }

        [ForeignKey("CompanyId")]
        public virtual CompanyOrPerson Company { get; set; } = null!;

        [Required]
        public bool IsCustomPaletteActive { get; set; } = false; // false -> Bootstrap; true -> Външни CSS файлове

        // =========================================================================
        // СВЪРЗВАНЕ С НОВАТА СИСТЕМА ЗА ЦЕНТРАЛНИ CSS ТЕМИ
        // =========================================================================
        public int? ThemeId { get; set; } // Външен ключ към таблица GlobalThemes

        [ForeignKey("ThemeId")]
        public virtual GlobalTheme? GlobalTheme { get; set; }

        [Required]
        public bool IsDarkUserPreference { get; set; } = true; // bit: true = Тъмна, false = Светла
    }
}
