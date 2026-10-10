using EnterpriseReportSystem.Models.Entities;

namespace EnterpriseReportSystem.Services.Interfaces;

public interface ITrackableAsset
{
    string Id { get; set; }          // Ушна марка за ДПЖ/ЕПЖ или Баркод за стока
    string Name { get; set; }        // Име на животното или наименование на продукта
    AssetGroup Group { get; set; }   // Възрастова/стокова група

    // Териториална йерархия
    string Municipality { get; set; }
    string TownOrVillage { get; set; }
    string Facility { get; set; }     // Обект / Ферма / Склад

    // Показатели за количествената оборотка
    int InitialCount { get; set; }
    int InputCount { get; set; }     // Увеличение през периода (+)
    int OutputCount { get; set; }    // Намаление през периода (-)
    int FinalCount { get; }          // Краен баланс (Автоматично изчисление)
}
