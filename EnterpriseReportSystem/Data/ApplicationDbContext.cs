using EnterpriseReportSystem.Common.Constants;
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

        // ТВОЯТА ФИНАНСОВА ЗАЩИТА: Задаваме децинална точност за валутните курсове (18 цифри общо, 5 след запетаята)
        modelBuilder.Entity<ExchangeRateHistory>()
            .Property(e => e.Rate)
            .HasColumnType("decimal(18,5)");
        // ==========================================
        // ТВОЯТА ЗАЩИТА С ГЛОБАЛНИ КОНСТАНТИ 
        // ==========================================

        // 1. ЗАЩИТА ЗА ВАЛУТИТЕ (Currencies)
        modelBuilder.Entity<Currency>(c =>
        {
            c.Property(x => x.Code).IsRequired().HasMaxLength(ValidationConstants.CurrencyCodeLength);
            c.Property(x => x.Name).IsRequired().HasMaxLength(ValidationConstants.CurrencyNameMaxLength);
        });

        // 2. ЗАЩИТА ЗА СУБЕКТИТЕ (CompaniesOrPersons)
        modelBuilder.Entity<CompanyOrPerson>(c =>
        {
            c.Property(x => x.ClientCode).IsRequired().HasMaxLength(ValidationConstants.ClientCodeMaxLength);
            c.Property(x => x.Name).IsRequired().HasMaxLength(ValidationConstants.CompanyNameMaxLength);
            c.Property(x => x.TaxIdentifier).IsRequired().HasMaxLength(ValidationConstants.TaxIdentifierMaxLength);
        });

        // 3. ЗАЩИТА ЗА ФИЗИЧЕСКИТЕ ОБЕКТИ (Facilities)
        modelBuilder.Entity<Facility>(f =>
        {
            f.Property(x => x.FacilityCode).IsRequired().HasMaxLength(ValidationConstants.FacilityCodeMaxLength);
            f.Property(x => x.Name).IsRequired().HasMaxLength(ValidationConstants.FacilityNameMaxLength);
            f.Property(x => x.PostalCode).IsRequired().HasMaxLength(ValidationConstants.PostalCodeMaxLength);
            f.Property(x => x.Notes).HasMaxLength(ValidationConstants.NotesMaxLength);
        });

        // 4. ЗАЩИТА ЗА ЙЕРАРХИЯТА (Нива 1, 2, 3)
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
        // 5. ЗАЩИТА ЗА ЖУРНАЛА НА ТРАНСАКЦИИТЕ (AssetTransactions)
        modelBuilder.Entity<AssetTransaction>(t =>
        {
            t.Property(x => x.AssetId).IsRequired().HasMaxLength(ValidationConstants.AssetIdMaxLength);
            t.Property(x => x.FromFacility).IsRequired().HasMaxLength(ValidationConstants.FacilityTextMaxLength);
            t.Property(x => x.ToFacility).IsRequired().HasMaxLength(ValidationConstants.FacilityTextMaxLength);
            t.Property(x => x.DocumentReference).IsRequired().HasMaxLength(ValidationConstants.DocumentRefMaxLength);
        });

        // 6. ЗАЩИТА ЗА ТИПОВЕТЕ ОПЕРАЦИИ (OperationTypes)
        modelBuilder.Entity<OperationType>(o =>
        {
            o.Property(x => x.Name).IsRequired().HasMaxLength(ValidationConstants.OperationNameMaxLength);
        });

        // 7. ЗАЩИТА ЗА ОДИТНИЯ ДНЕВНИК (FacilityLogs)
        modelBuilder.Entity<FacilityLog>(fl =>
        {
            fl.Property(x => x.EventType).IsRequired().HasMaxLength(ValidationConstants.LogEventTypeMaxLength);
            fl.Property(x => x.OldValue).IsRequired().HasMaxLength(ValidationConstants.LogValueMaxLength);
            fl.Property(x => x.NewValue).IsRequired().HasMaxLength(ValidationConstants.LogValueMaxLength);
        });
        // ==========================================
        // ГЕОГРАФСКА ЗАЩИТА СЪС САМОСТОЯТЕЛНИ КОНСТАНТИ
        // ==========================================
        modelBuilder.Entity<Region>(r =>
        {
            r.Property(x => x.Name).IsRequired().HasMaxLength(ValidationConstants.RegionNameMaxLength); // 40
        });

        modelBuilder.Entity<Municipality>(m =>
        {
            m.Property(x => x.Name).IsRequired().HasMaxLength(ValidationConstants.MunicipalityNameMaxLength); // 60
        });

        modelBuilder.Entity<TownOrVillage>(t =>
        {
            t.Property(x => x.Name).IsRequired().HasMaxLength(ValidationConstants.TownOrVillageNameMaxLength); // 100
            t.Property(x => x.PostalCode).IsRequired().HasMaxLength(ValidationConstants.PostalCodeMaxLength); // 10
        });

        // ==========================================
        // ЗАЩИТА ЗА СУБЕКТИТЕ И ФИЗИЧЕСКИТЕ АДРЕСИ
        // ==========================================
        modelBuilder.Entity<CompanyOrPerson>(c =>
        {
            c.Property(x => x.ClientCode).IsRequired().HasMaxLength(ValidationConstants.ClientCodeMaxLength); // 20
            c.Property(x => x.Name).IsRequired().HasMaxLength(ValidationConstants.CompanyNameMaxLength); // 150
            c.Property(x => x.TaxIdentifier).IsRequired().HasMaxLength(ValidationConstants.TaxIdentifierMaxLength); // 15

            // Новата колона за физически адрес на фирмата/лицето
            c.Property(x => x.Address).IsRequired().HasMaxLength(ValidationConstants.CompanyAddressMaxLength); // 150
        });

        modelBuilder.Entity<Facility>(f =>
        {
            f.Property(x => x.FacilityCode).IsRequired().HasMaxLength(ValidationConstants.FacilityCodeMaxLength); // 30
            f.Property(x => x.Name).IsRequired().HasMaxLength(ValidationConstants.FacilityNameMaxLength); // 100
            f.Property(x => x.PostalCode).IsRequired().HasMaxLength(ValidationConstants.PostalCodeMaxLength); // 10
            f.Property(x => x.Notes).HasMaxLength(ValidationConstants.NotesMaxLength); // 500

            // Новата колона за точен адрес / местност на самата ферма/база
            f.Property(x => x.Address).IsRequired().HasMaxLength(ValidationConstants.FacilityAddressMaxLength); // 150
        });

        // ==========================================
        // ТИПОВЕ ОПЕРАЦИИ И СЧЕТОВОДЕН ЖУРНАЛ
        // ==========================================
        modelBuilder.Entity<OperationType>(o =>
        {
            o.Property(x => x.Code).IsRequired().HasMaxLength(ValidationConstants.OperationCodeMaxLength); // 20
            o.Property(x => x.Name).IsRequired().HasMaxLength(ValidationConstants.OperationNameMaxLength); // 100
        });

        modelBuilder.Entity<AssetTransaction>(t =>
        {
            t.Property(x => x.AssetId).IsRequired().HasMaxLength(ValidationConstants.AssetIdMaxLength); // 50
            t.Property(x => x.FromFacility).IsRequired().HasMaxLength(ValidationConstants.FacilityTextMaxLength); // 100
            t.Property(x => x.ToFacility).IsRequired().HasMaxLength(ValidationConstants.FacilityTextMaxLength); // 100
            t.Property(x => x.DocumentReference).IsRequired().HasMaxLength(ValidationConstants.DocumentRefMaxLength); // 150

            // Новата колона за счетоводни бележки по трансакцията
            t.Property(x => x.Notes).HasMaxLength(ValidationConstants.TransactionNotesMaxLength); // 500
        });



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
