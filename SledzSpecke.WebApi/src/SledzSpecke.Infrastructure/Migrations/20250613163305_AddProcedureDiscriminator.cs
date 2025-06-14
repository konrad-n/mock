using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SledzSpecke.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProcedureDiscriminator : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Procedures",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "SmkVersion",
                table: "Procedures",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "ProcedureGroup",
                table: "Procedures",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OperatorCode",
                table: "Procedures",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Location",
                table: "Procedures",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Procedures",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "AssistantData",
                table: "Procedures",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Comments",
                table: "Procedures",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CountA",
                table: "Procedures",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CountB",
                table: "Procedures",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Procedures",
                type: "character varying(21)",
                maxLength: 21,
                nullable: false,
                defaultValue: "Procedure");

            // Update existing procedures to have the correct discriminator value
            migrationBuilder.Sql("UPDATE \"Procedures\" SET \"Discriminator\" = 'Procedure' WHERE \"Discriminator\" IS NULL OR \"Discriminator\" = '';");

            migrationBuilder.AddColumn<string>(
                name: "Institution",
                table: "Procedures",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InternshipName",
                table: "Procedures",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ModuleId",
                table: "Procedures",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProcedureName",
                table: "Procedures",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProcedureOldSmk_ProcedureRequirementId",
                table: "Procedures",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProcedureRequirementId",
                table: "Procedures",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Supervisor",
                table: "Procedures",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comments",
                table: "Procedures");

            migrationBuilder.DropColumn(
                name: "CountA",
                table: "Procedures");

            migrationBuilder.DropColumn(
                name: "CountB",
                table: "Procedures");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Procedures");

            migrationBuilder.DropColumn(
                name: "Institution",
                table: "Procedures");

            migrationBuilder.DropColumn(
                name: "InternshipName",
                table: "Procedures");

            migrationBuilder.DropColumn(
                name: "ModuleId",
                table: "Procedures");

            migrationBuilder.DropColumn(
                name: "ProcedureName",
                table: "Procedures");

            migrationBuilder.DropColumn(
                name: "ProcedureOldSmk_ProcedureRequirementId",
                table: "Procedures");

            migrationBuilder.DropColumn(
                name: "ProcedureRequirementId",
                table: "Procedures");

            migrationBuilder.DropColumn(
                name: "Supervisor",
                table: "Procedures");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Procedures",
                type: "integer",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<int>(
                name: "SmkVersion",
                table: "Procedures",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "ProcedureGroup",
                table: "Procedures",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OperatorCode",
                table: "Procedures",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Location",
                table: "Procedures",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Procedures",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "AssistantData",
                table: "Procedures",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);
        }
    }
}
