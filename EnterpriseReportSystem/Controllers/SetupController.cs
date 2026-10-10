using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EnterpriseReportSystem.Data;
using EnterpriseReportSystem.Models.Entities;
using EnterpriseReportSystem.ViewModels; // Точният път до твоята папка
using System.Linq;
using System.Threading.Tasks;

using EnterpriseReportSystem.Common.Enum; // Adding the exact subfolder path


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

    // 3. GET: Зарежда празната HTML форма за Приход / Разход
    [HttpGet]
    public async Task<IActionResult> LogOperation()
    {
        var model = new AssetOperationViewModel();

        // Зареждаме само финансовите операции (Приход/Разход), които променят количествата
        var operationsFromDb = await _context.OperationTypes.ToListAsync();
        model.OperationOptions = operationsFromDb
            .Select(o => new SelectListItem { Value = o.Code, Text = o.Name })
            .ToList();

        return View(model);
    }

    // 4. POST: Записва трансакцията за Приход / Разход в базата данни
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LogOperation(AssetOperationViewModel model)
    {
        if (ModelState.IsValid)
        {
            var operation = await _context.OperationTypes.FirstOrDefaultAsync(o => o.Code == model.SelectedOperationCode);

            // Safely check the enum mapping from the correct namespace
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


// 5. GET: Loads the empty Logistic Transfer form
[HttpGet]
public IActionResult TransferAsset()
{
    return View(new AssetTransferViewModel());
}

// 6. POST: Processes the movement and updates the lifecycle status in SQL Server
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> TransferAsset(AssetTransferViewModel model)
{
    if (ModelState.IsValid)
    {
        // 1. Намираме специалната счетоводна операция за вътрешно преместване
        var transferOp = await _context.OperationTypes
            .FirstOrDefaultAsync(o => o.Code == "OP_AGE_TRANSFER" || o.Code == "OP_TRANSFER")
            ?? new OperationType { Code = "OP_TRANSFER", Name = "Вътрешно преместване", Direction = OperationDirection.Internal, IsFinancial = true };

        // 2. Създаваме запис в Журнала на трансакциите за историческа следа (Audit Trail)
        var transaction = new AssetTransaction
        {
            AssetId = model.AssetId,
            Quantity = 1, // Местим конкретния индивидуален актив
            FromFacility = "Предходен обект",
            ToFacility = model.NewFacility,
            TransactionDate = model.TransferDate,
            DocumentReference = model.DocumentReference,
            Operation = transferOp
        };
        _context.AssetTransactions.Add(transaction);

        // 3. АВТОМАТИЗАЦИЯ НА ЖИЗНЕНИЯ ЦИКЪЛ: Ако потребителят маркира обекта като ликвидиран/мъртъв (IsActive = false)
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
