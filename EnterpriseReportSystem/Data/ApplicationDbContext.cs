using Microsoft.EntityFrameworkCore;
using EnterpriseReportSystem.Models.Entities;

namespace EnterpriseReportSystem.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Дефинираме таблиците в SQL Server
    public DbSet<AssetGroup> AssetGroups { get; set; } = null!;
    public DbSet<OperationType> OperationTypes { get; set; } = null!;
    public DbSet<AssetTransaction> AssetTransactions { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Поставяме първични ключове на номенклатурите, тъй като нямат стандартно ID
        modelBuilder.Entity<AssetGroup>().HasKey(g => g.Code);
        modelBuilder.Entity<OperationType>().HasKey(o => o.Code);
    }
}
