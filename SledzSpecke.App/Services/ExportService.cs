using ClosedXML.Excel;
using Microsoft.Extensions.Logging;
using SledzSpecke.Core.Models;
using SledzSpecke.Core.Models.Enums;
using SledzSpecke.Infrastructure.Database;
using System.Text;

namespace SledzSpecke.App.Services
{
    public class ExportService
    {
        private readonly DatabaseService _databaseService;
        private readonly ILogger<ExportService> _logger;

        public ExportService(DatabaseService databaseService, ILogger<ExportService> logger)
        {
            _databaseService = databaseService;
            _logger = logger;
        }

        public async Task<string> ExportToSMKAsync(SMKExportOptions options)
        {
            try
            {
                // Get current specialization with all related data
                var specialization = await _databaseService.GetCurrentSpecializationAsync();

                if (specialization == null)
                {
                    throw new Exception("No active specialization found.");
                }

                // Load related data
                var courses = await _databaseService.QueryAsync<Course>("SELECT * FROM Courses WHERE SpecializationId = ?", specialization.Id);
                var internships = await _databaseService.QueryAsync<Internship>("SELECT * FROM Internships WHERE SpecializationId = ?", specialization.Id);
                var procedures = await _databaseService.QueryAsync<MedicalProcedure>("SELECT * FROM MedicalProcedures WHERE SpecializationId = ?", specialization.Id);

                // Load procedure entries
                foreach (var procedure in procedures)
                {
                    procedure.Entries = await _databaseService.QueryAsync<ProcedureEntry>(
                        "SELECT * FROM ProcedureEntries WHERE ProcedureId = ?", procedure.Id);
                    procedure.CompletedCount = procedure.Entries.Count;
                }

                // Prepare data for export based on options
                var data = new List<Dictionary<string, string>>();

                // Filter data based on options
                if (options.IncludeCourses)
                {
                    foreach (var course in courses.Where(c => FilterByModule(c.Module, options.ModuleFilter)))
                    {
                        if (!FilterByDate(course.CompletionDate, options.UseCustomDateRange ? options.StartDate : null, options.UseCustomDateRange ? options.EndDate : null))
                            continue;

                        var courseData = new Dictionary<string, string>
                        {
                            ["Typ"] = "Kurs",
                            ["Nazwa"] = course.Name,
                            ["Data rozpoczęcia"] = course.ScheduledDate?.ToString("dd.MM.yyyy") ?? "",
                            ["Data zakończenia"] = course.CompletionDate?.ToString("dd.MM.yyyy") ?? "",
                            ["Status"] = GetStatusText(course.IsCompleted),
                            ["Moduł"] = GetModuleText(course.Module)
                        };
                        data.Add(courseData);
                    }
                }

                if (options.IncludeInternships)
                {
                    foreach (var internship in internships.Where(i => FilterByModule(i.Module, options.ModuleFilter)))
                    {
                        if (!FilterByDate(internship.EndDate, options.UseCustomDateRange ? options.StartDate : null, options.UseCustomDateRange ? options.EndDate : null))
                            continue;

                        var internshipData = new Dictionary<string, string>
                        {
                            ["Typ"] = "Staż",
                            ["Nazwa"] = internship.Name,
                            ["Data rozpoczęcia"] = internship.StartDate?.ToString("dd.MM.yyyy") ?? "",
                            ["Data zakończenia"] = internship.EndDate?.ToString("dd.MM.yyyy") ?? "",
                            ["Status"] = GetStatusText(internship.IsCompleted),
                            ["Moduł"] = GetModuleText(internship.Module),
                            ["Miejsce"] = internship.Location ?? "",
                            ["Opiekun"] = internship.SupervisorName ?? ""
                        };
                        data.Add(internshipData);
                    }
                }

                if (options.IncludeProcedures)
                {
                    var proceduresGrouped = procedures
                        .Where(p => FilterByModule(p.Module, options.ModuleFilter))
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
                            ["Moduł"] = GetModuleText(group.Key.Module)
                        };
                        data.Add(procedureData);
                    }
                }

                // Generate export file
                string filePath;
                if (options.Format == ExportFormat.Excel)
                {
                    filePath = await ExportToExcelAsync(data, options);
                }
                else
                {
                    filePath = await ExportToCsvAsync(data, options);
                }

                _logger.LogInformation("Export completed successfully. File saved at: {FilePath}", filePath);
                return filePath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during export");
                throw;
            }
        }

        private async Task<string> ExportToExcelAsync(List<Dictionary<string, string>> data, SMKExportOptions options)
        {
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
                    var rowValues = columns.Select(col => row.ContainsKey(col) ? EscapeCsvValue(row[col]) : "").ToList();
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