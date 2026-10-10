using System.Collections.Generic;

namespace EnterpriseReportSystem.Models.Entities;

public class FileProcessSummary
{
    public int TotalLinesRead { get; set; }
    public int SuccessfulLines { get; set; }
    public int FailedLines => TotalLinesRead - SuccessfulLines;
    public double SuccessPercentage => TotalLinesRead > 0 ? ((double)SuccessfulLines / TotalLinesRead) * 100 : 0;

    // Списък с филтрираните технически грешки
    public List<ValidationError> Errors { get; set; } = new();

    // Списък със сравнителните редове за таблицата на разликите
    public List<DiffReportLine> DiffLines { get; set; } = new();

    public string SelectedErrorType { get; set; } = string.Empty;
}

// Помощен клас за реда с разлики
public class DiffReportLine
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int SystemCount { get; set; }
    public int FileCount { get; set; }
    public int Difference => SystemCount - FileCount;
    public string Status { get; set; } = string.Empty; // Съвпада, Липсва, Разминаване
}
