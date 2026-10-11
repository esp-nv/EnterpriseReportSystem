using EnterpriseReportSystem.Models.Entities.Hierarchy;
using EnterpriseReportSystem.Models.Entities.Structure;
using System;

namespace EnterpriseReportSystem.Models.Entities.Ledger;

public class AssetTransaction : ISoftDeletable
{
    // Уникален идентификатор на самата трансакция
    public int Id { get; set; }

    // КОРЕКЦИЯ 1: Идентификаторът на актива е число (ушна марка или инвентарен номер)
    public int AssetId { get; set; }

    // Какъв тип е операцията (Раждане, Възрастово преместване, Продажба, Брак)
    public int OperationId { get; set; } // Външен числови ключ
    public OperationType Operation { get; set; } = new();

    // От коя група/локация се мести (използва се при вътрешни премествания)
    public int? FromGroupId { get; set; }
    public AssetGroup? FromGroup { get; set; }

    // КОРЕКЦИЯ 2: Сменяме текстовия FromFacility с релация към заведените ти обекти/ферми
    public int? FromFacilityId { get; set; }
    public Facility? FromFacility { get; set; }

    // Към коя група/локация отива
    public int? ToGroupId { get; set; }
    public AssetGroup? ToGroup { get; set; }

    // КОРЕКЦИЯ 3: Сменяме текстовия ToFacility с релация към дестинацията на обекта/фермата
    public int? ToFacilityId { get; set; }
    public Facility? ToFacility { get; set; }

    // Количество (За ЕПЖ/Продукти може да е повече от 1)
    public int Quantity { get; set; }

    // Дата на събитието
    public DateTime TransactionDate { get; set; } = DateTime.Now;

    // Документ или основание за операцията (напр. Протокол №5, Акт за раждане)
    public string DocumentReference { get; set; } = string.Empty;

    // ИМПЛЕМЕНТАЦИЯ НА SOFT DELETE
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedOn { get; set; }

    public string Notes { get; set; } = string.Empty;  // НОВОТО СВОЙСТВО ЗА СЧЕТОВОДНИ БЕЛЕЖКИ ПО ТРАНСАКЦИЯТА
}
