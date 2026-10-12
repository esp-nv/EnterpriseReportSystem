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
        // Проверка: Ако таблицата е празна, наливане на мастър данните от JSON веднага
        if (!await _context.SystemUsers.AnyAsync())
        {
            DbInitializer.Seed(_context);
            await _context.SaveChangesAsync();
        }

        // Сега вече Супер Администраторът с Id = 1 ще съществува на 100%
        var user = await _context.SystemUsers.FirstOrDefaultAsync(u => u.Id == 1);

        if (user == null)
        {
            return NotFound("⚠️ [Критична грешка]: Инициализаторът не успя да заведе потребители.");
        }

        return View(user);
    }

    // 2. ЗАПИСВАНЕ НА НАСТРОЙКИТЕ ОТ ЕКРАНА (POST)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveSettings(int id, string layoutType, string themeType, bool isMenuExpanded)
    {
        var user = await _context.SystemUsers.FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
        {
            return NotFound();
        }

        user.LayoutType = layoutType;
        user.ThemeType = themeType.Trim().ToLower(); // Пазим в чист малък регистър за CSS
        user.IsMenuExpanded = isMenuExpanded;

        _context.SystemUsers.Update(user);
        await _context.SaveChangesAsync();

        TempData["SettingsSuccess"] = "🎉 Визуалната тема и стил на менюто бяха обновени в SQL Server на секундата!";

        // Пренасочваме към Централния панел с отчети (затваряме настройките)
        return RedirectToAction("Index", "Report");
    }
}
