using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EnterpriseReportSystem.Models.Entities.Ledger;
using EnterpriseReportSystem.Common.Constants;

namespace EnterpriseReportSystem.Data.Configurations;

public class AssetTransactionConfiguration : IEntityTypeConfiguration<AssetTransaction>
{
    // за транзакциите на активите с техните релационни защити против изтриване - счетоводен блок, който управлява числовите релации
    public void Configure(EntityTypeBuilder<AssetTransaction> builder)
    {
        builder.Property(x => x.AssetId).IsRequired();
        builder.Property(x => x.DocumentReference).IsRequired().HasMaxLength(ValidationConstants.DocumentRefMaxLength);
        builder.Property(x => x.Notes).HasMaxLength(ValidationConstants.TransactionNotesMaxLength);

        // Връзки към обектите
        builder.HasOne(x => x.FromFacility)
            .WithMany()
            .HasForeignKey(x => x.FromFacilityId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ToFacility)
            .WithMany()
            .HasForeignKey(x => x.ToFacilityId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Operation)
            .WithMany()
            .HasForeignKey(x => x.OperationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
