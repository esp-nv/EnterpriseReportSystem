using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EnterpriseReportSystem.Models.Entities.Structure;
using EnterpriseReportSystem.Common.Constants;

namespace EnterpriseReportSystem.Data.Configurations;

public class FacilityConfiguration : IEntityTypeConfiguration<Facility>
{
    // за физическите обекти, ферми и бази
    public void Configure(EntityTypeBuilder<Facility> builder)
    {
        builder.Property(x => x.FacilityCode).IsRequired().HasMaxLength(ValidationConstants.FacilityCodeMaxLength); // 30
        builder.Property(x => x.Name).IsRequired().HasMaxLength(ValidationConstants.FacilityNameMaxLength); // 100
        builder.Property(x => x.PostalCode).IsRequired().HasMaxLength(ValidationConstants.PostalCodeMaxLength); // 10
        builder.Property(x => x.Notes).HasMaxLength(ValidationConstants.NotesMaxLength); // 500
        builder.Property(x => x.Address).IsRequired().HasMaxLength(ValidationConstants.FacilityAddressMaxLength); // 150

        builder.HasOne(f => f.CompanyOrPerson)
            .WithMany(c => c.Facilities)
            .HasForeignKey(f => f.CompanyOrPersonId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(f => f.TownOrVillage)
            .WithMany()
            .HasForeignKey(f => f.TownOrVillageId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
