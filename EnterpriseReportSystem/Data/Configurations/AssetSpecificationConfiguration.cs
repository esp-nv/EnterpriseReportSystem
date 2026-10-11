using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EnterpriseReportSystem.Models.Entities.Hierarchy;
using EnterpriseReportSystem.Common.Constants;

namespace EnterpriseReportSystem.Data.Configurations;

public class AssetSpecificationConfiguration : IEntityTypeConfiguration<AssetSpecification>
{
    public void Configure(EntityTypeBuilder<AssetSpecification> builder)
    {
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(ValidationConstants.SpecificationNameMaxLength); // 100 символа за порода/модел

        // Свързваме с Подгрупата без каскадно триене
        builder.HasOne(x => x.AssetSubGroup)
            .WithMany()
            .HasForeignKey(x => x.AssetSubGroupId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
