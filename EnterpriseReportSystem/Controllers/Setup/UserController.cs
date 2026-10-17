using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EnterpriseReportSystem.Data;
using EnterpriseReportSystem.Models.Entities.Structure;
using EnterpriseReportSystem.ViewModels;

namespace EnterpriseReportSystem.Controllers.Setup;

[Route("User/[action]")]
public class UserController : Controller
{
    private readonly ApplicationDbContext _context;

    public UserController(ApplicationDbContext context)
    {
        _context = context;
    }

    // 1. ИЗВЛИЧАНЕ НА НАСТРОЙКИТЕ И ЗАРЕЖДАНЕ НА VIEW MODEL (GET)
    [HttpGet]
    public async Task<IActionResult> Settings()
    {
        var user = await _context.SystemUsers
            .FirstOrDefaultAsync(u => u.Id == 1); // Тестов администратор #1

        if (user == null) return NotFound();

        var viewModel = new SettingsViewModel
        {
            UserId = user.Id,
            Username = user.Username,
            IsGlobalAdmin = user.IsGlobalAdmin,
            CompanyId = user.CompanyId,
            LayoutType = user.LayoutType,
            IsMenuExpanded = user.IsMenuExpanded,
            IsCustomPaletteActive = user.IsCustomPaletteActive,
            ThemeId = user.ThemeId,
            IsDarkUserPreference = user.IsDarkUserPreference,
            AvailableThemes = await _context.GlobalThemes.AsNoTracking().ToListAsync()
        };

        return View(viewModel);
    }

    // 2. ЗАПЕЧАТВАНЕ, СТРОГА LINQ ПРОВЕРКА НА ПАРОЛИТЕ И КРОС-ВАЛИДАЦИЯ (POST)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveSettings(SettingsViewModel model)
    {
        // Извличаме оригиналния запис на потребителя от SQL Server за сравнение
        var dbUser = await _context.SystemUsers.FirstOrDefaultAsync(u => u.Id == model.UserId);
        if (dbUser == null) return NotFound();

        // ЛОГИКА 1: Проверка на текущата парола чрез LINQ при опит за промяна
        if (!string.IsNullOrWhiteSpace(model.NewPassword))
        {
            // Проверяваме дали въведената стара парола съвпада напълно с тази в базата
            if (dbUser.Password != model.CurrentPassword)
            {
                ModelState.AddModelError(nameof(model.CurrentPassword), "⚠️ Въведената текуща парола е невалидна.");
            }
        }

        // Ако крос-валидацията от SettingsViewModel или LINQ проверката открие грешка:
        if (!ModelState.IsValid)
        {
            // Презареждаме списъка с теми, за да не се счупи падащото меню на екрана
            model.AvailableThemes = await _context.GlobalThemes.AsNoTracking().ToListAsync();
            return View("Settings", model);
        }

        // ЛОГИКА 2: Записване на промените на чисто в базата данни
        dbUser.Username = model.Username; // Обновяваме името на оператора (отключеното поле)
        dbUser.LayoutType = model.LayoutType;
        dbUser.IsMenuExpanded = model.IsMenuExpanded;
        dbUser.IsCustomPaletteActive = model.IsCustomPaletteActive;
        dbUser.IsDarkUserPreference = model.IsDarkUserPreference;

        // Застраховка на темите
        if (!model.IsCustomPaletteActive)
        {
            dbUser.ThemeId = 1;
        }
        else
        {
            dbUser.ThemeId = model.ThemeId;
        }

        // Ако LINQ проверката е преминала успешно и имаме нова валидирана парола, я запечатваме
        if (!string.IsNullOrWhiteSpace(model.NewPassword))
        {
            dbUser.Password = model.NewPassword;
        }

        // Записваме трансакцията финално в SQL Server
        await _context.SaveChangesAsync();

        // Презареждаме екрана чисто, за да светнат новите визуални настройки веднага
        return RedirectToAction("Settings");
    }
}
