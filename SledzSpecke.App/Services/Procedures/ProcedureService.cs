using SledzSpecke.App.Helpers;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Authentication;
using SledzSpecke.App.Services.Database;
using SledzSpecke.App.Services.Specialization;
using System.Text.Json;
using System.Text.Json.Serialization;

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

        #region Wymagania procedur

        public async Task<List<ProcedureRequirement>> GetAvailableProcedureRequirementsAsync(int? moduleId = null)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"Pobieranie wymagań procedur dla modułu {moduleId}");

                var module = await this.databaseService.GetModuleAsync(moduleId.Value);
                if (module == null || string.IsNullOrEmpty(module.Structure))
                {
                    System.Diagnostics.Debug.WriteLine("Nie znaleziono modułu lub struktura jest pusta");
                    return new List<ProcedureRequirement>();
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    AllowTrailingCommas = true,
                    ReadCommentHandling = JsonCommentHandling.Skip,
                    Converters =
                    {
                        new JsonStringEnumConverter(),
                        new ModuleTypeJsonConverter()
                    }
                };

                var moduleStructure = JsonSerializer.Deserialize<ModuleStructure>(module.Structure, options);

                if (moduleStructure?.Procedures == null)
                {
                    System.Diagnostics.Debug.WriteLine("Brak procedur w strukturze modułu");
                    return new List<ProcedureRequirement>();
                }

                System.Diagnostics.Debug.WriteLine($"Znaleziono {moduleStructure.Procedures.Count} procedur");
                return moduleStructure.Procedures;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas pobierania wymagań procedur: {ex.Message}");
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                return new List<ProcedureRequirement>();
            }
        }

        #endregion

        #region Stary SMK

        public async Task<List<RealizedProcedureOldSMK>> GetOldSMKProceduresAsync(int? moduleId = null)
        {
            try
            {
                var user = await this.authService.GetCurrentUserAsync();
                if (user == null)
                {
                    return new List<RealizedProcedureOldSMK>();
                }

                // Pobierz moduł, aby określić zakres lat
                if (moduleId.HasValue)
                {
                    var module = await this.databaseService.GetModuleAsync(moduleId.Value);
                    if (module != null)
                    {
                        int startYear = module.Type == ModuleType.Basic ? 1 : 3;
                        int endYear = module.Type == ModuleType.Basic ? 2 : 6;

                        // Konstruuj zapytanie SQL z filtrowaniem po latach
                        var sql = "SELECT * FROM RealizedProcedureOldSMK WHERE SpecializationId = ? AND Year BETWEEN ? AND ?";
                        return await this.databaseService.QueryAsync<RealizedProcedureOldSMK>(sql, user.SpecializationId, startYear, endYear);
                    }
                }

                // Jeśli nie podano modułu, zwróć wszystkie procedury
                var defaultSql = "SELECT * FROM RealizedProcedureOldSMK WHERE SpecializationId = ?";
                return await this.databaseService.QueryAsync<RealizedProcedureOldSMK>(defaultSql, user.SpecializationId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas pobierania procedur starego SMK: {ex.Message}");
                return new List<RealizedProcedureOldSMK>();
            }
        }

        public async Task<RealizedProcedureOldSMK> GetOldSMKProcedureAsync(int procedureId)
        {
            try
            {
                var sql = "SELECT * FROM RealizedProcedureOldSMK WHERE ProcedureId = ?";
                var results = await this.databaseService.QueryAsync<RealizedProcedureOldSMK>(sql, procedureId);
                return results.FirstOrDefault();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas pobierania procedury starego SMK: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> SaveOldSMKProcedureAsync(RealizedProcedureOldSMK procedure)
        {
            try
            {
                // Ustaw specjalizację jeśli nie jest ustawiona
                if (procedure.SpecializationId <= 0)
                {
                    var user = await this.authService.GetCurrentUserAsync();
                    if (user == null)
                    {
                        return false;
                    }

                    procedure.SpecializationId = user.SpecializationId;
                }

                // Ustaw status synchronizacji
                if (procedure.SyncStatus == SyncStatus.Synced)
                {
                    procedure.SyncStatus = SyncStatus.Modified;
                }
                else if (procedure.SyncStatus != SyncStatus.Modified)
                {
                    procedure.SyncStatus = SyncStatus.NotSynced;
                }

                // Zapisz procedurę
                int result;
                if (procedure.ProcedureId == 0)
                {
                    // Nowa procedura
                    result = await this.databaseService.InsertAsync(procedure);
                }
                else
                {
                    // Aktualizacja istniejącej procedury
                    result = await this.databaseService.UpdateAsync(procedure);
                }

                return result > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas zapisywania procedury starego SMK: {ex.Message}");
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

                // Nie można usunąć zsynchronizowanej procedury
                if (procedure.SyncStatus == SyncStatus.Synced)
                {
                    return false;
                }

                // Usuń procedurę
                var sql = "DELETE FROM RealizedProcedureOldSMK WHERE ProcedureId = ?";
                int result = await this.databaseService.ExecuteAsync(sql, procedureId);

                return result > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas usuwania procedury starego SMK: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region Nowy SMK

        public async Task<List<RealizedProcedureNewSMK>> GetNewSMKProceduresAsync(int? moduleId = null, int? requirementId = null)
        {
            try
            {
                var user = await this.authService.GetCurrentUserAsync();
                if (user == null)
                {
                    return new List<RealizedProcedureNewSMK>();
                }

                var sql = "SELECT * FROM RealizedProcedureNewSMK WHERE SpecializationId = ?";
                var parameters = new List<object> { user.SpecializationId };

                if (moduleId.HasValue)
                {
                    sql += " AND ModuleId = ?";
                    parameters.Add(moduleId.Value);
                }

                if (requirementId.HasValue)
                {
                    sql += " AND ProcedureRequirementId = ?";
                    parameters.Add(requirementId.Value);
                }

                sql += " ORDER BY Date DESC";

                var procedures = await this.databaseService.QueryAsync<RealizedProcedureNewSMK>(sql, parameters.ToArray());

                // Dodaj informacje o nazwach procedur z modułu
                if (moduleId.HasValue)
                {
                    var requirements = await this.GetAvailableProcedureRequirementsAsync(moduleId);
                    foreach (var procedure in procedures)
                    {
                        var requirement = requirements.FirstOrDefault(r => r.Id == procedure.ProcedureRequirementId);
                        if (requirement != null)
                        {
                            procedure.ProcedureName = requirement.Name;
                            // W NewSMKProceduresListViewModel pobierzemy nazwę stażu z innych źródeł
                            procedure.InternshipName = string.Empty;
                        }
                    }
                }

                return procedures;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas pobierania procedur nowego SMK: {ex.Message}");
                return new List<RealizedProcedureNewSMK>();
            }
        }

        public async Task<RealizedProcedureNewSMK> GetNewSMKProcedureAsync(int procedureId)
        {
            try
            {
                var sql = "SELECT * FROM RealizedProcedureNewSMK WHERE ProcedureId = ?";
                var results = await this.databaseService.QueryAsync<RealizedProcedureNewSMK>(sql, procedureId);

                var procedure = results.FirstOrDefault();
                if (procedure != null && procedure.ModuleId.HasValue)
                {
                    // Dodaj informacje o nazwie procedury
                    var requirements = await this.GetAvailableProcedureRequirementsAsync(procedure.ModuleId);
                    var requirement = requirements.FirstOrDefault(r => r.Id == procedure.ProcedureRequirementId);
                    if (requirement != null)
                    {
                        procedure.ProcedureName = requirement.Name;
                        procedure.InternshipName = string.Empty;
                    }
                }

                return procedure;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas pobierania procedury nowego SMK: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> SaveNewSMKProcedureAsync(RealizedProcedureNewSMK procedure)
        {
            try
            {
                // Ustaw specjalizację jeśli nie jest ustawiona
                if (procedure.SpecializationId <= 0)
                {
                    var user = await this.authService.GetCurrentUserAsync();
                    if (user == null)
                    {
                        return false;
                    }

                    procedure.SpecializationId = user.SpecializationId;
                }

                // Ustaw datę jeśli nie jest ustawiona
                if (procedure.Date == default)
                {
                    procedure.Date = DateTime.Now;
                }

                // Ustaw status synchronizacji
                if (procedure.SyncStatus == SyncStatus.Synced)
                {
                    procedure.SyncStatus = SyncStatus.Modified;
                }
                else if (procedure.SyncStatus != SyncStatus.Modified)
                {
                    procedure.SyncStatus = SyncStatus.NotSynced;
                }

                // Zapisz procedurę
                int result;
                if (procedure.ProcedureId == 0)
                {
                    // Nowa procedura
                    result = await this.databaseService.InsertAsync(procedure);
                }
                else
                {
                    // Aktualizacja istniejącej procedury
                    result = await this.databaseService.UpdateAsync(procedure);
                }

                return result > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas zapisywania procedury nowego SMK: {ex.Message}");
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

                // Nie można usunąć zsynchronizowanej procedury
                if (procedure.SyncStatus == SyncStatus.Synced)
                {
                    return false;
                }

                // Usuń procedurę
                var sql = "DELETE FROM RealizedProcedureNewSMK WHERE ProcedureId = ?";
                int result = await this.databaseService.ExecuteAsync(sql, procedureId);

                return result > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas usuwania procedury nowego SMK: {ex.Message}");
                return false;
            }
        }

        #endregion
        public async Task<ProcedureSummary> GetProcedureSummaryAsync(int? moduleId = null, int? requirementId = null)
        {
            try
            {
                var summary = new ProcedureSummary();

                // Jeśli podano ID wymagania, pobierz wartości wymagane z modułu
                if (requirementId.HasValue && moduleId.HasValue)
                {
                    var requirements = await this.GetAvailableProcedureRequirementsAsync(moduleId);
                    var requirement = requirements.FirstOrDefault(r => r.Id == requirementId.Value);
                    if (requirement != null)
                    {
                        summary.RequiredCountA = requirement.RequiredCountA;
                        summary.RequiredCountB = requirement.RequiredCountB;
                    }
                }
                // W przeciwnym razie sumuj wszystkie wymagania
                else if (moduleId.HasValue)
                {
                    var requirements = await this.GetAvailableProcedureRequirementsAsync(moduleId);
                    summary.RequiredCountA = requirements.Sum(r => r.RequiredCountA);
                    summary.RequiredCountB = requirements.Sum(r => r.RequiredCountB);
                }

                // Pobierz bieżącego użytkownika i wersję SMK
                var user = await this.authService.GetCurrentUserAsync();
                if (user == null)
                {
                    return summary;
                }

                // Obliczanie statystyk dla starego SMK
                if (user.SmkVersion == SmkVersion.Old)
                {
                    if (moduleId.HasValue)
                    {
                        var module = await this.databaseService.GetModuleAsync(moduleId.Value);
                        if (module != null)
                        {
                            // Pobierz procedury tylko dla lat odpowiadających modułowi
                            int startYear = module.Type == ModuleType.Basic ? 1 : 3;
                            int endYear = module.Type == ModuleType.Basic ? 2 : 6;

                            // Sprawdź, czy specjalizacja ma moduł podstawowy
                            var modules = await this.databaseService.GetModulesAsync(user.SpecializationId);
                            if (!modules.Any(m => m.Type == ModuleType.Basic))
                            {
                                // Jeśli nie ma modułu podstawowego, wszystkie lata są dla modułu specjalistycznego
                                startYear = 1;
                            }

                            // Konstruuj zapytanie SQL
                            var sql = "SELECT Code, SyncStatus FROM RealizedProcedureOldSMK WHERE SpecializationId = ? AND Year BETWEEN ? AND ?";
                            var parameters = new List<object> { user.SpecializationId, startYear, endYear };

                            // POPRAWKA: Jeśli podano ID wymagania, filtruj najpierw po nim
                            if (requirementId.HasValue)
                            {
                                sql += " AND ProcedureRequirementId = ?";
                                parameters.Add(requirementId.Value);
                            }
                            // W przeciwnym przypadku, filtruj po stażach w module
                            else
                            {
                                // Pobierz staże w module
                                var internships = await this.databaseService.GetInternshipsAsync(moduleId: moduleId);

                                if (internships.Any())
                                {
                                    sql += " AND (";
                                    for (int i = 0; i < internships.Count; i++)
                                    {
                                        if (i > 0) sql += " OR ";
                                        sql += "InternshipId = ?";
                                        parameters.Add(internships[i].InternshipId);
                                    }
                                    sql += ")";
                                }
                            }

                            var procedures = await this.databaseService.QueryAsync<RealizedProcedureOldSMK>(sql, parameters.ToArray());

                            // Zliczanie procedur
                            foreach (var procedure in procedures)
                            {
                                if (procedure.Code == "A - operator")
                                {
                                    summary.CompletedCountA++;
                                    if (procedure.SyncStatus == SyncStatus.Synced)
                                    {
                                        summary.ApprovedCountA++;
                                    }
                                }
                                else if (procedure.Code == "B - asysta")
                                {
                                    summary.CompletedCountB++;
                                    if (procedure.SyncStatus == SyncStatus.Synced)
                                    {
                                        summary.ApprovedCountB++;
                                    }
                                }
                            }
                        }
                    }
                }
                // Obliczanie statystyk dla nowego SMK
                else
                {
                    // Konstruuj zapytanie SQL
                    var sql = "SELECT CountA, CountB, SyncStatus FROM RealizedProcedureNewSMK WHERE SpecializationId = ?";
                    var parameters = new List<object> { user.SpecializationId };

                    if (moduleId.HasValue)
                    {
                        sql += " AND ModuleId = ?";
                        parameters.Add(moduleId.Value);
                    }

                    if (requirementId.HasValue)
                    {
                        sql += " AND ProcedureRequirementId = ?";
                        parameters.Add(requirementId.Value);
                    }

                    var procedures = await this.databaseService.QueryAsync<RealizedProcedureNewSMK>(sql, parameters.ToArray());

                    // Zliczanie procedur
                    foreach (var procedure in procedures)
                    {
                        summary.CompletedCountA += procedure.CountA;
                        summary.CompletedCountB += procedure.CountB;

                        if (procedure.SyncStatus == SyncStatus.Synced)
                        {
                            summary.ApprovedCountA += procedure.CountA;
                            summary.ApprovedCountB += procedure.CountB;
                        }
                    }
                }

                return summary;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas pobierania statystyk procedur: {ex.Message}");
                return new ProcedureSummary();
            }
        }

        public async Task<List<RealizedProcedureOldSMK>> GetOldSMKProceduresAsync(int? moduleId = null, int? year = null, int? requirementId = null)
        {
            try
            {
                var user = await this.authService.GetCurrentUserAsync();
                if (user == null)
                {
                    return new List<RealizedProcedureOldSMK>();
                }

                var sql = "SELECT * FROM RealizedProcedureOldSMK WHERE SpecializationId = ?";
                var parameters = new List<object> { user.SpecializationId };

                // Jeśli podano ModuleId, filtruj po stażach w tym module
                if (moduleId.HasValue)
                {
                    // Pobierz staże w module
                    var internships = await this.databaseService.GetInternshipsAsync(moduleId: moduleId);

                    if (internships.Any())
                    {
                        sql += " AND (";
                        for (int i = 0; i < internships.Count; i++)
                        {
                            if (i > 0) sql += " OR ";
                            sql += "InternshipId = ?";
                            parameters.Add(internships[i].InternshipId);
                        }
                        sql += ")";
                    }
                }

                // Jeśli podano rok, dodatkowo filtruj po nim
                if (year.HasValue)
                {
                    sql += " AND Year = ?";
                    parameters.Add(year.Value);
                }

                // Jeśli podano requirementId, dodatkowo filtruj po nim
                if (requirementId.HasValue)
                {
                    sql += " AND ProcedureRequirementId = ?";
                    parameters.Add(requirementId.Value);
                }

                sql += " ORDER BY Date DESC";

                System.Diagnostics.Debug.WriteLine($"SQL query: {sql}");
                System.Diagnostics.Debug.WriteLine($"Parameters: {string.Join(", ", parameters)}");

                var results = await this.databaseService.QueryAsync<RealizedProcedureOldSMK>(sql, parameters.ToArray());
                System.Diagnostics.Debug.WriteLine($"Pobrano {results.Count} procedur starego SMK");

                return results;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas pobierania procedur starego SMK: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                return new List<RealizedProcedureOldSMK>();
            }
        }

        public async Task<(int completed, int total)> GetProcedureStatisticsForModuleAsync(int moduleId)
        {
            try
            {
                var user = await this.authService.GetCurrentUserAsync();
                if (user == null)
                {
                    return (0, 0);
                }

                var module = await this.databaseService.GetModuleAsync(moduleId);
                if (module == null)
                {
                    return (0, 0);
                }

                int completedCount = 0;
                int totalRequired = module.TotalProceduresA + module.TotalProceduresB;

                // Dla starego SMK
                if (user.SmkVersion == SmkVersion.Old)
                {
                    var procedures = await this.GetOldSMKProceduresAsync(moduleId: moduleId);
                    completedCount = procedures.Count;
                }
                // Dla nowego SMK
                else
                {
                    var procedures = await this.GetNewSMKProceduresAsync(moduleId: moduleId);
                    completedCount = procedures.Sum(p => p.CountA + p.CountB);
                }

                return (completedCount, totalRequired);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas pobierania statystyk procedur: {ex.Message}");
                return (0, 0);
            }
        }
    }
}