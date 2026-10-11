namespace EnterpriseReportSystem.Common.Constants;

public static class ValidationConstants
{
    // Валутни константи
    public const int CurrencyCodeLength = 3;
    public const int CurrencyNameMaxLength = 50;

    // Субекти (Фирми и Физически лица)
    public const int ClientCodeMaxLength = 20;
    public const int CompanyNameMaxLength = 150;
    public const int TaxIdentifierMaxLength = 15;

    // Физически обекти (Ферми и Бази)
    public const int FacilityCodeMaxLength = 30;
    public const int FacilityNameMaxLength = 100;
    public const int PostalCodeMaxLength = 10;
    public const int NotesMaxLength = 500;

    // Номенклатурна Йерархия (Нива 1, 2, 3)
    public const int HierarchyCodeMaxLength = 20;
    public const int HierarchyNameMaxLength = 100;
    public const int MeasurementUnitMaxLength = 10;
}
