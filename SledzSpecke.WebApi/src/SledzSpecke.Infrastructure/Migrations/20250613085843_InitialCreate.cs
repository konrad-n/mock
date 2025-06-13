using System;
using Microsoft.EntityFrameworkCore.Migrations;

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
                name: "Absences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SpecializationId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DurationInDays = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    DocumentPath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsApproved = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    ApprovedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ApprovedBy = table.Column<int>(type: "integer", nullable: true),
                    SyncStatus = table.Column<string>(type: "text", nullable: false, defaultValue: "NotSynced"),
                    AdditionalFields = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Absences", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    SpecializationId = table.Column<int>(type: "integer", nullable: false),
                    ModuleId = table.Column<int>(type: "integer", nullable: true),
                    CourseType = table.Column<int>(type: "integer", nullable: false),
                    CourseName = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    CourseNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    InstitutionName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CompletionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    HasCertificate = table.Column<bool>(type: "boolean", nullable: false),
                    CertificateNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    IsApproved = table.Column<bool>(type: "boolean", nullable: false),
                    ApprovalDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ApproverName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    SyncStatus = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Internships",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    SpecializationId = table.Column<int>(type: "integer", nullable: false),
                    ModuleId = table.Column<int>(type: "integer", nullable: true),
                    InstitutionName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    DepartmentName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    SupervisorName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DaysCount = table.Column<int>(type: "integer", nullable: false),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsApproved = table.Column<bool>(type: "boolean", nullable: false),
                    ApprovalDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ApproverName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    SyncStatus = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Internships", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Publications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SpecializationId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Title = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Authors = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Journal = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Publisher = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    PublicationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Volume = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Issue = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Pages = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DOI = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    PMID = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ISBN = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    URL = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Abstract = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                    Keywords = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    FilePath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsFirstAuthor = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    IsCorrespondingAuthor = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    IsPeerReviewed = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    SyncStatus = table.Column<string>(type: "text", nullable: false, defaultValue: "NotSynced"),
                    AdditionalFields = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Publications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Recognitions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SpecializationId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Title = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Institution = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DaysReduction = table.Column<int>(type: "integer", nullable: false),
                    IsApproved = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    ApprovedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ApprovedBy = table.Column<int>(type: "integer", nullable: true),
                    DocumentPath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    SyncStatus = table.Column<string>(type: "text", nullable: false, defaultValue: "NotSynced"),
                    AdditionalFields = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recognitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SelfEducations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SpecializationId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Provider = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Publisher = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DurationHours = table.Column<int>(type: "integer", nullable: true),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CertificatePath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    URL = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ISBN = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DOI = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CreditHours = table.Column<int>(type: "integer", nullable: false),
                    SyncStatus = table.Column<string>(type: "text", nullable: false, defaultValue: "NotSynced"),
                    AdditionalFields = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SelfEducations", x => x.Id);
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
                name: "MedicalShifts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
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
                    ApproverRole = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalShifts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MedicalShifts_Internships_InternshipId",
                        column: x => x.InternshipId,
                        principalTable: "Internships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Procedures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
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
                    Status = table.Column<int>(type: "integer", maxLength: 20, nullable: false),
                    SyncStatus = table.Column<int>(type: "integer", nullable: false),
                    AdditionalFields = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SmkVersion = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Procedures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Procedures_Internships_InternshipId",
                        column: x => x.InternshipId,
                        principalTable: "Internships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                name: "IX_Absences_SpecializationId",
                table: "Absences",
                column: "SpecializationId");

            migrationBuilder.CreateIndex(
                name: "IX_Absences_Type",
                table: "Absences",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Absences_UserId",
                table: "Absences",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Absences_UserId_StartDate_EndDate",
                table: "Absences",
                columns: new[] { "UserId", "StartDate", "EndDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Courses_CompletionDate",
                table: "Courses",
                column: "CompletionDate");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_CourseType",
                table: "Courses",
                column: "CourseType");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_ModuleId",
                table: "Courses",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_SpecializationId",
                table: "Courses",
                column: "SpecializationId");

            migrationBuilder.CreateIndex(
                name: "IX_Internships_ModuleId",
                table: "Internships",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_Internships_SpecializationId",
                table: "Internships",
                column: "SpecializationId");

            migrationBuilder.CreateIndex(
                name: "IX_Internships_StartDate_EndDate",
                table: "Internships",
                columns: new[] { "StartDate", "EndDate" });

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
                name: "IX_Publications_IsFirstAuthor",
                table: "Publications",
                column: "IsFirstAuthor");

            migrationBuilder.CreateIndex(
                name: "IX_Publications_IsPeerReviewed",
                table: "Publications",
                column: "IsPeerReviewed");

            migrationBuilder.CreateIndex(
                name: "IX_Publications_PublicationDate",
                table: "Publications",
                column: "PublicationDate");

            migrationBuilder.CreateIndex(
                name: "IX_Publications_SpecializationId",
                table: "Publications",
                column: "SpecializationId");

            migrationBuilder.CreateIndex(
                name: "IX_Publications_Type",
                table: "Publications",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Publications_UserId",
                table: "Publications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Publications_UserId_SpecializationId",
                table: "Publications",
                columns: new[] { "UserId", "SpecializationId" });

            migrationBuilder.CreateIndex(
                name: "IX_Recognitions_IsApproved",
                table: "Recognitions",
                column: "IsApproved");

            migrationBuilder.CreateIndex(
                name: "IX_Recognitions_SpecializationId",
                table: "Recognitions",
                column: "SpecializationId");

            migrationBuilder.CreateIndex(
                name: "IX_Recognitions_Type",
                table: "Recognitions",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Recognitions_UserId",
                table: "Recognitions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Recognitions_UserId_SpecializationId",
                table: "Recognitions",
                columns: new[] { "UserId", "SpecializationId" });

            migrationBuilder.CreateIndex(
                name: "IX_SelfEducations_IsCompleted",
                table: "SelfEducations",
                column: "IsCompleted");

            migrationBuilder.CreateIndex(
                name: "IX_SelfEducations_SpecializationId",
                table: "SelfEducations",
                column: "SpecializationId");

            migrationBuilder.CreateIndex(
                name: "IX_SelfEducations_Type",
                table: "SelfEducations",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_SelfEducations_UserId",
                table: "SelfEducations",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SelfEducations_UserId_SpecializationId",
                table: "SelfEducations",
                columns: new[] { "UserId", "SpecializationId" });

            migrationBuilder.CreateIndex(
                name: "IX_SelfEducations_Year",
                table: "SelfEducations",
                column: "Year");

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
                name: "Absences");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "MedicalShifts");

            migrationBuilder.DropTable(
                name: "Modules");

            migrationBuilder.DropTable(
                name: "Procedures");

            migrationBuilder.DropTable(
                name: "Publications");

            migrationBuilder.DropTable(
                name: "Recognitions");

            migrationBuilder.DropTable(
                name: "SelfEducations");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Specializations");

            migrationBuilder.DropTable(
                name: "Internships");
        }
    }
}
