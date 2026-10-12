using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnterpriseReportSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddGlobalAdminRoleToUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsGlobalAdmin",
                table: "SystemUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsGlobalAdmin",
                table: "SystemUsers");
        }
    }
}
