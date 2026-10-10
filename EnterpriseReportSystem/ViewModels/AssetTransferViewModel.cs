using System;
using System.ComponentModel.DataAnnotations;

namespace EnterpriseReportSystem.ViewModels;

public class AssetTransferViewModel
{
    [Required(ErrorMessage = "Идентификаторът (Ушна марка / Сериен номер) е задължителен!")]
    public string AssetId { get; set; } = string.Empty;

    // Счетоводни дестинации
    [Required(ErrorMessage = "Моля, въведете новата община!")]
    public string NewMunicipality { get; set; } = string.Empty;

    [Required(ErrorMessage = "Моля, въведете новото населено място!")]
    public string NewTownOrVillage { get; set; } = string.Empty;

    [Required(ErrorMessage = "Моля, въведете новия конкретен обект/ферма!")]
    public string NewFacility { get; set; } = string.Empty;

    [Required(ErrorMessage = "Моля, въведете дата на преместването!")]
    public DateTime TransferDate { get; set; } = DateTime.Now;

    [Required(ErrorMessage = "Основанието / Документът е задължителен!")]
    public string DocumentReference { get; set; } = string.Empty; // напр. Протокол №45

    // УПРАВЛЕНИЕ НА ЖИЗНЕНИЯ ЦИКЪЛ (Чекбоксът за ликвидация/смърт)
    public bool IsActive { get; set; } = true; // Ако потребителят го отмаркира, обектът става неактивен
}
