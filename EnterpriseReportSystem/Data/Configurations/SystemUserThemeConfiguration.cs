using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EnterpriseReportSystem.Models.Entities.Structure;
using EnterpriseReportSystem.Common.Constants;

namespace EnterpriseReportSystem.Data.Configurations;

public class SystemUserThemeConfiguration : IEntityTypeConfiguration<SystemUserTheme>
{
    public void Configure(EntityTypeBuilder<SystemUserTheme> builder)
    {
        // 1. Първичен ключ
        builder.HasKey(x => x.Id);

        // 2. Валидация за контекста на елемента
        builder.Property(x => x.ElementContext)
            .IsRequired()
            .HasMaxLength(ValidationConstants.UIElementKeyMaxLength); // 50

        // 3. Валидация за Bootstrap класа
        builder.Property(x => x.BootstrapClass)
            .IsRequired()
            .HasMaxLength(ValidationConstants.ThemeBootstrapClassMaxLength); // 100

        // 4. Валидация за WordPress описанието на свойството
        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(ValidationConstants.UIElementDescMaxLength); // 200

        // 5. РЕЛАЦИЯ ЕДНО КЪМ МНОГО: Свързваме свойствата с мастър елементите (котвите)
        builder.HasOne(t => t.SystemUIElement)
            .WithMany()
            .HasForeignKey(t => t.SystemUIElementId)
            .OnDelete(DeleteBehavior.Cascade); // При изтриване на котва, се изчистват и свойствата ѝ

        // 6. Релация към потребителите
        builder.HasOne(t => t.SystemUser)
            .WithMany()
            .HasForeignKey(t => t.SystemUserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
