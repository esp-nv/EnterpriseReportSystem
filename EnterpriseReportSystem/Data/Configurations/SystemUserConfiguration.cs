using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EnterpriseReportSystem.Models.Entities.Structure;
using EnterpriseReportSystem.Common.Constants;

namespace EnterpriseReportSystem.Data.Configurations;

public class SystemUserConfiguration : IEntityTypeConfiguration<SystemUser>
{
    public void Configure(EntityTypeBuilder<SystemUser> builder)
    {
        // 1. Потребителски идентификатори, извлечени строго от константите
        builder.Property(x => x.Username)
            .IsRequired()
            .HasMaxLength(ValidationConstants.UsernameMaxLength);

        builder.Property(x => x.Password)
            .IsRequired()
            .HasMaxLength(ValidationConstants.PasswordMaxLength);

        builder.Property(x => x.IsGlobalAdmin)
            .IsRequired();

        builder.Property(x => x.WishListMenuIds)
            .HasMaxLength(ValidationConstants.WishListMenuIdsMaxLength);

        // 2. Системно оформление и радио-селект параметри
        builder.Property(x => x.LayoutType)
            .IsRequired();

        builder.Property(x => x.IsMenuExpanded)
            .IsRequired();

        // ДОБАВЕНО: Задължително бит-поле, което контролира радио бутоните във формата
        builder.Property(x => x.IsCustomPaletteActive)
            .IsRequired();

        builder.Property(x => x.IsDarkUserPreference)
            .IsRequired();

        // 3. РЕЛАЦИЯ КЪМ СУБЕКТИТЕ / КОМПАНИИТЕ
        builder.HasOne(x => x.Company)
            .WithMany()
            .HasForeignKey(x => x.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        // 4. РЕЛАЦИЯ КЪМ ЦЕНТРАЛНИЯ КАТАЛОГ НА ТЕМИТЕ (GlobalThemes)
        builder.HasOne(x => x.GlobalTheme)
            .WithMany()
            .HasForeignKey(x => x.ThemeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
