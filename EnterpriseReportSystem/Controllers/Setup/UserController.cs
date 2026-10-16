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
    // ПРЕМАХНАТ ЛОКАЛЕН ROUTE - СЕГА СЕ КОМАНДВА ОД ГЛОБАЛНИЯ НАД КЛАСА!
    public async Task<IActionResult> Settings()
    {
        var user = await _context.SystemUsers
            .Include(u => u.GlobalTheme)
            .FirstOrDefaultAsync(u => u.Id == 1);

        if (user == null) return NotFound();

        ViewBag.GlobalThemes = await _context.GlobalThemes.AsNoTracking().ToListAsync();

        return View(user);
    }

    // 2. ЗАПЕЧАТВАНЕ НА НАСТРОЙКИТЕ (POST)
    [HttpPost]
    [ValidateAntiForgeryToken]
    // ПРЕМАХНАТ ЛОКАЛЕН ROUTE - АВТОМАТИЧНО СТАВА User/SaveSettings!
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

        if (!isCustomPaletteActive)
        {
            dbUser.ThemeId = 1;
        }
        else
        {
            dbUser.ThemeId = model.ThemeId <= 0 ? 1 : model.ThemeId;
        }

        await _context.SaveChangesAsync();
        return RedirectToAction("Settings");
    }
}
