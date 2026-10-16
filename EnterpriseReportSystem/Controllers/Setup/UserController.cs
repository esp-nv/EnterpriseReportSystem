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
        var dbUser = await _context.SystemUsers.FirstOrDefaultAsync(u => u.Id == model.Id);
        if (dbUser == null) return NotFound();

        string customPaletteRaw = Request.Form["IsCustomPaletteActive"].ToString();
        string darkPreferenceRaw = Request.Form["IsDarkUserPreference"].ToString();

        bool isCustomPaletteActive = !string.IsNullOrEmpty(customPaletteRaw) && customPaletteRaw.ToLower() == "true";
        bool isDarkUserPreference = !string.IsNullOrEmpty(darkPreferenceRaw) && darkPreferenceRaw.ToLower() == "true";

        dbUser.Username = model.Username;
        dbUser.Password = model.Password;
        dbUser.LayoutType = model.LayoutType;
        dbUser.IsMenuExpanded = model.IsMenuExpanded;

        dbUser.IsCustomPaletteActive = isCustomPaletteActive;
        dbUser.IsDarkUserPreference = isDarkUserPreference;

        // ЖЕЛЯЗНА ЗАЩИТА: Ако не е активна външна палитра, твърдо заковаваме запис 1
        if (!isCustomPaletteActive)
        {
            dbUser.ThemeId = 1; // Твърда фабрична котва
        }
        else
        {
            // Ако се опитва да избере празно или невалидно ID за външна тема, пак връщаме 1
            dbUser.ThemeId = (model.ThemeId == 1 || model.ThemeId == null) ? 1 : model.ThemeId;
        }

        await _context.SaveChangesAsync();
        return RedirectToAction("Settings");
    }

}
