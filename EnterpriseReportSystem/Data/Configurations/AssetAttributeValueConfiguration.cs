using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EnterpriseReportSystem.Models.Entities.Ledger;
using EnterpriseReportSystem.Common.Constants;

namespace EnterpriseReportSystem.Data.Configurations;

public class AssetAttributeValueConfiguration : IEntityTypeConfiguration<AssetAttributeValue>
{
    public void Configure(EntityTypeBuilder<AssetAttributeValue> builder)
    {
        builder.Property(x => x.Value)
            .IsRequired()
            .HasMaxLength(ValidationConstants.AttributeValueMaxLength); // 250 знака

        // Защита на връзките против каскадно изтриване в ЕАВ модела
        builder.HasOne(x => x.Transaction)
            .WithMany()
            .HasForeignKey(x => x.TransactionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.AssetAttribute)
            .WithMany()
            .HasForeignKey(x => x.AssetAttributeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
