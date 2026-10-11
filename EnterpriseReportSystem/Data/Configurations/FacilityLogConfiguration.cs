using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EnterpriseReportSystem.Models.Entities.Structure;
using EnterpriseReportSystem.Common.Constants;

namespace EnterpriseReportSystem.Data.Configurations;

public class FacilityLogConfiguration : IEntityTypeConfiguration<FacilityLog>
{
    // за логовете на събитията на обектите с техните релационни защити против изтриване  и за сигурност на одитния дневник

    public void Configure(EntityTypeBuilder<FacilityLog> builder)
    {
        builder.Property(x => x.EventType).IsRequired().HasMaxLength(ValidationConstants.LogEventTypeMaxLength);
        builder.Property(x => x.OldValue).IsRequired().HasMaxLength(ValidationConstants.LogValueMaxLength);
        builder.Property(x => x.NewValue).IsRequired().HasMaxLength(ValidationConstants.LogValueMaxLength);

        builder.HasOne(fl => fl.Facility)
            .WithMany(f => f.FacilityLogs)
            .HasForeignKey(fl => fl.FacilityId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
