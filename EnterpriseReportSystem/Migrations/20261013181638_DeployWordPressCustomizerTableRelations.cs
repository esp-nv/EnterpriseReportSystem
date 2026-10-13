using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnterpriseReportSystem.Migrations
{
    /// <inheritdoc />
    public partial class DeployWordPressCustomizerTableRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "SystemUserThemes",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "SystemUIElementId",
                table: "SystemUserThemes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "SystemUIElements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ElementKey = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemUIElements", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SystemUserThemes_SystemUIElementId",
                table: "SystemUserThemes",
                column: "SystemUIElementId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemUIElements_ElementKey",
                table: "SystemUIElements",
                column: "ElementKey",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SystemUserThemes_SystemUIElements_SystemUIElementId",
                table: "SystemUserThemes",
                column: "SystemUIElementId",
                principalTable: "SystemUIElements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SystemUserThemes_SystemUIElements_SystemUIElementId",
                table: "SystemUserThemes");

            migrationBuilder.DropTable(
                name: "SystemUIElements");

            migrationBuilder.DropIndex(
                name: "IX_SystemUserThemes_SystemUIElementId",
                table: "SystemUserThemes");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "SystemUserThemes");

            migrationBuilder.DropColumn(
                name: "SystemUIElementId",
                table: "SystemUserThemes");
        }
    }
}
