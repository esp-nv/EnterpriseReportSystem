using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EnterpriseReportSystem.Data;
using EnterpriseReportSystem.Models.Entities.Structure;

namespace EnterpriseReportSystem.Controllers.Setup;

[Route("User/[action]")]
public class UserController : Controller
{
    private readonly ApplicationDbContext _context;

    public UserController(ApplicationDbContext context)
    {
        _context = context;
    }

    // 1. ИЗВЛИЧАНЕ НА НАСТРОЙКИТЕ (GET)
    [HttpGet]
    [Route("User/Settings")]
    public async Task<IActionResult> Settings()
    {
        // Извличаме текущия оператор безопасно заедно с прикачената му тема
        var user = await _context.SystemUsers
            .Include(u => u.GlobalTheme)
            .FirstOrDefaultAsync(u => u.Id == 1); // Мапваме към тестовия администратор #1

        if (user == null) return NotFound();

        // Извличаме абсолютно всички готови теми от новата каталог-таблица
        ViewBag.GlobalThemes = await _context.GlobalThemes.AsNoTracking().ToListAsync();

        return View(user);
    }

    // 2. ЗАПЕЧАТВАНЕ НА НАСТРОЙКИТЕ (POST)
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("User/SaveSettings")]
    public async Task<IActionResult> SaveSettings(SystemUser model)
    {
        var dbUser = await _context.SystemUsers.FirstOrDefaultAsync(u => u.Id == model.Id);
        if (dbUser == null) return NotFound();

        // Ръчно извличаме суровите стойности "true"/"false" от уеб радио бутоните
        string customPaletteRaw = Request.Form["IsCustomPaletteActive"].ToString();
        string darkPreferenceRaw = Request.Form["IsDarkUserPreference"].ToString();

        bool isCustomPaletteActive = !string.IsNullOrEmpty(customPaletteRaw) && customPaletteRaw.ToLower() == "true";
        bool isDarkUserPreference = !string.IsNullOrEmpty(darkPreferenceRaw) && darkPreferenceRaw.ToLower() == "true";

        // Записваме базовите операторски параметри
        dbUser.Username = model.Username;
        dbUser.Password = model.Password;
        // Моделът автоматично ще обвърже и запечата булевия флаг IsMenuExpanded в базата
        dbUser.LayoutType = model.LayoutType;
        dbUser.IsMenuExpanded = model.IsMenuExpanded; // Запечатва True или False на чисто


        // Прехвърляме валидираните булеви радио флагове към базата данни
        dbUser.IsCustomPaletteActive = isCustomPaletteActive;
        dbUser.IsDarkUserPreference = isDarkUserPreference;

        // ЖЕЛЯЗНА ЗАЩИТА ЗА ТЕМИТЕ:
        if (!isCustomPaletteActive)
        {
            // Ако е избран фабричен Bootstrap режим (0) -> Заковаваме строго тема 1
            dbUser.ThemeId = 1;
        }
        else
        {
            // Ако се опитва да запише невалидно ID, автоматично връщаме фабричната котва 1
            dbUser.ThemeId = model.ThemeId <= 0 ? 1 : model.ThemeId;
        }

        // Записваме трансакцията на чисто в SQL Server
        await _context.SaveChangesAsync();

        // Мигновено презареждаме екрана, за да може визуалните промени да светнат веднага!
        return RedirectToAction("Settings");
    }
}
