using EnterpriseReportSystem.Models.Entities;
using EnterpriseReportSystem.Models.Entities.Hierarchy;
using EnterpriseReportSystem.Models.Entities.Ledger;
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

    // 1. ДЕФИНИРАНЕ НА ВСИЧКИ РЕЛАЦИОННИ ТАБЛИЦИ В SQL SERVER
    public DbSet<MainType> MainTypes { get; set; } = null!;
    public DbSet<AssetGroup> AssetGroups { get; set; } = null!;
    public DbSet<AssetSubGroup> AssetSubGroups { get; set; } = null!;
    public DbSet<OperationType> OperationTypes { get; set; } = null!;
    public DbSet<AssetTransaction> AssetTransactions { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 1. ПОСТАВЯНЕ НА ЧИСЛОВИТЕ ПЪРВИЧНИ КЛЮЧОВЕ (PRIMARY KEYS)
        modelBuilder.Entity<MainType>().HasKey(m => m.Id);
        modelBuilder.Entity<AssetGroup>().HasKey(g => g.Id);
        modelBuilder.Entity<AssetSubGroup>().HasKey(s => s.Id);
        modelBuilder.Entity<OperationType>().HasKey(o => o.Code);

        // 2. КОНФИГУРИРАНЕ НА ГЕОГРАФСКИТЕ ВРЪЗКИ (ОБЛАСТ -> ОБЩИНА -> СЕЛО)
        modelBuilder.Entity<Municipality>()
            .HasOne(m => m.Region)
            .WithMany(r => r.Municipalities)
            .HasForeignKey(m => m.RegionId)
            .OnDelete(DeleteBehavior.Restrict);

        // Update exactly this block around Line 43-47 to match the corrected collection property:
        modelBuilder.Entity<TownOrVillage>()
            .HasOne(t => t.Municipality)
            .WithMany(m => m.TownsOrVillages) // ADDED THE 's' HERE to match the Municipality entity collection
            .HasForeignKey(t => t.MunicipalityId)
            .OnDelete(DeleteBehavior.Restrict);


        // 3. КОНФИГУРИРАНЕ НА ЙЕРАРХИЧНИТЕ ВРЪЗКИ ЧРЕЗ ЧИСЛОВИ ИДЕНТИФИКАТОРИ (Id КЛЮЧОВЕ)
        modelBuilder.Entity<AssetGroup>()
            .HasOne(g => g.MainType)
            .WithMany(m => m.AssetGroups)
            .HasForeignKey(g => g.MainTypeId) // ПРАВИЛНИЯТ ЧИСЛОВ КЛЮЧ
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<AssetSubGroup>()
            .HasOne(s => s.AssetGroup)
            .WithMany(g => g.AssetSubGroups)
            .HasForeignKey(s => s.AssetGroupId) // ПРАВИЛНИЯТ ЧИСЛОВ КЛЮЧ
            .OnDelete(DeleteBehavior.Restrict);

        // 4. КОНФИГУРИРАНЕ НА МУЛТИ-ФИРМЕНИТЕ ОБЕКТИ И АДМИНИСТРАЦИЯТА
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

        // ТВОЯТА ЗЛАТНА ЗАЩИТА: Предотвратяване на каскадно триене за абсолютно ВСИЧКИ връзки
        foreach (var foreignKey in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
        {
            foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
        }

        // ТВОЯТ ГЛОБАЛЕН ФИЛТЪР: Скриване на логически изтритите трансакции през Soft Delete
        modelBuilder.Entity<AssetTransaction>().HasQueryFilter(t => !t.IsDeleted);
    }


    // 6. ТВОЯТА МАГИЯ ЗА АВТОМАТИЧЕН SOFT DELETE
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
                entry.State = EntityState.Modified; // Превръщаме триенето в обновяване
                entry.Entity.IsDeleted = true;      // Маркираме като изтрит
                entry.Entity.DeletedOn = System.DateTime.Now;
            }
        }
    }
}
