using ClosedXML.Excel;
using Microsoft.Extensions.Logging;
using SledzSpecke.App.Services.Interfaces;
using SledzSpecke.Core.Models;
using SledzSpecke.Core.Models.Enums;
using SledzSpecke.Infrastructure.Database;
using System.Text;

namespace SledzSpecke.App.Services.Implementations
{
    public class ExportService : IExportService
    {
        private readonly IDatabaseService _databaseService;
        private readonly ILogger<ExportService> _logger;

        public ExportService(
            IDatabaseService databaseService,
            ILogger<ExportService> logger)
        {
            this._databaseService = databaseService;
            this._logger = logger;
        }

        public async Task<string> ExportToSMKAsync(SMKExportOptions options)
        {
            try
            {
                // Get current specialization with all related data
                var specialization = await this._databaseService.GetCurrentSpecializationAsync();

                if (specialization == null)
                {
                    throw new Exception("No active specialization found.");
                }

                string filePath = string.Empty;

                // Export based on selected type
                if (options.ExportType == SMKExportType.General)
                {
                    filePath = await this.ExportGeneralToExcelAsync(specialization, options);
                }
                else if (options.ExportType == SMKExportType.Procedures)
                {
                    filePath = await this.ExportProceduresToExcelAsync(specialization, options);
                }
                else if (options.ExportType == SMKExportType.DutyShifts)
                {
                    filePath = await this.ExportDutyShiftsToExcelAsync(specialization, options);
                }

                this._logger.LogInformation("Export completed successfully. File saved at: {FilePath}", filePath);
                return filePath;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error during export");
                throw;
            }
        }

        private async Task<string> ExportGeneralToExcelAsync(Specialization specialization, SMKExportOptions options)
        {
            // Load related data
            var courses = await this._databaseService.QueryAsync<Course>("SELECT * FROM Courses WHERE SpecializationId = ?", specialization.Id);
            var internships = await this._databaseService.QueryAsync<Internship>("SELECT * FROM Internships WHERE SpecializationId = ?", specialization.Id);
            var procedures = await this._databaseService.QueryAsync<MedicalProcedure>("SELECT * FROM MedicalProcedures WHERE SpecializationId = ?", specialization.Id);

            // Load procedure entries
            foreach (var procedure in procedures)
            {
                procedure.Entries = await this._databaseService.QueryAsync<ProcedureEntry>(
                    "SELECT * FROM ProcedureEntries WHERE ProcedureId = ?", procedure.Id);
                procedure.CompletedCount = procedure.Entries.Count;
            }

            // Prepare data for export based on options
            var data = new List<Dictionary<string, string>>();

            // Filter data based on options
            if (options.IncludeCourses)
            {
                foreach (var course in courses.Where(c => this.FilterByModule(c.Module, options.ModuleFilter)))
                {
                    if (!this.FilterByDate(course.CompletionDate, options.UseCustomDateRange ? options.StartDate : null, options.UseCustomDateRange ? options.EndDate : null))
                        continue;

                    var courseData = new Dictionary<string, string>
                    {
                        ["Typ"] = "Kurs",
                        ["Nazwa"] = course.Name,
                        ["Data rozpoczęcia"] = course.ScheduledDate?.ToString("dd.MM.yyyy") ?? "",
                        ["Data zakończenia"] = course.CompletionDate?.ToString("dd.MM.yyyy") ?? "",
                        ["Status"] = this.GetStatusText(course.IsCompleted),
                        ["Moduł"] = this.GetModuleText(course.Module)
                    };
                    data.Add(courseData);
                }
            }

            if (options.IncludeInternships)
            {
                foreach (var internship in internships.Where(i => this.FilterByModule(i.Module, options.ModuleFilter)))
                {
                    if (!this.FilterByDate(internship.EndDate, options.UseCustomDateRange ? options.StartDate : null, options.UseCustomDateRange ? options.EndDate : null))
                        continue;

                    var internshipData = new Dictionary<string, string>
                    {
                        ["Typ"] = "Staż",
                        ["Nazwa"] = internship.Name,
                        ["Data rozpoczęcia"] = internship.StartDate?.ToString("dd.MM.yyyy") ?? "",
                        ["Data zakończenia"] = internship.EndDate?.ToString("dd.MM.yyyy") ?? "",
                        ["Status"] = this.GetStatusText(internship.IsCompleted),
                        ["Moduł"] = this.GetModuleText(internship.Module),
                        ["Miejsce"] = internship.Location ?? "",
                        ["Opiekun"] = internship.SupervisorName ?? ""
                    };
                    data.Add(internshipData);
                }
            }

            if (options.IncludeProcedures)
            {
                var proceduresGrouped = procedures
                    .Where(p => this.FilterByModule(p.Module, options.ModuleFilter))
                    .GroupBy(p => new { p.Name, p.ProcedureType, p.Module })
                    .ToList();

                foreach (var group in proceduresGrouped)
                {
                    var entries = group.SelectMany(p => p.Entries).ToList();

                    if (options.UseCustomDateRange && options.StartDate.HasValue && options.EndDate.HasValue &&
                        !entries.Any(e => e.Date >= options.StartDate && e.Date <= options.EndDate))
                        continue;

                    var procedureData = new Dictionary<string, string>
                    {
                        ["Typ"] = "Procedura",
                        ["Nazwa"] = group.Key.Name,
                        ["Kod"] = group.Key.ProcedureType == ProcedureType.TypeA ? "A" : "B",
                        ["Wymagane"] = group.Sum(p => p.RequiredCount).ToString(),
                        ["Wykonane"] = group.Sum(p => p.CompletedCount).ToString(),
                        ["Status"] = group.Sum(p => p.CompletedCount) >= group.Sum(p => p.RequiredCount) ? "Ukończono" : "W trakcie",
                        ["Moduł"] = this.GetModuleText(group.Key.Module)
                    };
                    data.Add(procedureData);
                }
            }

            // Create a unique filename
            string fileName = $"SMK_Export_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            string filePath = Path.Combine(FileSystem.CacheDirectory, fileName);

            // Ensure all rows have the same columns by finding all unique column names
            var columns = data.SelectMany(d => d.Keys).Distinct().ToList();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("SMK_Export");

                // Add header row
                for (int i = 0; i < columns.Count; i++)
                {
                    worksheet.Cell(1, i + 1).Value = columns[i];
                    worksheet.Cell(1, i + 1).Style.Font.Bold = true;
                }

                // Add data rows
                for (int rowIndex = 0; rowIndex < data.Count; rowIndex++)
                {
                    var rowData = data[rowIndex];
                    for (int colIndex = 0; colIndex < columns.Count; colIndex++)
                    {
                        string colName = columns[colIndex];
                        worksheet.Cell(rowIndex + 2, colIndex + 1).Value = rowData.ContainsKey(colName) ? rowData[colName] : "";
                    }
                }

                // Auto-fit columns
                worksheet.Columns().AdjustToContents();

                // Save the workbook
                workbook.SaveAs(filePath);
            }

            return filePath;
        }

        private async Task<string> ExportProceduresToExcelAsync(Specialization specialization, SMKExportOptions options)
        {
            // Create a unique filename
            string fileName = $"SMK_Procedury_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            string filePath = Path.Combine(FileSystem.CacheDirectory, fileName);

            // Load procedures and entries
            var procedures = await this._databaseService.QueryAsync<MedicalProcedure>(
                "SELECT * FROM MedicalProcedures WHERE SpecializationId = ?", specialization.Id);

            // Collect all procedure entries in the format required by SMK
            var procedureEntries = new List<(string PatientName, string PatientGender, DateTime Date, string PerformingDoctor, string AssistingDoctors, string ProcedureGroup)>();

            foreach (var procedure in procedures.Where(p => this.FilterByModule(p.Module, options.ModuleFilter)))
            {
                // Load entries for this procedure
                var entries = await this._databaseService.QueryAsync<ProcedureEntry>(
                    "SELECT * FROM ProcedureEntries WHERE ProcedureId = ?", procedure.Id);

                // Filter by date range if needed
                if (options.UseCustomDateRange && options.StartDate.HasValue && options.EndDate.HasValue)
                {
                    entries = entries.Where(e => e.Date >= options.StartDate && e.Date <= options.EndDate).ToList();
                }

                // Skip if no entries
                if (!entries.Any())
                    continue;

                // Get internship name for the procedure group
                string internshipName = "Nieokreślony";
                if (procedure.InternshipId.HasValue)
                {
                    var internship = await this._databaseService.GetByIdAsync<Internship>(procedure.InternshipId.Value);
                    if (internship != null)
                    {
                        internshipName = internship.Name;
                    }
                }

                // Create entry for each procedure execution
                foreach (var entry in entries)
                {
                    string procedureWithType = $"{procedure.Name} (Kod {(procedure.ProcedureType == ProcedureType.TypeA ? "A" : "B")})";

                    // Determine the procedure group - use the specific one if available
                    string procedureGroup = !string.IsNullOrEmpty(entry.ProcedureGroup) ?
                        entry.ProcedureGroup :
                        $"{procedureWithType} - {internshipName}";

                    // Get the user settings for doctor's name
                    var settings = await this._databaseService.GetUserSettingsAsync();
                    string doctorName = settings.Username ?? "Lekarz";

                    string assistingDoctors = procedure.ProcedureType == ProcedureType.TypeA ?
                        string.IsNullOrEmpty(entry.FirstAssistantData) ? entry.SupervisorName : entry.FirstAssistantData :
                        doctorName;

                    // Add second assistant if available
                    if (!string.IsNullOrEmpty(entry.SecondAssistantData))
                    {
                        assistingDoctors += $", {entry.SecondAssistantData}";
                    }

                    procedureEntries.Add((
                        PatientName: entry.PatientId,
                        PatientGender: entry.PatientGender ?? "",
                        entry.Date,
                        PerformingDoctor: procedure.ProcedureType == ProcedureType.TypeA ? doctorName : entry.SupervisorName,
                        AssistingDoctors: assistingDoctors,
                        ProcedureGroup: procedureGroup
                    ));
                }
            }

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Procedury");

                // Add header row with SMK format
                worksheet.Cell(1, 1).Value = "Imię i nazwisko";
                worksheet.Cell(1, 2).Value = "Data";
                worksheet.Cell(1, 3).Value = "Osoba Wykonująca";
                worksheet.Cell(1, 4).Value = "Dane Asystentów";
                worksheet.Cell(1, 5).Value = "Procedura z grupy";

                // Format headers - highlight required fields
                var headerRange = worksheet.Range("A1:E1");
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;

                // Add data rows
                int row = 2;
                foreach (var entry in procedureEntries.OrderBy(e => e.Date))
                {
                    worksheet.Cell(row, 1).Value = entry.PatientName;
                    worksheet.Cell(row, 2).Value = entry.Date.ToString("yyyy-MM-dd"); // SMK format
                    worksheet.Cell(row, 3).Value = entry.PerformingDoctor;
                    worksheet.Cell(row, 4).Value = entry.AssistingDoctors;
                    worksheet.Cell(row, 5).Value = entry.ProcedureGroup;
                    row++;
                }

                // Add explanation sheet
                var infoSheet = workbook.Worksheets.Add("Instrukcja");
                infoSheet.Cell(1, 1).Value = "Instrukcja importu danych do SMK";
                infoSheet.Cell(1, 1).Style.Font.Bold = true;
                infoSheet.Cell(1, 1).Style.Font.FontSize = 14;

                infoSheet.Cell(3, 1).Value = "Format danych:";
                infoSheet.Cell(3, 1).Style.Font.Bold = true;

                infoSheet.Cell(4, 1).Value = "1. Imię i nazwisko - identyfikator pacjenta";
                infoSheet.Cell(5, 1).Value = "2. Data - data w formacie RRRR-MM-DD";
                infoSheet.Cell(6, 1).Value = "3. Osoba Wykonująca - lekarz wykonujący procedurę";
                infoSheet.Cell(7, 1).Value = "4. Dane Asystentów - osoby asystujące przy procedurze";
                infoSheet.Cell(8, 1).Value = "5. Procedura z grupy - nazwa procedury i grupa";

                // Auto-adjust columns width
                worksheet.Columns().AdjustToContents();
                infoSheet.Columns().AdjustToContents();

                // Save workbook
                workbook.SaveAs(filePath);
            }

            return filePath;
        }

        private async Task<string> ExportDutyShiftsToExcelAsync(Specialization specialization, SMKExportOptions options)
        {
            // Create a unique filename
            string fileName = $"SMK_Dyzury_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            string filePath = Path.Combine(FileSystem.CacheDirectory, fileName);

            // Load duty shifts
            var dutyShifts = await this._databaseService.QueryAsync<DutyShift>(
                "SELECT * FROM DutyShifts WHERE SpecializationId = ?", specialization.Id);

            // Filter by date range if needed
            if (options.UseCustomDateRange && options.StartDate.HasValue && options.EndDate.HasValue)
            {
                dutyShifts = dutyShifts.Where(d => d.StartDate >= options.StartDate && d.StartDate <= options.EndDate).ToList();
            }

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Dyżury");

                // Add header row with SMK format
                worksheet.Cell(1, 1).Value = "Liczba godzin";
                worksheet.Cell(1, 2).Value = "Liczba minut";
                worksheet.Cell(1, 3).Value = "Data rozpoczęcia";
                worksheet.Cell(1, 4).Value = "Nazwa komórki organizacyjnej";

                // Format headers
                var headerRange = worksheet.Range("A1:D1");
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;

                // Add data rows
                int row = 2;
                foreach (var duty in dutyShifts.OrderByDescending(d => d.StartDate))
                {
                    // Calculate hours and minutes for SMK format
                    int totalHours = duty.DurationHoursInt;
                    int totalMinutes = duty.DurationMinutes;

                    worksheet.Cell(row, 1).Value = totalHours;
                    worksheet.Cell(row, 2).Value = totalMinutes;
                    worksheet.Cell(row, 3).Value = duty.StartDate.ToString("yyyy-MM-dd"); // SMK format
                    worksheet.Cell(row, 4).Value = !string.IsNullOrEmpty(duty.DepartmentName) ? duty.DepartmentName : duty.Location;
                    row++;
                }

                // Add summary row
                worksheet.Cell(row + 1, 1).Value = "Suma:";
                worksheet.Cell(row + 1, 1).Style.Font.Bold = true;

                // Calculate total duration
                double totalDuration = dutyShifts.Sum(d => d.DurationHours);
                int sumHours = (int)Math.Floor(totalDuration);
                int sumMinutes = (int)Math.Round((totalDuration - sumHours) * 60);

                worksheet.Cell(row + 1, 2).Value = $"{sumHours} godz. {sumMinutes} min.";
                worksheet.Cell(row + 1, 2).Style.Font.Bold = true;

                // Add explanation sheet
                var infoSheet = workbook.Worksheets.Add("Instrukcja");
                infoSheet.Cell(1, 1).Value = "Instrukcja importu danych do SMK";
                infoSheet.Cell(1, 1).Style.Font.Bold = true;
                infoSheet.Cell(1, 1).Style.Font.FontSize = 14;

                infoSheet.Cell(3, 1).Value = "Format danych:";
                infoSheet.Cell(3, 1).Style.Font.Bold = true;

                infoSheet.Cell(4, 1).Value = "1. Liczba godzin - całkowita liczba godzin dyżuru";
                infoSheet.Cell(5, 1).Value = "2. Liczba minut - dodatkowe minuty (ponad pełne godziny)";
                infoSheet.Cell(6, 1).Value = "3. Data rozpoczęcia - data w formacie RRRR-MM-DD";
                infoSheet.Cell(7, 1).Value = "4. Nazwa komórki organizacyjnej - miejsce dyżuru";

                // Auto-adjust columns width
                worksheet.Columns().AdjustToContents();
                infoSheet.Columns().AdjustToContents();

                // Save workbook
                workbook.SaveAs(filePath);
            }

            return filePath;
        }

        private async Task<string> ExportToCsvAsync(List<Dictionary<string, string>> data, SMKExportOptions options)
        {
            // Create a unique filename
            string fileName = $"SMK_Export_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
            string filePath = Path.Combine(FileSystem.CacheDirectory, fileName);

            // Ensure all rows have the same columns by finding all unique column names
            var columns = data.SelectMany(d => d.Keys).Distinct().ToList();

            using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                // Write header row
                await writer.WriteLineAsync(string.Join(";", columns));

                // Write data rows
                foreach (var row in data)
                {
                    var rowValues = columns.Select(col => row.ContainsKey(col) ? this.EscapeCsvValue(row[col]) : "").ToList();
                    await writer.WriteLineAsync(string.Join(";", rowValues));
                }
            }

            return filePath;
        }

        private string EscapeCsvValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return "";

            // If the value contains a semicolon, quote, or newline, wrap it in quotes and escape any quotes
            if (value.Contains(';') || value.Contains('"') || value.Contains('\n') || value.Contains('\r'))
            {
                return $"\"{value.Replace("\"", "\"\"")}\"";
            }

            return value;
        }

        private bool FilterByModule(ModuleType module, SMKExportModuleFilter filter)
        {
            return filter switch
            {
                SMKExportModuleFilter.All => true,
                SMKExportModuleFilter.BasicOnly => module == ModuleType.Basic,
                SMKExportModuleFilter.SpecialisticOnly => module == ModuleType.Specialistic,
                _ => true
            };
        }

        private bool FilterByDate(DateTime? date, DateTime? startDate, DateTime? endDate)
        {
            if (!date.HasValue)
                return true;

            if (startDate.HasValue && date < startDate)
                return false;

            if (endDate.HasValue && date > endDate)
                return false;

            return true;
        }

        private string GetStatusText(bool isCompleted)
        {
            return isCompleted ? "Ukończono" : "W trakcie";
        }

        private string GetModuleText(ModuleType module)
        {
            return module == ModuleType.Basic ? "Podstawowy" : "Specjalistyczny";
        }
    }
}