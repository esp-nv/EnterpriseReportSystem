using System.ComponentModel.DataAnnotations;
using EnterpriseReportSystem.Common.Enum;

namespace EnterpriseReportSystem.Models.Entities.Ledger;

public class OperationType
{
    // 1. Уникален числов идентификатор (Identity)
    public int Id { get; set; }

    // 2. Системен код (напр. OP_BUY, OP_BIRTH)
    [Required]
    public string Code { get; set; } = string.Empty;

    // 3. Наименование на български език за потребителя
    [Required]
    public string Name { get; set; } = string.Empty;

    // 4. Твоят Combobox (Enum посока: Приход = 1, Разход = 2, Трансфер = 3)
    public OperationDirection Direction { get; set; }

    // 5. Контролният флаг, който ще използваме да броим или проверяваме финансови суми
    public bool IsFinancial { get; set; }
}
