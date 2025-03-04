using OfficeOpenXml;
using OfficeOpenXml.Style;
using SledzSpecke.App.Helpers;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Database;
using SledzSpecke.App.Services.SmkStrategy;
using SledzSpecke.App.Services.Specialization;

namespace SledzSpecke.App.Services.Export
{
    public class ExportService : IExportService
    {
        private readonly IDatabaseService databaseService;
        private readonly ISpecializationService specializationService;
        private readonly ISmkVersionStrategy smkStrategy;
        private string lastExportFilePath;

        public ExportService(
            IDatabaseService databaseService,
            ISpecializationService specializationService,
            ISmkVersionStrategy smkStrategy)
        {
            this.databaseService = databaseService;
            this.specializationService = specializationService;
            this.smkStrategy = smkStrategy;

            // Inicjalizacja biblioteki EPPlus
            ExcelHelper.Initialize();
        }

        public async Task<string> ExportToExcelAsync(ExportOptions options)
        {
            try
            {
                // Upewnij się, że katalog eksportu istnieje
                await FileAccessHelper.EnsureFolderExistsAsync(Constants.ExportsPath);

                // Pobierz dane specjalizacji
                var specialization = await this.specializationService.GetCurrentSpecializationAsync();
                if (specialization == null)
                {
                    throw new Exception("Nie znaleziono aktywnej specjalizacji");
                }

                // Pobierz dane użytkownika
                var user = await this.specializationService.GetCurrentUserAsync();
                if (user == null)
                {
                    throw new Exception("Nie znaleziono aktywnego użytkownika");
                }

                // Utwórz nazwy pliku i pełną ścieżkę
                string fileName = $"SledzSpecke_{specialization.Name}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                string filePath = Path.Combine(Constants.ExportsPath, fileName);

                using (var package = new ExcelPackage())
                {
                    // Dodaj arkusz z podsumowaniem
                    this.AddSummaryWorksheet(package, specialization, user, options);

                    // Dodaj arkusze z danymi, jeśli są zaznaczone w opcjach
                    if (options.IncludeShifts)
                    {
                        // Pobierz dyżury
                        var shifts = await this.GetShiftsForExportAsync(specialization.SpecializationId, options);
                        this.AddMedicalShiftsWorksheet(package, shifts, options.FormatForOldSMK);
                    }

                    if (options.IncludeProcedures)
                    {
                        // Pobierz procedury
                        var procedures = await this.GetProceduresForExportAsync(specialization.SpecializationId, options);
                        this.AddProceduresWorksheet(package, procedures, options.FormatForOldSMK);
                    }

                    if (options.IncludeInternships)
                    {
                        // Pobierz staże
                        var internships = await this.GetInternshipsForExportAsync(specialization.SpecializationId, options);
                        this.AddInternshipsWorksheet(package, internships, options.FormatForOldSMK);
                    }

                    if (options.IncludeCourses)
                    {
                        // Pobierz kursy
                        var courses = await this.GetCoursesForExportAsync(specialization.SpecializationId, options);
                        this.AddCoursesWorksheet(package, courses, options.FormatForOldSMK);
                    }

                    if (options.IncludeSelfEducation)
                    {
                        // Pobierz samokształcenie
                        var selfEducationItems = await this.GetSelfEducationForExportAsync(specialization.SpecializationId, options);
                        this.AddSelfEducationWorksheet(package, selfEducationItems, options.FormatForOldSMK);
                    }

                    if (options.IncludePublications)
                    {
                        // Pobierz publikacje
                        var publications = await this.GetPublicationsForExportAsync(specialization.SpecializationId, options);
                        this.AddPublicationsWorksheet(package, publications, options.FormatForOldSMK);
                    }

                    if (options.IncludeEducationalActivities)
                    {
                        // Pobierz aktywności edukacyjne
                        var activities = await this.GetEducationalActivitiesForExportAsync(specialization.SpecializationId, options);
                        this.AddEducationalActivitiesWorksheet(package, activities, options.FormatForOldSMK);
                    }

                    if (options.IncludeAbsences)
                    {
                        // Pobierz nieobecności
                        var absences = await this.GetAbsencesForExportAsync(specialization.SpecializationId, options);
                        this.AddAbsencesWorksheet(package, absences, options.FormatForOldSMK);
                    }

                    if (options.IncludeRecognitions)
                    {
                        // Pobierz uznania/skrócenia
                        var recognitions = await this.GetRecognitionsForExportAsync(specialization.SpecializationId, options);
                        this.AddRecognitionsWorksheet(package, recognitions, options.FormatForOldSMK);
                    }

                    // Zapisz plik Excel
                    await File.WriteAllBytesAsync(filePath, package.GetAsByteArray());
                }

                // Zapisz datę eksportu
                await this.SaveLastExportDateAsync(DateTime.Now);

                // Zapisz ścieżkę do ostatniego pliku eksportu
                this.lastExportFilePath = filePath;

                return filePath;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error during export: {ex.Message}");
                throw;
            }
        }

        public async Task<DateTime?> GetLastExportDateAsync()
        {
            return await Helpers.Settings.GetLastExportDateAsync();
        }

        public Task<string> GetLastExportFilePathAsync()
        {
            return Task.FromResult(this.lastExportFilePath);
        }

        public async Task SaveLastExportDateAsync(DateTime date)
        {
            await Helpers.Settings.SetLastExportDateAsync(date);
        }

        public async Task<bool> ShareExportFileAsync(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                return false;
            }

            try
            {
                return await FileAccessHelper.ShareFileAsync(filePath, "Eksport danych specjalizacji");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error sharing export file: {ex.Message}");
                return false;
            }
        }

        /*
         * Private Methods for Data Retrieval
         */

        private async Task<List<MedicalShift>> GetShiftsForExportAsync(int specializationId, ExportOptions options)
        {
            // Pobierz wszystkie staże dla specjalizacji lub modułu
            var internships = options.ModuleId.HasValue
                ? await this.databaseService.GetInternshipsAsync(moduleId: options.ModuleId)
                : await this.databaseService.GetInternshipsAsync(specializationId: specializationId);

            var result = new List<MedicalShift>();

            // Pobierz dyżury dla każdego stażu
            foreach (var internship in internships)
            {
                var shifts = await this.databaseService.GetMedicalShiftsAsync(internship.InternshipId);

                // Filtruj po datach, jeśli podano zakres dat
                if (options.StartDate != DateTime.MinValue && options.EndDate != DateTime.MinValue)
                {
                    shifts = shifts.Where(s => s.Date >= options.StartDate && s.Date <= options.EndDate).ToList();
                }

                // Dodaj dane o stażu do każdego dyżuru
                foreach (var shift in shifts)
                {
                    // Używamy AdditionalFields do tymczasowego przechowywania danych o stażu na potrzeby eksportu
                    var additionalFields = new Dictionary<string, object>();
                    if (!string.IsNullOrEmpty(shift.AdditionalFields))
                    {
                        try
                        {
                            additionalFields = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(shift.AdditionalFields);
                        }
                        catch
                        {
                            additionalFields = new Dictionary<string, object>();
                        }
                    }

                    additionalFields["InternshipName"] = internship.InternshipName;
                    additionalFields["InternshipLocation"] = internship.DepartmentName;
                    shift.AdditionalFields = System.Text.Json.JsonSerializer.Serialize(additionalFields);
                }

                result.AddRange(shifts);
            }

            return result;
        }

        private async Task<List<Procedure>> GetProceduresForExportAsync(int specializationId, ExportOptions options)
        {
            // Pobierz wszystkie staże dla specjalizacji lub modułu
            var internships = options.ModuleId.HasValue
                ? await this.databaseService.GetInternshipsAsync(moduleId: options.ModuleId)
                : await this.databaseService.GetInternshipsAsync(specializationId: specializationId);

            var result = new List<Procedure>();

            // Pobierz procedury dla każdego stażu
            foreach (var internship in internships)
            {
                var procedures = await this.databaseService.GetProceduresAsync(internship.InternshipId);

                // Dodaj nazwę stażu do każdej procedury
                foreach (var procedure in procedures)
                {
                    // Używamy AdditionalFields do tymczasowego przechowywania danych o stażu na potrzeby eksportu
                    var additionalFields = new Dictionary<string, object>();
                    if (!string.IsNullOrEmpty(procedure.AdditionalFields))
                    {
                        try
                        {
                            additionalFields = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(procedure.AdditionalFields);
                        }
                        catch
                        {
                            additionalFields = new Dictionary<string, object>();
                        }
                    }

                    additionalFields["InternshipName"] = internship.InternshipName;
                    additionalFields["InternshipLocation"] = internship.DepartmentName;
                    procedure.AdditionalFields = System.Text.Json.JsonSerializer.Serialize(additionalFields);
                }

                // Filtruj po datach, jeśli podano zakres dat
                if (options.StartDate != DateTime.MinValue && options.EndDate != DateTime.MinValue)
                {
                    procedures = procedures.Where(p => p.Date >= options.StartDate && p.Date <= options.EndDate).ToList();
                }

                result.AddRange(procedures);
            }

            return result;
        }

        private async Task<List<Internship>> GetInternshipsForExportAsync(int specializationId, ExportOptions options)
        {
            var internships = options.ModuleId.HasValue
                ? await this.databaseService.GetInternshipsAsync(moduleId: options.ModuleId)
                : await this.databaseService.GetInternshipsAsync(specializationId: specializationId);

            // Filtruj po datach, jeśli podano zakres dat
            if (options.StartDate != DateTime.MinValue && options.EndDate != DateTime.MinValue)
            {
                internships = internships.Where(i =>
                    (i.StartDate >= options.StartDate && i.StartDate <= options.EndDate) ||
                    (i.EndDate >= options.StartDate && i.EndDate <= options.EndDate) ||
                    (i.StartDate <= options.StartDate && i.EndDate >= options.EndDate)
                ).ToList();
            }

            return internships;
        }

        private async Task<List<Course>> GetCoursesForExportAsync(int specializationId, ExportOptions options)
        {
            var courses = options.ModuleId.HasValue
                ? await this.databaseService.GetCoursesAsync(moduleId: options.ModuleId)
                : await this.databaseService.GetCoursesAsync(specializationId: specializationId);

            // Filtruj po datach, jeśli podano zakres dat
            if (options.StartDate != DateTime.MinValue && options.EndDate != DateTime.MinValue)
            {
                courses = courses.Where(c => c.CompletionDate >= options.StartDate && c.CompletionDate <= options.EndDate).ToList();
            }

            return courses;
        }

        private async Task<List<SelfEducation>> GetSelfEducationForExportAsync(int specializationId, ExportOptions options)
        {
            var selfEducationItems = options.ModuleId.HasValue
                ? await this.databaseService.GetSelfEducationItemsAsync(moduleId: options.ModuleId)
                : await this.databaseService.GetSelfEducationItemsAsync(specializationId: specializationId);

            // Filtracja po roku, jeśli nie ma filtracji po datach
            if (options.StartDate != DateTime.MinValue && options.EndDate != DateTime.MinValue)
            {
                int startYear = options.StartDate.Year;
                int endYear = options.EndDate.Year;
                selfEducationItems = selfEducationItems.Where(s => s.Year >= startYear && s.Year <= endYear).ToList();
            }

            return selfEducationItems;
        }

        private async Task<List<Publication>> GetPublicationsForExportAsync(int specializationId, ExportOptions options)
        {
            var publications = options.ModuleId.HasValue
                ? await this.databaseService.GetPublicationsAsync(moduleId: options.ModuleId)
                : await this.databaseService.GetPublicationsAsync(specializationId: specializationId);

            // Nie ma filtracji po datach, ponieważ model Publication nie zawiera pola daty
            return publications;
        }

        private async Task<List<EducationalActivity>> GetEducationalActivitiesForExportAsync(int specializationId, ExportOptions options)
        {
            var activities = options.ModuleId.HasValue
                ? await this.databaseService.GetEducationalActivitiesAsync(moduleId: options.ModuleId)
                : await this.databaseService.GetEducationalActivitiesAsync(specializationId: specializationId);

            // Filtruj po datach, jeśli podano zakres dat
            if (options.StartDate != DateTime.MinValue && options.EndDate != DateTime.MinValue)
            {
                activities = activities.Where(a =>
                    (a.StartDate >= options.StartDate && a.StartDate <= options.EndDate) ||
                    (a.EndDate >= options.StartDate && a.EndDate <= options.EndDate) ||
                    (a.StartDate <= options.StartDate && a.EndDate >= options.EndDate)
                ).ToList();
            }

            return activities;
        }

        private async Task<List<Absence>> GetAbsencesForExportAsync(int specializationId, ExportOptions options)
        {
            var absences = await this.databaseService.GetAbsencesAsync(specializationId);

            // Filtruj po datach, jeśli podano zakres dat
            if (options.StartDate != DateTime.MinValue && options.EndDate != DateTime.MinValue)
            {
                absences = absences.Where(a =>
                    (a.StartDate >= options.StartDate && a.StartDate <= options.EndDate) ||
                    (a.EndDate >= options.StartDate && a.EndDate <= options.EndDate) ||
                    (a.StartDate <= options.StartDate && a.EndDate >= options.EndDate)
                ).ToList();
            }

            return absences;
        }

        private async Task<List<Models.Recognition>> GetRecognitionsForExportAsync(int specializationId, ExportOptions options)
        {
            var recognitions = await this.databaseService.GetRecognitionsAsync(specializationId);

            // Filtruj po datach, jeśli podano zakres dat
            if (options.StartDate != DateTime.MinValue && options.EndDate != DateTime.MinValue)
            {
                recognitions = recognitions.Where(r =>
                    (r.StartDate >= options.StartDate && r.StartDate <= options.EndDate) ||
                    (r.EndDate >= options.StartDate && r.EndDate <= options.EndDate) ||
                    (r.StartDate <= options.StartDate && r.EndDate >= options.EndDate)
                ).ToList();
            }

            return recognitions;
        }

        /*
         * Private Methods for Excel Worksheets
         */

        private void AddSummaryWorksheet(ExcelPackage package, Models.Specialization specialization, User user, ExportOptions options)
        {
            var worksheet = package.Workbook.Worksheets.Add("Podsumowanie");

            // Title
            worksheet.Cells[1, 1].Value = "PODSUMOWANIE DANYCH SPECJALIZACJI";
            worksheet.Cells[1, 1, 1, 3].Merge = true;
            worksheet.Cells[1, 1].Style.Font.Bold = true;
            worksheet.Cells[1, 1].Style.Font.Size = 16;
            worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            // Metadane użytkownika
            worksheet.Cells[3, 1].Value = "Użytkownik:";
            worksheet.Cells[3, 2].Value = user.Username;

            // Specialization Info
            worksheet.Cells[4, 1].Value = "Nazwa specjalizacji:";
            worksheet.Cells[4, 2].Value = specialization.Name;

            worksheet.Cells[5, 1].Value = "Data rozpoczęcia:";
            worksheet.Cells[5, 2].Value = specialization.StartDate;
            worksheet.Cells[5, 2].Style.Numberformat.Format = "yyyy-MM-dd";

            worksheet.Cells[6, 1].Value = "Planowana data zakończenia:";
            worksheet.Cells[6, 2].Value = specialization.PlannedEndDate;
            worksheet.Cells[6, 2].Style.Numberformat.Format = "yyyy-MM-dd";

            worksheet.Cells[7, 1].Value = "Obliczona data zakończenia:";
            worksheet.Cells[7, 2].Value = specialization.CalculatedEndDate;
            worksheet.Cells[7, 2].Style.Numberformat.Format = "yyyy-MM-dd";

            // Wersja SMK
            worksheet.Cells[8, 1].Value = "Wersja SMK:";
            worksheet.Cells[8, 2].Value = user.SmkVersion == SmkVersion.New ? "Nowa" : "Stara";

            // Export Options
            worksheet.Cells[10, 1].Value = "Zakres eksportowanych danych:";
            worksheet.Cells[10, 1].Style.Font.Bold = true;

            worksheet.Cells[11, 1].Value = "Od:";
            worksheet.Cells[11, 2].Value = options.StartDate;
            worksheet.Cells[11, 2].Style.Numberformat.Format = "yyyy-MM-dd";

            worksheet.Cells[12, 1].Value = "Do:";
            worksheet.Cells[12, 2].Value = options.EndDate;
            worksheet.Cells[12, 2].Style.Numberformat.Format = "yyyy-MM-dd";

            // Selected Categories
            worksheet.Cells[14, 1].Value = "Eksportowane kategorie:";
            worksheet.Cells[14, 1].Style.Font.Bold = true;

            int row = 15;

            if (options.IncludeInternships)
            {
                worksheet.Cells[row++, 1].Value = "- Staże kierunkowe";
            }

            if (options.IncludeCourses)
            {
                worksheet.Cells[row++, 1].Value = "- Kursy specjalizacyjne";
            }

            if (options.IncludeProcedures)
            {
                worksheet.Cells[row++, 1].Value = "- Zabiegi i procedury";
            }

            if (options.IncludeShifts)
            {
                worksheet.Cells[row++, 1].Value = "- Dyżury medyczne";
            }

            if (options.IncludeSelfEducation)
            {
                worksheet.Cells[row++, 1].Value = "- Samokształcenie";
            }

            if (options.IncludePublications)
            {
                worksheet.Cells[row++, 1].Value = "- Publikacje";
            }

            if (options.IncludeAbsences)
            {
                worksheet.Cells[row++, 1].Value = "- Nieobecności";
            }

            if (options.IncludeEducationalActivities)
            {
                worksheet.Cells[row++, 1].Value = "- Działalność edukacyjna";
            }

            if (options.IncludeRecognitions)
            {
                worksheet.Cells[row++, 1].Value = "- Uznania/skrócenia";
            }

            // Format specyficzny dla SMK
            worksheet.Cells[row + 1, 1].Value = "Format eksportu:";
            worksheet.Cells[row + 1, 1].Style.Font.Bold = true;
            worksheet.Cells[row + 2, 1].Value = options.FormatForOldSMK ? "Stara wersja SMK" : "Nowa wersja SMK";

            // Export Info
            worksheet.Cells[row + 4, 1].Value = "Data wygenerowania:";
            worksheet.Cells[row + 4, 2].Value = DateTime.Now;
            worksheet.Cells[row + 4, 2].Style.Numberformat.Format = "yyyy-MM-dd HH:mm:ss";

            // Autofit columns
            worksheet.Columns[1, 3].AutoFit();
        }

        private void AddInternshipsWorksheet(ExcelPackage package, List<Internship> internships, bool oldSmkFormat)
        {
            var worksheet = package.Workbook.Worksheets.Add("Staże kierunkowe");

            // Headers
            int col = 1;
            worksheet.Cells[1, col++].Value = "Nazwa stażu";
            worksheet.Cells[1, col++].Value = "Nazwa instytucji";
            worksheet.Cells[1, col++].Value = "Nazwa oddziału";
            worksheet.Cells[1, col++].Value = "Data rozpoczęcia";
            worksheet.Cells[1, col++].Value = "Data zakończenia";
            worksheet.Cells[1, col++].Value = "Liczba dni";
            worksheet.Cells[1, col++].Value = "Rok szkolenia";
            worksheet.Cells[1, col++].Value = "Status";

            if (oldSmkFormat)
            {
                worksheet.Cells[1, col++].Value = "Kierownik stażu"; // Specyficzne dla starej wersji SMK
            }

            // Format nagłówków
            this.FormatHeaders(worksheet, col - 1);

            // Data
            for (int i = 0; i < internships.Count; i++)
            {
                int row = i + 2;
                col = 1;

                var internship = internships[i];

                worksheet.Cells[row, col++].Value = internship.InternshipName;
                worksheet.Cells[row, col++].Value = internship.InstitutionName;
                worksheet.Cells[row, col++].Value = internship.DepartmentName;

                worksheet.Cells[row, col++].Value = internship.StartDate;
                worksheet.Cells[row, col - 1].Style.Numberformat.Format = "yyyy-MM-dd";

                worksheet.Cells[row, col++].Value = internship.EndDate;
                worksheet.Cells[row, col - 1].Style.Numberformat.Format = "yyyy-MM-dd";

                worksheet.Cells[row, col++].Value = internship.DaysCount;
                worksheet.Cells[row, col++].Value = internship.Year;

                // Status
                string status = internship.IsCompleted ?
                    (internship.IsApproved ? "Ukończony i zatwierdzony" : "Ukończony") :
                    "W trakcie";
                worksheet.Cells[row, col++].Value = status;

                // Dodatkowe pola dla starej wersji SMK
                if (oldSmkFormat && !string.IsNullOrEmpty(internship.AdditionalFields))
                {
                    try
                    {
                        var additionalFields = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(internship.AdditionalFields);

                        if (additionalFields.TryGetValue("OldSMKField1", out object supervisor))
                        {
                            worksheet.Cells[row, col++].Value = supervisor?.ToString();
                        }
                        else
                        {
                            col++;
                        }
                    }
                    catch
                    {
                        col++;
                    }
                }
            }

            // Autofit columns
            worksheet.Columns[1, col - 1].AutoFit();
        }

        private void AddCoursesWorksheet(ExcelPackage package, List<Course> courses, bool oldSmkFormat)
        {
            var worksheet = package.Workbook.Worksheets.Add("Kursy");

            // Headers
            int col = 1;
            worksheet.Cells[1, col++].Value = "Nazwa kursu";
            worksheet.Cells[1, col++].Value = "Typ kursu";
            worksheet.Cells[1, col++].Value = "Numer kursu";
            worksheet.Cells[1, col++].Value = "Instytucja";
            worksheet.Cells[1, col++].Value = "Data ukończenia";
            worksheet.Cells[1, col++].Value = "Rok szkolenia";

            if (oldSmkFormat)
            {
                worksheet.Cells[1, col++].Value = "Numer kolejny";
            }

            worksheet.Cells[1, col++].Value = "Posiada certyfikat";
            worksheet.Cells[1, col++].Value = "Numer certyfikatu";
            worksheet.Cells[1, col++].Value = "Data certyfikatu";

            // Format nagłówków
            this.FormatHeaders(worksheet, col - 1);

            // Data
            for (int i = 0; i < courses.Count; i++)
            {
                int row = i + 2;
                col = 1;

                var course = courses[i];

                worksheet.Cells[row, col++].Value = course.CourseName;
                worksheet.Cells[row, col++].Value = course.CourseType;
                worksheet.Cells[row, col++].Value = course.CourseNumber;
                worksheet.Cells[row, col++].Value = course.InstitutionName;

                worksheet.Cells[row, col++].Value = course.CompletionDate;
                worksheet.Cells[row, col - 1].Style.Numberformat.Format = "yyyy-MM-dd";

                worksheet.Cells[row, col++].Value = course.Year;

                if (oldSmkFormat)
                {
                    worksheet.Cells[row, col++].Value = course.CourseSequenceNumber;
                }

                worksheet.Cells[row, col++].Value = course.HasCertificate ? "Tak" : "Nie";
                worksheet.Cells[row, col++].Value = course.CertificateNumber;

                if (course.CertificateDate.HasValue)
                {
                    worksheet.Cells[row, col++].Value = course.CertificateDate;
                    worksheet.Cells[row, col - 1].Style.Numberformat.Format = "yyyy-MM-dd";
                }
                else
                {
                    col++;
                }
            }

            // Autofit columns
            worksheet.Columns[1, col - 1].AutoFit();
        }

        private void AddProceduresWorksheet(ExcelPackage package, List<Procedure> procedures, bool oldSmkFormat)
        {
            var worksheet = package.Workbook.Worksheets.Add("Procedury");

            // Headers
            int col = 1;
            worksheet.Cells[1, col++].Value = "Data";
            worksheet.Cells[1, col++].Value = "Kod zabiegu";
            worksheet.Cells[1, col++].Value = "Operator/Asysta";
            worksheet.Cells[1, col++].Value = "Miejsce wykonania";
            worksheet.Cells[1, col++].Value = "Inicjały pacjenta";
            worksheet.Cells[1, col++].Value = "Płeć pacjenta";

            if (!oldSmkFormat)
            {
                worksheet.Cells[1, col++].Value = "Dane asysty"; // Tylko w nowej wersji SMK
            }

            worksheet.Cells[1, col++].Value = "Grupa procedur";
            worksheet.Cells[1, col++].Value = "Status";
            worksheet.Cells[1, col++].Value = "Nazwa stażu";

            if (oldSmkFormat)
            {
                worksheet.Cells[1, col++].Value = "Osoba wykonująca"; // Specyficzne dla starej wersji SMK
            }

            // Format nagłówków
            this.FormatHeaders(worksheet, col - 1);

            // Data
            for (int i = 0; i < procedures.Count; i++)
            {
                int row = i + 2;
                col = 1;

                var procedure = procedures[i];
                string internshipName = string.Empty;

                // Pobierz dane o stażu z AdditionalFields
                if (!string.IsNullOrEmpty(procedure.AdditionalFields))
                {
                    try
                    {
                        var additionalFields = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(procedure.AdditionalFields);
                        if (additionalFields.TryGetValue("InternshipName", out object name))
                        {
                            internshipName = name?.ToString() ?? string.Empty;
                        }
                    }
                    catch
                    {
                        // Ignorowanie błędów deserializacji
                    }
                }

                worksheet.Cells[row, col++].Value = procedure.Date;
                worksheet.Cells[row, col - 1].Style.Numberformat.Format = "yyyy-MM-dd";

                worksheet.Cells[row, col++].Value = procedure.Code;
                worksheet.Cells[row, col++].Value = procedure.OperatorCode;
                worksheet.Cells[row, col++].Value = procedure.Location;
                worksheet.Cells[row, col++].Value = procedure.PatientInitials;
                worksheet.Cells[row, col++].Value = procedure.PatientGender;

                if (!oldSmkFormat)
                {
                    worksheet.Cells[row, col++].Value = procedure.AssistantData;
                }

                worksheet.Cells[row, col++].Value = procedure.ProcedureGroup;
                worksheet.Cells[row, col++].Value = procedure.Status;
                worksheet.Cells[row, col++].Value = internshipName;

                if (oldSmkFormat)
                {
                    worksheet.Cells[row, col++].Value = procedure.PerformingPerson;
                }
            }

            // Autofit columns
            worksheet.Columns[1, col - 1].AutoFit();
        }

        private void AddMedicalShiftsWorksheet(ExcelPackage package, List<MedicalShift> shifts, bool oldSmkFormat)
        {
            var worksheet = package.Workbook.Worksheets.Add("Dyżury medyczne");

            // Headers
            int col = 1;
            worksheet.Cells[1, col++].Value = "Data";
            worksheet.Cells[1, col++].Value = "Godziny";
            worksheet.Cells[1, col++].Value = "Minuty";
            worksheet.Cells[1, col++].Value = "Miejsce";
            worksheet.Cells[1, col++].Value = "Rok szkolenia";
            worksheet.Cells[1, col++].Value = "Nazwa stażu";

            if (oldSmkFormat)
            {
                worksheet.Cells[1, col++].Value = "Osoba nadzorująca";
                worksheet.Cells[1, col++].Value = "Oddział";
            }

            // Format nagłówków
            this.FormatHeaders(worksheet, col - 1);

            // Data
            for (int i = 0; i < shifts.Count; i++)
            {
                int row = i + 2;
                col = 1;

                var shift = shifts[i];
                string internshipName = string.Empty;

                // Pobierz dane o stażu z AdditionalFields
                if (!string.IsNullOrEmpty(shift.AdditionalFields))
                {
                    try
                    {
                        var additionalFields = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(shift.AdditionalFields);
                        if (additionalFields.TryGetValue("InternshipName", out object name))
                        {
                            internshipName = name?.ToString() ?? string.Empty;
                        }
                    }
                    catch
                    {
                        // Ignorowanie błędów deserializacji
                    }
                }

                worksheet.Cells[row, col++].Value = shift.Date;
                worksheet.Cells[row, col - 1].Style.Numberformat.Format = "yyyy-MM-dd";

                worksheet.Cells[row, col++].Value = shift.Hours;
                worksheet.Cells[row, col++].Value = shift.Minutes;
                worksheet.Cells[row, col++].Value = shift.Location;
                worksheet.Cells[row, col++].Value = shift.Year;
                worksheet.Cells[row, col++].Value = internshipName;

                if (oldSmkFormat)
                {
                    // Dodatkowe pola dla starej wersji SMK
                    if (!string.IsNullOrEmpty(shift.AdditionalFields))
                    {
                        try
                        {
                            var additionalFields = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(shift.AdditionalFields);

                            if (additionalFields.TryGetValue("OldSMKField1", out object field1))
                            {
                                worksheet.Cells[row, col++].Value = field1?.ToString();
                            }
                            else
                            {
                                col++;
                            }

                            if (additionalFields.TryGetValue("OldSMKField2", out object field2))
                            {
                                worksheet.Cells[row, col++].Value = field2?.ToString();
                            }
                            else
                            {
                                col++;
                            }
                        }
                        catch
                        {
                            col += 2;
                        }
                    }
                    else
                    {
                        col += 2;
                    }
                }
            }

            // Autofit columns
            worksheet.Columns[1, col - 1].AutoFit();
        }

        private void AddSelfEducationWorksheet(ExcelPackage package, List<SelfEducation> selfEducationItems, bool oldSmkFormat)
        {
            var worksheet = package.Workbook.Worksheets.Add("Samokształcenie");

            // Headers
            int col = 1;
            worksheet.Cells[1, col++].Value = "Rok";
            worksheet.Cells[1, col++].Value = "Typ";
            worksheet.Cells[1, col++].Value = "Tytuł";
            worksheet.Cells[1, col++].Value = "Wydawca";

            if (oldSmkFormat)
            {
                worksheet.Cells[1, col++].Value = "Dodatkowe informacje";
            }

            // Format nagłówków
            this.FormatHeaders(worksheet, col - 1);

            // Data
            for (int i = 0; i < selfEducationItems.Count; i++)
            {
                int row = i + 2;
                col = 1;

                var item = selfEducationItems[i];

                worksheet.Cells[row, col++].Value = item.Year;
                worksheet.Cells[row, col++].Value = item.Type;
                worksheet.Cells[row, col++].Value = item.Title;
                worksheet.Cells[row, col++].Value = item.Publisher;

                if (oldSmkFormat && !string.IsNullOrEmpty(item.AdditionalFields))
                {
                    try
                    {
                        var additionalFields = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(item.AdditionalFields);
                        string additionalInfo = string.Empty;

                        foreach (var field in additionalFields)
                        {
                            additionalInfo += $"{field.Key}: {field.Value}, ";
                        }

                        // Usunięcie ostatniego przecinka i spacji
                        if (additionalInfo.Length > 2)
                        {
                            additionalInfo = additionalInfo.Substring(0, additionalInfo.Length - 2);
                        }

                        worksheet.Cells[row, col++].Value = additionalInfo;
                    }
                    catch
                    {
                        col++;
                    }
                }
                else if (oldSmkFormat)
                {
                    col++;
                }
            }

            // Autofit columns
            worksheet.Columns[1, col - 1].AutoFit();
        }

        private void AddPublicationsWorksheet(ExcelPackage package, List<Publication> publications, bool oldSmkFormat)
        {
            var worksheet = package.Workbook.Worksheets.Add("Publikacje");

            // Headers
            int col = 1;
            worksheet.Cells[1, col++].Value = "Opis publikacji";
            worksheet.Cells[1, col++].Value = "Ścieżka do pliku";

            if (oldSmkFormat)
            {
                worksheet.Cells[1, col++].Value = "Dodatkowe informacje";
            }

            // Format nagłówków
            this.FormatHeaders(worksheet, col - 1);

            // Data
            for (int i = 0; i < publications.Count; i++)
            {
                int row = i + 2;
                col = 1;

                var publication = publications[i];

                worksheet.Cells[row, col++].Value = publication.Description;
                worksheet.Cells[row, col++].Value = publication.FilePath;

                if (oldSmkFormat && !string.IsNullOrEmpty(publication.AdditionalFields))
                {
                    try
                    {
                        var additionalFields = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(publication.AdditionalFields);
                        string additionalInfo = string.Empty;

                        foreach (var field in additionalFields)
                        {
                            additionalInfo += $"{field.Key}: {field.Value}, ";
                        }

                        // Usunięcie ostatniego przecinka i spacji
                        if (additionalInfo.Length > 2)
                        {
                            additionalInfo = additionalInfo.Substring(0, additionalInfo.Length - 2);
                        }

                        worksheet.Cells[row, col++].Value = additionalInfo;
                    }
                    catch
                    {
                        col++;
                    }
                }
                else if (oldSmkFormat)
                {
                    col++;
                }
            }

            // Autofit columns
            worksheet.Columns[1, col - 1].AutoFit();
        }

        private void AddEducationalActivitiesWorksheet(ExcelPackage package, List<EducationalActivity> activities)
        {
            var worksheet = package.Workbook.Worksheets.Add("Działalność edukacyjna");

            // Headers
            int col = 1;
            worksheet.Cells[1, col++].Value = "Typ";
            worksheet.Cells[1, col++].Value = "Tytuł";
            worksheet.Cells[1, col++].Value = "Opis";
            worksheet.Cells[1, col++].Value = "Data rozpoczęcia";
            worksheet.Cells[1, col++].Value = "Data zakończenia";

            // Format nagłówków
            this.FormatHeaders(worksheet, col - 1);

            // Data
            for (int i = 0; i < activities.Count; i++)
            {
                int row = i + 2;
                col = 1;

                var activity = activities[i];

                worksheet.Cells[row, col++].Value = activity.Type.ToString();
                worksheet.Cells[row, col++].Value = activity.Title;
                worksheet.Cells[row, col++].Value = activity.Description;

                worksheet.Cells[row, col++].Value = activity.StartDate;
                worksheet.Cells[row, col - 1].Style.Numberformat.Format = "yyyy-MM-dd";

                worksheet.Cells[row, col++].Value = activity.EndDate;
                worksheet.Cells[row, col - 1].Style.Numberformat.Format = "yyyy-MM-dd";
            }

            // Autofit columns
            worksheet.Columns[1, col - 1].AutoFit();
        }

        private void AddAbsencesWorksheet(ExcelPackage package, List<Absence> absences)
        {
            var worksheet = package.Workbook.Worksheets.Add("Nieobecności");

            // Headers
            int col = 1;
            worksheet.Cells[1, col++].Value = "Typ";
            worksheet.Cells[1, col++].Value = "Data rozpoczęcia";
            worksheet.Cells[1, col++].Value = "Data zakończenia";
            worksheet.Cells[1, col++].Value = "Liczba dni";
            worksheet.Cells[1, col++].Value = "Przedłuża specjalizację";
            worksheet.Cells[1, col++].Value = "Opis";

            // Format nagłówków
            this.FormatHeaders(worksheet, col - 1);

            // Data
            for (int i = 0; i < absences.Count; i++)
            {
                int row = i + 2;
                col = 1;

                var absence = absences[i];

                worksheet.Cells[row, col++].Value = absence.Type.ToString();

                worksheet.Cells[row, col++].Value = absence.StartDate;
                worksheet.Cells[row, col - 1].Style.Numberformat.Format = "yyyy-MM-dd";

                worksheet.Cells[row, col++].Value = absence.EndDate;
                worksheet.Cells[row, col - 1].Style.Numberformat.Format = "yyyy-MM-dd";

                worksheet.Cells[row, col++].Value = absence.DaysCount;
                worksheet.Cells[row, col++].Value = absence.ExtendsSpecialization ? "Tak" : "Nie";
                worksheet.Cells[row, col++].Value = absence.Description;
            }

            // Autofit columns
            worksheet.Columns[1, col - 1].AutoFit();
        }

        private void AddRecognitionsWorksheet(ExcelPackage package, List<Models.Recognition> recognitions)
        {
            var worksheet = package.Workbook.Worksheets.Add("Uznania/skrócenia");

            // Headers
            int col = 1;
            worksheet.Cells[1, col++].Value = "Typ";
            worksheet.Cells[1, col++].Value = "Opis";
            worksheet.Cells[1, col++].Value = "Data rozpoczęcia";
            worksheet.Cells[1, col++].Value = "Data zakończenia";
            worksheet.Cells[1, col++].Value = "Liczba dni skrócenia";

            // Format nagłówków
            this.FormatHeaders(worksheet, col - 1);

            // Data
            for (int i = 0; i < recognitions.Count; i++)
            {
                int row = i + 2;
                col = 1;

                var recognition = recognitions[i];

                worksheet.Cells[row, col++].Value = recognition.Type.ToString();
                worksheet.Cells[row, col++].Value = recognition.Description;

                worksheet.Cells[row, col++].Value = recognition.StartDate;
                worksheet.Cells[row, col - 1].Style.Numberformat.Format = "yyyy-MM-dd";

                worksheet.Cells[row, col++].Value = recognition.EndDate;
                worksheet.Cells[row, col - 1].Style.Numberformat.Format = "yyyy-MM-dd";

                worksheet.Cells[row, col++].Value = recognition.DaysReduction;
            }

            // Autofit columns
            worksheet.Columns[1, col - 1].AutoFit();
        }

        // Metoda pomocnicza do formatowania nagłówków
        private void FormatHeaders(ExcelWorksheet worksheet, int columnCount)
        {
            for (int i = 1; i <= columnCount; i++)
            {
                worksheet.Cells[1, i].Style.Font.Bold = true;
                worksheet.Cells[1, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[1, i].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                worksheet.Cells[1, i].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[1, i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[1, i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }
        }
    }
}