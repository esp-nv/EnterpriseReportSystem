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

        // 1. Зареждаме подгрупите
        var subGroups = await _context.AssetSubGroups.ToListAsync();
        model.SubGroupOptions = subGroups
            .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = $"{s.Code} - {s.Name}" })
            .ToList();

        // 2. ФИНАНСОВА НАСТРОЙКА: Зареждаме реалните валути от SQL Server
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
