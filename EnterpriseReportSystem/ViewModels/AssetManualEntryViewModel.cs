using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EnterpriseReportSystem.ViewModels;

public class AssetManualEntryViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string SelectedSubGroupCode { get; set; } = string.Empty;

    public string Municipality { get; set; } = string.Empty;
    public string TownOrVillage { get; set; } = string.Empty;
    public string Facility { get; set; } = string.Empty;

    // ХОЛДИНГОВИ КОЛИЧЕСТВЕНИ И ФИНАНСОВИ ПОЛЕТА
    public int InitialCount { get; set; } = 1;
    public decimal Price { get; set; } // НОВА ЕДИНИЧНА ЦЕНА
    public int SelectedCurrencyId { get; set; } // НОВА ИЗБРАНА ВАЛУТА
    public decimal Weight { get; set; } // НОВО ТЕГЛО (КГ)

    // Времевите счетоводни индикатори
    public DateTime ManufactureOrBirthDate { get; set; } = DateTime.Now;
    public DateTime InvoiceOrPurchaseDate { get; set; } = DateTime.Now;
    public string InvoiceReference { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty; // БЕЛЕЖКИ И КООРДИНАТИ

    public List<SelectListItem> SubGroupOptions { get; set; } = new();
    public List<SelectListItem> CurrencyOptions { get; set; } = new(); // НОВИ ВАЛУТНИ ОПЦИИ
}
