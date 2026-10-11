using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EnterpriseReportSystem.ViewModels;

public class AssetManualEntryViewModel
{
    // СВОБОДНО ТЕКСТОВО ПОЛЕ (Ушна марка / Код)
    public string AssetId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    // ВЗАИМОСВЪРЗАНИ СЧЕТОВОДНИ СПИСЪЦИ
    public int SelectedMainTypeId { get; set; } // Ниво 1: ДМА, ДНМА, Животно, Машина
    public int SelectedGroupId { get; set; }     // Ниво 2: Група (напр. ЕРД, Трактори)
    public int SelectedSubGroupId { get; set; }  // Ниво 3: Подгрупа (напр. Крави, Комбайни)

    public string Municipality { get; set; } = string.Empty;
    public string TownOrVillage { get; set; } = string.Empty;
    public string Facility { get; set; } = string.Empty;

    public int InitialCount { get; set; } = 1;
    public decimal Price { get; set; }
    public int SelectedCurrencyId { get; set; }
    public decimal Weight { get; set; }

    public DateTime ManufactureOrBirthDate { get; set; } = DateTime.Now;
    public DateTime InvoiceOrPurchaseDate { get; set; } = DateTime.Now;
    public string InvoiceReference { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;

    // ОПЦИИ ЗА ПАДАЩИТЕ МЕНЮТА
    public List<SelectListItem> MainTypeOptions { get; set; } = new();
    public List<SelectListItem> GroupOptions { get; set; } = new();
    public List<SelectListItem> SubGroupOptions { get; set; } = new();
    public List<SelectListItem> CurrencyOptions { get; set; } = new();

    public int SelectedSpecificationId { get; set; } // Избраната Порода / Модел (Числов ключ)
    public List<SelectListItem> SpecificationOptions { get; set; } = new(); // Опции за екрана

}
