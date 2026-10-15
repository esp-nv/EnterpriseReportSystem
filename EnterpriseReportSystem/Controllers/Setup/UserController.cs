using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EnterpriseReportSystem.Data;
using EnterpriseReportSystem.Models.Entities.Structure;

namespace EnterpriseReportSystem.Controllers.Setup;

public class UserController : Controller
{
    private readonly ApplicationDbContext _context;

    public UserController(ApplicationDbContext context)
    {
        _context = context;
    }
    [HttpGet]
    [Route("User/Settings")]
    public async Task<IActionResult> Settings()
    {
        // 1. Извличаме оператора заедно с неговата активна тема по релацията
        var user = await _context.SystemUsers
            .Include(u => u.GlobalTheme)
            .FirstOrDefaultAsync(u => u.Id == 1); // Тестов администратор #1

        if (user == null) return NotFound();

        // 2. Извличаме целия списък с теми от новата каталог-таблица
        ViewBag.GlobalThemes = await _context.GlobalThemes.AsNoTracking().ToListAsync();

        return View(user);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("User/SaveSettings")]
    public async Task<IActionResult> SaveSettings(SystemUser model)
    {
        // 1. Извличаме оригиналния потребител от базата данни, за да го модифицираме
        var dbUser = await _context.SystemUsers.FirstOrDefaultAsync(u => u.Id == model.Id);
        if (dbUser == null) return NotFound();

        // 2. СИГУРНО РЪЧНО ИЗВЛИЧАНЕ НА РАДИО БУТОНИТЕ ОТ ФОРМАТА
        // Четем суровите стойности "true"/"false" от браузъра и ги превръщаме в чист bool
        string customPaletteRaw = Request.Form["IsCustomPaletteActive"].ToString();
        string darkPreferenceRaw = Request.Form["IsDarkUserPreference"].ToString();

        bool isCustomPaletteActive = !string.IsNullOrEmpty(customPaletteRaw) && customPaletteRaw.ToLower() == "true";
        bool isDarkUserPreference = !string.IsNullOrEmpty(darkPreferenceRaw) && darkPreferenceRaw.ToLower() == "true";

        // 3. ЗАПЕЧАТВАНЕ НА ВСИЧКИ ПАРАМЕТРИ В SQL SERVER
        dbUser.Username = model.Username;
        dbUser.Password = model.Password;
        dbUser.LayoutType = model.LayoutType;
        dbUser.IsMenuExpanded = model.IsMenuExpanded;

        // Прехвърляме валидираните булеви радио флагове
        dbUser.IsCustomPaletteActive = isCustomPaletteActive;
        dbUser.IsDarkUserPreference = isDarkUserPreference;

        // Ако е избран фабричен Bootstrap режим, зануляваме ThemeId в базата данни
        if (!isCustomPaletteActive)
        {
            dbUser.ThemeId = null;
        }
        else
        {
            dbUser.ThemeId = model.ThemeId;
        }


        // Записваме трансакцията на чисто в SQL Server
        await _context.SaveChangesAsync();

        // 4. Мигновено връщане към страницата за настройки, за да се види визуалният ефект веднага
        return RedirectToAction("Settings");
    }

}
