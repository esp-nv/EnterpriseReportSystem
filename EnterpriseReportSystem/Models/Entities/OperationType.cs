using EnterpriseReportSystem.Common.Enum;

namespace EnterpriseReportSystem.Models.Entities;

public class OperationType
{
    // Уникален код на операцията (напр. OP_BIRTH, OP_SALE)
    public string Code { get; set; } = string.Empty;

    // Наименование за потребителя (напр. Раждане, Продажба)
    public string Name { get; set; } = string.Empty;

    // Посока на движението (Income = 1, Expense = 2, Internal = 3)
    public OperationDirection Direction { get; set; }

    // НОВОТО ПОЛЕ: true = променя бройката в Оборотката, false = само хронологично събитие (напр. ваксинация, сервиз)
    public bool IsFinancial { get; set; } = true;
}
