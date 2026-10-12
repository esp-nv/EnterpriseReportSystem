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
    // 1. ЗАРЕЖДАНЕ НА ЕКРАНА ЗА НАСТРОЙКИ (С АВТОМАТИЧНА ЗАЩИТА ЗА ПРАЗНА БАЗА)
    [HttpGet]
    public async Task<IActionResult> Settings()
    {
        // Проверка: Ако таблицата е празна, наливане на мастър данните от JSON веднага!
        if (!await _context.SystemUsers.AnyAsync())
        {
            DbInitializer.Seed(_context);
        }

        // Сега вече Иван с Id=1 ще съществува на 100%!
        var user = await _context.SystemUsers.FirstOrDefaultAsync(u => u.Id == 1);

        if (user == null)
        {
            return NotFound("Потребителят не е намерен в базата данни. Моля, проверете дали JSON файлът съществува в InputFiles.");
        }

        return View(user);
    }


    // 2. ЗАПИСВАНЕ НА ПЕРСОНАЛНИТЕ ТЕМИ И ИНТЕРФЕЙСИ (POST)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveSettings(int id, string layoutType, string themeType, bool isMenuExpanded)
    {
        var user = await _context.SystemUsers.FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
        {
            return NotFound();
        }

        // Интелигентно обновяване на персоналния холдингов профил
        user.LayoutType = layoutType;
        user.ThemeType = themeType;
        user.IsMenuExpanded = isMenuExpanded;

        _context.SystemUsers.Update(user);
        await _context.SaveChangesAsync();

        TempData["SettingsSuccess"] = "🎉 Интерфейсът и визуалната тема бяха обновени в SQL Server на секундата!";

        return RedirectToAction("Settings");
    }
}
