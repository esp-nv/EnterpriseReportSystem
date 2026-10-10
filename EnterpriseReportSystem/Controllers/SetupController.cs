using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EnterpriseReportSystem.Data;
using EnterpriseReportSystem.Models.Entities;
using EnterpriseReportSystem.ViewModels; // Точният път до твоята папка
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseReportSystem.Controllers;

public class SetupController : Controller
{
    private readonly ApplicationDbContext _context;

    public SetupController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Hierarchy()
    {
        var mainTypes = await _context.MainTypes.ToListAsync();
        var groups = await _context.AssetGroups.ToListAsync();

        ViewBag.MainTypes = mainTypes.Select(m => new SelectListItem { Value = m.Code, Text = m.Name }).ToList();
        ViewBag.Groups = groups.Select(g => new SelectListItem { Value = g.Code, Text = g.Name }).ToList();

        return View(new HierarchyCreateViewModel());
    }

    [HttpGet]
    public async Task<IActionResult> CreateAsset()
    {
        var model = new AssetManualEntryViewModel();
        var subGroups = await _context.AssetSubGroups.ToListAsync();
        model.SubGroupOptions = subGroups.Select(s => new SelectListItem { Value = s.Code, Text = s.Name }).ToList();
        return View(model);
    }
}
