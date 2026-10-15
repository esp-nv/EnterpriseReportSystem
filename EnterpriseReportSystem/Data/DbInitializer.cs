using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using EnterpriseReportSystem.Models.Entities.Structure;
using EnterpriseReportSystem.Common.Enum;

namespace EnterpriseReportSystem.Data;

public static class DbInitializer
{
    public static async Task<string> SeedAsync(ApplicationDbContext context, string webRootPath)
    {
        try
        {
            // 1. ДИНАМИЧНО НАЛИВАНЕ НА ЦЕНТРАЛНИЯ КАТАЛОГ ЗА CSS ТЕМИ ОТ JSON ФАЙЛА
            if (!await context.GlobalThemes.AnyAsync())
            {
                var themesPath = Path.Combine(webRootPath, "InputFiles", "Structure", "00_GlobalThemes.json");
                if (File.Exists(themesPath))
                {
                    var themesJson = await File.ReadAllTextAsync(themesPath);
                    var themes = JsonSerializer.Deserialize<List<GlobalTheme>>(themesJson);
                    if (themes != null)
                    {
                        await context.GlobalThemes.AddRangeAsync(themes);
                        await context.SaveChangesAsync();
                    }
                }
            }

            // 2. ДИНАМИЧНО НАЛИВАНЕ НА ПОТРЕБИТЕЛИТЕ (С МАПВАНЕ НА LAYOUT ENUM)
            if (!await context.SystemUsers.AnyAsync())
            {
                var usersPath = Path.Combine(webRootPath, "InputFiles", "Structure", "2_SystemUsers.json");
                if (File.Exists(usersPath))
                {
                    var usersJson = await File.ReadAllTextAsync(usersPath);
                    using var doc = JsonDocument.Parse(usersJson);
                    var root = doc.RootElement;

                    foreach (var userEl in root.EnumerateArray())
                    {
                        // Преобразуваме текстовия низ от JSON файла директно към нашия нов сигурен Enum
                        string jsonLayout = userEl.GetProperty("LayoutType").GetString() ?? "DASHBOARD";
                        AppLayoutType finalLayout = jsonLayout.ToUpper() switch
                        {
                            "RIBBON" => AppLayoutType.Ribbon,
                            "NAVBAR" => AppLayoutType.Navbar,
                            _ => AppLayoutType.Dashboard
                        };

                        var newUser = new SystemUser
                        {
                            Id = userEl.GetProperty("Id").GetInt32(),
                            Username = userEl.GetProperty("Username").GetString() ?? string.Empty,
                            Password = userEl.GetProperty("Password").GetString() ?? string.Empty,
                            LayoutType = finalLayout, // Записва се като чист int в SQL Server
                            IsMenuExpanded = userEl.GetProperty("IsMenuExpanded").GetBoolean(),
                            ThemeType = userEl.GetProperty("ThemeType").GetString() ?? "LIGHT",
                            IsGlobalAdmin = userEl.GetProperty("IsGlobalAdmin").GetBoolean(),
                            WishListMenuIds = userEl.GetProperty("WishListMenuIds").GetString() ?? string.Empty,
                            CompanyId = userEl.GetProperty("CompanyId").GetInt32(),
                            IsCustomPaletteActive = userEl.GetProperty("IsCustomPaletteActive").GetBoolean(),

                            // Четем новите свойства от потребителския JSON файл
                            ThemeId = userEl.TryGetProperty("ThemeId", out var tProp) ? tUr(tProp) : 1, // Фиксиран бекъп към Id 1
                            IsDarkUserPreference = !userEl.TryGetProperty("IsDarkUserPreference", out var dProp) || dProp.GetBoolean()
                        };

                        await context.SystemUsers.AddAsync(newUser);
                    }

                    await context.SaveChangesAsync();
                }
            }

            return "SUCCESS"; // Всички папки са налети автоматично без нито един ред твърд код!
        }
        catch (Exception ex)
        {
            return $"⚠️ [Критична грешка при Рефлекшън наливане]: Детайли: {ex.InnerException?.Message ?? ex.Message}";
        }
    }

    private static int? tUr(JsonElement prop) => prop.ValueKind == JsonValueKind.Null ? null : prop.GetInt32();
}
