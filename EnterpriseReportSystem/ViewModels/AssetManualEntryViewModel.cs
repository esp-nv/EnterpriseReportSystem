using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EnterpriseReportSystem.ViewModels; // Коренната папка ViewModels

public class AssetManualEntryViewModel
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string SelectedSubGroupCode { get; set; } = string.Empty;

    public string Municipality { get; set; } = string.Empty;
    public string TownOrVillage { get; set; } = string.Empty;
    public string Facility { get; set; } = string.Empty;

    public int InitialCount { get; set; } = 1;

    // Времевите счетоводни индикатори
    public DateTime ManufactureOrBirthDate { get; set; } = DateTime.Now;
    public DateTime InvoiceOrPurchaseDate { get; set; } = DateTime.Now;
    public string InvoiceReference { get; set; } = string.Empty;

    public List<SelectListItem> SubGroupOptions { get; set; } = new();
}
