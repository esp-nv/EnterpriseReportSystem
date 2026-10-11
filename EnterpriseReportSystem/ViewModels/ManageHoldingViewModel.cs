using EnterpriseReportSystem.Models.Entities;
using EnterpriseReportSystem.Models.Entities.Structure;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace EnterpriseReportSystem.ViewModels;

public class ManageHoldingViewModel
{
    // Полета за създаване на нов Субект (Фирма / Лице)
    public string ClientCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string TaxIdentifier { get; set; } = string.Empty;
    public int RegisteredTownId { get; set; }

    // Списъци за динамично зареждане и визуализация в Dashboard-а
    public List<SelectListItem> TownOptions { get; set; } = new();
    public List<CompanyOrPerson> ExistingCompanies { get; set; } = new();
}
