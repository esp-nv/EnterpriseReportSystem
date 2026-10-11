using EnterpriseReportSystem.Common.Enum;
using EnterpriseReportSystem.Data;
using EnterpriseReportSystem.Models.Entities.Hierarchy;
using EnterpriseReportSystem.Models.Entities.Ledger;
using EnterpriseReportSystem.Services.Interfaces; // Импортираме твоите интерфейси
using EnterpriseReportSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseReportSystem.Controllers.Setup;

public class SetupController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IReportService _reportService; // ТВОЯТ СЕРВИЗЕН СЛОЙ ОТ STUDENT MANAGEMENT

    public SetupController(ApplicationDbContext context, IReportService reportService)
    {
        _context = context;
        _reportService = reportService;
    }

    // ==========================================
    // 2. УПРАВЛЕНИЕ НА АКТИВИТЕ (CreateAsset.cshtml)
    // ==========================================

    [HttpGet]
    public async Task<IActionResult> CreateAsset()
    {
        var model = new AssetManualEntryViewModel();
        var subGroups = await _context.AssetSubGroups.ToListAsync();
        
        model.SubGroupOptions = subGroups
            .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = $"{s.Code} - {s.Name}" })
            .ToList();
            
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateAsset(AssetManualEntryViewModel model)
    {
        if (ModelState.IsValid)
        {
            var birthOp = await _context.OperationTypes.FirstOrDefaultAsync(o => o.Code == "OP_BUY") 
                ?? new OperationType { Code = "OP_BUY", Name = "Покупка / Заприходяване", Direction = OperationDirection.Income, IsFinancial = true };

            if (birthOp.Id == 0) { _context.OperationTypes.Add(birthOp); await _context.SaveChangesAsync(); }

            // ИЗВИКВАМЕ ЧИСТИЯ СЕРВИЗЕН МЕТОД (КАКТО В STUDENT MANAGEMENT!)
            // Извикваме метода директно по ред на променливите, без назоваване (Изчиства грешката!)
            await _reportService.CreateTransactionAsync(
                1,
                model.Quantity,
                null,
                1,
                model.AcquisitionDate,
                model.DocumentReference,
                model.Notes,
                birthOp.Id
            );


            TempData["AssetSuccess"] = "🎉 Новият актив е успешно заведен през сервизния слой!";
            return RedirectToAction("CreateAsset");
        }
        return RedirectToAction("CreateAsset");
    }
}
