using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SledzSpecke.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Phase2EntitiesWithSafeConversion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // First, add a temporary column for the conversion
            migrationBuilder.AddColumn<int>(
                name: "StatusTemp",
                table: "Procedures",
                type: "integer",
                nullable: true);

            // Convert string values to integers
            migrationBuilder.Sql(@"
                UPDATE ""Procedures""
                SET ""StatusTemp"" = 
                    CASE 
                        WHEN ""Status"" = 'Completed' THEN 1
                        WHEN ""Status"" = 'PartiallyCompleted' THEN 2
                        WHEN ""Status"" = 'Approved' THEN 3
                        WHEN ""Status"" = 'NotApproved' THEN 4
                        WHEN ""Status"" = 'Pending' THEN 5
                        ELSE 5 -- Default to Pending
                    END
            ");

            // Drop the old column
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Procedures");

            // Rename temporary column
            migrationBuilder.RenameColumn(
                name: "StatusTemp",
                table: "Procedures",
                newName: "Status");

            // Make it non-nullable with default value
            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Procedures",
                type: "integer",
                maxLength: 20,
                nullable: false,
                defaultValue: 5);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Procedures",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Procedures",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "SmkVersion",
                table: "Procedures",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Procedures",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "MedicalShifts",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "MedicalShifts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "MedicalShifts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

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

            migrationBuilder.AddForeignKey(
                name: "FK_MedicalShifts_Internships_InternshipId",
                table: "MedicalShifts",
                column: "InternshipId",
                principalTable: "Internships",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Procedures_Internships_InternshipId",
                table: "Procedures",
                column: "InternshipId",
                principalTable: "Internships",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MedicalShifts_Internships_InternshipId",
                table: "MedicalShifts");

            migrationBuilder.DropForeignKey(
                name: "FK_Procedures_Internships_InternshipId",
                table: "Procedures");

            migrationBuilder.DropTable(
                name: "Absences");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "Internships");

            migrationBuilder.DropTable(
                name: "Publications");

            migrationBuilder.DropTable(
                name: "Recognitions");

            migrationBuilder.DropTable(
                name: "SelfEducations");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Procedures");

            migrationBuilder.DropColumn(
                name: "SmkVersion",
                table: "Procedures");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Procedures");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "MedicalShifts");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "MedicalShifts");

            // Add temporary column for the reverse conversion
            migrationBuilder.AddColumn<string>(
                name: "StatusTemp",
                table: "Procedures",
                type: "character varying(20)",
                nullable: true);

            // Convert integer values back to strings
            migrationBuilder.Sql(@"
                UPDATE ""Procedures""
                SET ""StatusTemp"" = 
                    CASE 
                        WHEN ""Status"" = 1 THEN 'Completed'
                        WHEN ""Status"" = 2 THEN 'PartiallyCompleted'
                        WHEN ""Status"" = 3 THEN 'Approved'
                        WHEN ""Status"" = 4 THEN 'NotApproved'
                        WHEN ""Status"" = 5 THEN 'Pending'
                        ELSE 'Pending' -- Default to Pending
                    END
            ");

            // Drop the old column
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Procedures");

            // Rename temporary column
            migrationBuilder.RenameColumn(
                name: "StatusTemp",
                table: "Procedures",
                newName: "Status");

            // Make it non-nullable
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Procedures",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Pending");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Procedures",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "MedicalShifts",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);
        }
    }
}
