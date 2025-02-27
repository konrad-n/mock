using SledzSpecke.Core.Models;
using SledzSpecke.Models;

namespace SledzSpecke.Services
{
    public class ExportService
    {
        private readonly DataManager _dataManager;



        public ExportService(DataManager dataManager)
        {
            _dataManager = dataManager;
        }

        public async Task<string> ExportToSMKAsync(SMKExportOptions options)
        {
            try
            {
                var specialization = await _dataManager.LoadSpecializationAsync();

                // Przygotuj dane do eksportu na podstawie opcji
                var data = new List<Dictionary<string, string>>();

                // Dodaj kursy jeśli zaznaczone
                if (options.IncludeCourses)
                {
                    foreach (var course in specialization.RequiredCourses.Where(c => FilterByModule(c.Module, options.SelectedModule)))
                    {
                        if (!FilterByDate(course.CompletionDate, options.StartDate, options.EndDate))
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

                // Dodaj staże jeśli zaznaczone
                if (options.IncludeInternships)
                {
                    foreach (var internship in specialization.RequiredInternships.Where(i => FilterByModule(i.Module, options.SelectedModule)))
                    {
                        if (!FilterByDate(internship.EndDate, options.StartDate, options.EndDate))
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

                // Dodaj procedury jeśli zaznaczone
                if (options.IncludeProcedures)
                {
                    var proceduresGrouped = specialization.RequiredProcedures
                        .Where(p => FilterByModule(p.Module, options.SelectedModule))
                        .GroupBy(p => new { p.Name, p.ProcedureType, p.Module })
                        .ToList();

                    foreach (var group in proceduresGrouped)
                    {
                        var entries = group.SelectMany(p => p.Entries).ToList();

                        if (options.UseCustomDateRange && !entries.Any(e =>
                            e.Date >= options.StartDate && e.Date <= options.EndDate))
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

                // Zapisz dane do pliku
                string filename;

                if (options.Format == ExportFormat.Excel)
                {
                    filename = await ExportToExcel(data, options);
                }
                else
                {
                    filename = await ExportToCsv(data, options);
                }

                return filename;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error exporting data: {ex.Message}");
                throw;
            }
        }

        private async Task<string> ExportToExcel(List<Dictionary<string, string>> data, SMKExportOptions options)
        {
            // W rzeczywistej implementacji tutaj byłoby generowanie pliku Excel
            // Dla celów demonstracyjnych zwracamy tylko nazwę pliku
            string filename = Path.Combine(FileSystem.CacheDirectory, $"SMK_Export_{DateTime.Now.ToString("yyyyMMdd")}.xlsx");

            // Tutaj byłby kod generowania pliku Excel
            await Task.Delay(1000); // Symulacja czasu potrzebnego na generowanie pliku

            return filename;
        }

        private async Task<string> ExportToCsv(List<Dictionary<string, string>> data, SMKExportOptions options)
        {
            // W rzeczywistej implementacji tutaj byłoby generowanie pliku CSV
            // Dla celów demonstracyjnych zwracamy tylko nazwę pliku
            string filename = Path.Combine(FileSystem.CacheDirectory, $"SMK_Export_{DateTime.Now.ToString("yyyyMMdd")}.csv");

            // Tutaj byłby kod generowania pliku CSV
            await Task.Delay(1000); // Symulacja czasu potrzebnego na generowanie pliku

            return filename;
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

    public class SMKExportOptions
    {
        public bool IncludeCourses { get; set; } = true;
        public bool IncludeInternships { get; set; } = true;
        public bool IncludeProcedures { get; set; } = true;
        public ExportFormat Format { get; set; } = ExportFormat.Excel;
        public SMKExportModuleFilter SelectedModule { get; set; } = SMKExportModuleFilter.All;
        public bool UseCustomDateRange { get; set; } = false;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public enum ExportFormat
    {
        Excel,
        Csv
    }

    public enum SMKExportModuleFilter
    {
        All,
        BasicOnly,
        SpecialisticOnly
    }
}