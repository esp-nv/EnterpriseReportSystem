using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EnterpriseReportSystem.Data;
using EnterpriseReportSystem.Models.Entities.Structure;

namespace EnterpriseReportSystem.Controllers.Setup; // СИНХРОНИЗИРАН NAMESPACE ПЪТ

public class HoldingController : Controller
{
    private readonly ApplicationDbContext _context;

    public HoldingController(ApplicationDbContext context)
    {
        _context = context;
    }

    // ==========================================
    // 5. УПРАВЛЕНИЕ НА ФИРМИ И ОБЕКТИ
    // ==========================================

    [HttpGet]
    public async Task<IActionResult> ManageHolding()
    {
        var townsList = await _context.TownsOrVillages.ToListAsync();
        var companiesList = await _context.CompaniesOrPersons.Include(c => c.RegisteredTown).ToListAsync();

        ViewData["Towns"] = townsList.Select(t => new SelectListItem { Value = t.Id.ToString(), Text = t.Name }).ToList();
        ViewData["Companies"] = companiesList ?? new List<CompanyOrPerson>();

        return View(new CompanyOrPerson());
    }

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
                RegisteredTownId = model.RegisteredTownId,
                // Защитаваме задължителното поле за адрес
                Address = string.IsNullOrEmpty(model.Address) ? "Административно седалище" : model.Address.Trim()
            };

            _context.CompaniesOrPersons.Add(newEntity);
            await _context.SaveChangesAsync();
            TempData["HoldingSuccess"] = $"🎉 Субектът '{model.Name}' беше записан успешно!";
        }
        return RedirectToAction("ManageHolding");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateFacility(string facilityCode, string facilityName, string postalCode, int townId, int companyId, string notes, string address)
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
                Notes = string.IsNullOrEmpty(notes) ? "Първоначално заприходяване" : notes.Trim(),
                CreatedOn = DateTime.Now,
                // Защитаваме задължителното локационно поле за адрес
                Address = string.IsNullOrEmpty(address) ? "Стопански двор / Основна база" : address.Trim()
            };

            _context.Facilities.Add(facility);
            await _context.SaveChangesAsync();

            var auditLog = new FacilityLog
            {
                FacilityId = facility.Id,
                LogDate = DateTime.Now,
                EventType = "CREATED",
                OldValue = "[Няма - Нова регистрация]",
                NewValue = $"Код: {facility.FacilityCode}; Име: {facility.Name}; Адрес: {facility.Address}"
            };
            _context.FacilityLogs.Add(auditLog);
            await _context.SaveChangesAsync();

            TempData["HoldingSuccess"] = $"🎉 Физическият обект '{facilityName}' беше заведен успешно!";
        }
        return RedirectToAction("ManageHolding");
    }
}
