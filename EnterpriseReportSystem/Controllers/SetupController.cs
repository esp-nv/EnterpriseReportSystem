using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EnterpriseReportSystem.Data;
using EnterpriseReportSystem.Models.Entities;
using EnterpriseReportSystem.ViewModels;
using EnterpriseReportSystem.Common.Enum;
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

    // 1. GET: Зарежда екрана на адрес /Setup/Hierarchy
    [HttpGet]
    public async Task<IActionResult> Hierarchy()
    {
        await PopulateHierarchyListsAsync();
        return View(new HierarchyCreateViewModel());
    }

    // 2. POST: Поема записите на СЪЩИЯ адрес /Setup/Hierarchy (Елиминира 404 грешките напълно)
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
        var operationsFromDb = await _context.OperationTypes.ToListAsync();
        model.OperationOptions = operationsFromDb.Select(o => new SelectListItem { Value = o.Code, Text = o.Name }).ToList();
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

    // ==========================================
    // 3. РЕДАКЦИЯ И ИЗТРИВАНЕ НА ЙЕРАРХИЯ
    // ==========================================

    // GET: Зарежда формата за редакция на Главен Тип
    [HttpGet]
    public async Task<IActionResult> EditMainType(string id)
    {
        var mainType = await _context.MainTypes.FirstOrDefaultAsync(m => m.Code == id);
        if (mainType == null) return NotFound();

        var model = new HierarchyCreateViewModel
        {
            MainTypeCode = mainType.Code,
            MainTypeName = mainType.Name
        };
        return View(model);
    }

    // POST: Записва промените за Главен Тип
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditMainType(HierarchyCreateViewModel model)
    {
        var mainType = await _context.MainTypes.FirstOrDefaultAsync(m => m.Code == model.MainTypeCode);
        if (mainType != null)
        {
            mainType.Name = model.MainTypeName.Trim();
            await _context.SaveChangesAsync();
            TempData["Success"] = $"✏️ Главният тип '{model.MainTypeName}' беше редактиран успешно!";
        }
        return RedirectToAction("Hierarchy");
    }

    // POST: Изтриване на Главен Тип (Съобразено с твоята Restrict защита)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteMainType(string id)
    {
        var mainType = await _context.MainTypes.Include(m => m.AssetGroups).FirstOrDefaultAsync(m => m.Code == id);
        if (mainType == null) return NotFound();

        if (mainType.AssetGroups.Any())
        {
            TempData["Error"] = $"❌ Не можете да изтриете този тип, защото към него има свързани Групи активи!";
            return RedirectToAction("Hierarchy");
        }

        _context.MainTypes.Remove(mainType);
        await _context.SaveChangesAsync();
        TempData["Success"] = "🗑️ Главният тип беше изтрит успешно от SQL Server!";
        return RedirectToAction("Hierarchy");
    }

    // ==========================================
    // РЕДАКЦИЯ И ИЗТРИВАНЕ НА ГРУПИ (НИВО 2)
    // ==========================================

    [HttpGet]
    public async Task<IActionResult> EditGroup(string id)
    {
        var group = await _context.AssetGroups.FirstOrDefaultAsync(g => g.Code == id);
        if (group == null) return NotFound();

        var mainTypes = await _context.MainTypes.ToListAsync();
        ViewBag.MainTypes = mainTypes.Select(m => new SelectListItem { Value = m.Code, Text = m.Name }).ToList();

        var model = new HierarchyCreateViewModel
        {
            GroupCode = group.Code,
            GroupName = group.Name,
            SelectedMainTypeCode = group.MainTypeCode
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditGroup(HierarchyCreateViewModel model)
    {
        var group = await _context.AssetGroups.FirstOrDefaultAsync(g => g.Code == model.GroupCode);
        if (group != null)
        {
            group.Name = model.GroupName.Trim();
            group.MainTypeCode = model.SelectedMainTypeCode;
            await _context.SaveChangesAsync();
            TempData["Success"] = $"✏️ Групата '{model.GroupName}' беше редактирана успешно!";
        }
        return RedirectToAction("Hierarchy");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteGroup(string id)
    {
        var group = await _context.AssetGroups.Include(g => g.AssetSubGroups).FirstOrDefaultAsync(g => g.Code == id);
        if (group == null) return NotFound();

        if (group.AssetSubGroups.Any())
        {
            TempData["Error"] = $"❌ Не можете да изтриете тази Група, защото към нея има свързани Подгрупи активи!";
            return RedirectToAction("Hierarchy");
        }

        _context.AssetGroups.Remove(group);
        await _context.SaveChangesAsync();
        TempData["Success"] = "🗑️ Счетоводната група беше изтрита успешно от SQL Server!";
        return RedirectToAction("Hierarchy");
    }

    // ==========================================
    // РЕДАКЦИЯ И ИЗТРИВАНЕ НА ПОДГРУПИ (НИВО 3)
    // ==========================================

    [HttpGet]
    public async Task<IActionResult> EditSubGroup(string id)
    {
        var subGroup = await _context.AssetSubGroups.FirstOrDefaultAsync(s => s.Code == id);
        if (subGroup == null) return NotFound();

        var groups = await _context.AssetGroups.ToListAsync();
        ViewBag.Groups = groups.Select(g => new SelectListItem { Value = g.Code, Text = g.Name }).ToList();

        var model = new HierarchyCreateViewModel
        {
            SubGroupCode = subGroup.Code,
            SubGroupName = subGroup.Name,
            SelectedGroupCode = subGroup.AssetGroupCode
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditSubGroup(HierarchyCreateViewModel model)
    {
        var subGroup = await _context.AssetSubGroups.FirstOrDefaultAsync(s => s.Code == model.SubGroupCode);
        if (subGroup != null)
        {
            subGroup.Name = model.SubGroupName.Trim();
            subGroup.AssetGroupCode = model.SelectedGroupCode;
            await _context.SaveChangesAsync();
            TempData["Success"] = $"✏️ Подгрупата '{model.SubGroupName}' беше редактирана успешно!";
        }
        return RedirectToAction("Hierarchy");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteSubGroup(string id)
    {
        var subGroup = await _context.AssetSubGroups.FirstOrDefaultAsync(s => s.Code == id);
        if (subGroup == null) return NotFound();

        // Забележка: Тук при реално въведени активи Entity Framework автоматично 
        // ще хвърли изключение поради Restrict, но ние пречистваме контрола
        _context.AssetSubGroups.Remove(subGroup);
        await _context.SaveChangesAsync();
        TempData["Success"] = "🗑️ Оперативната подгрупа беше изтрита успешно!";
        return RedirectToAction("Hierarchy");
    }


}
