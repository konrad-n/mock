using ClosedXML.Excel;
using Microsoft.Extensions.Logging;
using SledzSpecke.App.Services.Interfaces;
using SledzSpecke.Core.Models;
using SledzSpecke.Core.Models.Enums;
using SledzSpecke.Infrastructure.Database;

namespace SledzSpecke.App.Services.Implementations
{
    public class ExportService : IExportService
    {
        private readonly IDatabaseService databaseService;
        private readonly ILogger<ExportService> logger;

        public ExportService(
            IDatabaseService databaseService,
            ILogger<ExportService> logger)
        {
            this.databaseService = databaseService;
            this.logger = logger;
        }

        public async Task<string> ExportToSMKAsync(SmkExportOptions options)
        {
            try
            {
                var specialization = await this.databaseService.GetCurrentSpecializationAsync();

                if (specialization == null)
                {
                    throw new OperationCanceledException("No active specialization found.");
                }

                string filePath = string.Empty;

                if (options.ExportType == SmkExportType.General)
                {
                    filePath = await this.ExportGeneralToExcelAsync(specialization, options);
                }
                else if (options.ExportType == SmkExportType.Procedures)
                {
                    filePath = await this.ExportProceduresToExcelAsync(specialization, options);
                }
                else if (options.ExportType == SmkExportType.DutyShifts)
                {
                    filePath = await this.ExportDutyShiftsToExcelAsync(specialization, options);
                }

                this.logger.LogInformation("Export completed successfully. File saved at: {FilePath}", filePath);
                return filePath;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error during export");
                throw;
            }
        }

        private async Task<string> ExportGeneralToExcelAsync(Specialization specialization, SmkExportOptions options)
        {
            var courses = await this.databaseService.QueryAsync<Course>("SELECT * FROM Courses WHERE SpecializationId = ?", specialization.Id);
            var internships = await this.databaseService.QueryAsync<Internship>("SELECT * FROM Internships WHERE SpecializationId = ?", specialization.Id);
            var procedures = await this.databaseService.QueryAsync<MedicalProcedure>("SELECT * FROM MedicalProcedures WHERE SpecializationId = ?", specialization.Id);

            foreach (var procedure in procedures)
            {
                procedure.Entries = await this.databaseService.QueryAsync<ProcedureEntry>(
                    "SELECT * FROM ProcedureEntries WHERE ProcedureId = ?", procedure.Id);
                procedure.CompletedCount = procedure.Entries.Count;
            }

            var data = new List<Dictionary<string, string>>();

            if (options.IncludeCourses)
            {
                foreach (var course in courses.Where(c => this.FilterByModule(c.Module, options.ModuleFilter)))
                {
                    if (!this.FilterByDate(course.CompletionDate, options.UseCustomDateRange ? options.StartDate : null, options.UseCustomDateRange ? options.EndDate : null))
                    {
                        continue;
                    }

                    var courseData = new Dictionary<string, string>
                    {
                        ["Typ"] = "Kurs",
                        ["Nazwa"] = course.Name,
                        ["Data rozpoczecia"] = course.ScheduledDate?.ToString("dd.MM.yyyy")
                            ?? string.Empty,
                        ["Data zakonczenia"] = course.CompletionDate?.ToString("dd.MM.yyyy")
                            ?? string.Empty,
                        ["Status"] = this.GetStatusText(course.IsCompleted),
                        ["Modul"] = this.GetModuleText(course.Module),
                    };

                    data.Add(courseData);
                }
            }

            if (options.IncludeInternships)
            {
                foreach (var internship in internships.Where(i => this.FilterByModule(i.Module, options.ModuleFilter)))
                {
                    if (!this.FilterByDate(internship.EndDate, options.UseCustomDateRange ? options.StartDate : null, options.UseCustomDateRange ? options.EndDate : null))
                    {
                        continue;
                    }

                    Dictionary<string, string> internshipData = new Dictionary<string, string>
                    {
                        ["Typ"] = "Staz",
                        ["Nazwa"] = internship.Name,
                        ["Data rozpoczecia"] = internship.StartDate?.ToString("dd.MM.yyyy") ?? string.Empty,
                        ["Data zakonczenia"] = internship.EndDate?.ToString("dd.MM.yyyy") ?? string.Empty,
                        ["Status"] = this.GetStatusText(internship.IsCompleted),
                        ["Modul"] = this.GetModuleText(internship.Module),
                        ["Miejsce"] = internship.Location
                            ?? string.Empty,
                        ["Opiekun"] = internship.SupervisorName
                            ?? string.Empty,
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

                    if (options.UseCustomDateRange
                        && options.StartDate.HasValue
                        && options.EndDate.HasValue
                        && !entries.Any(e => e.Date >= options.StartDate && e.Date <= options.EndDate))
                    {
                        continue;
                    }

                    var procedureData = new Dictionary<string, string>
                    {
                        ["Typ"] = "Procedura",
                        ["Nazwa"] = group.Key.Name,
                        ["Kod"] = group.Key.ProcedureType == ProcedureType.TypeA
                            ? "A"
                            : "B",
                        ["Wymagane"] = group.Sum(p => p.RequiredCount).ToString(),
                        ["Wykonane"] = group.Sum(p => p.CompletedCount).ToString(),
                        ["Status"] = group.Sum(p => p.CompletedCount) >= group.Sum(p => p.RequiredCount)
                            ? "Ukonczono"
                            : "W trakcie",
                        ["Modul"] = this.GetModuleText(group.Key.Module),
                    };
                    data.Add(procedureData);
                }
            }

            string fileName = $"SMKexport_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            string filePath = Path.Combine(FileSystem.CacheDirectory, fileName);

            var columns = data.SelectMany(d => d.Keys).Distinct().ToList();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("SMKexport");

                for (int i = 0; i < columns.Count; i++)
                {
                    worksheet.Cell(1, i + 1).Value = columns[i];
                    worksheet.Cell(1, i + 1).Style.Font.Bold = true;
                }

                for (int rowIndex = 0; rowIndex < data.Count; rowIndex++)
                {
                    var rowData = data[rowIndex];
                    for (int colIndex = 0; colIndex < columns.Count; colIndex++)
                    {
                        string colName = columns[colIndex];
                        worksheet.Cell(rowIndex + 2, colIndex + 1).Value = rowData.ContainsKey(colName)
                            ? rowData[colName]
                            : string.Empty;
                    }
                }

                worksheet.Columns().AdjustToContents();
                workbook.SaveAs(filePath);
            }

            return filePath;
        }

        private async Task<string> ExportProceduresToExcelAsync(Specialization specialization, SmkExportOptions options)
        {
            string fileName = $"SMKprocedury_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            string filePath = Path.Combine(FileSystem.CacheDirectory, fileName);
            var procedures = await this.databaseService.QueryAsync<MedicalProcedure>(
                "SELECT * FROM MedicalProcedures WHERE SpecializationId = ?", specialization.Id);
            var procedureEntries = new List<(string PatientName, string PatientGender, DateTime Date, string PerformingDoctor, string AssistingDoctors, string ProcedureGroup)>();

            foreach (var procedure in procedures.Where(p => this.FilterByModule(p.Module, options.ModuleFilter)))
            {
                var entries = await this.databaseService.QueryAsync<ProcedureEntry>(
                    "SELECT * FROM ProcedureEntries WHERE ProcedureId = ?", procedure.Id);

                if (options.UseCustomDateRange
                    && options.StartDate.HasValue
                    && options.EndDate.HasValue)
                {
                    entries = entries.Where(e => e.Date >= options.StartDate && e.Date <= options.EndDate).ToList();
                }

                if (entries.Count == 0)
                {
                    continue;
                }

                string internshipName = "Nieokreslony";
                if (procedure.InternshipId.HasValue)
                {
                    var internship = await this.databaseService.GetByIdAsync<Internship>(procedure.InternshipId.Value);
                    if (internship != null)
                    {
                        internshipName = internship.Name;
                    }
                }

                foreach (var entry in entries)
                {
                    string procedureWithType = $"{procedure.Name} (Kod {(procedure.ProcedureType == ProcedureType.TypeA ? "A" : "B")})";
                    string procedureGroup = !string.IsNullOrEmpty(entry.ProcedureGroup)
                        ? entry.ProcedureGroup
                        : $"{procedureWithType} - {internshipName}";

                    var settings = await this.databaseService.GetUserSettingsAsync();
                    string doctorName = settings.Username ?? "Lekarz";

                    string assistingDoctors = procedure.ProcedureType == ProcedureType.TypeA
                        ? string.IsNullOrEmpty(entry.FirstAssistantData)
                            ? entry.SupervisorName
                            : entry.FirstAssistantData
                        : doctorName;

                    if (!string.IsNullOrEmpty(entry.SecondAssistantData))
                    {
                        assistingDoctors += $", {entry.SecondAssistantData}";
                    }

                    procedureEntries.Add((
                        PatientName: entry.PatientId,
                        PatientGender: entry.PatientGender ?? string.Empty,
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

                worksheet.Cell(1, 1).Value = "Imie i nazwisko";
                worksheet.Cell(1, 2).Value = "Data";
                worksheet.Cell(1, 3).Value = "Osoba Wykonujaca";
                worksheet.Cell(1, 4).Value = "Dane Asystentów";
                worksheet.Cell(1, 5).Value = "Procedura z grupy";

                var headerRange = worksheet.Range("A1:E1");
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;

                int row = 2;
                foreach (var entry in procedureEntries.OrderBy(e => e.Date))
                {
                    worksheet.Cell(row, 1).Value = entry.PatientName;
                    worksheet.Cell(row, 2).Value = entry.Date.ToString("yyyy-MM-dd");
                    worksheet.Cell(row, 3).Value = entry.PerformingDoctor;
                    worksheet.Cell(row, 4).Value = entry.AssistingDoctors;
                    worksheet.Cell(row, 5).Value = entry.ProcedureGroup;
                    row++;
                }

                var infoSheet = workbook.Worksheets.Add("Instrukcja");
                infoSheet.Cell(1, 1).Value = "Instrukcja importu danych do SMK";
                infoSheet.Cell(1, 1).Style.Font.Bold = true;
                infoSheet.Cell(1, 1).Style.Font.FontSize = 14;

                infoSheet.Cell(3, 1).Value = "Format danych:";
                infoSheet.Cell(3, 1).Style.Font.Bold = true;

                infoSheet.Cell(4, 1).Value = "1. Imie i nazwisko - identyfikator pacjenta";
                infoSheet.Cell(5, 1).Value = "2. Data - data w formacie RRRR-MM-DD";
                infoSheet.Cell(6, 1).Value = "3. Osoba Wykonujaca - lekarz wykonujacy procedure";
                infoSheet.Cell(7, 1).Value = "4. Dane Asystentów - osoby asystujace przy procedurze";
                infoSheet.Cell(8, 1).Value = "5. Procedura z grupy - nazwa procedury i grupa";

                worksheet.Columns().AdjustToContents();
                infoSheet.Columns().AdjustToContents();

                workbook.SaveAs(filePath);
            }

            return filePath;
        }

        private async Task<string> ExportDutyShiftsToExcelAsync(Specialization specialization, SmkExportOptions options)
        {
            string fileName = $"SMKdyzury_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            string filePath = Path.Combine(FileSystem.CacheDirectory, fileName);

            var dutyShifts = await this.databaseService.QueryAsync<DutyShift>(
                "SELECT * FROM DutyShifts WHERE SpecializationId = ?", specialization.Id);

            if (options.UseCustomDateRange
                && options.StartDate.HasValue
                && options.EndDate.HasValue)
            {
                dutyShifts = dutyShifts.Where(d => d.StartDate >= options.StartDate && d.StartDate <= options.EndDate).ToList();
            }

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Dyzury");

                worksheet.Cell(1, 1).Value = "Liczba godzin";
                worksheet.Cell(1, 2).Value = "Liczba minut";
                worksheet.Cell(1, 3).Value = "Data rozpoczecia";
                worksheet.Cell(1, 4).Value = "Nazwa komórki organizacyjnej";

                var headerRange = worksheet.Range("A1:D1");
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;

                int row = 2;
                foreach (var duty in dutyShifts.OrderByDescending(d => d.StartDate))
                {
                    int totalHours = duty.DurationHoursInt;
                    int totalMinutes = duty.DurationMinutes;

                    worksheet.Cell(row, 1).Value = totalHours;
                    worksheet.Cell(row, 2).Value = totalMinutes;
                    worksheet.Cell(row, 3).Value = duty.StartDate.ToString("yyyy-MM-dd")
                    worksheet.Cell(row, 4).Value = !string.IsNullOrEmpty(duty.DepartmentName) ? duty.DepartmentName : duty.Location;
                    row++;
                }

                worksheet.Cell(row + 1, 1).Value = "Suma:";
                worksheet.Cell(row + 1, 1).Style.Font.Bold = true;

                double totalDuration = dutyShifts.Sum(d => d.DurationHours);
                int sumHours = (int)Math.Floor(totalDuration);
                int sumMinutes = (int)Math.Round((totalDuration - sumHours) * 60);

                worksheet.Cell(row + 1, 2).Value = $"{sumHours} godz. {sumMinutes} min.";
                worksheet.Cell(row + 1, 2).Style.Font.Bold = true;

                var infoSheet = workbook.Worksheets.Add("Instrukcja");
                infoSheet.Cell(1, 1).Value = "Instrukcja importu danych do SMK";
                infoSheet.Cell(1, 1).Style.Font.Bold = true;
                infoSheet.Cell(1, 1).Style.Font.FontSize = 14;

                infoSheet.Cell(3, 1).Value = "Format danych:";
                infoSheet.Cell(3, 1).Style.Font.Bold = true;

                infoSheet.Cell(4, 1).Value = "1. Liczba godzin - calkowita liczba godzin dyzuru";
                infoSheet.Cell(5, 1).Value = "2. Liczba minut - dodatkowe minuty (ponad pelne godziny)";
                infoSheet.Cell(6, 1).Value = "3. Data rozpoczecia - data w formacie RRRR-MM-DD";
                infoSheet.Cell(7, 1).Value = "4. Nazwa komórki organizacyjnej - miejsce dyzuru";

                worksheet.Columns().AdjustToContents();
                infoSheet.Columns().AdjustToContents();
                workbook.SaveAs(filePath);
            }

            return filePath;
        }

        private bool FilterByModule(ModuleType module, SmkExportModuleFilter filter)
        {
            return filter switch
            {
                SmkExportModuleFilter.All => true,
                SmkExportModuleFilter.BasicOnly => module == ModuleType.Basic,
                SmkExportModuleFilter.SpecialisticOnly => module == ModuleType.Specialistic,
                _ => true
            };
        }

        private bool FilterByDate(DateTime? date, DateTime? startDate, DateTime? endDate)
        {
            if (!date.HasValue)
            {
                return true;
            }

            if (startDate.HasValue && date < startDate)
            {
                return false;
            }

            if (endDate.HasValue && date > endDate)
            {
                return false;
            }

            return true;
        }

        private string GetStatusText(bool isCompleted)
        {
            return isCompleted ?
                "Ukonczono" :
                "W trakcie";
        }

        private string GetModuleText(ModuleType module)
        {
            return module == ModuleType.Basic
                ? "Podstawowy"
                : "Specjalistyczny";
        }
    }
}
