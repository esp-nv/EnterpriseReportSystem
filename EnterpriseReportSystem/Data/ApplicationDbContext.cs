using EnterpriseReportSystem.Common.Constants;
using EnterpriseReportSystem.Models.Entities;
using EnterpriseReportSystem.Models.Entities.Hierarchy;
using EnterpriseReportSystem.Models.Entities.Ledger;
using EnterpriseReportSystem.Models.Entities.Location;
using EnterpriseReportSystem.Models.Entities.Structure;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
    public DbSet<TownOrVillage> TownsOrVillages { get; set; } = null!;

    // 3. МУЛТИ-ФИРМЕНА СТРУКТУРА НА ХОЛДИНГА И ОДИТЕН ДНЕВНИК
    public DbSet<CompanyOrPerson> CompaniesOrPersons { get; set; } = null!;
    public DbSet<Facility> Facilities { get; set; } = null!;
    public DbSet<FacilityLog> FacilityLogs { get; set; } = null!;

    // 4. СЧЕТОВОДЕН ЖУРНАЛ НА ОПЕРАЦИИТЕ
    public DbSet<OperationType> OperationTypes { get; set; } = null!;
    public DbSet<AssetTransaction> AssetTransactions { get; set; } = null!;

    // 5. ВАЛУТНА СЧЕТОВОДНА АРХИТЕКТУРА
    public DbSet<Currency> Currencies { get; set; } = null!;
    public DbSet<ExchangeRateHistory> ExchangeRateHistories { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ==========================================
        // РЕЛАЦИОННИ ВРЪЗКИ (FOREIGN KEYS)
        // ==========================================
        modelBuilder.Entity<Municipality>()
            .HasOne(m => m.Region)
            .WithMany(r => r.Municipalities)
            .HasForeignKey(m => m.RegionId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TownOrVillage>()
            .HasOne(t => t.Municipality)
            .WithMany(m => m.TownsOrVillages)
            .HasForeignKey(t => t.MunicipalityId)
            .OnDelete(DeleteBehavior.Restrict);

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

        // Настройки за децимална точност (18 цифри общо, 5 след запетаята)
        modelBuilder.Entity<ExchangeRateHistory>()
            .Property(e => e.Rate)
            .HasColumnType("decimal(18,5)");

        // ==========================================
        // ЖЕЛЯЗНА ЗАЩИТА С ТВОИТЕ НЕЗАВИСИМИ КОНСТАНТИ
        // ==========================================

        // 1. Географски нива (Самостоятелни)
        modelBuilder.Entity<Region>(r =>
        {
            r.Property(x => x.Name).IsRequired().HasMaxLength(ValidationConstants.RegionNameMaxLength);
        });

        modelBuilder.Entity<Municipality>(m =>
        {
            m.Property(x => x.Name).IsRequired().HasMaxLength(ValidationConstants.MunicipalityNameMaxLength);
        });

        modelBuilder.Entity<TownOrVillage>(t =>
        {
            t.Property(x => x.Name).IsRequired().HasMaxLength(ValidationConstants.TownOrVillageNameMaxLength);
            t.Property(x => x.PostalCode).IsRequired().HasMaxLength(ValidationConstants.PostalCodeMaxLength);
        });

        // 2. Валутна архитектура
        modelBuilder.Entity<Currency>(c =>
        {
            c.Property(x => x.Code).IsRequired().HasMaxLength(ValidationConstants.CurrencyCodeLength);
            c.Property(x => x.Name).IsRequired().HasMaxLength(ValidationConstants.CurrencyNameMaxLength);
        });

        // 3. Мулти-фирмена холдингова структура + Новите Адреси
        modelBuilder.Entity<CompanyOrPerson>(c =>
        {
            c.Property(x => x.ClientCode).IsRequired().HasMaxLength(ValidationConstants.ClientCodeMaxLength);
            c.Property(x => x.Name).IsRequired().HasMaxLength(ValidationConstants.CompanyNameMaxLength);
            c.Property(x => x.TaxIdentifier).IsRequired().HasMaxLength(ValidationConstants.TaxIdentifierMaxLength);
            c.Property(x => x.Address).IsRequired().HasMaxLength(ValidationConstants.CompanyAddressMaxLength);
        });

        modelBuilder.Entity<Facility>(f =>
        {
            f.Property(x => x.FacilityCode).IsRequired().HasMaxLength(ValidationConstants.FacilityCodeMaxLength);
            f.Property(x => x.Name).IsRequired().HasMaxLength(ValidationConstants.FacilityNameMaxLength);
            f.Property(x => x.PostalCode).IsRequired().HasMaxLength(ValidationConstants.PostalCodeMaxLength);
            f.Property(x => x.Notes).HasMaxLength(ValidationConstants.NotesMaxLength);
            f.Property(x => x.Address).IsRequired().HasMaxLength(ValidationConstants.FacilityAddressMaxLength);
        });

        // 4. Номенклатурна Йерархия
        modelBuilder.Entity<MainType>(m => {
            m.Property(x => x.Code).IsRequired().HasMaxLength(ValidationConstants.HierarchyCodeMaxLength);
            m.Property(x => x.Name).IsRequired().HasMaxLength(ValidationConstants.HierarchyNameMaxLength);
        });

        modelBuilder.Entity<AssetGroup>(g => {
            g.Property(x => x.Code).IsRequired().HasMaxLength(ValidationConstants.HierarchyCodeMaxLength);
            g.Property(x => x.Name).IsRequired().HasMaxLength(ValidationConstants.HierarchyNameMaxLength);
        });

        modelBuilder.Entity<AssetSubGroup>(s => {
            s.Property(x => x.Code).IsRequired().HasMaxLength(ValidationConstants.HierarchyCodeMaxLength);
            s.Property(x => x.Name).IsRequired().HasMaxLength(ValidationConstants.HierarchyNameMaxLength);
            s.Property(x => x.MeasurementUnit).IsRequired().HasMaxLength(ValidationConstants.MeasurementUnitMaxLength);
        });

        // 5. Типове Операции и Счетоводен Журнал + Новите Бележки
        modelBuilder.Entity<OperationType>(o =>
        {
            o.Property(x => x.Code).IsRequired().HasMaxLength(ValidationConstants.OperationCodeMaxLength);
            o.Property(x => x.Name).IsRequired().HasMaxLength(ValidationConstants.OperationNameMaxLength);
        });

        modelBuilder.Entity<AssetTransaction>(t =>
        {
            t.Property(x => x.AssetId).IsRequired().HasMaxLength(ValidationConstants.AssetIdMaxLength);
            t.Property(x => x.FromFacility).IsRequired().HasMaxLength(ValidationConstants.FacilityTextMaxLength);
            t.Property(x => x.ToFacility).IsRequired().HasMaxLength(ValidationConstants.FacilityTextMaxLength);
            t.Property(x => x.DocumentReference).IsRequired().HasMaxLength(ValidationConstants.DocumentRefMaxLength);
            t.Property(x => x.Notes).HasMaxLength(ValidationConstants.TransactionNotesMaxLength);
        });

        // 5.1 ЗАЩИТА ЗА ЖУРНАЛА НА ТРАНСАКЦИИТЕ (AssetTransactions)
        modelBuilder.Entity<AssetTransaction>(t =>
        {
            t.Property(x => x.AssetId).IsRequired();
            t.Property(x => x.DocumentReference).IsRequired().HasMaxLength(ValidationConstants.DocumentRefMaxLength);
            t.Property(x => x.Notes).HasMaxLength(ValidationConstants.TransactionNotesMaxLength);

            // Счетоводни числови връзки без каскадно триене
            t.HasOne(x => x.FromFacility)
             .WithMany()
             .HasForeignKey(x => x.FromFacilityId)
             .OnDelete(DeleteBehavior.Restrict);

            t.HasOne(x => x.ToFacility)
             .WithMany()
             .HasForeignKey(x => x.ToFacilityId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // 6. Одитен Дневник
        modelBuilder.Entity<FacilityLog>(fl =>
        {
            fl.Property(x => x.EventType).IsRequired().HasMaxLength(ValidationConstants.LogEventTypeMaxLength);
            fl.Property(x => x.OldValue).IsRequired().HasMaxLength(ValidationConstants.LogValueMaxLength);
            fl.Property(x => x.NewValue).IsRequired().HasMaxLength(ValidationConstants.LogValueMaxLength);
        });

        // ГЛОБАЛНО ПРЕДОТВРАТЯВАНЕ НА КАСКАДНО ТРИЕНЕ
        foreach (var foreignKey in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
        {
            foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
        }

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
