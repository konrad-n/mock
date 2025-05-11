using SledzSpecke.App.Exceptions;
using SledzSpecke.App.Helpers;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;

namespace SledzSpecke.App.Services.Database
{
    public partial class DatabaseService
    {
        public async Task<RealizedInternshipNewSMK> GetRealizedInternshipNewSMKAsync(int id)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                var internship = await this.database.Table<RealizedInternshipNewSMK>().FirstOrDefaultAsync(i => i.RealizedInternshipId == id);
                if (internship == null)
                {
                    throw new ResourceNotFoundException(
                        $"RealizedInternshipNewSMK with ID {id} not found",
                        $"Nie znaleziono zrealizowanego stażu w nowym SMK o ID {id}");
                }
                return internship;
            },
            new Dictionary<string, object> { { "RealizedInternshipId", id } },
            $"Nie udało się pobrać zrealizowanego stażu w nowym SMK o ID {id}");
        }

        public async Task<List<RealizedInternshipNewSMK>> GetRealizedInternshipsNewSMKAsync(int? specializationId = null, int? moduleId = null, int? internshipRequirementId = null)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                var query = this.database.Table<RealizedInternshipNewSMK>();

                if (specializationId.HasValue)
                {
                    query = query.Where(i => i.SpecializationId == specializationId);
                }

                if (moduleId.HasValue)
                {
                    query = query.Where(i => i.ModuleId == moduleId);
                }

                if (internshipRequirementId.HasValue)
                {
                    query = query.Where(i => i.InternshipRequirementId == internshipRequirementId);
                }

                return await query.ToListAsync();
            },
            new Dictionary<string, object> {
                { "SpecializationId", specializationId },
                { "ModuleId", moduleId },
                { "InternshipRequirementId", internshipRequirementId }
            },
            "Nie udało się pobrać listy zrealizowanych staży w nowym SMK");
        }

        public async Task<int> SaveRealizedInternshipNewSMKAsync(RealizedInternshipNewSMK internship)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (internship == null)
                {
                    throw new InvalidInputException(
                        "RealizedInternshipNewSMK cannot be null",
                        "Zrealizowany staż w nowym SMK nie może być pusty");
                }

                if (internship.RealizedInternshipId != 0)
                {
                    return await this.database.UpdateAsync(internship);
                }
                else
                {
                    return await this.database.InsertAsync(internship);
                }
            },
            new Dictionary<string, object> {
                { "RealizedInternship", internship?.RealizedInternshipId },
                { "SpecializationId", internship?.SpecializationId },
                { "ModuleId", internship?.ModuleId },
                { "InternshipRequirementId", internship?.InternshipRequirementId }
            },
            "Nie udało się zapisać danych zrealizowanego stażu w nowym SMK");
        }

        public async Task<int> DeleteRealizedInternshipNewSMKAsync(RealizedInternshipNewSMK internship)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (internship == null)
                {
                    throw new InvalidInputException(
                        "RealizedInternshipNewSMK cannot be null",
                        "Zrealizowany staż w nowym SMK nie może być pusty");
                }

                return await this.database.DeleteAsync(internship);
            },
            new Dictionary<string, object> { { "RealizedInternship", internship?.RealizedInternshipId } },
            "Nie udało się usunąć zrealizowanego stażu w nowym SMK");
        }

        public async Task<RealizedInternshipOldSMK> GetRealizedInternshipOldSMKAsync(int id)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                var internship = await this.database.Table<RealizedInternshipOldSMK>().FirstOrDefaultAsync(i => i.RealizedInternshipId == id);
                if (internship == null)
                {
                    throw new ResourceNotFoundException(
                        $"RealizedInternshipOldSMK with ID {id} not found",
                        $"Nie znaleziono zrealizowanego stażu w starym SMK o ID {id}");
                }
                return internship;
            },
            new Dictionary<string, object> { { "RealizedInternshipId", id } },
            $"Nie udało się pobrać zrealizowanego stażu w starym SMK o ID {id}");
        }

        public async Task<List<RealizedInternshipOldSMK>> GetRealizedInternshipsOldSMKAsync(int? specializationId = null, int? year = null)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                var query = this.database.Table<RealizedInternshipOldSMK>();

                if (specializationId.HasValue)
                {
                    query = query.Where(i => i.SpecializationId == specializationId);
                }

                if (year.HasValue)
                {
                    query = query.Where(i => i.Year == year);
                }

                return await query.ToListAsync();
            },
            new Dictionary<string, object> { { "SpecializationId", specializationId }, { "Year", year } },
            "Nie udało się pobrać listy zrealizowanych staży w starym SMK");
        }

        public async Task<int> SaveRealizedInternshipOldSMKAsync(RealizedInternshipOldSMK internship)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (internship == null)
                {
                    throw new InvalidInputException(
                        "RealizedInternshipOldSMK cannot be null",
                        "Zrealizowany staż w starym SMK nie może być pusty");
                }

                if (internship.RealizedInternshipId != 0)
                {
                    return await this.database.UpdateAsync(internship);
                }
                else
                {
                    return await this.database.InsertAsync(internship);
                }
            },
            new Dictionary<string, object> {
                { "RealizedInternship", internship?.RealizedInternshipId },
                { "SpecializationId", internship?.SpecializationId },
                { "Year", internship?.Year }
            },
            "Nie udało się zapisać danych zrealizowanego stażu w starym SMK");
        }

        public async Task<int> DeleteRealizedInternshipOldSMKAsync(RealizedInternshipOldSMK internship)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (internship == null)
                {
                    throw new InvalidInputException(
                        "RealizedInternshipOldSMK cannot be null",
                        "Zrealizowany staż w starym SMK nie może być pusty");
                }

                return await this.database.DeleteAsync(internship);
            },
            new Dictionary<string, object> { { "RealizedInternship", internship?.RealizedInternshipId } },
            "Nie udało się usunąć zrealizowanego stażu w starym SMK");
        }

        public async Task FixRealizedInternshipNames()
        {
            await this.InitializeAsync();
            await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                // Pobierz wszystkie realizacje bez poprawnej nazwy
                var realizationsToFix = await this.database.Table<RealizedInternshipOldSMK>()
                    .Where(r => r.InternshipName == "Staż bez nazwy" || r.InternshipName == null)
                    .ToListAsync();

                _loggingService.LogInformation($"Znaleziono {realizationsToFix.Count} realizacji do naprawy");

                if (realizationsToFix.Count == 0)
                {
                    return;
                }

                // Pobierz wszystkie staże
                var internships = await this.database.Table<Internship>()
                    .Where(i => i.InternshipId < 0) // Tylko wymagania stażowe mają ID < 0
                    .ToListAsync();

                if (internships.Count == 0)
                {
                    _loggingService.LogWarning("Nie znaleziono wymagań stażowych do naprawy nazw");
                    return;
                }

                // Przypisz pierwszy staż do wszystkich realizacji bez nazwy
                // To rozwiązanie tymczasowe, aby pokazać dane
                var firstInternship = internships.FirstOrDefault();
                if (firstInternship != null)
                {
                    foreach (var realization in realizationsToFix)
                    {
                        realization.InternshipName = firstInternship.InternshipName;
                        await this.database.UpdateAsync(realization);
                    }

                    _loggingService.LogInformation($"Naprawiono nazwy realizacji, przypisując '{firstInternship.InternshipName}'");
                }
            },
            null, "Nie udało się naprawić nazw zrealizowanych staży", 2, 1500);
        }

        public async Task MigrateInternshipDataAsync()
        {
            await this.InitializeAsync();
            await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                // Sprawdzenie istnienia kolumny w tabeli
                bool internshipRequirementIdExists = false;
                bool moduleIdExists = false;

                try
                {
                    var testQuery = "SELECT InternshipRequirementId FROM RealizedInternshipNewSMK LIMIT 1";
                    await this.database.ExecuteScalarAsync<int>(testQuery);
                    internshipRequirementIdExists = true;
                }
                catch
                {
                    await this.database.ExecuteAsync("ALTER TABLE RealizedInternshipNewSMK ADD COLUMN InternshipRequirementId INTEGER");
                }

                try
                {
                    var testQuery = "SELECT ModuleId FROM RealizedInternshipNewSMK LIMIT 1";
                    await this.database.ExecuteScalarAsync<int>(testQuery);
                    moduleIdExists = true;
                }
                catch
                {
                    await this.database.ExecuteAsync("ALTER TABLE RealizedInternshipNewSMK ADD COLUMN ModuleId INTEGER");
                }

                // Sprawdź i napraw istniejące realizacje z null InternshipName
                try
                {
                    var realizationsWithNullNames = await this.database.Table<RealizedInternshipOldSMK>()
                        .Where(r => r.InternshipName == null)
                        .ToListAsync();

                    foreach (var realization in realizationsWithNullNames)
                    {
                        _loggingService.LogInformation($"Znaleziono realizację z pustą nazwą stażu, ID: {realization.RealizedInternshipId}");
                        // Próba naprawy - szukamy w oryginalnych stażach
                        var originalInternship = await this.database.Table<Internship>()
                            .FirstOrDefaultAsync(i => i.SpecializationId == realization.SpecializationId &&
                                                      i.DaysCount == realization.DaysCount);

                        if (originalInternship != null && !string.IsNullOrEmpty(originalInternship.InternshipName))
                        {
                            realization.InternshipName = originalInternship.InternshipName;
                            await this.database.UpdateAsync(realization);
                            _loggingService.LogInformation($"Naprawiono nazwę stażu: {realization.InternshipName}");
                        }
                        else
                        {
                            // Jeśli nie udało się znaleźć odpowiedniego stażu, użyj wartości domyślnej
                            realization.InternshipName = "Staż bez nazwy";
                            await this.database.UpdateAsync(realization);
                            _loggingService.LogInformation($"Ustawiono domyślną nazwę stażu dla ID: {realization.RealizedInternshipId}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _loggingService.LogError(ex, $"Błąd podczas naprawy realizacji: {ex.Message}", new Dictionary<string, object> { { "ExceptionDetails", ex } });
                }

                // Pobierz aktualną wersję SMK użytkownika
                var userId = await Helpers.SettingsHelper.GetCurrentUserIdAsync();
                var user = await this.GetUserAsync(userId);

                if (user == null)
                {
                    return;
                }

                // Pobranie wszystkich istniejących staży
                var internships = await this.database.Table<Internship>().Where(i => i.InternshipId > 0).ToListAsync();

                // Sprawdź, czy już istnieją realizacje dla tych staży
                var existingNewSMK = await this.database.Table<RealizedInternshipNewSMK>().ToListAsync();
                var existingOldSMK = await this.database.Table<RealizedInternshipOldSMK>().ToListAsync();

                // Jeśli realizacje już istnieją, pomijamy migrację
                if ((user.SmkVersion == SmkVersion.New && existingNewSMK.Count > 0) ||
                    (user.SmkVersion == SmkVersion.Old && existingOldSMK.Count > 0))
                {
                    return;
                }

                // Migracja danych
                foreach (var internship in internships)
                {
                    if (user.SmkVersion == SmkVersion.New)
                    {
                        // Ignorujemy staże z ID < 0 (to są wymagania stażowe, nie realizacje)
                        if (internship.InternshipId < 0)
                        {
                            continue;
                        }

                        var existingInternship = existingNewSMK
                            .FirstOrDefault(i => i.SpecializationId == internship.SpecializationId &&
                                               i.InternshipName == internship.InternshipName);

                        if (existingInternship != null)
                        {
                            continue;
                        }

                        var realizedInternship = new RealizedInternshipNewSMK
                        {
                            SpecializationId = internship.SpecializationId,
                            ModuleId = internship.ModuleId,
                            InternshipRequirementId = internship.InternshipId, // Ustawienie ID wymagania
                            InternshipName = internship.InternshipName ?? "Staż bez nazwy", // Upewniamy się, że nazwa nie jest null
                            InstitutionName = internship.InstitutionName,
                            DepartmentName = internship.DepartmentName,
                            StartDate = internship.StartDate,
                            EndDate = internship.EndDate,
                            DaysCount = internship.DaysCount,
                            IsCompleted = internship.IsCompleted,
                            IsApproved = internship.IsApproved,
                            IsRecognition = internship.IsRecognition,
                            RecognitionReason = internship.RecognitionReason,
                            RecognitionDaysReduction = internship.RecognitionDaysReduction,
                            IsPartialRealization = internship.IsPartialRealization,
                            SupervisorName = internship.SupervisorName,
                            SyncStatus = internship.SyncStatus,
                            AdditionalFields = internship.AdditionalFields
                        };

                        await this.database.InsertAsync(realizedInternship);
                    }
                    else // Stary SMK
                    {
                        // Ignorujemy staże z ID < 0 (to są wymagania stażowe, nie realizacje)
                        if (internship.InternshipId < 0)
                        {
                            continue;
                        }

                        var existingInternship = existingOldSMK
                            .FirstOrDefault(i => i.SpecializationId == internship.SpecializationId &&
                                               i.InternshipName == internship.InternshipName &&
                                               i.Year == internship.Year);

                        if (existingInternship != null)
                        {
                            continue;
                        }

                        var realizedInternship = new RealizedInternshipOldSMK
                        {
                            SpecializationId = internship.SpecializationId,
                            InternshipName = internship.InternshipName ?? "Staż bez nazwy", // Upewniamy się, że nazwa nie jest null
                            InstitutionName = internship.InstitutionName,
                            DepartmentName = internship.DepartmentName,
                            StartDate = internship.StartDate,
                            EndDate = internship.EndDate,
                            DaysCount = internship.DaysCount,
                            IsCompleted = internship.IsCompleted,
                            IsApproved = internship.IsApproved,
                            Year = internship.Year,
                            RequiresApproval = false, // Domyślna wartość
                            SupervisorName = internship.SupervisorName,
                            SyncStatus = internship.SyncStatus,
                            AdditionalFields = internship.AdditionalFields
                        };

                        await this.database.InsertAsync(realizedInternship);
                    }
                }
            },
            null, "Nie udało się zmigrować danych stażowych", 2, 2000);
        }
    }
}