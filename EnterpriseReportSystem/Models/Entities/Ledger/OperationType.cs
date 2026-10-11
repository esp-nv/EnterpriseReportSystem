using EnterpriseReportSystem.Common.Enum;

namespace EnterpriseReportSystem.Models.Entities.Ledger;

public class OperationType
{
    // ТВОЯТ НОВ ЧИСЛОВ ПЪРВИЧЕН КЛЮЧ
    public int Id { get; set; }

    // Уникален код на операцията (напр. OP_BIRTH, OP_SALE)
    public string Code { get; set; } = string.Empty;

    // Наименование за потребителя (напр. Раждане, Продажба)
    public string Name { get; set; } = string.Empty;

    // Посока на движението (Income = 1, Expense = 2, Internal = 3)
    public OperationDirection Direction { get; set; }

    // true = променя бройката в Оборотката, false = само хронологично събитие
    public bool IsFinancial { get; set; } = true;
}
