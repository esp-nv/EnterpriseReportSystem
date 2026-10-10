using System;
using System.Collections.Generic;
using System.Linq;
using EnterpriseReportSystem.Data;
using EnterpriseReportSystem.Models.Entities;
using EnterpriseReportSystem.Services.Interfaces;

namespace EnterpriseReportSystem.Services.Implementations;

public class ReportService : IReportService
{
    private readonly ApplicationDbContext _context;

    // Инжектираме базата данни директно чрез Dependency Injection
    public ReportService(ApplicationDbContext context)
    {
        _context = context;
    }

    // 1. ГЕНЕРИРАНЕ НА ОБОРОТКА ДИРЕКТНО ОТ SQL ТАБЛИЦИТЕ
    public List<ITrackableAsset> GenerateTurnoverReport(List<AssetTransaction> transactions, List<ITrackableAsset> currentRegistry)
    {
        // В реалния метод ще четем директно от базата данни (_context.AssetTransactions)
        // За да сме гъвкави, C# ще групира трансакциите по сметка/група и ще изчисли балансите

        var result = new List<ITrackableAsset>();

        // Взимаме всички налични активи от регистъра
        foreach (var asset in currentRegistry)
        {
            // Намираме трансакциите в реалната таблица за това конкретно ID
            var assetTransactions = _context.AssetTransactions
                .Where(t => t.AssetId == asset.Id && !t.IsDeleted)
                .ToList();

            // Изчисляваме Прихода (+) въз основа на числовите стойности на OperationDirection
            asset.InputCount = assetTransactions
                .Where(t => (int)t.Operation.Direction == 1) // Income = 1
                .Sum(t => t.Quantity);

            // Изчисляваме Разхода (-)
            asset.OutputCount = assetTransactions
                .Where(t => (int)t.Operation.Direction == 2) // Expense = 2
                .Sum(t => t.Quantity);

            result.Add(asset);
        }

        return result;
    }

    // 2. ДИНАМИЧНО ФИЛТРИРАНЕ НА ТАБЛИЦИТЕ ПО КРИТЕРИИ
    public List<ITrackableAsset> FilterAssets(List<ITrackableAsset> assets, string municipality, string town, string groupCode)
    {
        var query = assets.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(municipality))
        {
            query = query.Where(x => x.Municipality.Equals(municipality, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(town))
        {
            query = query.Where(x => x.TownOrVillage.Equals(town, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(groupCode))
        {
            query = query.Where(x => x.Group.Code == groupCode);
        }

        return query.ToList();
    }
}
