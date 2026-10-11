namespace EnterpriseReportSystem.Models.Entities;

public class Currency
{
    public int Id { get; set; } // Автоматичен цифров номер
    public string Code { get; set; } = string.Empty; // напр. USD, EUR, BGN
    public string Name { get; set; } = string.Empty; // напр. Щатски долар
}
