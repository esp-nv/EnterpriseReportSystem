namespace EnterpriseReportSystem.ViewModels; // Коренната папка ViewModels

public class HierarchyCreateViewModel
{
    public string MainTypeCode { get; set; } = string.Empty;
    public string MainTypeName { get; set; } = string.Empty;

    public string GroupCode { get; set; } = string.Empty;
    public string GroupName { get; set; } = string.Empty;
    public string SelectedMainTypeCode { get; set; } = string.Empty;

    public string SubGroupCode { get; set; } = string.Empty;
    public string SubGroupName { get; set; } = string.Empty;
    public string SelectedGroupCode { get; set; } = string.Empty;
}
