using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EnterpriseReportSystem.Models.Entities.Structure;
using EnterpriseReportSystem.Common.Constants;

namespace EnterpriseReportSystem.Data.Configurations;

public class SystemUIElementConfiguration : IEntityTypeConfiguration<SystemUIElement>
{
    public void Configure(EntityTypeBuilder<SystemUIElement> builder)
    {
        // 1. Първичен ключ
        builder.HasKey(x => x.Id);

        // 2. Валидация за уникалния ключ на зоната (без магически числа)
        builder.Property(x => x.ElementKey)
            .IsRequired()
            .HasMaxLength(ValidationConstants.UIElementKeyMaxLength); // 50

        // 3. Валидация за четимото име на зоната
        builder.Property(x => x.DisplayName)
            .IsRequired()
            .HasMaxLength(ValidationConstants.UIElementNameMaxLength); // 100

        // 4. Индексиране за по-бързо LINQ търсене
        builder.HasIndex(x => x.ElementKey).IsUnique();
    }
}
