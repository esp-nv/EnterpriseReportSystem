using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using EnterpriseReportSystem.Services.Interfaces;
using EnterpriseReportSystem.Models.Entities;
using EnterpriseReportSystem.Data;
using System.Linq;
using System.Collections.Generic;

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
    public IActionResult Index()
    {
        // Подготвяме падащите менюта директно от реалните SQL таблици
        ViewBag.Municipalities = _context.AssetTransactions
            .Select(t => t.ToFacility) // За пример използваме наличните обекти
            .Distinct()
            .Select(m => new SelectListItem { Value = m, Text = m })
            .ToList();

        ViewBag.Groups = _context.AssetGroups
            .Select(g => new SelectListItem { Value = g.Code, Text = g.Name })
            .ToList();

        // За старт подаваме празен списък, докато потребителят не натисне "Търси"
        var emptyRegistry = new List<ITrackableAsset>();
        return View(emptyRegistry);
    }
}
