using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EnterpriseReportSystem.Models.Entities.Structure;
using EnterpriseReportSystem.Common.Constants;

namespace EnterpriseReportSystem.Data.Configurations;

public class CompanyOrPersonConfiguration : IEntityTypeConfiguration<CompanyOrPerson>
{
    //за мулти-фирмените юридически лица в холдинга и техния физически адрес
    public void Configure(EntityTypeBuilder<CompanyOrPerson> builder)
    {
        builder.Property(x => x.ClientCode).IsRequired().HasMaxLength(ValidationConstants.ClientCodeMaxLength); // 20
        builder.Property(x => x.Name).IsRequired().HasMaxLength(ValidationConstants.CompanyNameMaxLength); // 150
        builder.Property(x => x.TaxIdentifier).IsRequired().HasMaxLength(ValidationConstants.TaxIdentifierMaxLength); // 15
        builder.Property(x => x.Address).IsRequired().HasMaxLength(ValidationConstants.CompanyAddressMaxLength); // 150

        builder.HasOne(c => c.RegisteredTown)
            .WithMany()
            .HasForeignKey(c => c.RegisteredTownId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
