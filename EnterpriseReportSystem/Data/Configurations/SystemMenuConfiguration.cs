using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EnterpriseReportSystem.Models.Entities.Structure;
using EnterpriseReportSystem.Common.Constants;

namespace EnterpriseReportSystem.Data.Configurations;

public class SystemMenuConfiguration : IEntityTypeConfiguration<SystemMenu>
{
    public void Configure(EntityTypeBuilder<SystemMenu> builder)
    {
        // КОРЕКЦИЯ: Нула магически числа! Използваме централизираните константи
        builder.Property(x => x.CustomCssClass)
            .HasMaxLength(ValidationConstants.MenuCssClassMaxLength); // 150

        builder.Property(x => x.CustomShortcut)
            .HasMaxLength(ValidationConstants.MenuShortcutMaxLength); // 30

        // КАСКАДНА САМООБВЪРЗВАЩА СЕ ВРЪЗКА (Self-Referencing Relationship)
        builder.HasOne(x => x.ParentMenu)
            .WithMany(x => x.ChildMenus)
            .HasForeignKey(x => x.ParentMenuId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
