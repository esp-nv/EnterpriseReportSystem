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
            // 1. ДИНАМИЧНО НАЛИВАНЕ НА ЦЕНТРАЛНИЯ КАТАЛОГ ЗА CSS ТЕМИ СЪС СТРОГИ ID-ТА
            if (!await context.GlobalThemes.AnyAsync())
            {
                var themesPath = Path.Combine(webRootPath, "InputFiles", "Structure", "00_GlobalThemes.json");
                if (File.Exists(themesPath))
                {
                    var themesJson = await File.ReadAllTextAsync(themesPath);
                    var themes = JsonSerializer.Deserialize<List<GlobalTheme>>(themesJson);
                    if (themes != null)
                    {
                        // Отключваме брояча на SQL Server за таблица GlobalThemes
                        await context.Database.OpenConnectionAsync();
                        await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT GlobalThemes ON");

                        await context.GlobalThemes.AddRangeAsync(themes);
                        await context.SaveChangesAsync();

                        await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT GlobalThemes OFF");
                        context.Database.CloseConnection();
                    }
                }
            }

            // 2. ДИНАМИЧНО НАЛИВАНЕ НА ПОТРЕБИТЕЛИТЕ СЪС СТРОГИ ID-ТА
            if (!await context.SystemUsers.AnyAsync())
            {
                var usersPath = Path.Combine(webRootPath, "InputFiles", "Structure", "2_SystemUsers.json");
                if (File.Exists(usersPath))
                {
                    var usersJson = await File.ReadAllTextAsync(usersPath);
                    using var doc = JsonDocument.Parse(usersJson);
                    var root = doc.RootElement;

                    var usersList = new List<SystemUser>();

                    foreach (var userEl in root.EnumerateArray())
                    {
                        string jsonLayout = userEl.GetProperty("LayoutType").GetString() ?? "DASHBOARD";
                        AppLayoutType finalLayout = jsonLayout.ToUpper() switch
                        {
                            "RIBBON" => AppLayoutType.Ribbon,
                            "NAVBAR" => AppLayoutType.Navbar,
                            _ => AppLayoutType.Dashboard
                        };

                        var newUser = new SystemUser
                        {
                            Id = userEl.GetProperty("Id").GetInt32(), // Връщаме оригиналното ID от JSON
                            Username = userEl.GetProperty("Username").GetString() ?? string.Empty,
                            Password = userEl.GetProperty("Password").GetString() ?? string.Empty,
                            LayoutType = finalLayout,
                            IsMenuExpanded = userEl.GetProperty("IsMenuExpanded").GetBoolean(),
                            ThemeType = userEl.GetProperty("ThemeType").GetString() ?? "LIGHT",
                            IsGlobalAdmin = userEl.GetProperty("IsGlobalAdmin").GetBoolean(),
                            WishListMenuIds = userEl.GetProperty("WishListMenuIds").GetString() ?? string.Empty,
                            CompanyId = userEl.GetProperty("CompanyId").GetInt32(),
                            IsCustomPaletteActive = userEl.GetProperty("IsCustomPaletteActive").GetBoolean(),
                            ThemeId = userEl.TryGetProperty("ThemeId", out var tProp) ? tUr(tProp) : 1,
                            IsDarkUserPreference = !userEl.TryGetProperty("IsDarkUserPreference", out var dProp) || dProp.GetBoolean()
                        };

                        usersList.Add(newUser);
                    }

                    if (usersList.Any())
                    {
                        // Отключваме брояча на SQL Server за таблица SystemUsers
                        await context.Database.OpenConnectionAsync();
                        await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT SystemUsers ON");

                        await context.SystemUsers.AddRangeAsync(usersList);
                        await context.SaveChangesAsync();

                        await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT SystemUsers OFF");
                        context.Database.CloseConnection();
                    }
                }
            }

            return "SUCCESS";
        }
        catch (Exception ex)
        {
            if (context.Database.GetDbConnection().State == System.Data.ConnectionState.Open)
            {
                context.Database.CloseConnection();
            }
            return $"⚠️ [Критична грешка при Рефлекшън наливане]: Детайли: {ex.InnerException?.Message ?? ex.Message}";
        }
    }

    private static int? tUr(JsonElement prop) => prop.ValueKind == JsonValueKind.Null ? null : prop.GetInt32();
}
