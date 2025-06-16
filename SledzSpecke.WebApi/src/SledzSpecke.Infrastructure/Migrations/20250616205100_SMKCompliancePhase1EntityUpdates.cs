using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace SledzSpecke.Infrastructure.Migrations;

public partial class SMKCompliancePhase1EntityUpdates : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Add Country column to User's CorrespondenceAddress
        migrationBuilder.AddColumn<string>(
            name: "CorrespondenceAddress_Country",
            table: "Users",
            type: "character varying(100)",
            maxLength: 100,
            nullable: true,
            defaultValue: "Polska");

        // Add SecondName to User
        migrationBuilder.AddColumn<string>(
            name: "SecondName",
            table: "Users",
            type: "character varying(100)",
            maxLength: 100,
            nullable: true);

        // Add Status to Specialization
        migrationBuilder.AddColumn<int>(
            name: "Status",
            table: "Specializations",
            type: "integer",
            nullable: false,
            defaultValue: 1); // Active

        // Update Procedure table - replace OperatorCode with ExecutionType
        migrationBuilder.DropColumn(
            name: "OperatorCode",
            table: "Procedures");

        migrationBuilder.AddColumn<string>(
            name: "ExecutionType",
            table: "Procedures",
            type: "character varying(20)",
            maxLength: 20,
            nullable: false,
            defaultValue: "CodeA");

        migrationBuilder.AddColumn<string>(
            name: "Name",
            table: "Procedures",
            type: "character varying(300)",
            maxLength: 300,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<DateTime>(
            name: "PerformedDate",
            table: "Procedures",
            type: "timestamp with time zone",
            nullable: false,
            defaultValue: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

        migrationBuilder.AddColumn<string>(
            name: "SupervisorName",
            table: "Procedures",
            type: "character varying(200)",
            maxLength: 200,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "SupervisorPwz",
            table: "Procedures",
            type: "character varying(20)",
            maxLength: 20,
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "ModuleId",
            table: "Procedures",
            type: "integer",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<string>(
            name: "PatientInfo",
            table: "Procedures",
            type: "character varying(500)",
            maxLength: 500,
            nullable: true);

        // Add new columns for ProcedureNewSmk
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

        // Update Course table with CMKP fields
        migrationBuilder.AddColumn<string>(
            name: "CmkpCertificateNumber",
            table: "Courses",
            type: "character varying(100)",
            maxLength: 100,
            nullable: true);

        migrationBuilder.AddColumn<bool>(
            name: "IsVerifiedByCmkp",
            table: "Courses",
            type: "boolean",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<DateTime>(
            name: "CmkpVerificationDate",
            table: "Courses",
            type: "timestamp with time zone",
            nullable: true);

        migrationBuilder.AddColumn<DateTime>(
            name: "StartDate",
            table: "Courses",
            type: "timestamp with time zone",
            nullable: false,
            defaultValue: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

        migrationBuilder.AddColumn<DateTime>(
            name: "EndDate",
            table: "Courses",
            type: "timestamp with time zone",
            nullable: false,
            defaultValue: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

        migrationBuilder.AddColumn<int>(
            name: "DurationDays",
            table: "Courses",
            type: "integer",
            nullable: false,
            defaultValue: 1);

        migrationBuilder.AddColumn<int>(
            name: "DurationHours",
            table: "Courses",
            type: "integer",
            nullable: false,
            defaultValue: 8);

        migrationBuilder.AddColumn<string>(
            name: "OrganizerName",
            table: "Courses",
            type: "character varying(200)",
            maxLength: 200,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Location",
            table: "Courses",
            type: "character varying(200)",
            maxLength: 200,
            nullable: true);

        // Drop old columns from SelfEducations
        migrationBuilder.DropColumn(
            name: "UserId",
            table: "SelfEducations");

        migrationBuilder.DropColumn(
            name: "SpecializationId",
            table: "SelfEducations");

        migrationBuilder.DropColumn(
            name: "Year",
            table: "SelfEducations");

        migrationBuilder.DropColumn(
            name: "IsCompleted",
            table: "SelfEducations");

        // Add new columns to SelfEducations
        migrationBuilder.AddColumn<int>(
            name: "ModuleId",
            table: "SelfEducations",
            type: "integer",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<string>(
            name: "PublicationTitle",
            table: "SelfEducations",
            type: "character varying(500)",
            maxLength: 500,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "JournalName",
            table: "SelfEducations",
            type: "character varying(200)",
            maxLength: 200,
            nullable: true);

        migrationBuilder.AddColumn<bool>(
            name: "IsPeerReviewed",
            table: "SelfEducations",
            type: "boolean",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<string>(
            name: "Role",
            table: "SelfEducations",
            type: "text",
            nullable: true);

        // Create indexes
        migrationBuilder.CreateIndex(
            name: "IX_Procedures_ModuleId",
            table: "Procedures",
            column: "ModuleId");

        migrationBuilder.CreateIndex(
            name: "IX_SelfEducations_ModuleId",
            table: "SelfEducations",
            column: "ModuleId");

        // Add foreign key constraints
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

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // Drop foreign key constraints
        migrationBuilder.DropForeignKey(
            name: "FK_Procedures_Modules_ModuleId",
            table: "Procedures");

        migrationBuilder.DropForeignKey(
            name: "FK_SelfEducations_Modules_ModuleId",
            table: "SelfEducations");

        // Drop indexes
        migrationBuilder.DropIndex(
            name: "IX_Procedures_ModuleId",
            table: "Procedures");

        migrationBuilder.DropIndex(
            name: "IX_SelfEducations_ModuleId",
            table: "SelfEducations");

        // Revert SelfEducations
        migrationBuilder.DropColumn(
            name: "ModuleId",
            table: "SelfEducations");

        migrationBuilder.DropColumn(
            name: "PublicationTitle",
            table: "SelfEducations");

        migrationBuilder.DropColumn(
            name: "JournalName",
            table: "SelfEducations");

        migrationBuilder.DropColumn(
            name: "IsPeerReviewed",
            table: "SelfEducations");

        migrationBuilder.DropColumn(
            name: "Role",
            table: "SelfEducations");

        migrationBuilder.AddColumn<int>(
            name: "UserId",
            table: "SelfEducations",
            type: "integer",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<int>(
            name: "SpecializationId",
            table: "SelfEducations",
            type: "integer",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<int>(
            name: "Year",
            table: "SelfEducations",
            type: "integer",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<bool>(
            name: "IsCompleted",
            table: "SelfEducations",
            type: "boolean",
            nullable: false,
            defaultValue: false);

        // Revert Course changes
        migrationBuilder.DropColumn(
            name: "CmkpCertificateNumber",
            table: "Courses");

        migrationBuilder.DropColumn(
            name: "IsVerifiedByCmkp",
            table: "Courses");

        migrationBuilder.DropColumn(
            name: "CmkpVerificationDate",
            table: "Courses");

        migrationBuilder.DropColumn(
            name: "StartDate",
            table: "Courses");

        migrationBuilder.DropColumn(
            name: "EndDate",
            table: "Courses");

        migrationBuilder.DropColumn(
            name: "DurationDays",
            table: "Courses");

        migrationBuilder.DropColumn(
            name: "DurationHours",
            table: "Courses");

        migrationBuilder.DropColumn(
            name: "OrganizerName",
            table: "Courses");

        migrationBuilder.DropColumn(
            name: "Location",
            table: "Courses");

        // Revert Procedure changes
        migrationBuilder.DropColumn(
            name: "ExecutionType",
            table: "Procedures");

        migrationBuilder.DropColumn(
            name: "Name",
            table: "Procedures");

        migrationBuilder.DropColumn(
            name: "PerformedDate",
            table: "Procedures");

        migrationBuilder.DropColumn(
            name: "SupervisorName",
            table: "Procedures");

        migrationBuilder.DropColumn(
            name: "SupervisorPwz",
            table: "Procedures");

        migrationBuilder.DropColumn(
            name: "ModuleId",
            table: "Procedures");

        migrationBuilder.DropColumn(
            name: "PatientInfo",
            table: "Procedures");

        migrationBuilder.DropColumn(
            name: "RequiredCountCodeA",
            table: "Procedures");

        migrationBuilder.DropColumn(
            name: "RequiredCountCodeB",
            table: "Procedures");

        migrationBuilder.AddColumn<string>(
            name: "OperatorCode",
            table: "Procedures",
            type: "character varying(10)",
            maxLength: 10,
            nullable: true);

        // Revert Specialization changes
        migrationBuilder.DropColumn(
            name: "Status",
            table: "Specializations");

        // Revert User changes
        migrationBuilder.DropColumn(
            name: "SecondName",
            table: "Users");

        migrationBuilder.DropColumn(
            name: "CorrespondenceAddress_Country",
            table: "Users");
    }
}