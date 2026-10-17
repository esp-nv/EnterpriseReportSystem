using System.ComponentModel.DataAnnotations;

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

    // Спецификации на активите (Породи / Модели)
    public const int SpecificationNameMaxLength = 100;

    // Ограничения за Динамични Характеристики (EAV модел)
    public const int AttributeNameMaxLength = 60;   // напр. "Пол", "Номер на двигател"
    public const int AttributeValueMaxLength = 250;  // напр. "Женско", "ДВГ-987654"

      // Системни потребители, пароли и менюта (ИЗЧИСТЕНИ ОТ МАГИЧЕСКИ ЧИСЛА)
    public const int UsernameMaxLength = 50;
    public const int PasswordMaxLength = 255;
    public const int LayoutTypeMaxLength = 30;

    public const int MenuNameMaxLength = 100;
    public const int LanguageCodeMaxLength = 10;
    public const int MenuCssClassMaxLength = 150;
    public const int MenuShortcutMaxLength = 30;

   public const int WishListMenuIdsMaxLength = 200;  // Максимална дължина на списъка с избрани менюта (като низ от ID-та, разделени със запетаи)

    // Валутна децимална точност
    public const int ExchangeRatePrecision = 18;
    public const int ExchangeRateScale = 5;

   public const int ThemeNameMaxLength = 100;// Приятелско име: "Корпоративно синьо"

    public const int FileNameMaxLength = 150; // Име на файла: "corporate_blue.css"

    public const int FileSizeMaxLength = 50; // Максимална дължина на размера на файла (напр. "0 KB")


    
        // ... твоите съществуващи константи (напр. UsernameMaxLength = 50 и т.н.) ...

        // МИНИМАЛНИ ДЪЛЖИНИ
        public const int UsernameMinLength = 3;
        public const int PasswordMinLength = 8;

        // РЕГУЛЯРНИ ИЗРАЗИ ЗА СИГУРНОСТ (REGEX)
        public const string PasswordUpperRegex = @"[A-Z]";
        public const string PasswordLowerRegex = @"[a-z]";
        public const string PasswordDigitRegex = @"[0-9]";
        public const string PasswordSpecialRegex = @"[\W_]";
}



