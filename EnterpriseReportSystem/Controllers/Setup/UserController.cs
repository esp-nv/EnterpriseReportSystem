using System;
using System.Linq;
using System.Threading.Tasks;
using EnterpriseReportSystem.Data;
using EnterpriseReportSystem.Models.Entities.Structure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseReportSystem.Controllers.Setup;

public class UserController : Controller
{
    private readonly ApplicationDbContext _context;

    public UserController(ApplicationDbContext context)
    {
        _context = context;
    }

    // 1. ЗАРЕЖДАНЕ НА ЕКРАНА ЗА НАСТРОЙКИ (GET)
    [HttpGet]
    public async Task<IActionResult> Settings()
    {
        // 1. Взимаме текущия потребител (Админ с Id = 1)
        var user = await _context.SystemUsers.FirstOrDefaultAsync(u => u.Id == 1);
        if (user == null) return NotFound();

        // 2. ИЗВЛИЧАМЕ ВСИЧКИ СВОЙСТВА, ЗАЕДНО С МАСTЪР ЗОНИТЕ ЧРЕЗ .INCLUDE()
        var userStyles = await _context.SystemUserThemes
            .Include(t => t.SystemUIElement)
            .Where(t => t.SystemUserId == user.Id)
            .ToListAsync();

        // 3. ИЗВЛИЧАМЕ И ЦЕЛИТЕ МАСTЪР КОТВИ ЗА ДИНАМИЧНИТЕ МЕНЮТА
        var uiElements = await _context.SystemUIElements.AsNoTracking().ToListAsync();

        // Подаваме данните към екрана
        ViewBag.UserStyles = userStyles;
        ViewBag.UIElements = uiElements;

        return View(user);
    }


    // 2. ЗАПИСВАНЕ НА НАСТРОЙКИТЕ (POST) - С КОРЕКЦИЯ НА ОБВЪРЗВАНЕТО И ПРЕНАСОЧВАНЕТО
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveSettings(int Id, string LayoutType, string ThemeType, bool IsCustomPaletteActive)
    {
        // 1. Намираме основния потребител в SQL Server
        var user = await _context.SystemUsers.FirstOrDefaultAsync(u => u.Id == Id);
        if (user == null)
        {
            return NotFound();
        }

        // 2. Записваме новите глобални флагове и изчистените Bootstrap режими
        user.LayoutType = LayoutType;          // "RIBBON" или "DASHBOARD"
        user.ThemeType = ThemeType.ToLower();  // "light" или "dark"
        user.IsCustomPaletteActive = IsCustomPaletteActive;

        _context.SystemUsers.Update(user);
        await _context.SaveChangesAsync();

        // 3. АКО Е АКТИВИРАН USER.CSS -> АКТУАЛИЗИРАМЕ И КЛАСОВЕТЕ В НОВАТА ТАБЛИЦА
        if (IsCustomPaletteActive)
        {
            var userStyles = await _context.SystemUserThemes.Where(t => t.SystemUserId == Id).ToListAsync();

            // Пример за динамичен ъпдейт според избора (може да се разшири с палитрите)
            var navbarStyle = userStyles.FirstOrDefault(s => s.ElementContext == "navbar");
            if (navbarStyle != null)
            {
                navbarStyle.BootstrapClass = ThemeType == "light" ? "text-bg-light" : "text-bg-dark";
                _context.SystemUserThemes.Update(navbarStyle);
            }

            var sidebarStyle = userStyles.FirstOrDefault(s => s.ElementContext == "sidebar");
            if (sidebarStyle != null)
            {
                sidebarStyle.BootstrapClass = ThemeType == "light" ? "bg-light" : "bg-dark";
                _context.SystemUserThemes.Update(sidebarStyle);
            }

            await _context.SaveChangesAsync();
        }

      //  TempData["SettingsSuccess"] = "🎉 Всички интерфейсни промени бяха запечатани успешно в SQL Server!";

        // Пренасочваме към чистата Home страница по твоя план
        return RedirectToAction("Index", "Home");
    }

}
