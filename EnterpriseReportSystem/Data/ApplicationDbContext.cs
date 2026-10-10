using Microsoft.EntityFrameworkCore;
using EnterpriseReportSystem.Models.Entities;
using System.Linq;

namespace EnterpriseReportSystem.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<AssetGroup> AssetGroups { get; set; } = null!;
    public DbSet<OperationType> OperationTypes { get; set; } = null!;
    public DbSet<AssetTransaction> AssetTransactions { get; set; } = null!;
    public object MainTypes { get; internal set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 1. Първични ключове на номенклатурите
        modelBuilder.Entity<AssetGroup>().HasKey(g => g.Code);
        modelBuilder.Entity<OperationType>().HasKey(o => o.Code);

        // 2. Твоят код: Предотвратяване на каскадно триене за ВСИЧКИ връзки в системата
        foreach (var foreignKey in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
        {
            foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
        }

        // 3. Автоматичен Глобален Филтър за Soft Delete за трансакциите (или всеки друг наследник)
        modelBuilder.Entity<AssetTransaction>()
            .HasQueryFilter(t => !t.IsDeleted);
    }

    // 4. Магията на автоматизацията: Пренаписваме метода SaveChanges, 
    // за да може при команда .Remove() обектът да се маркира като изтрит, вместо да се заличи от SQL
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
            switch (entry.State)
            {
                case EntityState.Deleted:
                    entry.State = EntityState.Modified; // Сменяме операцията на Update
                    entry.Entity.IsDeleted = true;      // Маркираме като изтрит
                    entry.Entity.DeletedOn = System.DateTime.Now;
                    break;
            }
        }
    }
}
