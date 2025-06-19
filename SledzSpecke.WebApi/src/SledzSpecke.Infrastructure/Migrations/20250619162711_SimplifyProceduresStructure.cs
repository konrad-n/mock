using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SledzSpecke.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SimplifyProceduresStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExemptionEndDate",
                table: "MedicalShifts");

            migrationBuilder.DropColumn(
                name: "ExemptionReason",
                table: "MedicalShifts");

            migrationBuilder.DropColumn(
                name: "ExemptionStartDate",
                table: "MedicalShifts");

            migrationBuilder.DropColumn(
                name: "IsExempt",
                table: "MedicalShifts");

            migrationBuilder.CreateTable(
                name: "ProcedureRequirements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ModuleId = table.Column<int>(type: "integer", nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    RequiredAsOperator = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    RequiredAsAssistant = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcedureRequirements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProcedureRequirements_Modules_ModuleId",
                        column: x => x.ModuleId,
                        principalTable: "Modules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProcedureRealizations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RequirementId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Date = table.Column<DateTime>(type: "date", nullable: false),
                    Location = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcedureRealizations", x => x.Id);
                    table.CheckConstraint("CK_ProcedureRealizations_Year", "\"Year\" IS NULL OR (\"Year\" >= 1 AND \"Year\" <= 6)");
                    table.ForeignKey(
                        name: "FK_ProcedureRealizations_ProcedureRequirements_RequirementId",
                        column: x => x.RequirementId,
                        principalTable: "ProcedureRequirements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProcedureRealizations_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProcedureRealizations_Date",
                table: "ProcedureRealizations",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_ProcedureRealizations_RequirementId",
                table: "ProcedureRealizations",
                column: "RequirementId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcedureRealizations_UserId",
                table: "ProcedureRealizations",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcedureRealizations_UserId_RequirementId_Date",
                table: "ProcedureRealizations",
                columns: new[] { "UserId", "RequirementId", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_ProcedureRequirements_ModuleId",
                table: "ProcedureRequirements",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcedureRequirements_ModuleId_Code",
                table: "ProcedureRequirements",
                columns: new[] { "ModuleId", "Code" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProcedureRealizations");

            migrationBuilder.DropTable(
                name: "ProcedureRequirements");

            migrationBuilder.AddColumn<DateTime>(
                name: "ExemptionEndDate",
                table: "MedicalShifts",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExemptionReason",
                table: "MedicalShifts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExemptionStartDate",
                table: "MedicalShifts",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsExempt",
                table: "MedicalShifts",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
