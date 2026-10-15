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
        // Взимаме потребителя чисто, без връзки към несъществуващи обекти
        var user = await _context.SystemUsers.FirstOrDefaultAsync();
        if (user == null) return NotFound();

        // Подаваме празен списък, за да зареди изгледа безопасно преди миграцията
        ViewBag.GlobalThemes = new List<GlobalTheme>();

        return View(user);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveSettings(SystemUser model)
    {
        var dbUser = await _context.SystemUsers.FirstOrDefaultAsync(u => u.Id == model.Id);
        if (dbUser == null) return NotFound();

        dbUser.LayoutType = model.LayoutType;
        dbUser.IsMenuExpanded = model.IsMenuExpanded;
        dbUser.ThemeId = model.ThemeId;
        dbUser.IsCustomPaletteActive = model.IsCustomPaletteActive;

        await _context.SaveChangesAsync();
        return RedirectToAction("Index", "Home");
    }
}
