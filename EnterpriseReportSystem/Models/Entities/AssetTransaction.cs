using System;

namespace EnterpriseReportSystem.Models.Entities;

public class AssetTransaction
{
    // Уникален идентификатор на самата трансакция
    public string Id { get; set; } = Guid.NewGuid().ToString();

    // Към кое конкретно животно (ушна марка) или продукт се отнася тази трансакция
    public string AssetId { get; set; } = string.Empty;

    // Какъв тип е операцията (Раждане, Възрастово преместване, Продажба, Брак)
    public OperationType Operation { get; set; } = new();

    // От коя група/локация се мести (използва се при вътрешни премествания)
    public AssetGroup? FromGroup { get; set; }
    public string FromFacility { get; set; } = string.Empty;

    // Към коя група/локация отива
    public AssetGroup? ToGroup { get; set; }
    public string ToFacility { get; set; } = string.Empty;

    // Количество (За ЕПЖ/Продукти може да е повече от 1, за единично ДПЖ обикновено е 1)
    public int Quantity { get; set; }

    // Дата на събитието
    public DateTime TransactionDate { get; set; } = DateTime.Now;

    // Документ или основание за операцията (напр. Протокол №5, Акт за раждане)
    public string DocumentReference { get; set; } = string.Empty;
}
