namespace EnterpriseReportSystem.Common.Enum
{
    /// <summary>
    /// ОПРЕДЕЛЯ КОЙ РЕЖИМ НА КОНТРАСТ ПОДДЪРЖА КАЧЕНИЯТ CSS СТИЛ
    /// </summary>
    public enum ThemeSupportType
    {
        LightOnly = 1, // Темата е компилирана строго само за Светъл режим
        DarkOnly = 2,  // Темата е компилирана строго само за Тъмен режим
        Universal = 3  // Универсална тема - съдържа в себе си и двата изгледа (И двете)
    }
}
