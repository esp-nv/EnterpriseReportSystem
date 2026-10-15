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
        // 1. Взимаме първия потребител за тест или текущо логнатия
        var user = await _context.SystemUsers
            .Include(u => u.GlobalTheme)
            .FirstOrDefaultAsync();

        if (user == null) return NotFound();

        // 2. Наливаме списъка с всички достъпни теми в уеб каталога
        ViewBag.GlobalThemes = await _context.GlobalThemes.ToListAsync();

        return View(user);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveSettings(SystemUser model)
    {
        var dbUser = await _context.SystemUsers.FirstOrDefaultAsync(u => u.Id == model.Id);
        if (dbUser == null) return NotFound();

        // Обновяваме само трите чисти параметъра по новия ни план
        dbUser.LayoutType = model.LayoutType;
        dbUser.IsMenuExpanded = model.IsMenuExpanded;
        dbUser.ThemeId = model.ThemeId;
        dbUser.IsCustomPaletteActive = model.IsCustomPaletteActive;

        await _context.SaveChangesAsync();
        return RedirectToAction("Index", "Home");
    }
}
