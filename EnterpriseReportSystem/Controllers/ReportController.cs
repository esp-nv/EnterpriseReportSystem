using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EnterpriseReportSystem.Services.Interfaces;
using EnterpriseReportSystem.Data;

namespace EnterpriseReportSystem.Controllers;

public class ReportController : Controller
{
    private readonly IReportService _reportService;
    private readonly ApplicationDbContext _context;

    // Инжектираме услугата и контекста автоматично през DI
    public ReportController(IReportService reportService, ApplicationDbContext context)
    {
        _reportService = reportService;
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        // КОРЕКЦИЯ: Издърпваме обектите на фермите и взимаме правилно техните ИД и Имена
        var facilities = await _context.AssetTransactions
            .Select(t => t.ToFacility)
            .Where(f => f != null)
            .Distinct()
            .ToListAsync();

        ViewBag.Municipalities = facilities
            .Select(m => new SelectListItem { Value = m.Id.ToString(), Text = m.Name })
            .ToList();

        // Зареждаме и счетоводните групи чрез числовия им Id
        var groups = await _context.AssetGroups.ToListAsync();
        ViewBag.Groups = groups
            .Select(g => new SelectListItem { Value = g.Id.ToString(), Text = g.Name })
            .ToList();

        // За старт подаваме празен списък, докато потребителят не натисне "Търси"
        var emptyRegistry = new List<ITrackableAsset>();
        return View(emptyRegistry);
    }
}
