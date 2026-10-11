using EnterpriseReportSystem.Models.Entities;
using EnterpriseReportSystem.Models.Entities.Hierarchy;
using EnterpriseReportSystem.Models.Entities.Ledger;
using EnterpriseReportSystem.Models.Entities.Location;
using EnterpriseReportSystem.Models.Entities.Structure;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseReportSystem.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // 1. РЕЛАЦИОННА ЙЕРАРХИЯ НА АКТИВИТЕ (НИВА 1, 2, 3)
    public DbSet<MainType> MainTypes { get; set; } = null!;
    public DbSet<AssetGroup> AssetGroups { get; set; } = null!;
    public DbSet<AssetSubGroup> AssetSubGroups { get; set; } = null!;

    // 2. ТРИСТЕМЕННА ГЕОГРАФСКА ЛОКАЦИЯ (ОБЛАСТ -> ОБЩИНА -> СЕЛО/ГРАД)
    public DbSet<Region> Regions { get; set; } = null!;
    public DbSet<Municipality> Municipalities { get; set; } = null!;
    public DbSet<TownOrVillage> TownsOrVillages { get; set; } = null!; // ОФИЦИАЛНО С "S" НА TOWNS

    // 3. МУЛТИ-ФИРМЕНА СТРУКТУРА НА ХОЛДИНГА И ОДИТЕН ДНЕВНИК
    public DbSet<CompanyOrPerson> CompaniesOrPersons { get; set; } = null!;
    public DbSet<Facility> Facilities { get; set; } = null!;
    public DbSet<FacilityLog> FacilityLogs { get; set; } = null!;

    // 4. СЧЕТОВОДЕН ЖУРНАЛ НА ОПЕРАЦИИТЕ
    public DbSet<OperationType> OperationTypes { get; set; } = null!;
    public DbSet<AssetTransaction> AssetTransactions { get; set; } = null!;

    // 5. ВАЛУТНА СЧЕТОВОДНА АРХИТЕКТУРА
    public DbSet<Currency> Currencies { get; set; } = null!;
    public DbSet<ExchangeRateHistory> ExchangeRateHistories { get; set; } = null!; // ТАЗИ ЛИПСВАШЕ


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // КОНФИГУРИРАНЕ НА ГЕОГРАФСКИТЕ ВРЪЗКИ (ОБЛАСТ -> ОБЩИНА -> СЕЛО)
        modelBuilder.Entity<Municipality>()
            .HasOne(m => m.Region)
            .WithMany(r => r.Municipalities)
            .HasForeignKey(m => m.RegionId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TownOrVillage>()
            .HasOne(t => t.Municipality)
            .WithMany(m => m.TownsOrVillages) // СИНХРОНИЗИРАНО С "S"
            .HasForeignKey(t => t.MunicipalityId)
            .OnDelete(DeleteBehavior.Restrict);

        // КОНФИГУРИРАНЕ НА МУЛТИ-ФИРМЕНИТЕ ОБЕКТИ И АДМИНИСТРАЦИЯТА
        modelBuilder.Entity<CompanyOrPerson>()
            .HasOne(c => c.RegisteredTown)
            .WithMany()
            .HasForeignKey(c => c.RegisteredTownId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Facility>()
            .HasOne(f => f.CompanyOrPerson)
            .WithMany(c => c.Facilities)
            .HasForeignKey(f => f.CompanyOrPersonId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Facility>()
            .HasOne(f => f.TownOrVillage)
            .WithMany()
            .HasForeignKey(f => f.TownOrVillageId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<FacilityLog>()
            .HasOne(fl => fl.Facility)
            .WithMany(f => f.FacilityLogs)
            .HasForeignKey(fl => fl.FacilityId)
            .OnDelete(DeleteBehavior.Restrict);

        // КОНФИГУРИРАНЕ НА ХРОНОЛОГИЯТА НА ВАЛУТНИТЕ КУРСОВЕ
        modelBuilder.Entity<ExchangeRateHistory>()
            .HasOne(e => e.FromCurrency)
            .WithMany()
            .HasForeignKey(e => e.FromCurrencyId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ExchangeRateHistory>()
            .HasOne(e => e.ToCurrency)
            .WithMany()
            .HasForeignKey(e => e.ToCurrencyId)
            .OnDelete(DeleteBehavior.Restrict);

        // Намери конфигурацията на ExchangeRateHistory в OnModelCreating и добави третия ред:
        modelBuilder.Entity<ExchangeRateHistory>()
            .HasOne(e => e.FromCurrency).WithMany().HasForeignKey(e => e.FromCurrencyId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ExchangeRateHistory>()
            .HasOne(e => e.ToCurrency).WithMany().HasForeignKey(e => e.ToCurrencyId).OnDelete(DeleteBehavior.Restrict);

        // ТВОЯТА ФИНАНСОВА ЗАЩИТА: Задаваме децинална точност за валутните курсове (18 цифри общо, 4 след запетаята)
        modelBuilder.Entity<ExchangeRateHistory>()
            .Property(e => e.Rate)
            .HasColumnType("decimal(18,5)");




        // ТВОЯТА ЗЛАТНА ЗАЩИТА: Предотвратяване на каскадно триене за абсолютно ВСИЧКИ връзки
        foreach (var foreignKey in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
        {
            foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
        }

        // ТВОЯТ ГЛОБАЛЕН ФИЛТЪР: Скриване на логически изтритите трансакции през Soft Delete
        modelBuilder.Entity<AssetTransaction>().HasQueryFilter(t => !t.IsDeleted);
    }

    public override int SaveChanges()
    {
        UpdateSoftDeleteStatuses();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateSoftDeleteStatuses();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateSoftDeleteStatuses()
    {
        foreach (var entry in ChangeTracker.Entries<ISoftDeletable>())
        {
            if (entry.State == EntityState.Deleted)
            {
                entry.State = EntityState.Modified;
                entry.Entity.IsDeleted = true;
                entry.Entity.DeletedOn = System.DateTime.Now;
            }
        }
    }
}
