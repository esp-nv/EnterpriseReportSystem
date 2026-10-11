using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EnterpriseReportSystem.Models.Entities.Location;
using EnterpriseReportSystem.Common.Constants;

namespace EnterpriseReportSystem.Data.Configurations;

public class MunicipalityConfiguration : IEntityTypeConfiguration<Municipality>
{
    //за Общините с техните релационни защити против изтриване
    public void Configure(EntityTypeBuilder<Municipality> builder)
    {
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(ValidationConstants.MunicipalityNameMaxLength); // 60

        builder.HasOne(m => m.Region)
            .WithMany(r => r.Municipalities)
            .HasForeignKey(m => m.RegionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
