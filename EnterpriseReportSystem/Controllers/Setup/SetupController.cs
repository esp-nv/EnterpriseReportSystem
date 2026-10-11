using EnterpriseReportSystem.Common.Enum;
using EnterpriseReportSystem.Data;
using EnterpriseReportSystem.Models.Entities.Ledger;
using EnterpriseReportSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseReportSystem.Controllers.Setup; // СИНХРОНИЗИРАН NAMESPACE ПЪТ

public class SetupController : Controller
{
    private readonly ApplicationDbContext _context;

    public SetupController(ApplicationDbContext context)
    {
        _context = context;
    }

    // ==========================================
    // 2. УПРАВЛЕНИЕ НА АКТИВИТЕ (CreateAsset.cshtml)
    // ==========================================

    [HttpGet]
    public async Task<IActionResult> CreateAsset()
    {
        var model = new AssetManualEntryViewModel();
        var subGroups = await _context.AssetSubGroups.ToListAsync();

        // КОРЕКЦИЯ: Подаваме числовия Id като стринг за падащото меню
        model.SubGroupOptions = subGroups
            .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = $"{s.Code} - {s.Name}" })
            .ToList();

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateAsset(AssetManualEntryViewModel model)
    {
        if (ModelState.IsValid && int.TryParse(model.SelectedSubGroupCode, out int subGroupId))
        {
            var birthOp = await _context.OperationTypes.FirstOrDefaultAsync(o => o.Code == "OP_BUY")
                ?? new OperationType { Code = "OP_BUY", Name = "Покупка / Заприходяване", Direction = OperationDirection.Income, IsFinancial = true };

            if (birthOp.Id == 0) { _context.OperationTypes.Add(birthOp); await _context.SaveChangesAsync(); }

            var transaction = new AssetTransaction
            {
                AssetId = model.AssetId.Trim().ToUpper(),
                Quantity = model.Quantity,
                FromFacility = "Външен доставчик",
                ToFacility = "Първоначален обект",
                TransactionDate = model.AcquisitionDate,
                DocumentReference = string.IsNullOrEmpty(model.DocumentReference) ? "Първоначално завеждане" : model.DocumentReference.Trim(),
                Notes = model.Notes ?? "Заприходяване на нов оперативен актив",
                OperationId = birthOp.Id
            };

            _context.AssetTransactions.Add(transaction);
            await _context.SaveChangesAsync();

            TempData["AssetSuccess"] = $"🎉 Активът с Идентификатор '{model.AssetId}' е успешно заведен в SQL Server!";
            return RedirectToAction("CreateAsset");
        }
        return RedirectToAction("CreateAsset");
    }

    // ==========================================
    // 3. СЧЕТОВОДЕН ЖУРНАЛ (LogOperation.cshtml)
    // ==========================================

    [HttpGet]
    public async Task<IActionResult> LogOperation()
    {
        var model = new AssetOperationViewModel();
        var operationsFromDb = await _context.OperationTypes.ToListAsync();

        // КОРЕКЦИЯ: Падащото меню работи с числовия Id
        model.OperationOptions = operationsFromDb
            .Select(o => new SelectListItem { Value = o.Id.ToString(), Text = o.Name })
            .ToList();

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LogOperation(AssetOperationViewModel model)
    {
        if (ModelState.IsValid && int.TryParse(model.SelectedOperationCode, out int opId))
        {
            var operation = await _context.OperationTypes.FindAsync(opId);

            bool isExpense = operation != null && operation.Direction == OperationDirection.Expense;
            bool isIncome = operation != null && operation.Direction == OperationDirection.Income;

            var transaction = new AssetTransaction
            {
                AssetId = model.AssetId.Trim().ToUpper(),
                Quantity = model.Quantity,
                FromFacility = isExpense ? model.Facility : "Външен източник",
                ToFacility = isIncome ? model.Facility : "Изписване от обекта",
                TransactionDate = model.TransactionDate,
                DocumentReference = model.DocumentReference.Trim(),
                Notes = "Хронологична оперативна трансакция",
                OperationId = opId
            };

            _context.AssetTransactions.Add(transaction);
            await _context.SaveChangesAsync();

            TempData["OpSuccess"] = "🎉 Операцията е записана успешно в счетоводния Журнал на SQL Server!";
            return RedirectToAction("LogOperation");
        }
        return RedirectToAction("LogOperation");
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
                .FirstOrDefaultAsync(o => o.Code == "OP_TRANSFER")
                ?? new OperationType { Code = "OP_TRANSFER", Name = "Вътрешно преместване", Direction = OperationDirection.Internal, IsFinancial = true };

            if (transferOp.Id == 0) { _context.OperationTypes.Add(transferOp); await _context.SaveChangesAsync(); }

            var transaction = new AssetTransaction
            {
                AssetId = model.AssetId.Trim().ToUpper(),
                Quantity = 1,
                FromFacility = "Предходен обект",
                ToFacility = model.NewFacility.Trim(),
                TransactionDate = model.TransferDate,
                DocumentReference = model.DocumentReference.Trim(),
                Notes = "Автоматичен вътрешно-холдингов логистичен трансфер",
                OperationId = transferOp.Id
            };
            _context.AssetTransactions.Add(transaction);

            if (!model.IsActive)
            {
                var offOp = await _context.OperationTypes.FirstOrDefaultAsync(o => o.Code == "OP_WRITE_OFF")
                            ?? new OperationType { Code = "OP_WRITE_OFF", Name = "Брак / Смърт", Direction = OperationDirection.Expense, IsFinancial = true };

                if (offOp.Id == 0) { _context.OperationTypes.Add(offOp); await _context.SaveChangesAsync(); }

                var offTransaction = new AssetTransaction
                {
                    AssetId = model.AssetId.Trim().ToUpper(),
                    Quantity = 1,
                    FromFacility = model.NewFacility.Trim(),
                    ToFacility = "Бракуван / Отписан",
                    TransactionDate = model.TransferDate,
                    DocumentReference = $"Автоматично отписване: {model.DocumentReference}",
                    Notes = "Изписване на умряло или бракувано животно/актив",
                    OperationId = offOp.Id
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
