using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnterpriseReportSystem.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AssetGroups",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsAnimal = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetGroups", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "OperationTypes",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Direction = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperationTypes", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "AssetTransactions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AssetId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OperationCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FromGroupCode = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    FromFacility = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ToGroupCode = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ToFacility = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentReference = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssetTransactions_AssetGroups_FromGroupCode",
                        column: x => x.FromGroupCode,
                        principalTable: "AssetGroups",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssetTransactions_AssetGroups_ToGroupCode",
                        column: x => x.ToGroupCode,
                        principalTable: "AssetGroups",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssetTransactions_OperationTypes_OperationCode",
                        column: x => x.OperationCode,
                        principalTable: "OperationTypes",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssetTransactions_FromGroupCode",
                table: "AssetTransactions",
                column: "FromGroupCode");

            migrationBuilder.CreateIndex(
                name: "IX_AssetTransactions_OperationCode",
                table: "AssetTransactions",
                column: "OperationCode");

            migrationBuilder.CreateIndex(
                name: "IX_AssetTransactions_ToGroupCode",
                table: "AssetTransactions",
                column: "ToGroupCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssetTransactions");

            migrationBuilder.DropTable(
                name: "AssetGroups");

            migrationBuilder.DropTable(
                name: "OperationTypes");
        }
    }
}
