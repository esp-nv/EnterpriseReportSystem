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
        if (!await _context.SystemUsers.AnyAsync())
        {
            DbInitializer.Seed(_context);
            await _context.SaveChangesAsync();
        }

        var user = await _context.SystemUsers.FirstOrDefaultAsync(u => u.Id == 1);
        if (user == null)
        {
            return NotFound("Потребителят не е намерен.");
        }

        return View(user);
    }

    // 2. ЗАПИСВАНЕ НА НАСТРОЙКИТЕ (POST) - С КОРЕКЦИЯ НА ОБВЪРЗВАНЕТО И ПРЕНАСОЧВАНЕТО
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveSettings(int Id, string LayoutType, string ThemeType, bool IsMenuExpanded)
    {
        var user = await _context.SystemUsers.FirstOrDefaultAsync(u => u.Id == Id);

        if (user == null)
        {
            return NotFound();
        }

        // Записваме точно зададените от екрана стойности
        user.LayoutType = LayoutType;
        user.ThemeType = ThemeType.Trim().ToLower(); // Пазим в малки букви за CSS файловете
        user.IsMenuExpanded = IsMenuExpanded;

        _context.SystemUsers.Update(user);
        await _context.SaveChangesAsync();

        TempData["SettingsSuccess"] = "🎉 Визуалната тема и стил на менюто бяха обновени в SQL Server на секундата!";

        // КОРЕКЦИЯ: Пренасочваме строго към празната Home страница (Home/Index)
        return RedirectToAction("Index", "Home");
    }
}
