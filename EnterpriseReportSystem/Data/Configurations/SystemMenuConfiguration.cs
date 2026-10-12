using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EnterpriseReportSystem.Models.Entities.Structure;
using EnterpriseReportSystem.Common.Constants;

namespace EnterpriseReportSystem.Data.Configurations;

public class SystemMenuConfiguration : IEntityTypeConfiguration<SystemMenu>
{
    public void Configure(EntityTypeBuilder<SystemMenu> builder)
    {
        builder.Property(x => x.Name).IsRequired().HasMaxLength(ValidationConstants.MenuNameMaxLength);
        builder.Property(x => x.Notes).HasMaxLength(ValidationConstants.NotesMaxLength);

        // КАСКАДНА САМООБВЪРЗВАЩА СЕ ВРЪЗКА (Self-Referencing Relationship)
        builder.HasOne(x => x.ParentMenu)
            .WithMany(x => x.ChildMenus)
            .HasForeignKey(x => x.ParentMenuId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
