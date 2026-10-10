namespace EnterpriseReportSystem.Models.Entities;

public class ValidationError
{
    public int LineNumber { get; set; }        // Номер на ред във файла
    public string RawContent { get; set; } = string.Empty; // Дефектният текст
    public string Reason { get; set; } = string.Empty;     // Кратък етикет (напр. Празно ID)
    public string Details { get; set; } = string.Empty;    // Подробно описание за потребителя
}
