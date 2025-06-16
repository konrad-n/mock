using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SledzSpecke.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TransitionUserSchemaWithDataMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Username",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_SelfEducations_IsCompleted",
                table: "SelfEducations");

            migrationBuilder.DropIndex(
                name: "IX_SelfEducations_SpecializationId",
                table: "SelfEducations");

            migrationBuilder.DropIndex(
                name: "IX_SelfEducations_UserId",
                table: "SelfEducations");

            migrationBuilder.DropIndex(
                name: "IX_SelfEducations_UserId_SpecializationId",
                table: "SelfEducations");

            migrationBuilder.DropColumn(
                name: "Bio",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PreferredLanguage",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PreferredTheme",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ProfilePicturePath",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SmkVersion",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SpecializationId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AdditionalFields",
                table: "SelfEducations");

            migrationBuilder.DropColumn(
                name: "CertificatePath",
                table: "SelfEducations");

            migrationBuilder.DropColumn(
                name: "CompletedAt",
                table: "SelfEducations");

            migrationBuilder.DropColumn(
                name: "CreditHours",
                table: "SelfEducations");

            migrationBuilder.DropColumn(
                name: "DOI",
                table: "SelfEducations");

            migrationBuilder.DropColumn(
                name: "DurationHours",
                table: "SelfEducations");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "SelfEducations");

            migrationBuilder.DropColumn(
                name: "ISBN",
                table: "SelfEducations");

            migrationBuilder.DropColumn(
                name: "Provider",
                table: "SelfEducations");

            migrationBuilder.DropColumn(
                name: "QualityScore",
                table: "SelfEducations");

            migrationBuilder.DropColumn(
                name: "SpecializationId",
                table: "SelfEducations");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "SelfEducations");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "SelfEducations");

            migrationBuilder.DropColumn(
                name: "URL",
                table: "SelfEducations");

            migrationBuilder.RenameColumn(
                name: "Username",
                table: "Users",
                newName: "CorrespondenceAddress_HouseNumber");

            migrationBuilder.RenameColumn(
                name: "FullName",
                table: "Users",
                newName: "CorrespondenceAddress_Street");

            migrationBuilder.RenameColumn(
                name: "Year",
                table: "SelfEducations",
                newName: "ModuleId");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "SelfEducations",
                newName: "Hours");

            migrationBuilder.RenameColumn(
                name: "Publisher",
                table: "SelfEducations",
                newName: "PublicationTitle");

            migrationBuilder.RenameColumn(
                name: "IsCompleted",
                table: "SelfEducations",
                newName: "IsPeerReviewed");

            migrationBuilder.RenameIndex(
                name: "IX_SelfEducations_Year",
                table: "SelfEducations",
                newName: "IX_SelfEducations_ModuleId");

            migrationBuilder.RenameColumn(
                name: "OperatorCode",
                table: "Procedures",
                newName: "SupervisorPwz");

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "Users",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateOfBirth",
                table: "Users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CorrespondenceAddress_ApartmentNumber",
                table: "Users",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CorrespondenceAddress_City",
                table: "Users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CorrespondenceAddress_Country",
                table: "Users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CorrespondenceAddress_PostalCode",
                table: "Users",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CorrespondenceAddress_Province",
                table: "Users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Pesel",
                table: "Users",
                type: "character varying(11)",
                maxLength: 11,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PwzNumber",
                table: "Users",
                type: "character varying(7)",
                maxLength: 7,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SecondName",
                table: "Users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ActualEndDate",
                table: "Specializations",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "BasicModuleCompletionDate",
                table: "Specializations",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasBasicModule",
                table: "Specializations",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasSpecializedModule",
                table: "Specializations",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "PlannedPesYear",
                table: "Specializations",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ProgramVariant",
                table: "Specializations",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Specializations",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "SelfEducations",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(2000)",
                oldMaxLength: 2000,
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "SelfEducations",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "JournalName",
                table: "SelfEducations",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "SelfEducations",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ModuleId",
                table: "Procedures",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExecutionType",
                table: "Procedures",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Procedures",
                type: "character varying(300)",
                maxLength: 300,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PatientInfo",
                table: "Procedures",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PerformedDate",
                table: "Procedures",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "ProcedureOldSmk_RequiredCountCodeA",
                table: "Procedures",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RequiredCountCodeA",
                table: "Procedures",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RequiredCountCodeB",
                table: "Procedures",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SupervisorName",
                table: "Procedures",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

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

            migrationBuilder.AddColumn<int>(
                name: "ModuleId1",
                table: "MedicalShifts",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SupervisorName",
                table: "MedicalShifts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "MedicalShifts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CompletedDays",
                table: "Internships",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Internships",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "PlannedDays",
                table: "Internships",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PlannedWeeks",
                table: "Internships",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Internships",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SupervisorPwz",
                table: "Internships",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CmkpCertificateNumber",
                table: "Courses",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CmkpVerificationDate",
                table: "Courses",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DurationDays",
                table: "Courses",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DurationHours",
                table: "Courses",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Courses",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsVerifiedByCmkp",
                table: "Courses",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Courses",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizerName",
                table: "Courses",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "Courses",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "AdditionalSelfEducationDays",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ModuleId = table.Column<int>(type: "integer", nullable: false),
                    InternshipId = table.Column<int>(type: "integer", nullable: false),
                    StartDate = table.Column<DateTime>(type: "date", nullable: false),
                    EndDate = table.Column<DateTime>(type: "date", nullable: false),
                    NumberOfDays = table.Column<int>(type: "integer", nullable: false),
                    Purpose = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    EventName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Location = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Organizer = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    IsApproved = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdditionalSelfEducationDays", x => x.Id);
                });

            // Populate data for existing users before creating unique indexes
            migrationBuilder.Sql(@"
                -- Update missing values first
                UPDATE ""Users"" SET ""PhoneNumber"" = '+48123456789' WHERE ""PhoneNumber"" IS NULL OR ""PhoneNumber"" = '';
                UPDATE ""Users"" SET ""DateOfBirth"" = '1990-01-01'::timestamp WHERE ""DateOfBirth"" IS NULL;
                
                -- Populate new fields with unique values for existing users
                DO $$
                DECLARE
                    user_record RECORD;
                    counter INTEGER := 1;
                BEGIN
                    FOR user_record IN SELECT ""Id"", ""CorrespondenceAddress_Street"", ""CorrespondenceAddress_HouseNumber"" FROM ""Users"" ORDER BY ""Id""
                    LOOP
                        UPDATE ""Users"" 
                        SET 
                            ""FirstName"" = CASE 
                                WHEN ""FirstName"" IS NULL OR ""FirstName"" = '' 
                                THEN COALESCE(SPLIT_PART(""CorrespondenceAddress_Street"", ' ', 1), 'User' || user_record.""Id"")
                                ELSE ""FirstName""
                            END,
                            ""LastName"" = CASE 
                                WHEN ""LastName"" IS NULL OR ""LastName"" = '' 
                                THEN 'Test' || user_record.""Id""
                                ELSE ""LastName""
                            END,
                            ""Pesel"" = CASE 
                                WHEN ""Pesel"" IS NULL OR ""Pesel"" = '' 
                                THEN LPAD((90000000000 + user_record.""Id"")::text, 11, '0')
                                ELSE ""Pesel""
                            END,
                            ""PwzNumber"" = CASE 
                                WHEN ""PwzNumber"" IS NULL OR ""PwzNumber"" = '' 
                                THEN LPAD((1000000 + user_record.""Id"")::text, 7, '0')
                                ELSE ""PwzNumber""
                            END,
                            ""CorrespondenceAddress_Street"" = CASE 
                                WHEN ""CorrespondenceAddress_Street"" = '' 
                                THEN 'Testowa'
                                ELSE ""CorrespondenceAddress_Street""
                            END,
                            ""CorrespondenceAddress_HouseNumber"" = CASE 
                                WHEN ""CorrespondenceAddress_HouseNumber"" = '' 
                                THEN user_record.""Id""::text
                                ELSE ""CorrespondenceAddress_HouseNumber""
                            END,
                            ""CorrespondenceAddress_PostalCode"" = CASE 
                                WHEN ""CorrespondenceAddress_PostalCode"" = '' 
                                THEN '00-001' 
                                ELSE ""CorrespondenceAddress_PostalCode"" 
                            END,
                            ""CorrespondenceAddress_City"" = CASE 
                                WHEN ""CorrespondenceAddress_City"" = '' 
                                THEN 'Warszawa' 
                                ELSE ""CorrespondenceAddress_City"" 
                            END,
                            ""CorrespondenceAddress_Province"" = CASE 
                                WHEN ""CorrespondenceAddress_Province"" = '' 
                                THEN 'mazowieckie' 
                                ELSE ""CorrespondenceAddress_Province"" 
                            END,
                            ""CorrespondenceAddress_Country"" = CASE 
                                WHEN ""CorrespondenceAddress_Country"" = '' 
                                THEN 'Polska' 
                                ELSE ""CorrespondenceAddress_Country"" 
                            END
                        WHERE ""Id"" = user_record.""Id"";
                        
                        counter := counter + 1;
                    END LOOP;
                END $$;
                
            ");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Pesel",
                table: "Users",
                column: "Pesel",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_PwzNumber",
                table: "Users",
                column: "PwzNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SelfEducations_Date",
                table: "SelfEducations",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_Procedures_ModuleId",
                table: "Procedures",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalShifts_ModuleId1",
                table: "MedicalShifts",
                column: "ModuleId1");

            migrationBuilder.CreateIndex(
                name: "IX_AdditionalSelfEducationDays_Dates",
                table: "AdditionalSelfEducationDays",
                columns: new[] { "StartDate", "EndDate" });

            migrationBuilder.CreateIndex(
                name: "IX_AdditionalSelfEducationDays_InternshipId",
                table: "AdditionalSelfEducationDays",
                column: "InternshipId");

            migrationBuilder.CreateIndex(
                name: "IX_AdditionalSelfEducationDays_ModuleId",
                table: "AdditionalSelfEducationDays",
                column: "ModuleId");

            // Handle NULL ModuleId values before adding foreign key constraints
            migrationBuilder.Sql(@"
                -- Update NULL ModuleId values to use the first available module
                UPDATE ""Procedures"" 
                SET ""ModuleId"" = (SELECT MIN(""Id"") FROM ""Modules"")
                WHERE ""ModuleId"" IS NULL OR ""ModuleId"" = 0;
                
                UPDATE ""MedicalShifts"" 
                SET ""ModuleId"" = (SELECT MIN(""Id"") FROM ""Modules"")
                WHERE ""ModuleId"" IS NULL;
                
                UPDATE ""Internships"" 
                SET ""ModuleId"" = (SELECT MIN(""Id"") FROM ""Modules"")
                WHERE ""ModuleId"" IS NULL;
                
                UPDATE ""Courses"" 
                SET ""ModuleId"" = (SELECT MIN(""Id"") FROM ""Modules"")
                WHERE ""ModuleId"" IS NULL;
            ");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Modules_ModuleId",
                table: "Courses",
                column: "ModuleId",
                principalTable: "Modules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Internships_Modules_ModuleId",
                table: "Internships",
                column: "ModuleId",
                principalTable: "Modules",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MedicalShifts_Modules_ModuleId1",
                table: "MedicalShifts",
                column: "ModuleId1",
                principalTable: "Modules",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Procedures_Modules_ModuleId",
                table: "Procedures",
                column: "ModuleId",
                principalTable: "Modules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SelfEducations_Modules_ModuleId",
                table: "SelfEducations",
                column: "ModuleId",
                principalTable: "Modules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Modules_ModuleId",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "FK_Internships_Modules_ModuleId",
                table: "Internships");

            migrationBuilder.DropForeignKey(
                name: "FK_MedicalShifts_Modules_ModuleId1",
                table: "MedicalShifts");

            migrationBuilder.DropForeignKey(
                name: "FK_Procedures_Modules_ModuleId",
                table: "Procedures");

            migrationBuilder.DropForeignKey(
                name: "FK_SelfEducations_Modules_ModuleId",
                table: "SelfEducations");

            migrationBuilder.DropTable(
                name: "AdditionalSelfEducationDays");

            migrationBuilder.DropIndex(
                name: "IX_Users_Pesel",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_PwzNumber",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_SelfEducations_Date",
                table: "SelfEducations");

            migrationBuilder.DropIndex(
                name: "IX_Procedures_ModuleId",
                table: "Procedures");

            migrationBuilder.DropIndex(
                name: "IX_MedicalShifts_ModuleId1",
                table: "MedicalShifts");

            migrationBuilder.DropColumn(
                name: "CorrespondenceAddress_ApartmentNumber",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CorrespondenceAddress_City",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CorrespondenceAddress_Country",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CorrespondenceAddress_PostalCode",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CorrespondenceAddress_Province",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Pesel",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PwzNumber",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SecondName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ActualEndDate",
                table: "Specializations");

            migrationBuilder.DropColumn(
                name: "BasicModuleCompletionDate",
                table: "Specializations");

            migrationBuilder.DropColumn(
                name: "HasBasicModule",
                table: "Specializations");

            migrationBuilder.DropColumn(
                name: "HasSpecializedModule",
                table: "Specializations");

            migrationBuilder.DropColumn(
                name: "PlannedPesYear",
                table: "Specializations");

            migrationBuilder.DropColumn(
                name: "ProgramVariant",
                table: "Specializations");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Specializations");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "SelfEducations");

            migrationBuilder.DropColumn(
                name: "JournalName",
                table: "SelfEducations");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "SelfEducations");

            migrationBuilder.DropColumn(
                name: "ExecutionType",
                table: "Procedures");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Procedures");

            migrationBuilder.DropColumn(
                name: "PatientInfo",
                table: "Procedures");

            migrationBuilder.DropColumn(
                name: "PerformedDate",
                table: "Procedures");

            migrationBuilder.DropColumn(
                name: "ProcedureOldSmk_RequiredCountCodeA",
                table: "Procedures");

            migrationBuilder.DropColumn(
                name: "RequiredCountCodeA",
                table: "Procedures");

            migrationBuilder.DropColumn(
                name: "RequiredCountCodeB",
                table: "Procedures");

            migrationBuilder.DropColumn(
                name: "SupervisorName",
                table: "Procedures");

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

            migrationBuilder.DropColumn(
                name: "ModuleId1",
                table: "MedicalShifts");

            migrationBuilder.DropColumn(
                name: "SupervisorName",
                table: "MedicalShifts");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "MedicalShifts");

            migrationBuilder.DropColumn(
                name: "CompletedDays",
                table: "Internships");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Internships");

            migrationBuilder.DropColumn(
                name: "PlannedDays",
                table: "Internships");

            migrationBuilder.DropColumn(
                name: "PlannedWeeks",
                table: "Internships");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Internships");

            migrationBuilder.DropColumn(
                name: "SupervisorPwz",
                table: "Internships");

            migrationBuilder.DropColumn(
                name: "CmkpCertificateNumber",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "CmkpVerificationDate",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "DurationDays",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "DurationHours",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "IsVerifiedByCmkp",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "OrganizerName",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Courses");

            migrationBuilder.RenameColumn(
                name: "CorrespondenceAddress_Street",
                table: "Users",
                newName: "FullName");

            migrationBuilder.RenameColumn(
                name: "CorrespondenceAddress_HouseNumber",
                table: "Users",
                newName: "Username");

            migrationBuilder.RenameColumn(
                name: "PublicationTitle",
                table: "SelfEducations",
                newName: "Publisher");

            migrationBuilder.RenameColumn(
                name: "ModuleId",
                table: "SelfEducations",
                newName: "Year");

            migrationBuilder.RenameColumn(
                name: "IsPeerReviewed",
                table: "SelfEducations",
                newName: "IsCompleted");

            migrationBuilder.RenameColumn(
                name: "Hours",
                table: "SelfEducations",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_SelfEducations_ModuleId",
                table: "SelfEducations",
                newName: "IX_SelfEducations_Year");

            migrationBuilder.RenameColumn(
                name: "SupervisorPwz",
                table: "Procedures",
                newName: "OperatorCode");

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "Users",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateOfBirth",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<string>(
                name: "Bio",
                table: "Users",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PreferredLanguage",
                table: "Users",
                type: "character varying(2)",
                maxLength: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PreferredTheme",
                table: "Users",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfilePicturePath",
                table: "Users",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SmkVersion",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "SpecializationId",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "SelfEducations",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(2000)",
                oldMaxLength: 2000);

            migrationBuilder.AddColumn<string>(
                name: "AdditionalFields",
                table: "SelfEducations",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CertificatePath",
                table: "SelfEducations",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedAt",
                table: "SelfEducations",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreditHours",
                table: "SelfEducations",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "DOI",
                table: "SelfEducations",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DurationHours",
                table: "SelfEducations",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "SelfEducations",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ISBN",
                table: "SelfEducations",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Provider",
                table: "SelfEducations",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "QualityScore",
                table: "SelfEducations",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SpecializationId",
                table: "SelfEducations",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "SelfEducations",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "SelfEducations",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "URL",
                table: "SelfEducations",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ModuleId",
                table: "Procedures",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SelfEducations_IsCompleted",
                table: "SelfEducations",
                column: "IsCompleted");

            migrationBuilder.CreateIndex(
                name: "IX_SelfEducations_SpecializationId",
                table: "SelfEducations",
                column: "SpecializationId");

            migrationBuilder.CreateIndex(
                name: "IX_SelfEducations_UserId",
                table: "SelfEducations",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SelfEducations_UserId_SpecializationId",
                table: "SelfEducations",
                columns: new[] { "UserId", "SpecializationId" });
        }
    }
}
