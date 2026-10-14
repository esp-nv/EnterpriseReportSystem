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
    public async Task<IActionResult> SaveSettings(SystemUser model, Dictionary<int, string> BootstrapClasses)
    {
        // 1. Намираме съществуващия потребител в базата данни на чисто
        var dbUser = await _context.SystemUsers.FirstOrDefaultAsync(u => u.Id == model.Id);
        if (dbUser == null) return NotFound();

        // 2. Обновяваме главните системни параметри от Раздел 1 и Раздел 4
        dbUser.LayoutType = model.LayoutType;
        dbUser.ThemeType = model.ThemeType;

        if (!string.IsNullOrEmpty(model.Username)) dbUser.Username = model.Username;
        if (!string.IsNullOrEmpty(model.Password)) dbUser.Password = model.Password;

        // 3. ЖЕЛЕЗЕН ЦИКЪЛ: Обновяваме атомарните свойства от Секция 3 в SQL Server
        if (BootstrapClasses != null && BootstrapClasses.Any())
        {
            foreach (var item in BootstrapClasses)
            {
                var themeId = item.Key;
                var selectedClass = item.Value;

                var dbThemeRow = await _context.SystemUserThemes
                    .FirstOrDefaultAsync(t => t.Id == themeId && t.SystemUserId == dbUser.Id);

                if (dbThemeRow != null)
                {
                    dbThemeRow.BootstrapClass = selectedClass;
                }
            }
        }

        // 4. ОФИЦИАЛНО ЗАПЕЧАТВАНЕ НА ТРАНСАКЦИЯТА В SQL SERVER
        await _context.SaveChangesAsync();

        // 5. ЗАТВАРЯНЕ НА ПРОЗОРЕЦА: Автоматично пренасочване към началната страница Home
        return RedirectToAction("Index", "Home");
    }


}
