using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EnterpriseReportSystem.ViewModels;

public class AssetOperationViewModel
{
    [Required(ErrorMessage = "Идентификаторът (Ушна марка / Сериен номер) е задължителен!")]
    public string AssetId { get; set; } = string.Empty;

    [Required(ErrorMessage = "Моля, изберете конкретен тип операция!")]
    public string SelectedOperationCode { get; set; } = string.Empty; // напр. OP_BIRTH, OP_SALE

    [Range(1, int.MaxValue, ErrorMessage = "Количеството трябва да бъде поне 1!")]
    public int Quantity { get; set; } = 1;

    [Required(ErrorMessage = "Моля, въведете дата на събитието!")]
    public DateTime TransactionDate { get; set; } = DateTime.Now;

    [Required(ErrorMessage = "Основанието / Документът е задължителен за одита!")]
    public string DocumentReference { get; set; } = string.Empty; // напр. Акт за раждане №5, Фактура №102

    // Локация, в която се извършва операцията
    public string Facility { get; set; } = string.Empty;

    // Списък за динамично зареждане на падащото меню с операции от SQL Server
    public List<SelectListItem> OperationOptions { get; set; } = new();
}
