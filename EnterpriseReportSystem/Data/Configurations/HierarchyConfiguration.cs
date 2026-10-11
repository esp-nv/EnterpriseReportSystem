using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EnterpriseReportSystem.Models.Entities.Hierarchy;
using EnterpriseReportSystem.Common.Constants;

namespace EnterpriseReportSystem.Data.Configurations;

//йерархията (Нива 1, 2 и 3)

public class MainTypeConfiguration : IEntityTypeConfiguration<MainType>
{
    public void Configure(EntityTypeBuilder<MainType> builder)
    {
        builder.Property(x => x.Code).IsRequired().HasMaxLength(ValidationConstants.HierarchyCodeMaxLength);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(ValidationConstants.HierarchyNameMaxLength);
    }
}

public class AssetGroupConfiguration : IEntityTypeConfiguration<AssetGroup>
{
    public void Configure(EntityTypeBuilder<AssetGroup> builder)
    {
        builder.Property(x => x.Code).IsRequired().HasMaxLength(ValidationConstants.HierarchyCodeMaxLength);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(ValidationConstants.HierarchyNameMaxLength);

        builder.HasOne(g => g.MainType)
            .WithMany(m => m.AssetGroups)
            .HasForeignKey(g => g.MainTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class AssetSubGroupConfiguration : IEntityTypeConfiguration<AssetSubGroup>
{
    public void Configure(EntityTypeBuilder<AssetSubGroup> builder)
    {
        builder.Property(x => x.Code).IsRequired().HasMaxLength(ValidationConstants.HierarchyCodeMaxLength);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(ValidationConstants.HierarchyNameMaxLength);
        builder.Property(x => x.MeasurementUnit).IsRequired().HasMaxLength(ValidationConstants.MeasurementUnitMaxLength);

        builder.HasOne(s => s.AssetGroup)
            .WithMany(g => g.AssetSubGroups)
            .HasForeignKey(s => s.AssetGroupId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
