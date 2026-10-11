using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EnterpriseReportSystem.Models.Entities;
using EnterpriseReportSystem.Common.Constants;

namespace EnterpriseReportSystem.Data.Configurations;

public class CurrencyConfiguration : IEntityTypeConfiguration<Currency>
{
    //за валутната архитектура
    public void Configure(EntityTypeBuilder<Currency> builder)
    {
        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(ValidationConstants.CurrencyCodeLength); // 3

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(ValidationConstants.CurrencyNameMaxLength); // 50
    }
}
