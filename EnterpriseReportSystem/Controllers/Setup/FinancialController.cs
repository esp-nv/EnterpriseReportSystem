using EnterpriseReportSystem.Data;
using EnterpriseReportSystem.Models.Entities;
using EnterpriseReportSystem.Models.Entities.Ledger;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseReportSystem.Controllers.Setup; // СИНХРОНИЗИРАН NAMESPACE ПЪТ

public class FinancialController : Controller
{
    private readonly ApplicationDbContext _context;

    public FinancialController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> ManageRates()
    {
        var currencies = await _context.Currencies.ToListAsync();
        ViewData["Currencies"] = currencies
            .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = $"{c.Code} - {c.Name}" })
            .ToList();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateExchangeRate(int fromCurrencyId, int toCurrencyId, decimal rate, DateTime rateDate, bool isFixed)
    {
        if (rate > 0 && fromCurrencyId != toCurrencyId)
        {
            var exchangeRate = new ExchangeRateHistory
            {
                FromCurrencyId = fromCurrencyId,
                ToCurrencyId = toCurrencyId,
                Rate = rate,
                RateDate = rateDate.Date,
                IsFixed = isFixed
            };

            _context.ExchangeRateHistories.Add(exchangeRate);
            await _context.SaveChangesAsync();
            TempData["RateSuccess"] = "🎉 Валутният курс е заведен успешно в регистъра!";
        }
        return RedirectToAction("ManageRates");
    }
}
