namespace EnterpriseReportSystem.Models.Entities;

public class AssetSubGroup
{
    public string Code { get; set; } = string.Empty; // напр. "TELETAS_UNDER_6M", "KRAVI"
    public string Name { get; set; } = string.Empty; // напр. "Телета до 6 месеца", "Възрастни крави"

    // Външен ключ към Основната Група (AssetGroup)
    public string AssetGroupCode { get; set; } = string.Empty;
    public AssetGroup AssetGroup { get; set; } = null!;
}
