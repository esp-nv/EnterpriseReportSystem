using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EnterpriseReportSystem.Data;
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

    // 1. ИЗВЛИЧАНЕ НА НАСТРОЙКИТЕ И ФИЛТРИРАНЕ НА ТЕМИТЕ (GET)
    [HttpGet]
    [Route("User/Settings")]
    public async Task<IActionResult> Settings()
    {
        // Извличаме текущия оператор безопасно от базата данни
        var user = await _context.SystemUsers
            .FirstOrDefaultAsync(u => u.Id == 1); // Тестов администратор #1

        if (user == null) return NotFound();

        // Прочитаме всички налични външни теми от каталога
        var allThemes = await _context.GlobalThemes.AsNoTracking().ToListAsync();

        // Сглобяваме нашия сигурен ViewModel (точно по модела на StudentManagement)
        var viewModel = new SettingsViewModel
        {
            UserId = user.Id,
            Username = user.Username,
            Password = user.Password,
            LayoutType = user.LayoutType,
            IsMenuExpanded = user.IsMenuExpanded,
            IsCustomPaletteActive = user.IsCustomPaletteActive,
            ThemeId = user.ThemeId,
            IsDarkUserPreference = user.IsDarkUserPreference,

            // Предаваме пълния каталог към контейнера за уеб изгледа
            AvailableThemes = allThemes
        };

        return View(viewModel);
    }

    // 2. ЗАПЕЧАТВАНЕ И СТРОГА КРОС-ВАЛИДАЦИЯ НА ДАННИТЕ (POST)
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("User/SaveSettings")]
    public async Task<IActionResult> SaveSettings(SettingsViewModel model)
    {
        // Ако крос-валидацията в SettingsViewModel (IValidatableObject) открие несъответствие:
        if (!ModelState.IsValid)
        {
            // Презареждаме списъка с теми от базата, за да не се счупи падащото меню на екрана
            model.AvailableThemes = await _context.GlobalThemes.AsNoTracking().ToListAsync();
            return View("Settings", model);
        }

        // Извличаме оригиналния запис от SQL Server за модификация
        var dbUser = await _context.SystemUsers.FirstOrDefaultAsync(u => u.Id == model.UserId);
        if (dbUser == null) return NotFound();

        // Записваме променените параметри в релационния профил
        dbUser.Username = model.Username;
        dbUser.Password = model.Password;
        dbUser.LayoutType = model.LayoutType;
        dbUser.IsMenuExpanded = model.IsMenuExpanded;
        dbUser.IsCustomPaletteActive = model.IsCustomPaletteActive;
        dbUser.IsDarkUserPreference = model.IsDarkUserPreference;

        // Прилагаме бизнес защитата на приоритетите
        if (!model.IsCustomPaletteActive)
        {
            dbUser.ThemeId = 1; // Фабрично закована котва
        }
        else
        {
            dbUser.ThemeId = model.ThemeId;
        }

        // Запечатваме трансакцията на чисто в базата данни
        await _context.SaveChangesAsync();

        // Мигновено презареждаме екрана, за да може визуалният ефект да влезе в сила
        return RedirectToAction("Settings");
    }
}
