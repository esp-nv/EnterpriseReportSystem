using EnterpriseReportSystem.Common.Constants;
using EnterpriseReportSystem.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnterpriseReportSystem.Data.Configurations;

public class ExchangeRateHistoryConfiguration : IEntityTypeConfiguration<ExchangeRateHistory>
{
    // за историята на обменните курсове с техните релационни защити против изтриване
    public void Configure(EntityTypeBuilder<ExchangeRateHistory> builder)
    {
        // КОРЕКЦИЯ: Използваме String Interpolation с константите — без твърд код!
        builder.Property(e => e.Rate)
            .HasColumnType($"decimal({ValidationConstants.ExchangeRatePrecision},{ValidationConstants.ExchangeRateScale})")
            .IsRequired();

        builder.HasOne(e => e.FromCurrency)
            .WithMany()
            .HasForeignKey(e => e.FromCurrencyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.ToCurrency)
            .WithMany()
            .HasForeignKey(e => e.ToCurrencyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
