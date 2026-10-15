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
    public async Task<IActionResult> Settings()
    {
        // 1. Извличаме текущия оператор безопасно заедно с прикачената му тема
        var user = await _context.SystemUsers
            .Include(u => u.GlobalTheme)
            .FirstOrDefaultAsync(u => u.Id == 1); // Мапваме към тестовия администратор #1

        if (user == null) return NotFound();

        // 2. Извличаме динамично всички достъпни теми от новата каталог-таблица
        ViewBag.GlobalThemes = await _context.GlobalThemes.ToListAsync();

        return View(user);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveSettings(SystemUser model)
    {
        var dbUser = await _context.SystemUsers.FirstOrDefaultAsync(u => u.Id == model.Id);
        if (dbUser == null) return NotFound();

        // Запечатваме новите релационни флагове директно в SQL Server
        dbUser.LayoutType = model.LayoutType;
        dbUser.IsMenuExpanded = model.IsMenuExpanded;
        dbUser.ThemeId = model.ThemeId;
        dbUser.IsCustomPaletteActive = model.IsCustomPaletteActive;

        await _context.SaveChangesAsync();

        // Връщаме оператора към главното табло след успешна трансакция
        return RedirectToAction("Index", "Home");
    }
}
