using Microsoft.AspNetCore.Mvc.Rendering;

namespace EnterpriseReportSystem.Models.ViewModels;

public class SmartImportViewModel
{
    // 1. СЕКЦИЯ: КАЧВАНЕ НА ВЪНШНИЯ ФАЙЛ
    public IFormFile? UploadedFile { get; set; }

    // 2. СЕКЦИЯ: ПОТРЕБИТЕЛСКИ РАЗДЕЛИТЕЛИ (С ПЪЛНА СВОБОДА)
    public string MainDelimiter { get; set; } = string.Empty;
    public string CustomMainDelimiter { get; set; } = string.Empty;

    public string InnerDelimiter { get; set; } = string.Empty;
    public string CustomInnerDelimiter { get; set; } = string.Empty;

    // 3. СЕКЦИЯ: СИГНАТУРИ И ПРАВИЛА (ОТКЛЮЧЕНА - БЕЗ ХАРДКОДНАТИ БГ/1000)
    // Потребителят има пълна свобода да въведе или да остави празно, ако файлът няма префикси
    public string AssetIdSignature { get; set; } = string.Empty;   // Свободен филтър: "Как започва Идентификаторът"
    public int? AssetIdRequiredLength { get; set; }                // Опционална дължина: Филтрира само ако потребителят въведе число

    public string ChipSignature { get; set; } = string.Empty;      // Свободен филтър: "Как започва Чипът"
    public int? ChipRequiredLength { get; set; }                   // Опционална дължина за електронния идентификатор

    // 4. СЕКЦИЯ: COLUMN MAPPING (Динамичен избор на колони)
    public int AssetIdColumnIndex { get; set; }
    public int ChipColumnIndex { get; set; }
    public int DateColumnIndex { get; set; }
    public int QuantityColumnIndex { get; set; }

    // 5. СЕКЦИЯ: ЧЕКБОКСОВЕ ЗА АКТИВНО СЛЕДЕНЕ
    public bool TrackAssetId { get; set; }
    public bool TrackChip { get; set; }
    public bool TrackDate { get; set; }
    public bool TrackQuantity { get; set; }

    // Опции за падащите менюта
    public List<SelectListItem> DelimiterOptions { get; set; } = new()
    {
        new() { Value = ";", Text = "Точка и запетая (;)" },
        new() { Value = ",", Text = "Запетая (,)" },
        new() { Value = " ", Text = "Интервал (Space)" },
        new() { Value = "\t", Text = "Табулация (Tab)" },
        new() { Value = "CUSTOM", Text = "✍️ Друг разделител (напиши сам...)" }
    };
}
