using EnterpriseReportSystem.Models.Entities;
using EnterpriseReportSystem.Models.Entities.Hierarchy;
using EnterpriseReportSystem.Models.Entities.Ledger;
using EnterpriseReportSystem.Models.Entities.Location;
using EnterpriseReportSystem.Models.Entities.Structure;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace EnterpriseReportSystem.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // РЕЛАЦИОННИ DB SET ТАБЛИЦИ
    public DbSet<MainType> MainTypes { get; set; } = null!;
    public DbSet<AssetGroup> AssetGroups { get; set; } = null!;
    public DbSet<AssetSubGroup> AssetSubGroups { get; set; } = null!;
    public DbSet<Region> Regions { get; set; } = null!;
    public DbSet<Municipality> Municipalities { get; set; } = null!;
    public DbSet<TownOrVillage> TownsOrVillages { get; set; } = null!;
    public DbSet<CompanyOrPerson> CompaniesOrPersons { get; set; } = null!;
    public DbSet<Facility> Facilities { get; set; } = null!;
    public DbSet<FacilityLog> FacilityLogs { get; set; } = null!;
    public DbSet<OperationType> OperationTypes { get; set; } = null!;
    public DbSet<AssetTransaction> AssetTransactions { get; set; } = null!;
    public DbSet<Currency> Currencies { get; set; } = null!;
    public DbSet<ExchangeRateHistory> ExchangeRateHistories { get; set; } = null!;
    public DbSet<AssetAttribute> AssetAttributes { get; set; } = null!;
    public DbSet<AssetAttributeValue> AssetAttributeValues { get; set; } = null!;

    // РЕЛАЦИОННИ DB SET ТАБЛИЦИ (ДОБАВЯНЕ НА СПЕЦИФИКАЦИИТЕ)
    public DbSet<AssetSpecification> AssetSpecifications { get; set; } = null!;

    // РЕЛАЦИОННИ DB SET ТАБЛИЦИ (СТРУКТУРНИ)
    public DbSet<SystemUser> SystemUsers { get; set; } = null!;
    public DbSet<SystemMenu> SystemMenus { get; set; } = null!;

    public DbSet<Language> Languages { get; set; } = null!;
    public DbSet<SystemMenuTranslation> SystemMenuTranslations { get; set; } = null!;


    // НОВО: Регистрираме централния конфигурационен каталог за външните CSS теми от bootstrap.build
    public DbSet<GlobalTheme> GlobalThemes { get; set; } = null!;


    public override int SaveChanges()
    {
        EnforceGlobalThemeProtection();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        EnforceGlobalThemeProtection();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void EnforceGlobalThemeProtection()
    {
        // Хващаме всеки опит за промяна (Modified) или изтриване (Deleted) на тема с Id = 1
        var violatedEntries = ChangeTracker.Entries<GlobalTheme>()
            .Where(e => e.State == EntityState.Modified || e.State == EntityState.Deleted)
            .Any(e => e.Entity.Id == 1);

        if (violatedEntries)
        {
            throw new InvalidOperationException("⚠️ [Критична системна защита]: Операцията е блокирана! Фабричният запис с Id = 1 е защитен котва и не може да бъде променян или изтриван.");
        }
    }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        base.OnModelCreating(modelBuilder);

        // ЖЕЛЯЗНО ПРАВИЛО: Автоматично сканиране и зареждане на всички конфигурационни класове от папката
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // ГЛОБАЛНО ПРЕДОТВРАТЯВАНЕ НА КАСКАДНО ТРИЕНЕ ЗА СИГУРНОСТ НА ДАННИТЕ
        foreach (var foreignKey in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
        {
            foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
        }

        // Глобален филтър за скриване на изтритите трансакции през Soft Delete
        modelBuilder.Entity<AssetTransaction>().HasQueryFilter(t => !t.IsDeleted);

        // ТВОЯТА НОВА СЧЕТОВОДНА ЗАЩИТА: Автоматично скриване на детайлите, ако главната трансакция е Soft Deleted
        modelBuilder.Entity<AssetAttributeValue>().HasQueryFilter(av => !av.Transaction.IsDeleted);


    }
}
