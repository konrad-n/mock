using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SledzSpecke.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MedicalShifts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InternshipId = table.Column<int>(type: "integer", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Hours = table.Column<int>(type: "integer", nullable: false),
                    Minutes = table.Column<int>(type: "integer", nullable: false),
                    Location = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    SyncStatus = table.Column<int>(type: "integer", nullable: false),
                    AdditionalFields = table.Column<string>(type: "text", nullable: true),
                    ApprovalDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ApproverName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ApproverRole = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalShifts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Procedures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InternshipId = table.Column<int>(type: "integer", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    Code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    OperatorCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    PerformingPerson = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Location = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PatientInitials = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    PatientGender = table.Column<char>(type: "character(1)", maxLength: 1, nullable: true),
                    AssistantData = table.Column<string>(type: "text", nullable: true),
                    ProcedureGroup = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    SyncStatus = table.Column<int>(type: "integer", nullable: false),
                    AdditionalFields = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Procedures", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Specializations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ProgramCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    SmkVersion = table.Column<int>(type: "integer", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PlannedEndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CalculatedEndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProgramStructure = table.Column<string>(type: "text", nullable: false),
                    CurrentModuleId = table.Column<int>(type: "integer", nullable: true),
                    DurationYears = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Specializations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false),
                    FullName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    SmkVersion = table.Column<int>(type: "integer", nullable: false),
                    SpecializationId = table.Column<int>(type: "integer", nullable: false),
                    RegistrationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Modules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    SpecializationId = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    SmkVersion = table.Column<int>(type: "integer", nullable: false),
                    Version = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Structure = table.Column<string>(type: "text", nullable: false),
                    CompletedInternships = table.Column<int>(type: "integer", nullable: false),
                    TotalInternships = table.Column<int>(type: "integer", nullable: false),
                    CompletedCourses = table.Column<int>(type: "integer", nullable: false),
                    TotalCourses = table.Column<int>(type: "integer", nullable: false),
                    CompletedProceduresA = table.Column<int>(type: "integer", nullable: false),
                    TotalProceduresA = table.Column<int>(type: "integer", nullable: false),
                    CompletedProceduresB = table.Column<int>(type: "integer", nullable: false),
                    TotalProceduresB = table.Column<int>(type: "integer", nullable: false),
                    CompletedShiftHours = table.Column<int>(type: "integer", nullable: false),
                    RequiredShiftHours = table.Column<int>(type: "integer", nullable: false),
                    WeeklyShiftHours = table.Column<double>(type: "double precision", nullable: false),
                    CompletedSelfEducationDays = table.Column<int>(type: "integer", nullable: false),
                    TotalSelfEducationDays = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Modules_Specializations_SpecializationId",
                        column: x => x.SpecializationId,
                        principalTable: "Specializations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MedicalShifts_Date",
                table: "MedicalShifts",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalShifts_InternshipId",
                table: "MedicalShifts",
                column: "InternshipId");

            migrationBuilder.CreateIndex(
                name: "IX_Modules_SpecializationId",
                table: "Modules",
                column: "SpecializationId");

            migrationBuilder.CreateIndex(
                name: "IX_Procedures_Code",
                table: "Procedures",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_Procedures_Date",
                table: "Procedures",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_Procedures_InternshipId",
                table: "Procedures",
                column: "InternshipId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MedicalShifts");

            migrationBuilder.DropTable(
                name: "Modules");

            migrationBuilder.DropTable(
                name: "Procedures");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Specializations");
        }
    }
}
