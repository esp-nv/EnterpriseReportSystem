using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnterpriseReportSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddSystemUserThemesFluentConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCustomPaletteActive",
                table: "SystemUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "SystemUserThemes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SystemUserId = table.Column<int>(type: "int", nullable: false),
                    ElementContext = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BootstrapClass = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemUserThemes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SystemUserThemes_SystemUsers_SystemUserId",
                        column: x => x.SystemUserId,
                        principalTable: "SystemUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SystemUserThemes_SystemUserId",
                table: "SystemUserThemes",
                column: "SystemUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SystemUserThemes");

            migrationBuilder.DropColumn(
                name: "IsCustomPaletteActive",
                table: "SystemUsers");
        }
    }
}
