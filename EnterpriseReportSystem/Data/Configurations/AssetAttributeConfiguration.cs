using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EnterpriseReportSystem.Models.Entities.Ledger;
using EnterpriseReportSystem.Common.Constants;

namespace EnterpriseReportSystem.Data.Configurations;

public class AssetAttributeConfiguration : IEntityTypeConfiguration<AssetAttribute>
{
    public void Configure(EntityTypeBuilder<AssetAttribute> builder)
    {
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(ValidationConstants.AttributeNameMaxLength); // 60 знака
    }
}
