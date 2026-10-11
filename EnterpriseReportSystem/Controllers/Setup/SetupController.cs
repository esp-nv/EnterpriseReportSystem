using EnterpriseReportSystem.Common.Enum;
using EnterpriseReportSystem.Data;
using EnterpriseReportSystem.Models.Entities.Ledger;
using EnterpriseReportSystem.Services.Interfaces;
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
    private readonly IReportService _reportService;

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

        // 1. Зареждаме Ниво 1 (ДМА, ДНМА, Животни)
        var mainTypes = await _context.MainTypes.ToListAsync();
        model.MainTypeOptions = mainTypes
            .Select(m => new SelectListItem { Value = m.Id.ToString(), Text = m.Name })
            .ToList();

        // 2. Зареждаме Ниво 2 (Групи)
        var groups = await _context.AssetGroups.ToListAsync();
        model.GroupOptions = groups
            .Select(g => new SelectListItem { Value = g.Id.ToString(), Text = $"{g.Code} - {g.Name}" })
            .ToList();

        // 3. Зареждаме Валутите
        var currencies = await _context.Currencies.ToListAsync();
        model.CurrencyOptions = currencies
            .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Code })
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

            // ТВОЕТО ПЕРФЕКТНО ИМЕНУВАНО ИЗВИКВАНЕ БЕЗ НИТО ЕДНА ГРЕШКА
            await _reportService.CreateTransactionAsync(
                assetId: model.Id,
                quantity: model.InitialCount, // Твоето име за количество
                fromFacilityId: null,
                toFacilityId: 1,
                transactionDate: model.InvoiceOrPurchaseDate, // Твоето име за дата
                docRef: model.InvoiceReference, // Твоето име за документ
                notes: "Първоначално заприходяване на актив: " + model.Name,
                opId: birthOp.Id
            );

            TempData["AssetSuccess"] = "🎉 Новият актив е заведен успешно чрез именувани аргументи!";
            return RedirectToAction("CreateAsset");
        }
        return RedirectToAction("CreateAsset");
    }

}
