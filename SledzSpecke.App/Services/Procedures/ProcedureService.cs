using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Authentication;
using SledzSpecke.App.Services.Database;
using SledzSpecke.App.Services.Specialization;
using System.Text.Json;

namespace SledzSpecke.App.Services.Procedures
{
    public class ProcedureService : IProcedureService
    {
        private readonly IDatabaseService databaseService;
        private readonly IAuthService authService;
        private readonly ISpecializationService specializationService;

        public ProcedureService(
            IDatabaseService databaseService,
            IAuthService authService,
            ISpecializationService specializationService)
        {
            this.databaseService = databaseService;
            this.authService = authService;
            this.specializationService = specializationService;
        }

        public async Task<List<ProcedureRequirement>> GetAvailableProcedureRequirementsAsync(int? moduleId = null)
        {
            try
            {
                var requirements = new List<ProcedureRequirement>();

                // Pobierz aktualny moduł
                var currentModule = moduleId.HasValue
                    ? await this.databaseService.GetModuleAsync(moduleId.Value)
                    : await this.specializationService.GetCurrentModuleAsync();

                if (currentModule == null || string.IsNullOrEmpty(currentModule.Structure))
                {
                    return requirements;
                }

                // Deserializuj strukturę modułu
                var options = new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    AllowTrailingCommas = true,
                    ReadCommentHandling = System.Text.Json.JsonCommentHandling.Skip,
                    Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
                };

                var moduleStructure = JsonSerializer.Deserialize<ModuleStructure>(currentModule.Structure, options);

                if (moduleStructure?.Procedures != null)
                {
                    requirements.AddRange(moduleStructure.Procedures);

                    // Dodaj informacje o stażu do każdego wymagania
                    foreach (var requirement in requirements)
                    {
                        if (requirement.InternshipId.HasValue && moduleStructure.Internships != null)
                        {
                            var internship = moduleStructure.Internships.FirstOrDefault(i => i.Id == requirement.InternshipId.Value);
                            if (internship != null)
                            {
                                requirement.InternshipName = internship.Name;
                            }
                        }
                    }
                }

                return requirements;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd w GetAvailableProcedureRequirementsAsync: {ex.Message}");
                return new List<ProcedureRequirement>();
            }
        }

        public async Task<ProcedureSummary> GetProcedureSummaryAsync(int? moduleId = null, int? procedureRequirementId = null)
        {
            try
            {
                var summary = new ProcedureSummary();

                // Pobierz aktualnego użytkownika i jego specjalizację
                var user = await this.authService.GetCurrentUserAsync();
                if (user == null)
                {
                    return summary;
                }

                // Dostosuj zapytanie w zależności od wersji SMK
                if (user.SmkVersion == SmkVersion.New)
                {
                    string query = "SELECT SUM(CountA) as CompletedCountA, SUM(CountB) as CompletedCountB FROM RealizedProcedureNewSMK WHERE SpecializationId = ?";

                    if (moduleId.HasValue)
                    {
                        query += " AND ModuleId = ?";
                    }

                    if (procedureRequirementId.HasValue)
                    {
                        query += " AND ProcedureRequirementId = ?";
                    }

                    var parameters = new List<object> { user.SpecializationId };
                    if (moduleId.HasValue) parameters.Add(moduleId.Value);
                    if (procedureRequirementId.HasValue) parameters.Add(procedureRequirementId.Value);

                    var results = await this.databaseService.QueryAsync<ProcedureSummary>(query, parameters.ToArray());
                    if (results.Count > 0)
                    {
                        summary = results[0];
                    }

                    // Pobierz także liczbę zatwierdzonych procedur
                    query = query.Replace("SUM(CountA)", "SUM(CASE WHEN SyncStatus = 1 THEN CountA ELSE 0 END)")
                               .Replace("SUM(CountB)", "SUM(CASE WHEN SyncStatus = 1 THEN CountB ELSE 0 END)");

                    var approvedResults = await this.databaseService.QueryAsync<ProcedureSummary>(query, parameters.ToArray());
                    if (approvedResults.Count > 0)
                    {
                        summary.ApprovedCountA = approvedResults[0].CompletedCountA;
                        summary.ApprovedCountB = approvedResults[0].CompletedCountB;
                    }

                    // Pobierz wymagane liczby procedur
                    var requirements = await this.GetAvailableProcedureRequirementsAsync(moduleId);

                    if (procedureRequirementId.HasValue)
                    {
                        var requirement = requirements.FirstOrDefault(r => r.Id == procedureRequirementId.Value);
                        if (requirement != null)
                        {
                            summary.RequiredCountA = requirement.RequiredCountA;
                            summary.RequiredCountB = requirement.RequiredCountB;
                        }
                    }
                    else
                    {
                        summary.RequiredCountA = requirements.Sum(r => r.RequiredCountA);
                        summary.RequiredCountB = requirements.Sum(r => r.RequiredCountB);
                    }
                }
                else // Old SMK
                {
                    string query = "SELECT COUNT(CASE WHEN Code = 'A - operator' THEN 1 END) as CompletedCountA, " +
                                  "COUNT(CASE WHEN Code = 'B - asysta' THEN 1 END) as CompletedCountB " +
                                  "FROM RealizedProcedureOldSMK WHERE SpecializationId = ?";

                    if (moduleId.HasValue)
                    {
                        // Dla starego SMK musimy filtrować po latach powiązanych z modułem
                        var module = await this.databaseService.GetModuleAsync(moduleId.Value);
                        if (module != null)
                        {
                            if (module.Type == ModuleType.Basic)
                            {
                                query += " AND Year IN (1, 2)";
                            }
                            else
                            {
                                query += " AND Year > 2";
                            }
                        }
                    }

                    var results = await this.databaseService.QueryAsync<ProcedureSummary>(query, user.SpecializationId);
                    if (results.Count > 0)
                    {
                        summary = results[0];
                    }

                    // Pobierz także liczbę zatwierdzonych procedur
                    query = query.Replace("COUNT(CASE", "COUNT(CASE WHEN SyncStatus = 1 AND");

                    var approvedResults = await this.databaseService.QueryAsync<ProcedureSummary>(query, user.SpecializationId);
                    if (approvedResults.Count > 0)
                    {
                        summary.ApprovedCountA = approvedResults[0].CompletedCountA;
                        summary.ApprovedCountB = approvedResults[0].CompletedCountB;
                    }

                    // Pobierz wymagane liczby procedur
                    var requirements = await this.GetAvailableProcedureRequirementsAsync(moduleId);
                    summary.RequiredCountA = requirements.Sum(r => r.RequiredCountA);
                    summary.RequiredCountB = requirements.Sum(r => r.RequiredCountB);
                }

                return summary;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd w GetProcedureSummaryAsync: {ex.Message}");
                return new ProcedureSummary();
            }
        }

        // Implementacja metod dla starej wersji SMK

        public async Task<List<RealizedProcedureOldSMK>> GetOldSMKProceduresAsync(int? moduleId = null, int? year = null)
        {
            try
            {
                var user = await this.authService.GetCurrentUserAsync();
                if (user == null)
                {
                    return new List<RealizedProcedureOldSMK>();
                }

                string query = "SELECT * FROM RealizedProcedureOldSMK WHERE SpecializationId = ?";
                var parameters = new List<object> { user.SpecializationId };

                if (moduleId.HasValue)
                {
                    // Dla starego SMK musimy filtrować po latach powiązanych z modułem
                    var module = await this.databaseService.GetModuleAsync(moduleId.Value);
                    if (module != null)
                    {
                        if (module.Type == ModuleType.Basic)
                        {
                            query += " AND Year IN (1, 2)";
                        }
                        else
                        {
                            query += " AND Year > 2";
                        }
                    }
                }

                if (year.HasValue)
                {
                    query += " AND Year = ?";
                    parameters.Add(year.Value);
                }

                query += " ORDER BY Date DESC";

                return await this.databaseService.QueryAsync<RealizedProcedureOldSMK>(query, parameters.ToArray());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd w GetOldSMKProceduresAsync: {ex.Message}");
                return new List<RealizedProcedureOldSMK>();
            }
        }

        public async Task<RealizedProcedureOldSMK> GetOldSMKProcedureAsync(int procedureId)
        {
            try
            {
                return await this.databaseService.QueryAsync<RealizedProcedureOldSMK>(
                    "SELECT * FROM RealizedProcedureOldSMK WHERE ProcedureId = ?", procedureId).ContinueWith(t =>
                        t.Result.FirstOrDefault());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd w GetOldSMKProcedureAsync: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> SaveOldSMKProcedureAsync(RealizedProcedureOldSMK procedure)
        {
            try
            {
                // Upewnij się, że specjalizacja jest ustawiona
                if (procedure.SpecializationId <= 0)
                {
                    var user = await this.authService.GetCurrentUserAsync();
                    if (user == null)
                    {
                        return false;
                    }
                    procedure.SpecializationId = user.SpecializationId;
                }

                // Zapisz procedurę
                int result = 0;
                if (procedure.ProcedureId == 0)
                {
                    result = await this.databaseService.InsertAsync(procedure);
                }
                else
                {
                    result = await this.databaseService.UpdateAsync(procedure);
                }

                // Aktualizuj statystyki modułu
                await this.UpdateModuleStatisticsAsync();

                return result > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd w SaveOldSMKProcedureAsync: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteOldSMKProcedureAsync(int procedureId)
        {
            try
            {
                var procedure = await this.GetOldSMKProcedureAsync(procedureId);
                if (procedure == null)
                {
                    return false;
                }

                // Procedury nie mogą być usunięte, jeśli są synchronizowane
                if (procedure.SyncStatus == SyncStatus.Synced)
                {
                    return false;
                }

                int result = await this.databaseService.ExecuteAsync(
                    "DELETE FROM RealizedProcedureOldSMK WHERE ProcedureId = ?", procedureId);

                // Aktualizuj statystyki modułu
                await this.UpdateModuleStatisticsAsync();

                return result > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd w DeleteOldSMKProcedureAsync: {ex.Message}");
                return false;
            }
        }

        // Implementacja metod dla nowej wersji SMK

        public async Task<List<RealizedProcedureNewSMK>> GetNewSMKProceduresAsync(int? moduleId = null, int? procedureRequirementId = null)
        {
            try
            {
                var user = await this.authService.GetCurrentUserAsync();
                if (user == null)
                {
                    return new List<RealizedProcedureNewSMK>();
                }

                string query = "SELECT * FROM RealizedProcedureNewSMK WHERE SpecializationId = ?";
                var parameters = new List<object> { user.SpecializationId };

                if (moduleId.HasValue)
                {
                    query += " AND ModuleId = ?";
                    parameters.Add(moduleId.Value);
                }

                if (procedureRequirementId.HasValue)
                {
                    query += " AND ProcedureRequirementId = ?";
                    parameters.Add(procedureRequirementId.Value);
                }

                query += " ORDER BY Date DESC";

                return await this.databaseService.QueryAsync<RealizedProcedureNewSMK>(query, parameters.ToArray());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd w GetNewSMKProceduresAsync: {ex.Message}");
                return new List<RealizedProcedureNewSMK>();
            }
        }

        public async Task<RealizedProcedureNewSMK> GetNewSMKProcedureAsync(int procedureId)
        {
            try
            {
                return await this.databaseService.QueryAsync<RealizedProcedureNewSMK>(
                    "SELECT * FROM RealizedProcedureNewSMK WHERE ProcedureId = ?", procedureId).ContinueWith(t =>
                        t.Result.FirstOrDefault());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd w GetNewSMKProcedureAsync: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> SaveNewSMKProcedureAsync(RealizedProcedureNewSMK procedure)
        {
            try
            {
                // Upewnij się, że specjalizacja jest ustawiona
                if (procedure.SpecializationId <= 0)
                {
                    var user = await this.authService.GetCurrentUserAsync();
                    if (user == null)
                    {
                        return false;
                    }
                    procedure.SpecializationId = user.SpecializationId;
                }

                // Zapisz procedurę
                int result = 0;
                if (procedure.ProcedureId == 0)
                {
                    procedure.Date = DateTime.Now;
                    result = await this.databaseService.InsertAsync(procedure);
                }
                else
                {
                    result = await this.databaseService.UpdateAsync(procedure);
                }

                // Aktualizuj statystyki modułu
                await this.UpdateModuleStatisticsAsync();

                return result > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd w SaveNewSMKProcedureAsync: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteNewSMKProcedureAsync(int procedureId)
        {
            try
            {
                var procedure = await this.GetNewSMKProcedureAsync(procedureId);
                if (procedure == null)
                {
                    return false;
                }

                // Procedury nie mogą być usunięte, jeśli są synchronizowane
                if (procedure.SyncStatus == SyncStatus.Synced)
                {
                    return false;
                }

                int result = await this.databaseService.ExecuteAsync(
                    "DELETE FROM RealizedProcedureNewSMK WHERE ProcedureId = ?", procedureId);

                // Aktualizuj statystyki modułu
                await this.UpdateModuleStatisticsAsync();

                return result > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd w DeleteNewSMKProcedureAsync: {ex.Message}");
                return false;
            }
        }

        // Metody pomocnicze

        private async Task UpdateModuleStatisticsAsync()
        {
            try
            {
                var currentModule = await this.specializationService.GetCurrentModuleAsync();
                if (currentModule != null)
                {
                    await this.specializationService.UpdateModuleProgressAsync(currentModule.ModuleId);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd w UpdateModuleStatisticsAsync: {ex.Message}");
            }
        }
    }
}