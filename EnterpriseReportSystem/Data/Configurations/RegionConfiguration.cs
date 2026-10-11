using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EnterpriseReportSystem.Models.Entities.Location;
using EnterpriseReportSystem.Common.Constants;

namespace EnterpriseReportSystem.Data.Configurations;

public class RegionConfiguration : IEntityTypeConfiguration<Region>
{
    // за Регионите с техните релационни защити против изтриване - изнеси географските настройки за Областите там
    public void Configure(EntityTypeBuilder<Region> builder)
    {
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(ValidationConstants.RegionNameMaxLength); // 40
    }
}
