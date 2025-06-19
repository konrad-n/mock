using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SledzSpecke.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemovePeselAndPwzFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Pesel",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_PwzNumber",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Pesel",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PwzNumber",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SupervisorPwz",
                table: "Procedures");

            migrationBuilder.DropColumn(
                name: "SupervisorPwz",
                table: "Internships");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
                name: "SupervisorPwz",
                table: "Procedures",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SupervisorPwz",
                table: "Internships",
                type: "text",
                nullable: true);

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
        }
    }
}
