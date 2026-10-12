using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EnterpriseReportSystem.Models.Entities.Structure;
using EnterpriseReportSystem.Common.Constants;

namespace EnterpriseReportSystem.Data.Configurations;

public class LanguageConfiguration : IEntityTypeConfiguration<Language>
{
    public void Configure(EntityTypeBuilder<Language> builder)
    {
        builder.Property(x => x.Code).IsRequired().HasMaxLength(ValidationConstants.LanguageCodeMaxLength);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(ValidationConstants.MenuNameMaxLength);
    }
}
