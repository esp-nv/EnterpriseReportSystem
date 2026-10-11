using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EnterpriseReportSystem.Data;
using EnterpriseReportSystem.ViewModels;
using EnterpriseReportSystem.Common.Enum;
using EnterpriseReportSystem.Models.Entities.Hierarchy;
using EnterpriseReportSystem.Models.Entities.Location;
using EnterpriseReportSystem.Models.Entities.Structure;
using EnterpriseReportSystem.Models.Entities.Ledger;
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

    // ==========================================
    // 1. УПРАВЛЕНИЕ НА ЙЕРАРХИЯТА (Hierarchy.cshtml)
    // ==========================================

    [HttpGet]
    public async Task<IActionResult> Hierarchy()
    {
        await PopulateHierarchyListsAsync();
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
            TempData["Success"] = $"🎉 Главният тип '{model.MainTypeName}' е записан успешно в SQL Server!";
        }
        else if (submitButton == "Group" && !string.IsNullOrEmpty(model.GroupCode))
        {
            var group = new AssetGroup { Code = model.GroupCode.Trim().ToUpper(), Name = model.GroupName.Trim(), MainTypeCode = model.SelectedMainTypeCode };
            _context.AssetGroups.Add(group);
            await _context.SaveChangesAsync();
            TempData["Success"] = $"🎉 Групата '{model.GroupName}' е записана успешно в SQL Server!";
        }
        else if (submitButton == "SubGroup" && !string.IsNullOrEmpty(model.SubGroupCode))
        {
            var subGroup = new AssetSubGroup { Code = model.SubGroupCode.Trim().ToUpper(), Name = model.SubGroupName.Trim(), AssetGroupCode = model.SelectedGroupCode };
            _context.AssetSubGroups.Add(subGroup);
            await _context.SaveChangesAsync();
            TempData["Success"] = $"🎉 Подгрупата '{model.SubGroupName}' е записана успешно в SQL Server!";
        }

        return RedirectToAction("Hierarchy");
    }

    private async Task PopulateHierarchyListsAsync()
    {
        var mainTypes = await _context.MainTypes.ToListAsync();
        var groups = await _context.AssetGroups.ToListAsync();

        ViewBag.MainTypes = mainTypes.Select(m => new SelectListItem { Value = m.Code, Text = m.Name }).ToList();
        ViewBag.Groups = groups.Select(g => new SelectListItem { Value = g.Code, Text = g.Name }).ToList();
    }

    // ==========================================
    // 2. УПРАВЛЕНИЕ НА АКТИВИТЕ (CreateAsset.cshtml)
    // ==========================================

    [HttpGet]
    public async Task<IActionResult> CreateAsset()
    {
        var model = new AssetManualEntryViewModel();
        var subGroups = await _context.AssetSubGroups.ToListAsync();
        model.SubGroupOptions = subGroups.Select(s => new SelectListItem { Value = s.Code, Text = s.Name }).ToList();
        return View(model);
    }

    // ==========================================
    // 3. СЧЕТОВОДЕН ЖУРНАЛ (LogOperation.cshtml)
    // ==========================================

    [HttpGet]
    public async Task<IActionResult> LogOperation()
    {
        var model = new AssetOperationViewModel();
        var operationsFromDb = await _context.OperationTypes.ToListAsync();
        model.OperationOptions = operationsFromDb.Select(o => new SelectListItem { Value = o.Code, Text = o.Name }).ToList();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LogOperation(AssetOperationViewModel model)
    {
        if (ModelState.IsValid)
        {
            var operation = await _context.OperationTypes.FirstOrDefaultAsync(o => o.Code == model.SelectedOperationCode);

            bool isExpense = operation != null && operation.Direction == OperationDirection.Expense;
            bool isIncome = operation != null && operation.Direction == OperationDirection.Income;

            var transaction = new AssetTransaction
            {
                AssetId = model.AssetId,
                Quantity = model.Quantity,
                FromFacility = isExpense ? model.Facility : "Външен източник",
                ToFacility = isIncome ? model.Facility : "Изписване от обекта",
                TransactionDate = model.TransactionDate,
                DocumentReference = model.DocumentReference,
                Operation = operation ?? new()
            };

            _context.AssetTransactions.Add(transaction);
            await _context.SaveChangesAsync();

            TempData["OpSuccess"] = "🎉 Операцията е записана успешно в счетоводния Журнал на SQL Server!";
            return RedirectToAction("LogOperation");
        }

        var operations = await _context.OperationTypes.ToListAsync();
        model.OperationOptions = operations.Select(o => new SelectListItem { Value = o.Code, Text = o.Name }).ToList();
        return View(model);
    }

    // ==========================================
    // 4. ЛОГИСТИЧЕН ТРАНСФЕР (TransferAsset.cshtml)
    // ==========================================

    [HttpGet]
    public IActionResult TransferAsset()
    {
        return View(new AssetTransferViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TransferAsset(AssetTransferViewModel model)
    {
        if (ModelState.IsValid)
        {
            var transferOp = await _context.OperationTypes
                .FirstOrDefaultAsync(o => o.Code == "OP_AGE_TRANSFER" || o.Code == "OP_TRANSFER")
                ?? new OperationType { Code = "OP_TRANSFER", Name = "Вътрешно преместване", Direction = OperationDirection.Internal, IsFinancial = true };

            var transaction = new AssetTransaction
            {
                AssetId = model.AssetId,
                Quantity = 1,
                FromFacility = "Предходен обект",
                ToFacility = model.NewFacility,
                TransactionDate = model.TransferDate,
                DocumentReference = model.DocumentReference,
                Operation = transferOp
            };
            _context.AssetTransactions.Add(transaction);

            if (!model.IsActive)
            {
                var offOp = await _context.OperationTypes.FirstOrDefaultAsync(o => o.Code == "OP_WRITE_OFF")
                            ?? new OperationType { Code = "OP_WRITE_OFF", Name = "Брак / Смърт", Direction = OperationDirection.Expense, IsFinancial = true };

                var offTransaction = new AssetTransaction
                {
                    AssetId = model.AssetId,
                    Quantity = 1,
                    FromFacility = model.NewFacility,
                    ToFacility = "Бракуван / Отписан",
                    TransactionDate = model.TransferDate,
                    DocumentReference = $"Автоматично отписване поради ликвидация: {model.DocumentReference}",
                    Operation = offOp
                };
                _context.AssetTransactions.Add(offTransaction);
            }

            await _context.SaveChangesAsync();

            TempData["TransferSuccess"] = "🎉 Логистичното преместване и статусът бяха актуализирани успешно в SQL Server!";
            return RedirectToAction("TransferAsset");
        }

        return View(model);
    }
}
