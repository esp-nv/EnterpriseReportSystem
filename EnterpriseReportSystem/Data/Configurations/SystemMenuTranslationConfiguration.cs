using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EnterpriseReportSystem.Models.Entities.Structure;
using EnterpriseReportSystem.Common.Constants;

namespace EnterpriseReportSystem.Data.Configurations;

public class SystemMenuTranslationConfiguration : IEntityTypeConfiguration<SystemMenuTranslation>
{
    public void Configure(EntityTypeBuilder<SystemMenuTranslation> builder)
    {
        builder.Property(x => x.TranslatedName).IsRequired().HasMaxLength(ValidationConstants.MenuNameMaxLength);
        builder.Property(x => x.TranslatedNotes).HasMaxLength(ValidationConstants.NotesMaxLength);

        builder.HasOne(x => x.SystemMenu)
            .WithMany()
            .HasForeignKey(x => x.SystemMenuId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Language)
            .WithMany()
            .HasForeignKey(x => x.LanguageId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
