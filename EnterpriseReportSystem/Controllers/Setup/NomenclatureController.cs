using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EnterpriseReportSystem.Data;
using EnterpriseReportSystem.Models.Entities.Hierarchy;
using EnterpriseReportSystem.ViewModels;

namespace EnterpriseReportSystem.Controllers.Setup; // СИНХРОНИЗИРАН NAMESPACE ПЪТ

public class NomenclatureController : Controller
{
    private readonly ApplicationDbContext _context;

    public NomenclatureController(ApplicationDbContext context)
    {
        _context = context;
    }

    // ==========================================
    // 1. УПРАВЛЕНИЕ НА ЙЕРАРХИЯТА (Hierarchy.cshtml)
    // ==========================================

    [HttpGet]
    public async Task<IActionResult> Hierarchy()
    {
        var mainTypes = await _context.MainTypes.ToListAsync();
        var groups = await _context.AssetGroups.ToListAsync();

        ViewData["MainTypes"] = mainTypes.Select(m => new SelectListItem { Value = m.Id.ToString(), Text = m.Name }).ToList();
        ViewData["Groups"] = groups.Select(g => new SelectListItem { Value = g.Id.ToString(), Text = g.Name }).ToList();

        return View(new HierarchyCreateViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Hierarchy(HierarchyCreateViewModel model, string submitButton)
    {
        if (submitButton == "MainType" && !string.IsNullOrEmpty(model.MainTypeCode))
        {
            var mainType = new MainType { Code = model.MainTypeCode.Trim().ToUpper(), Name = model.MainTypeName.Trim() };
            _context.MainTypes.Add(mainType);
            await _context.SaveChangesAsync();
            TempData["Success"] = $"🎉 Главният тип '{model.MainTypeName}' е записан успешно!";
        }
        else if (submitButton == "Group" && !string.IsNullOrEmpty(model.GroupCode))
        {
            if (int.TryParse(model.SelectedMainTypeCode, out int mainTypeId))
            {
                var group = new AssetGroup { Code = model.GroupCode.Trim().ToUpper(), Name = model.GroupName.Trim(), MainTypeId = mainTypeId };
                _context.AssetGroups.Add(group);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"🎉 Групата '{model.GroupName}' е записана успешно!";
            }
        }
        else if (submitButton == "SubGroup" && !string.IsNullOrEmpty(model.SubGroupCode))
        {
            if (int.TryParse(model.SelectedGroupCode, out int groupId))
            {
                var subGroup = new AssetSubGroup { Code = model.SubGroupCode.Trim().ToUpper(), Name = model.SubGroupName.Trim(), AssetGroupId = groupId, MeasurementUnit = "бр." };
                _context.AssetSubGroups.Add(subGroup);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"🎉 Подгрупата '{model.SubGroupName}' е записана успешно!";
            }
        }
        return RedirectToAction("Hierarchy");
    }
}
