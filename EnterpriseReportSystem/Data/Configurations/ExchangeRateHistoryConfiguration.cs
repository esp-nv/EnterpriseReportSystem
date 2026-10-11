using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EnterpriseReportSystem.Models.Entities;

namespace EnterpriseReportSystem.Data.Configurations;

public class ExchangeRateHistoryConfiguration : IEntityTypeConfiguration<ExchangeRateHistory>
{
    // за историята на обменните курсове с техните релационни защити против изтриване
    public void Configure(EntityTypeBuilder<ExchangeRateHistory> builder)
    {
        builder.Property(e => e.Rate)
            .HasColumnType("decimal(18,5)")
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
