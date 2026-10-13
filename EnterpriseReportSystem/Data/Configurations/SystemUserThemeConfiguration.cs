using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EnterpriseReportSystem.Models.Entities.Structure;
using EnterpriseReportSystem.Common.Constants; // Осигуряваме достъп до константите

namespace EnterpriseReportSystem.Data.Configurations;

public class SystemUserThemeConfiguration : IEntityTypeConfiguration<SystemUserTheme>
{
    public void Configure(EntityTypeBuilder<SystemUserTheme> builder)
    {
        // 1. Дефинираме първичния ключ
        builder.HasKey(x => x.Id);

        // 2. Валидация за контекста на елемента без магически числа
        builder.Property(x => x.ElementContext)
            .IsRequired()
            .HasMaxLength(ValidationConstants.ThemeElementContextMaxLength); // Използваме 50

        // 3. Валидация за Bootstrap класа без магически числа
        builder.Property(x => x.BootstrapClass)
            .IsRequired()
            .HasMaxLength(ValidationConstants.ThemeBootstrapClassMaxLength); // Използваме 100

        // 4. Изграждане на релацията
        builder.HasOne(t => t.SystemUser)
            .WithMany()
            .HasForeignKey(t => t.SystemUserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
