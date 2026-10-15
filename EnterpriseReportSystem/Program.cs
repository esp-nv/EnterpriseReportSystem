using EnterpriseReportSystem.Data;
using EnterpriseReportSystem.Services.Implementations;
using EnterpriseReportSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Добавяме и конфигурираме Entity Framework Core с SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// Регистрираме бизнес услугата за количествени ведомости и анализи
builder.Services.AddScoped<IReportService, ReportService>();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// АВТОМАТИЧНО НАПЪЛВАНЕ НА БАЗАТА ДАННИ ПРИ СТАРТИРАНЕ (УЕБ ВАРИАНТ)
// АВТОМАТИЧНО НАЛИВАНЕ НА БАЗАТА ДАННИ ПРИ СТАРТИРАНЕ (УЕБ ВАРИАНТ)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var env = services.GetRequiredService<IWebHostEnvironment>();

        // Извикваме новия асинхронен метод с правилните параметри
        string seedResult = DbInitializer.SeedAsync(context, env.WebRootPath).GetAwaiter().GetResult();

        if (seedResult != "SUCCESS")
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogWarning(seedResult);
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Х фатална грешка при първоначално стартиране на базата данни.");
    }
}

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
