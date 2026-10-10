using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using EnterpriseReportSystem.Data;
using EnterpriseReportSystem.Models.Entities;

namespace EnterpriseReportSystem.Controllers;

public class AuditController : Controller
{
    private readonly ApplicationDbContext _context;

    public AuditController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Compare()
    {
        return View(new FileProcessSummary());
    }

    [HttpPost]
    public async Task<IActionResult> Compare(IFormFile csvFile, string selectedErrorType)
    {
        var summary = new FileProcessSummary { SelectedErrorType = selectedErrorType };

        if (csvFile == null || csvFile.Length == 0)
        {
            ModelState.AddModelError("", "Моля, качете валиден текстов или CSV файл.");
            return View(summary);
        }

        var fileData = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        int currentLine = 0;

        using (var reader = new StreamReader(csvFile.OpenReadStream()))
        {
            string? line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                currentLine++;
                summary.TotalLinesRead = currentLine;

                if (string.IsNullOrWhiteSpace(line))
                {
                    summary.Errors.Add(new ValidationError { LineNumber = currentLine, RawContent = "[Празен ред]", Reason = "Празен ред", Details = "Редът не съдържа символи." });
                    continue;
                }

                var parts = line.Split(';');
                if (parts.Length < 2)
                {
                    summary.Errors.Add(new ValidationError { LineNumber = currentLine, RawContent = line, Reason = "Липсва разделител", Details = "Липсва ';' или втора колона за количество." });
                    continue;
                }

                string id = parts[0].Trim();
                string qtyStr = parts[1].Trim();

                if (string.IsNullOrEmpty(id))
                {
                    summary.Errors.Add(new ValidationError { LineNumber = currentLine, RawContent = line, Reason = "Празно ID", Details = "Идентификаторът на обекта е празен." });
                    continue;
                }

                if (!int.TryParse(qtyStr, out int qty) || qty < 0)
                {
                    summary.Errors.Add(new ValidationError { LineNumber = currentLine, RawContent = line, Reason = "Грешно количество", Details = $"Стойността '{qtyStr}' не е положително число." });
                    continue;
                }

                fileData[id] = qty;
                summary.SuccessfulLines++;
            }
        }

        if (!string.IsNullOrEmpty(selectedErrorType))
        {
            summary.Errors = summary.Errors.Where(e => e.Reason == selectedErrorType).ToList();
        }

        // Засичане с реалните данни от базата през Entity Framework
        var activeTransactions = _context.AssetTransactions.Where(t => !t.IsDeleted).ToList();
        // Тук в реалния код ще се изчисляват балансите от SQL таблицата...

        return View(summary);
    }
}
