using ClosedXML.Excel;
using SledzSpecke.Core.Interfaces.Services;
using SledzSpecke.Core.Models.Domain;
using System.Collections.Generic;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SledzSpecke.Infrastructure.Services
{
    public class ExcelExportService : IExcelExportService
    {
        private readonly IUserService _userService;

        public ExcelExportService(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<string> ExportProceduresToExcelAsync(List<ProcedureExecution> procedures)
        {
            var user = await _userService.GetCurrentUserAsync();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Procedury");

                // Dodaj nagłówki
                worksheet.Cell(1, 1).Value = "ID";
                worksheet.Cell(1, 2).Value = "Imię i nazwisko";
                worksheet.Cell(1, 3).Value = "Data";
                worksheet.Cell(1, 4).Value = "Osoba Wykonująca";
                worksheet.Cell(1, 5).Value = "Dane Asystentów";
                worksheet.Cell(1, 6).Value = "Procedura";

                // Dodaj dane
                int row = 2;
                foreach (var procedure in procedures)
                {
                    worksheet.Cell(row, 1).Value = row - 1;
                    worksheet.Cell(row, 2).Value = user.Name;
                    worksheet.Cell(row, 3).Value = procedure.ExecutionDate.ToString("yyyy-MM-dd");
                    worksheet.Cell(row, 4).Value = user.Name; // Można dostosować w zależności od typu procedury
                    worksheet.Cell(row, 5).Value = procedure.SupervisorId.HasValue ? "Supervisor" : ""; // Pobierz nazwisko supervisora

                    // Tutaj dodajemy informację o typie procedury zgodnie z tym co jest w bazie
                    worksheet.Cell(row, 6).Value = $"procedura {procedure.Type}"; // procedura A lub procedura B

                    row++;
                }

                // Formatuj jako tabela
                var range = worksheet.Range(1, 1, row - 1, 6);
                var table = range.CreateTable();

                // Dostosuj szerokość kolumn
                worksheet.Columns().AdjustToContents();

                // Zapisz do pliku
                string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string fileName = Path.Combine(documentsPath, $"Procedury_{DateTime.Now:yyyyMMdd}.xlsx");
                workbook.SaveAs(fileName);

                return fileName;
            }
        }

        public async Task<string> ExportDutiesToExcelAsync(List<Duty> duties)
        {
            // Podobna implementacja jak dla procedur
            var user = await _userService.GetCurrentUserAsync();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Dyżury");

                // Dodaj nagłówki
                worksheet.Cell(1, 1).Value = "ID";
                worksheet.Cell(1, 2).Value = "Imię i nazwisko";
                worksheet.Cell(1, 3).Value = "Data";
                worksheet.Cell(1, 4).Value = "Lokalizacja";
                worksheet.Cell(1, 5).Value = "Godziny";

                // Dodaj dane
                int row = 2;
                foreach (var duty in duties)
                {
                    worksheet.Cell(row, 1).Value = row - 1;
                    worksheet.Cell(row, 2).Value = user.Name;
                    worksheet.Cell(row, 3).Value = duty.StartTime.ToString("yyyy-MM-dd");
                    worksheet.Cell(row, 4).Value = duty.Location;
                    worksheet.Cell(row, 5).Value = duty.DurationInHours;
                    row++;
                }

                // Formatuj jako tabela
                var range = worksheet.Range(1, 1, row - 1, 6);
                var table = range.CreateTable();

                // Dostosuj szerokość kolumn
                worksheet.Columns().AdjustToContents();

                // Zapisz do pliku
                string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string fileName = Path.Combine(documentsPath, $"Dyzury_{DateTime.Now:yyyyMMdd}.xlsx");
                workbook.SaveAs(fileName);

                return fileName;
            }
        }

        public async Task<string> ExportAllDataToExcelAsync(
            List<ProcedureExecution> procedures,
            List<Duty> duties,
            List<Course> courses = null,
            List<Internship> internships = null)
        {
            var user = await _userService.GetCurrentUserAsync();

            using (var workbook = new XLWorkbook())
            {
                // Arkusz procedur
                var proceduresSheet = workbook.Worksheets.Add("Procedury");

                // Nagłówki procedur
                proceduresSheet.Cell(1, 1).Value = "ID";
                proceduresSheet.Cell(1, 2).Value = "Imię i nazwisko";
                proceduresSheet.Cell(1, 3).Value = "Data";
                proceduresSheet.Cell(1, 4).Value = "Osoba Wykonująca";
                proceduresSheet.Cell(1, 5).Value = "Dane Asystentów";
                proceduresSheet.Cell(1, 6).Value = "Procedura";

                // Dane procedur
                int row = 2;
                foreach (var procedure in procedures)
                {
                    proceduresSheet.Cell(row, 1).Value = row - 1;
                    proceduresSheet.Cell(row, 2).Value = user.Name;
                    proceduresSheet.Cell(row, 3).Value = procedure.ExecutionDate.ToString("yyyy-MM-dd");
                    proceduresSheet.Cell(row, 4).Value = user.Name;
                    proceduresSheet.Cell(row, 5).Value = procedure.SupervisorId.HasValue ? "Supervisor" : "";
                    proceduresSheet.Cell(row, 6).Value = $"procedura {procedure.Type}"; // Dodajemy typ procedury
                    row++;
                }

                // Formatuj jako tabela
                if (row > 2)
                {
                    var range = proceduresSheet.Range(1, 1, row - 1, 6);
                    var table = range.CreateTable();
                }

                // Dostosuj szerokość kolumn
                proceduresSheet.Columns().AdjustToContents();

                // Arkusz dyżurów
                var dutiesSheet = workbook.Worksheets.Add("Dyżury");

                // Nagłówki dyżurów
                dutiesSheet.Cell(1, 1).Value = "ID";
                dutiesSheet.Cell(1, 2).Value = "Imię i nazwisko";
                dutiesSheet.Cell(1, 3).Value = "Data";
                dutiesSheet.Cell(1, 4).Value = "Lokalizacja";
                dutiesSheet.Cell(1, 5).Value = "Godziny";

                // Dane dyżurów
                row = 2;
                foreach (var duty in duties)
                {
                    dutiesSheet.Cell(row, 1).Value = row - 1;
                    dutiesSheet.Cell(row, 2).Value = user.Name;
                    dutiesSheet.Cell(row, 3).Value = duty.StartTime.ToString("yyyy-MM-dd");
                    dutiesSheet.Cell(row, 4).Value = duty.Location;
                    dutiesSheet.Cell(row, 5).Value = duty.DurationInHours;
                    row++;
                }

                // Formatuj jako tabela
                if (row > 2)
                {
                    var range = dutiesSheet.Range(1, 1, row - 1, 6);
                    var table = range.CreateTable();
                }

                // Dostosuj szerokość kolumn
                dutiesSheet.Columns().AdjustToContents();

                // Dodaj arkusze dla kursów i staży jeśli dostępne
                // ...

                // Zapisz do pliku
                string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string fileName = Path.Combine(documentsPath, $"Dane_SMK_{DateTime.Now:yyyyMMdd}.xlsx");
                workbook.SaveAs(fileName);

                return fileName;
            }
        }
    }
}
