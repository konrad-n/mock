using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SledzSpecke.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAdditionalSelfEducationDaysHandlers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MedicalShifts_Modules_ModuleId1",
                table: "MedicalShifts");

            migrationBuilder.RenameColumn(
                name: "ModuleId1",
                table: "MedicalShifts",
                newName: "ModuleId");

            migrationBuilder.RenameIndex(
                name: "IX_MedicalShifts_ModuleId1",
                table: "MedicalShifts",
                newName: "IX_MedicalShifts_ModuleId");

            migrationBuilder.AddForeignKey(
                name: "FK_MedicalShifts_Modules_ModuleId",
                table: "MedicalShifts",
                column: "ModuleId",
                principalTable: "Modules",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MedicalShifts_Modules_ModuleId",
                table: "MedicalShifts");

            migrationBuilder.RenameColumn(
                name: "ModuleId",
                table: "MedicalShifts",
                newName: "ModuleId1");

            migrationBuilder.RenameIndex(
                name: "IX_MedicalShifts_ModuleId",
                table: "MedicalShifts",
                newName: "IX_MedicalShifts_ModuleId1");

            migrationBuilder.AddForeignKey(
                name: "FK_MedicalShifts_Modules_ModuleId1",
                table: "MedicalShifts",
                column: "ModuleId1",
                principalTable: "Modules",
                principalColumn: "Id");
        }
    }
}
