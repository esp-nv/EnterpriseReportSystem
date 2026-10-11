using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EnterpriseReportSystem.Models.Entities.Location;
using EnterpriseReportSystem.Common.Constants;

namespace EnterpriseReportSystem.Data.Configurations;

public class TownOrVillageConfiguration : IEntityTypeConfiguration<TownOrVillage>
{
    // за населените места, градове и села
    public void Configure(EntityTypeBuilder<TownOrVillage> builder)
    {
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(ValidationConstants.TownOrVillageNameMaxLength); // 100

        builder.Property(x => x.PostalCode)
            .IsRequired()
            .HasMaxLength(ValidationConstants.PostalCodeMaxLength); // 10

        builder.HasOne(t => t.Municipality)
            .WithMany(m => m.TownsOrVillages)
            .HasForeignKey(t => t.MunicipalityId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
