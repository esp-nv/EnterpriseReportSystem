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

    // Счетоводен Журнал (Трансакции)
    public const int AssetIdMaxLength = 50;
    public const int FacilityTextMaxLength = 100;
    public const int DocumentRefMaxLength = 150;

    // Типове Операции
    public const int OperationNameMaxLength = 100;

    // Одитен Дневник на Обектите
    public const int LogEventTypeMaxLength = 30;
    public const int LogValueMaxLength = 1000; // По-голяма дължина за детайлно описание на промените

    public const int CompanyAddressMaxLength = 150;

    public const int FacilityAddressMaxLength = 150; // — Специфична константа за точния адрес/местност на фермата или склада[local].
    public const int OperationCodeMaxLength = 20;
    //— Отделна къса константа за уникалния код на операцията(напр.OP_TRANSFER) [finance].
    public const int TransactionNotesMaxLength = 500; // — Специална константа за новото счетоводно поле Notes в Журнала [finance]

    public const int RegionNameMaxLength = 40; // За областите(напр. "Област Бургас") 40 символа са напълно достатъчни.
    public const int MunicipalityNameMaxLength = 60; //За общините 60 символа дават отлична гъвкавост.
    public const int TownOrVillageNameMaxLength = 100; // За населените места запазваме до 100 символа поради дълги комбинирани имена.
   

}
