using EnterpriseReportSystem.Common;
using EnterpriseReportSystem.Common.Enum;

namespace EnterpriseReportSystem.Models.Entities;

public class OperationType
{
    public string Code { get; set; } = string.Empty; // "OP_BIRTH", "OP_AGE_TRANSFER", "OP_SALE"
    public string Name { get; set; } = string.Empty; // "Раждане", "Възрастово преместване"
    public OperationDirection Direction { get; set; } // Връзка с енъма от папка Common
}
