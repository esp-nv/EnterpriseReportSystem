using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnterpriseReportSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddCurrencyAndExchangeRateTablesWithFivePrecision : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetTransactions_OperationTypes_OperationCode",
                table: "AssetTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyOrPerson_TownOrVillage_RegisteredTownId",
                table: "CompanyOrPerson");

            migrationBuilder.DropForeignKey(
                name: "FK_Facility_CompanyOrPerson_CompanyOrPersonId",
                table: "Facility");

            migrationBuilder.DropForeignKey(
                name: "FK_Facility_TownOrVillage_TownOrVillageId",
                table: "Facility");

            migrationBuilder.DropForeignKey(
                name: "FK_FacilityLog_Facility_FacilityId",
                table: "FacilityLog");

            migrationBuilder.DropTable(
                name: "TownOrVillage");

            migrationBuilder.DropTable(
                name: "Municipality");

            migrationBuilder.DropTable(
                name: "Region");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OperationTypes",
                table: "OperationTypes");

            migrationBuilder.DropIndex(
                name: "IX_AssetTransactions_OperationCode",
                table: "AssetTransactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FacilityLog",
                table: "FacilityLog");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Facility",
                table: "Facility");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CompanyOrPerson",
                table: "CompanyOrPerson");

            migrationBuilder.DropColumn(
                name: "OperationCode",
                table: "AssetTransactions");

            migrationBuilder.RenameTable(
                name: "FacilityLog",
                newName: "FacilityLogs");

            migrationBuilder.RenameTable(
                name: "Facility",
                newName: "Facilities");

            migrationBuilder.RenameTable(
                name: "CompanyOrPerson",
                newName: "CompaniesOrPersons");

            migrationBuilder.RenameIndex(
                name: "IX_FacilityLog_FacilityId",
                table: "FacilityLogs",
                newName: "IX_FacilityLogs_FacilityId");

            migrationBuilder.RenameIndex(
                name: "IX_Facility_TownOrVillageId",
                table: "Facilities",
                newName: "IX_Facilities_TownOrVillageId");

            migrationBuilder.RenameIndex(
                name: "IX_Facility_CompanyOrPersonId",
                table: "Facilities",
                newName: "IX_Facilities_CompanyOrPersonId");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyOrPerson_RegisteredTownId",
                table: "CompaniesOrPersons",
                newName: "IX_CompaniesOrPersons_RegisteredTownId");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "OperationTypes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "OperationTypes",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "OperationId",
                table: "AssetTransactions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OperationTypes",
                table: "OperationTypes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FacilityLogs",
                table: "FacilityLogs",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Facilities",
                table: "Facilities",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CompaniesOrPersons",
                table: "CompaniesOrPersons",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Currencies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currencies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Regions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Regions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExchangeRateHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FromCurrencyId = table.Column<int>(type: "int", nullable: false),
                    ToCurrencyId = table.Column<int>(type: "int", nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(18,5)", nullable: false),
                    RateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsFixed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExchangeRateHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExchangeRateHistories_Currencies_FromCurrencyId",
                        column: x => x.FromCurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExchangeRateHistories_Currencies_ToCurrencyId",
                        column: x => x.ToCurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Municipalities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Municipalities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Municipalities_Regions_RegionId",
                        column: x => x.RegionId,
                        principalTable: "Regions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TownsOrVillages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MunicipalityId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TownsOrVillages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TownsOrVillages_Municipalities_MunicipalityId",
                        column: x => x.MunicipalityId,
                        principalTable: "Municipalities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssetTransactions_OperationId",
                table: "AssetTransactions",
                column: "OperationId");

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeRateHistories_FromCurrencyId",
                table: "ExchangeRateHistories",
                column: "FromCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeRateHistories_ToCurrencyId",
                table: "ExchangeRateHistories",
                column: "ToCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Municipalities_RegionId",
                table: "Municipalities",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_TownsOrVillages_MunicipalityId",
                table: "TownsOrVillages",
                column: "MunicipalityId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetTransactions_OperationTypes_OperationId",
                table: "AssetTransactions",
                column: "OperationId",
                principalTable: "OperationTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CompaniesOrPersons_TownsOrVillages_RegisteredTownId",
                table: "CompaniesOrPersons",
                column: "RegisteredTownId",
                principalTable: "TownsOrVillages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Facilities_CompaniesOrPersons_CompanyOrPersonId",
                table: "Facilities",
                column: "CompanyOrPersonId",
                principalTable: "CompaniesOrPersons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Facilities_TownsOrVillages_TownOrVillageId",
                table: "Facilities",
                column: "TownOrVillageId",
                principalTable: "TownsOrVillages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FacilityLogs_Facilities_FacilityId",
                table: "FacilityLogs",
                column: "FacilityId",
                principalTable: "Facilities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetTransactions_OperationTypes_OperationId",
                table: "AssetTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_CompaniesOrPersons_TownsOrVillages_RegisteredTownId",
                table: "CompaniesOrPersons");

            migrationBuilder.DropForeignKey(
                name: "FK_Facilities_CompaniesOrPersons_CompanyOrPersonId",
                table: "Facilities");

            migrationBuilder.DropForeignKey(
                name: "FK_Facilities_TownsOrVillages_TownOrVillageId",
                table: "Facilities");

            migrationBuilder.DropForeignKey(
                name: "FK_FacilityLogs_Facilities_FacilityId",
                table: "FacilityLogs");

            migrationBuilder.DropTable(
                name: "ExchangeRateHistories");

            migrationBuilder.DropTable(
                name: "TownsOrVillages");

            migrationBuilder.DropTable(
                name: "Currencies");

            migrationBuilder.DropTable(
                name: "Municipalities");

            migrationBuilder.DropTable(
                name: "Regions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OperationTypes",
                table: "OperationTypes");

            migrationBuilder.DropIndex(
                name: "IX_AssetTransactions_OperationId",
                table: "AssetTransactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FacilityLogs",
                table: "FacilityLogs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Facilities",
                table: "Facilities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CompaniesOrPersons",
                table: "CompaniesOrPersons");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "OperationTypes");

            migrationBuilder.DropColumn(
                name: "OperationId",
                table: "AssetTransactions");

            migrationBuilder.RenameTable(
                name: "FacilityLogs",
                newName: "FacilityLog");

            migrationBuilder.RenameTable(
                name: "Facilities",
                newName: "Facility");

            migrationBuilder.RenameTable(
                name: "CompaniesOrPersons",
                newName: "CompanyOrPerson");

            migrationBuilder.RenameIndex(
                name: "IX_FacilityLogs_FacilityId",
                table: "FacilityLog",
                newName: "IX_FacilityLog_FacilityId");

            migrationBuilder.RenameIndex(
                name: "IX_Facilities_TownOrVillageId",
                table: "Facility",
                newName: "IX_Facility_TownOrVillageId");

            migrationBuilder.RenameIndex(
                name: "IX_Facilities_CompanyOrPersonId",
                table: "Facility",
                newName: "IX_Facility_CompanyOrPersonId");

            migrationBuilder.RenameIndex(
                name: "IX_CompaniesOrPersons_RegisteredTownId",
                table: "CompanyOrPerson",
                newName: "IX_CompanyOrPerson_RegisteredTownId");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "OperationTypes",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "OperationCode",
                table: "AssetTransactions",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OperationTypes",
                table: "OperationTypes",
                column: "Code");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FacilityLog",
                table: "FacilityLog",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Facility",
                table: "Facility",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CompanyOrPerson",
                table: "CompanyOrPerson",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Region",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Region", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Municipality",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RegionId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Municipality", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Municipality_Region_RegionId",
                        column: x => x.RegionId,
                        principalTable: "Region",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TownOrVillage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MunicipalityId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TownOrVillage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TownOrVillage_Municipality_MunicipalityId",
                        column: x => x.MunicipalityId,
                        principalTable: "Municipality",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssetTransactions_OperationCode",
                table: "AssetTransactions",
                column: "OperationCode");

            migrationBuilder.CreateIndex(
                name: "IX_Municipality_RegionId",
                table: "Municipality",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_TownOrVillage_MunicipalityId",
                table: "TownOrVillage",
                column: "MunicipalityId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetTransactions_OperationTypes_OperationCode",
                table: "AssetTransactions",
                column: "OperationCode",
                principalTable: "OperationTypes",
                principalColumn: "Code",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyOrPerson_TownOrVillage_RegisteredTownId",
                table: "CompanyOrPerson",
                column: "RegisteredTownId",
                principalTable: "TownOrVillage",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Facility_CompanyOrPerson_CompanyOrPersonId",
                table: "Facility",
                column: "CompanyOrPersonId",
                principalTable: "CompanyOrPerson",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Facility_TownOrVillage_TownOrVillageId",
                table: "Facility",
                column: "TownOrVillageId",
                principalTable: "TownOrVillage",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FacilityLog_Facility_FacilityId",
                table: "FacilityLog",
                column: "FacilityId",
                principalTable: "Facility",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
