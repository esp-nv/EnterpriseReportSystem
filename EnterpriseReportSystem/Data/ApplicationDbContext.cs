using Microsoft.EntityFrameworkCore;
using EnterpriseReportSystem.Models.Entities;
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

        // 2. ПОСТАВЯНЕ НА ПЪРВИЧНИТЕ КЛЮЧОВЕ (PRIMARY KEYS)
        modelBuilder.Entity<MainType>().HasKey(m => m.Code);
        modelBuilder.Entity<AssetGroup>().HasKey(g => g.Code);
        modelBuilder.Entity<AssetSubGroup>().HasKey(s => s.Code);
        modelBuilder.Entity<OperationType>().HasKey(o => o.Code);

        // 3. КОНФИГУРИРАНЕ НА ЙЕРАРХИЧНИТЕ ВРЪЗКИ (ONE-TO-MANY)
        modelBuilder.Entity<AssetGroup>()
            .HasOne(g => g.MainType)
            .WithMany(m => m.AssetGroups)
            .HasForeignKey(g => g.MainTypeCode)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<AssetSubGroup>()
            .HasOne(s => s.AssetGroup)
            .WithMany(g => g.AssetSubGroups)
            .HasForeignKey(s => s.AssetGroupCode)
            .OnDelete(DeleteBehavior.Restrict);

        // 4. ТВОЯТА ЗАЩИТА: Предотвратяване на каскадно триене за абсолютно ВСИЧКИ връзки
        foreach (var foreignKey in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
        {
            foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
        }

        // 5. ТВОЯТ ГЛОБАЛЕН ФИЛТЪР: Автоматично скриване на логически изтритите трансакции
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
