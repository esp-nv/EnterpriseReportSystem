using System;

namespace EnterpriseReportSystem.Models.Entities;

public class ExchangeRateHistory
{
    public int Id { get; set; }

    // Търсене по валута 1
    public int FromCurrencyId { get; set; }
    public Currency FromCurrency { get; set; } = null!;

    // Търсене по валута 2
    public int ToCurrencyId { get; set; }
    public Currency ToCurrency { get; set; } = null!;

    // Твоят диапазон и стойност на курса
    public decimal Rate { get; set; }
    public DateTime RateDate { get; set; } // Търсене по дата

    // Твоят чекбокс за пълна свобода: Фиксиран (true) / Плаващ (false)
    public bool IsFixed { get; set; }
}
