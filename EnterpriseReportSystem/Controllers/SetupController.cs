using EnterpriseReportSystem.Common.Enum;
using EnterpriseReportSystem.Data;
using EnterpriseReportSystem.Models.Entities;
using EnterpriseReportSystem.Models.Entities.Hierarchy;
using EnterpriseReportSystem.Models.Entities.Ledger;
using EnterpriseReportSystem.Models.Entities.Structure;

using EnterpriseReportSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

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
            // 1. Намираме съответния Главен тип в SQL по неговия текстов код
            var parentMainType = await _context.MainTypes.FirstOrDefaultAsync(m => m.Code == model.SelectedMainTypeCode);
            if (parentMainType == null)
            {
                TempData["Error"] = "❌ Избраният Главен тип не съществува!";
                return RedirectToAction("Hierarchy");
            }

            // 2. Записваме новата група, като закачаме числовия му Идентификатор (Id)
            var group = new AssetGroup
            {
                Code = model.GroupCode.Trim().ToUpper(),
                Name = model.GroupName.Trim(),
                MainTypeId = parentMainType.Id // ЧИЛИЯ КЛЮЧ
            };
            _context.AssetGroups.Add(group);
            await _context.SaveChangesAsync();
            TempData["Success"] = $"🎉 Групата '{model.GroupName}' е записана успешно в SQL Server!";
        }
        else if (submitButton == "SubGroup" && !string.IsNullOrEmpty(model.SubGroupCode))
        {
            // 3. Намираме съответната Група в SQL по нейния текстов код
            var parentGroup = await _context.AssetGroups.FirstOrDefaultAsync(g => g.Code == model.SelectedGroupCode);
            if (parentGroup == null)
            {
                TempData["Error"] = "❌ Избраната Основна група не съществува!";
                return RedirectToAction("Hierarchy");
            }

            // 4. Записваме подгрупата, като закачаме числовия й Идентификатор (Id)
            var subGroup = new AssetSubGroup
            {
                Code = model.SubGroupCode.Trim().ToUpper(),
                Name = model.SubGroupName.Trim(),
                AssetGroupId = parentGroup.Id // ЧИСЛОВИЯ КЛЮЧ
            };
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

    // ==========================================
    // 5. УПРАВЛЕНИЕ НА ФИРМИ И ФИЗИЧЕСКИ ЛИЦА
    // ==========================================

    // GET: Зарежда Холдинговия Dashboard
    [HttpGet]
    public async Task<IActionResult> ManageHolding()
    {
        // Извличаме населените места чисто
        var townsList = await _context.TownsOrVillages.ToListAsync();

        var townOptions = townsList
            .Select(t => new SelectListItem
            {
                Value = t.Id.ToString(),
                Text = t.Name
            })
            .ToList();

        // Извличаме компаниите / лицата
        var companiesList = await _context.CompaniesOrPersons
            .Include(c => c.RegisteredTown)
            .ToListAsync();

        // Заменяме ViewBag с ViewData за 100% защита против CS8619
        ViewData["Towns"] = townOptions;
        ViewData["Companies"] = companiesList ?? new List<CompanyOrPerson>();

        return View(new CompanyOrPerson());
    }

    // POST: Записва нов субект (Фирма или Физическо лице) директно през Ентити модела
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateCompany(CompanyOrPerson model)
    {
        if (model != null && !string.IsNullOrEmpty(model.ClientCode))
        {
            var newEntity = new CompanyOrPerson
            {
                ClientCode = model.ClientCode.Trim().ToUpper(),
                Name = model.Name.Trim(),
                TaxIdentifier = model.TaxIdentifier.Trim(),
                RegisteredTownId = model.RegisteredTownId
            };

            _context.CompaniesOrPersons.Add(newEntity);
            await _context.SaveChangesAsync();
            TempData["HoldingSuccess"] = $"🎉 Субектът '{model.Name}' беше записан успешно!";
        }
        return RedirectToAction("ManageHolding");
    }

    // ==========================================
    // 6. УПРАВЛЕНИЕ НА ФИЗИЧЕСКИ ОБЕКТИ (ФЕРМИ / БАЗИ)
    // ==========================================

    // POST: Записва нов физически обект с автоматичен одитен дневник (Log)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateFacility(string facilityCode, string facilityName, string postalCode, int townId, int companyId, string notes)
    {
        if (!string.IsNullOrEmpty(facilityCode) && !string.IsNullOrEmpty(facilityName))
        {
            var facility = new Facility
            {
                FacilityCode = facilityCode.Trim().ToUpper(),
                Name = facilityName.Trim(),
                PostalCode = postalCode.Trim(),
                TownOrVillageId = townId,
                CompanyOrPersonId = companyId,
                IsActive = true,
                Notes = string.IsNullOrEmpty(notes) ? "Първоначално завеждане" : notes.Trim(),
                CreatedOn = DateTime.Now
            };

            _context.Facilities.Add(facility);
            await _context.SaveChangesAsync();

            // АВТОМАТИЗАЦИЯ НА ОДИТНИЯ ДНЕВНИК (FacilityLog)
            var auditLog = new FacilityLog
            {
                FacilityId = facility.Id,
                LogDate = DateTime.Now,
                EventType = "CREATED",
                OldValue = "[Няма - Нова регистрация]",
                NewValue = $"Код: {facility.FacilityCode}; Име: {facility.Name}; Активен: True"
            };
            _context.FacilityLogs.Add(auditLog);
            await _context.SaveChangesAsync();

            TempData["HoldingSuccess"] = $"🎉 Физическият обект '{facilityName}' беше заведен успешно заедно с неговия Одитен дневник!";
        }
        return RedirectToAction("ManageHolding");
    }


    // ==========================================
    // 7. ВАЛУТЕН КАЛКУЛАТОР - ХРОНОЛОГИЯ НА КУРСОВЕТЕ
    // ==========================================

    // GET: Подготвя данните за валутните курсове
    [HttpGet]
    public async Task<IActionResult> ManageRates()
    {
        var currencies = await _context.Currencies.ToListAsync();

        // Превръщаме списъка с валути в опции за падащи менюта в твой стил
        ViewData["Currencies"] = currencies
            .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = $"{c.Code} - {c.Name}" })
            .ToList();

        return View();
    }

    // POST: Записва нов хронологичен валутен курс с твоята дата-диапазон логика
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateExchangeRate(int fromCurrencyId, int toCurrencyId, decimal rate, DateTime rateDate, bool isFixed)
    {
        if (rate > 0 && fromCurrencyId != toCurrencyId)
        {
            var exchangeRate = new ExchangeRateHistory
            {
                FromCurrencyId = fromCurrencyId,
                ToCurrencyId = toCurrencyId,
                Rate = rate,
                RateDate = rateDate.Date, // Записваме само чистата дата за проверка по диапазон
                IsFixed = isFixed
            };

            _context.ExchangeRateHistory.Add(exchangeRate);
            await _context.SaveChangesAsync();

            TempData["RateSuccess"] = "🎉 Валутният курс е заведен успешно в хронологичния регистър на SQL Server!";
        }
        else
        {
            TempData["RateError"] = "❌ Невалидни валути или стойност на курса!";
        }

        return RedirectToAction("ManageRates");
    }

}
