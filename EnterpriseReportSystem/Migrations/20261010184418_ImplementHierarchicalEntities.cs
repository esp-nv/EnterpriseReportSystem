using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnterpriseReportSystem.Migrations
{
    /// <inheritdoc />
    public partial class ImplementHierarchicalEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAnimal",
                table: "AssetGroups");

            migrationBuilder.AddColumn<string>(
                name: "MainTypeCode",
                table: "AssetGroups",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "AssetSubGroups",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AssetGroupCode = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetSubGroups", x => x.Code);
                    table.ForeignKey(
                        name: "FK_AssetSubGroups_AssetGroups_AssetGroupCode",
                        column: x => x.AssetGroupCode,
                        principalTable: "AssetGroups",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MainTypes",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MainTypes", x => x.Code);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssetGroups_MainTypeCode",
                table: "AssetGroups",
                column: "MainTypeCode");

            migrationBuilder.CreateIndex(
                name: "IX_AssetSubGroups_AssetGroupCode",
                table: "AssetSubGroups",
                column: "AssetGroupCode");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetGroups_MainTypes_MainTypeCode",
                table: "AssetGroups",
                column: "MainTypeCode",
                principalTable: "MainTypes",
                principalColumn: "Code",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetGroups_MainTypes_MainTypeCode",
                table: "AssetGroups");

            migrationBuilder.DropTable(
                name: "AssetSubGroups");

            migrationBuilder.DropTable(
                name: "MainTypes");

            migrationBuilder.DropIndex(
                name: "IX_AssetGroups_MainTypeCode",
                table: "AssetGroups");

            migrationBuilder.DropColumn(
                name: "MainTypeCode",
                table: "AssetGroups");

            migrationBuilder.AddColumn<bool>(
                name: "IsAnimal",
                table: "AssetGroups",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
