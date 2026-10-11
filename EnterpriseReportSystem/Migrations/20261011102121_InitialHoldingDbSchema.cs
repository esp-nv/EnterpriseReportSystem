using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnterpriseReportSystem.Migrations
{
    /// <inheritdoc />
    public partial class InitialHoldingDbSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetGroups_MainTypes_MainTypeCode",
                table: "AssetGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetSubGroups_AssetGroups_AssetGroupCode",
                table: "AssetSubGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetTransactions_AssetGroups_FromGroupCode",
                table: "AssetTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetTransactions_AssetGroups_ToGroupCode",
                table: "AssetTransactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MainTypes",
                table: "MainTypes");

            migrationBuilder.DropIndex(
                name: "IX_AssetTransactions_FromGroupCode",
                table: "AssetTransactions");

            migrationBuilder.DropIndex(
                name: "IX_AssetTransactions_ToGroupCode",
                table: "AssetTransactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AssetSubGroups",
                table: "AssetSubGroups");

            migrationBuilder.DropIndex(
                name: "IX_AssetSubGroups_AssetGroupCode",
                table: "AssetSubGroups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AssetGroups",
                table: "AssetGroups");

            migrationBuilder.DropIndex(
                name: "IX_AssetGroups_MainTypeCode",
                table: "AssetGroups");

            migrationBuilder.DropColumn(
                name: "FromGroupCode",
                table: "AssetTransactions");

            migrationBuilder.DropColumn(
                name: "ToGroupCode",
                table: "AssetTransactions");

            migrationBuilder.DropColumn(
                name: "AssetGroupCode",
                table: "AssetSubGroups");

            migrationBuilder.DropColumn(
                name: "MainTypeCode",
                table: "AssetGroups");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "MainTypes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "MainTypes",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "FromGroupId",
                table: "AssetTransactions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ToGroupId",
                table: "AssetTransactions",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "AssetSubGroups",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "AssetSubGroups",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "AssetGroupId",
                table: "AssetSubGroups",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "MeasurementUnit",
                table: "AssetSubGroups",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "AssetGroups",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "AssetGroups",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "MainTypeId",
                table: "AssetGroups",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_MainTypes",
                table: "MainTypes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AssetSubGroups",
                table: "AssetSubGroups",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AssetGroups",
                table: "AssetGroups",
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
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegionId = table.Column<int>(type: "int", nullable: false)
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
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MunicipalityId = table.Column<int>(type: "int", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "CompanyOrPerson",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TaxIdentifier = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegisteredTownId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyOrPerson", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyOrPerson_TownOrVillage_RegisteredTownId",
                        column: x => x.RegisteredTownId,
                        principalTable: "TownOrVillage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Facility",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FacilityCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TownOrVillageId = table.Column<int>(type: "int", nullable: false),
                    CompanyOrPersonId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Facility", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Facility_CompanyOrPerson_CompanyOrPersonId",
                        column: x => x.CompanyOrPersonId,
                        principalTable: "CompanyOrPerson",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Facility_TownOrVillage_TownOrVillageId",
                        column: x => x.TownOrVillageId,
                        principalTable: "TownOrVillage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FacilityLog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LogDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EventType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OldValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NewValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FacilityId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacilityLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FacilityLog_Facility_FacilityId",
                        column: x => x.FacilityId,
                        principalTable: "Facility",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssetTransactions_FromGroupId",
                table: "AssetTransactions",
                column: "FromGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetTransactions_ToGroupId",
                table: "AssetTransactions",
                column: "ToGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetSubGroups_AssetGroupId",
                table: "AssetSubGroups",
                column: "AssetGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetGroups_MainTypeId",
                table: "AssetGroups",
                column: "MainTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyOrPerson_RegisteredTownId",
                table: "CompanyOrPerson",
                column: "RegisteredTownId");

            migrationBuilder.CreateIndex(
                name: "IX_Facility_CompanyOrPersonId",
                table: "Facility",
                column: "CompanyOrPersonId");

            migrationBuilder.CreateIndex(
                name: "IX_Facility_TownOrVillageId",
                table: "Facility",
                column: "TownOrVillageId");

            migrationBuilder.CreateIndex(
                name: "IX_FacilityLog_FacilityId",
                table: "FacilityLog",
                column: "FacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_Municipality_RegionId",
                table: "Municipality",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_TownOrVillage_MunicipalityId",
                table: "TownOrVillage",
                column: "MunicipalityId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetGroups_MainTypes_MainTypeId",
                table: "AssetGroups",
                column: "MainTypeId",
                principalTable: "MainTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetSubGroups_AssetGroups_AssetGroupId",
                table: "AssetSubGroups",
                column: "AssetGroupId",
                principalTable: "AssetGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetTransactions_AssetGroups_FromGroupId",
                table: "AssetTransactions",
                column: "FromGroupId",
                principalTable: "AssetGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetTransactions_AssetGroups_ToGroupId",
                table: "AssetTransactions",
                column: "ToGroupId",
                principalTable: "AssetGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetGroups_MainTypes_MainTypeId",
                table: "AssetGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetSubGroups_AssetGroups_AssetGroupId",
                table: "AssetSubGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetTransactions_AssetGroups_FromGroupId",
                table: "AssetTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetTransactions_AssetGroups_ToGroupId",
                table: "AssetTransactions");

            migrationBuilder.DropTable(
                name: "FacilityLog");

            migrationBuilder.DropTable(
                name: "Facility");

            migrationBuilder.DropTable(
                name: "CompanyOrPerson");

            migrationBuilder.DropTable(
                name: "TownOrVillage");

            migrationBuilder.DropTable(
                name: "Municipality");

            migrationBuilder.DropTable(
                name: "Region");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MainTypes",
                table: "MainTypes");

            migrationBuilder.DropIndex(
                name: "IX_AssetTransactions_FromGroupId",
                table: "AssetTransactions");

            migrationBuilder.DropIndex(
                name: "IX_AssetTransactions_ToGroupId",
                table: "AssetTransactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AssetSubGroups",
                table: "AssetSubGroups");

            migrationBuilder.DropIndex(
                name: "IX_AssetSubGroups_AssetGroupId",
                table: "AssetSubGroups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AssetGroups",
                table: "AssetGroups");

            migrationBuilder.DropIndex(
                name: "IX_AssetGroups_MainTypeId",
                table: "AssetGroups");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "MainTypes");

            migrationBuilder.DropColumn(
                name: "FromGroupId",
                table: "AssetTransactions");

            migrationBuilder.DropColumn(
                name: "ToGroupId",
                table: "AssetTransactions");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "AssetSubGroups");

            migrationBuilder.DropColumn(
                name: "AssetGroupId",
                table: "AssetSubGroups");

            migrationBuilder.DropColumn(
                name: "MeasurementUnit",
                table: "AssetSubGroups");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "AssetGroups");

            migrationBuilder.DropColumn(
                name: "MainTypeId",
                table: "AssetGroups");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "MainTypes",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "FromGroupCode",
                table: "AssetTransactions",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ToGroupCode",
                table: "AssetTransactions",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "AssetSubGroups",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "AssetGroupCode",
                table: "AssetSubGroups",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "AssetGroups",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "MainTypeCode",
                table: "AssetGroups",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MainTypes",
                table: "MainTypes",
                column: "Code");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AssetSubGroups",
                table: "AssetSubGroups",
                column: "Code");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AssetGroups",
                table: "AssetGroups",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_AssetTransactions_FromGroupCode",
                table: "AssetTransactions",
                column: "FromGroupCode");

            migrationBuilder.CreateIndex(
                name: "IX_AssetTransactions_ToGroupCode",
                table: "AssetTransactions",
                column: "ToGroupCode");

            migrationBuilder.CreateIndex(
                name: "IX_AssetSubGroups_AssetGroupCode",
                table: "AssetSubGroups",
                column: "AssetGroupCode");

            migrationBuilder.CreateIndex(
                name: "IX_AssetGroups_MainTypeCode",
                table: "AssetGroups",
                column: "MainTypeCode");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetGroups_MainTypes_MainTypeCode",
                table: "AssetGroups",
                column: "MainTypeCode",
                principalTable: "MainTypes",
                principalColumn: "Code",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetSubGroups_AssetGroups_AssetGroupCode",
                table: "AssetSubGroups",
                column: "AssetGroupCode",
                principalTable: "AssetGroups",
                principalColumn: "Code",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetTransactions_AssetGroups_FromGroupCode",
                table: "AssetTransactions",
                column: "FromGroupCode",
                principalTable: "AssetGroups",
                principalColumn: "Code",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetTransactions_AssetGroups_ToGroupCode",
                table: "AssetTransactions",
                column: "ToGroupCode",
                principalTable: "AssetGroups",
                principalColumn: "Code",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
