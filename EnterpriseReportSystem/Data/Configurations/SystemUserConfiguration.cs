using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EnterpriseReportSystem.Models.Entities.Structure;
using EnterpriseReportSystem.Common.Constants;

namespace EnterpriseReportSystem.Data.Configurations;

public class SystemUserConfiguration : IEntityTypeConfiguration<SystemUser>
{
    public void Configure(EntityTypeBuilder<SystemUser> builder)
    {
        builder.Property(x => x.Username).IsRequired().HasMaxLength(ValidationConstants.UsernameMaxLength);
        builder.Property(x => x.Password).IsRequired().HasMaxLength(ValidationConstants.UsernameMaxLength);
        builder.Property(x => x.LayoutType).IsRequired().HasMaxLength(30);

        // Защитаваме текстовото поле за тема
        builder.Property(x => x.ThemeType).IsRequired().HasMaxLength(30); // напр. "PHOTOSHOP"

        builder.HasOne(x => x.Company)
            .WithMany()
            .HasForeignKey(x => x.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
