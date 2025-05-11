using SledzSpecke.App.Helpers;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Authentication;
using SledzSpecke.App.Services.Database;
using SledzSpecke.App.Services.Exceptions;
using SledzSpecke.App.Services.Logging;
using SledzSpecke.App.Services.Specialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SledzSpecke.App.Services.Procedures
{
    public class ProcedureService : BaseService, IProcedureService
    {
        private readonly IDatabaseService databaseService;
        private readonly IAuthService authService;
        private readonly ISpecializationService specializationService;

        public ProcedureService(
            IDatabaseService databaseService,
            IAuthService authService,
            ISpecializationService specializationService,
            IExceptionHandlerService exceptionHandler,
            ILoggingService logger) : base(exceptionHandler, logger)
        {
            this.databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
            this.authService = authService ?? throw new ArgumentNullException(nameof(authService));
            this.specializationService = specializationService ?? throw new ArgumentNullException(nameof(specializationService));
        }

        public async Task<List<ProcedureRequirement>> GetAvailableProcedureRequirementsAsync(int? moduleId = null)
        {
            return await SafeExecuteAsync(async () =>
            {
                if (!moduleId.HasValue)
                {
                    Logger.LogInformation("Zwrócono pustą listę wymagań procedurowych - nie podano ID modułu");
                    return new List<ProcedureRequirement>();
                }

                var module = await this.databaseService.GetModuleAsync(moduleId.Value);
                if (module == null || string.IsNullOrEmpty(module.Structure))
                {
                    Logger.LogWarning($"Moduł o ID {moduleId.Value} nie istnieje lub nie ma struktury");
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
                    Logger.LogInformation($"Moduł o ID {moduleId.Value} nie ma zdefiniowanych procedur");
                    return new List<ProcedureRequirement>();
                }

                return moduleStructure.Procedures;
            }, "Nie udało się pobrać dostępnych procedur",
               new Dictionary<string, object> { { "ModuleId", moduleId } },
               withRetry: true);
        }

        public async Task<List<RealizedProcedureOldSMK>> GetOldSMKProceduresAsync(int? moduleId = null, int? year = null, int? requirementId = null)
        {
            return await SafeExecuteAsync(async () =>
            {
                var user = await this.authService.GetCurrentUserAsync();
                if (user == null)
                {
                    Logger.LogWarning("Próba pobrania procedur bez zalogowanego użytkownika");
                    return new List<RealizedProcedureOldSMK>();
                }

                var sql = "SELECT * FROM RealizedProcedureOldSMK WHERE SpecializationId = ?";
                var parameters = new List<object> { user.SpecializationId };

                if (moduleId.HasValue)
                {
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

                if (year.HasValue)
                {
                    sql += " AND Year = ?";
                    parameters.Add(year.Value);
                }

                if (requirementId.HasValue)
                {
                    sql += " AND ProcedureRequirementId = ?";
                    parameters.Add(requirementId.Value);
                }

                sql += " ORDER BY Date DESC";
                var result = await this.databaseService.QueryAsync<RealizedProcedureOldSMK>(sql, parameters.ToArray());
                Logger.LogInformation($"Pobrano {result.Count} procedur dla starego SMK",
                    new Dictionary<string, object> {
                        { "ModuleId", moduleId },
                        { "Year", year },
                        { "RequirementId", requirementId },
                        { "SpecializationId", user.SpecializationId }
                    });

                return result;
            }, "Nie udało się pobrać procedur",
               new Dictionary<string, object> {
                   { "ModuleId", moduleId },
                   { "Year", year },
                   { "RequirementId", requirementId }
               });
        }

        public async Task<RealizedProcedureOldSMK> GetOldSMKProcedureAsync(int procedureId)
        {
            return await SafeExecuteAsync(async () =>
            {
                var sql = "SELECT * FROM RealizedProcedureOldSMK WHERE ProcedureId = ?";
                var results = await this.databaseService.QueryAsync<RealizedProcedureOldSMK>(sql, procedureId);

                var procedure = results.FirstOrDefault();
                if (procedure == null)
                {
                    Logger.LogWarning($"Nie znaleziono procedury o ID {procedureId}");
                }

                return procedure;
            }, "Nie udało się pobrać szczegółów procedury",
               new Dictionary<string, object> { { "ProcedureId", procedureId } },
               withRetry: true);
        }

        public async Task<bool> SaveOldSMKProcedureAsync(RealizedProcedureOldSMK procedure)
        {
            return await SafeExecuteAsync(async () =>
            {
                if (procedure == null)
                {
                    throw new ArgumentNullException(nameof(procedure), "Procedura nie może być pusta");
                }

                if (procedure.SpecializationId <= 0)
                {
                    var user = await this.authService.GetCurrentUserAsync();
                    if (user == null)
                    {
                        Logger.LogWarning("Próba zapisu procedury bez zalogowanego użytkownika");
                        return false;
                    }

                    procedure.SpecializationId = user.SpecializationId;
                }

                if (procedure.SyncStatus == SyncStatus.Synced)
                {
                    procedure.SyncStatus = SyncStatus.Modified;
                }
                else if (procedure.SyncStatus != SyncStatus.Modified)
                {
                    procedure.SyncStatus = SyncStatus.NotSynced;
                }

                int result;
                if (procedure.ProcedureId == 0)
                {
                    result = await this.databaseService.InsertAsync(procedure);
                    Logger.LogInformation($"Utworzono nową procedurę z ID {result}",
                        new Dictionary<string, object> {
                            { "SpecializationId", procedure.SpecializationId },
                            { "InternshipId", procedure.InternshipId },
                            { "Code", procedure.Code }
                        });
                }
                else
                {
                    result = await this.databaseService.UpdateAsync(procedure);
                    Logger.LogInformation($"Zaktualizowano procedurę o ID {procedure.ProcedureId}",
                        new Dictionary<string, object> {
                            { "SpecializationId", procedure.SpecializationId },
                            { "InternshipId", procedure.InternshipId },
                            { "Code", procedure.Code }
                        });
                }

                return result > 0;
            }, "Nie udało się zapisać procedury",
               new Dictionary<string, object> {
                   { "ProcedureId", procedure?.ProcedureId },
                   { "SpecializationId", procedure?.SpecializationId }
               });
        }

        public async Task<bool> DeleteOldSMKProcedureAsync(int procedureId)
        {
            return await SafeExecuteAsync(async () =>
            {
                var procedure = await this.GetOldSMKProcedureAsync(procedureId);
                if (procedure == null)
                {
                    Logger.LogWarning($"Próba usunięcia nieistniejącej procedury o ID {procedureId}");
                    return false;
                }

                if (procedure.SyncStatus == SyncStatus.Synced)
                {
                    Logger.LogWarning($"Próba usunięcia zsynchronizowanej procedury o ID {procedureId}");
                    return false;
                }

                var sql = "DELETE FROM RealizedProcedureOldSMK WHERE ProcedureId = ?";
                int result = await this.databaseService.ExecuteAsync(sql, procedureId);

                Logger.LogInformation($"Usunięto procedurę o ID {procedureId}",
                    new Dictionary<string, object> {
                        { "SpecializationId", procedure.SpecializationId },
                        { "InternshipId", procedure.InternshipId },
                        { "Code", procedure.Code }
                    });

                return result > 0;
            }, "Nie udało się usunąć procedury",
               new Dictionary<string, object> { { "ProcedureId", procedureId } });
        }

        public async Task<List<RealizedProcedureNewSMK>> GetNewSMKProceduresAsync(int? moduleId = null, int? requirementId = null)
        {
            return await SafeExecuteAsync(async () =>
            {
                var user = await this.authService.GetCurrentUserAsync();
                if (user == null)
                {
                    Logger.LogWarning("Próba pobrania procedur bez zalogowanego użytkownika");
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

                if (moduleId.HasValue)
                {
                    var requirements = await this.GetAvailableProcedureRequirementsAsync(moduleId);
                    foreach (var procedure in procedures)
                    {
                        var requirement = requirements.FirstOrDefault(r => r.Id == procedure.ProcedureRequirementId);
                        if (requirement != null)
                        {
                            procedure.ProcedureName = requirement.Name;
                            procedure.InternshipName = string.Empty;
                        }
                    }
                }

                Logger.LogInformation($"Pobrano {procedures.Count} procedur dla nowego SMK",
                    new Dictionary<string, object> {
                        { "ModuleId", moduleId },
                        { "RequirementId", requirementId },
                        { "SpecializationId", user.SpecializationId }
                    });

                return procedures;
            }, "Nie udało się pobrać procedur",
               new Dictionary<string, object> {
                   { "ModuleId", moduleId },
                   { "RequirementId", requirementId }
               });
        }

        public async Task<RealizedProcedureNewSMK> GetNewSMKProcedureAsync(int procedureId)
        {
            return await SafeExecuteAsync(async () =>
            {
                var sql = "SELECT * FROM RealizedProcedureNewSMK WHERE ProcedureId = ?";
                var results = await this.databaseService.QueryAsync<RealizedProcedureNewSMK>(sql, procedureId);

                var procedure = results.FirstOrDefault();
                if (procedure != null && procedure.ModuleId.HasValue)
                {
                    var requirements = await this.GetAvailableProcedureRequirementsAsync(procedure.ModuleId);
                    var requirement = requirements.FirstOrDefault(r => r.Id == procedure.ProcedureRequirementId);
                    if (requirement != null)
                    {
                        procedure.ProcedureName = requirement.Name;
                        procedure.InternshipName = string.Empty;
                    }
                }
                else
                {
                    Logger.LogWarning($"Nie znaleziono procedury o ID {procedureId}");
                }

                return procedure;
            }, "Nie udało się pobrać szczegółów procedury",
               new Dictionary<string, object> { { "ProcedureId", procedureId } },
               withRetry: true);
        }

        public async Task<bool> SaveNewSMKProcedureAsync(RealizedProcedureNewSMK procedure)
        {
            return await SafeExecuteAsync(async () =>
            {
                if (procedure == null)
                {
                    throw new ArgumentNullException(nameof(procedure), "Procedura nie może być pusta");
                }

                if (procedure.SpecializationId <= 0)
                {
                    var user = await this.authService.GetCurrentUserAsync();
                    if (user == null)
                    {
                        Logger.LogWarning("Próba zapisu procedury bez zalogowanego użytkownika");
                        return false;
                    }

                    procedure.SpecializationId = user.SpecializationId;
                }

                if (procedure.Date == default)
                {
                    procedure.Date = DateTime.Now;
                }

                if (procedure.SyncStatus == SyncStatus.Synced)
                {
                    procedure.SyncStatus = SyncStatus.Modified;
                }
                else if (procedure.SyncStatus != SyncStatus.Modified)
                {
                    procedure.SyncStatus = SyncStatus.NotSynced;
                }

                int result;
                if (procedure.ProcedureId == 0)
                {
                    result = await this.databaseService.InsertAsync(procedure);
                    Logger.LogInformation($"Utworzono nową procedurę z ID {result}",
                        new Dictionary<string, object> {
                            { "SpecializationId", procedure.SpecializationId },
                            { "ModuleId", procedure.ModuleId },
                            { "ProcedureRequirementId", procedure.ProcedureRequirementId },
                            { "CountA", procedure.CountA },
                            { "CountB", procedure.CountB }
                        });
                }
                else
                {
                    result = await this.databaseService.UpdateAsync(procedure);
                    Logger.LogInformation($"Zaktualizowano procedurę o ID {procedure.ProcedureId}",
                        new Dictionary<string, object> {
                            { "SpecializationId", procedure.SpecializationId },
                            { "ModuleId", procedure.ModuleId },
                            { "ProcedureRequirementId", procedure.ProcedureRequirementId },
                            { "CountA", procedure.CountA },
                            { "CountB", procedure.CountB }
                        });
                }

                return result > 0;
            }, "Nie udało się zapisać procedury",
               new Dictionary<string, object> {
                   { "ProcedureId", procedure?.ProcedureId },
                   { "SpecializationId", procedure?.SpecializationId },
                   { "ModuleId", procedure?.ModuleId }
               });
        }

        public async Task<bool> DeleteNewSMKProcedureAsync(int procedureId)
        {
            return await SafeExecuteAsync(async () =>
            {
                var procedure = await this.GetNewSMKProcedureAsync(procedureId);
                if (procedure == null)
                {
                    Logger.LogWarning($"Próba usunięcia nieistniejącej procedury o ID {procedureId}");
                    return false;
                }

                if (procedure.SyncStatus == SyncStatus.Synced)
                {
                    Logger.LogWarning($"Próba usunięcia zsynchronizowanej procedury o ID {procedureId}");
                    return false;
                }

                var sql = "DELETE FROM RealizedProcedureNewSMK WHERE ProcedureId = ?";
                int result = await this.databaseService.ExecuteAsync(sql, procedureId);

                Logger.LogInformation($"Usunięto procedurę o ID {procedureId}",
                    new Dictionary<string, object> {
                        { "SpecializationId", procedure.SpecializationId },
                        { "ModuleId", procedure.ModuleId },
                        { "ProcedureRequirementId", procedure.ProcedureRequirementId }
                    });

                return result > 0;
            }, "Nie udało się usunąć procedury",
               new Dictionary<string, object> { { "ProcedureId", procedureId } });
        }

        public async Task<ProcedureSummary> GetProcedureSummaryAsync(int? moduleId = null, int? requirementId = null)
        {
            return await SafeExecuteAsync(async () =>
            {
                var summary = new ProcedureSummary();

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
                else if (moduleId.HasValue)
                {
                    var requirements = await this.GetAvailableProcedureRequirementsAsync(moduleId);
                    summary.RequiredCountA = requirements.Sum(r => r.RequiredCountA);
                    summary.RequiredCountB = requirements.Sum(r => r.RequiredCountB);
                }

                var user = await this.authService.GetCurrentUserAsync();
                if (user == null)
                {
                    Logger.LogWarning("Próba obliczenia statystyk procedur bez zalogowanego użytkownika");
                    return summary;
                }

                if (user.SmkVersion == SmkVersion.Old)
                {
                    if (moduleId.HasValue)
                    {
                        var module = await this.databaseService.GetModuleAsync(moduleId.Value);
                        if (module != null)
                        {
                            int startYear = module.Type == ModuleType.Basic ? 1 : 3;
                            int endYear = module.Type == ModuleType.Basic ? 2 : 6;
                            var modules = await this.databaseService.GetModulesAsync(user.SpecializationId);
                            if (!modules.Any(m => m.Type == ModuleType.Basic))
                            {
                                startYear = 1;
                            }

                            var sql = "SELECT Code, SyncStatus FROM RealizedProcedureOldSMK WHERE SpecializationId = ? AND Year BETWEEN ? AND ?";
                            var parameters = new List<object> { user.SpecializationId, startYear, endYear };

                            if (requirementId.HasValue)
                            {
                                sql += " AND ProcedureRequirementId = ?";
                                parameters.Add(requirementId.Value);
                            }
                            else
                            {
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
                else
                {
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

                Logger.LogInformation($"Obliczono statystyki procedur",
                    new Dictionary<string, object> {
                        { "ModuleId", moduleId },
                        { "RequirementId", requirementId },
                        { "CompletedA", summary.CompletedCountA },
                        { "CompletedB", summary.CompletedCountB },
                        { "RequiredA", summary.RequiredCountA },
                        { "RequiredB", summary.RequiredCountB }
                    });

                return summary;
            }, "Nie udało się obliczyć statystyk procedur",
               new Dictionary<string, object> {
                   { "ModuleId", moduleId },
                   { "RequirementId", requirementId }
               });
        }

        public async Task<(int completed, int total)> GetProcedureStatisticsForModuleAsync(int moduleId)
        {
            return await SafeExecuteAsync(async () =>
            {
                var user = await this.authService.GetCurrentUserAsync();
                if (user == null)
                {
                    Logger.LogWarning("Próba obliczenia statystyk procedur bez zalogowanego użytkownika");
                    return (0, 0);
                }

                var module = await this.databaseService.GetModuleAsync(moduleId);
                if (module == null)
                {
                    Logger.LogWarning($"Próba obliczenia statystyk procedur dla nieistniejącego modułu o ID {moduleId}");
                    return (0, 0);
                }

                int completedCount = 0;
                int totalRequired = module.TotalProceduresA + module.TotalProceduresB;

                if (user.SmkVersion == SmkVersion.Old)
                {
                    var procedures = await this.GetOldSMKProceduresAsync(moduleId: moduleId);
                    completedCount = procedures.Count;
                }
                else
                {
                    var procedures = await this.GetNewSMKProceduresAsync(moduleId: moduleId);
                    completedCount = procedures.Sum(p => p.CountA + p.CountB);
                }

                Logger.LogInformation($"Obliczono statystyki procedur dla modułu",
                    new Dictionary<string, object> {
                        { "ModuleId", moduleId },
                        { "Completed", completedCount },
                        { "Total", totalRequired }
                    });

                return (completedCount, totalRequired);
            }, "Nie udało się obliczyć statystyk procedur dla modułu",
               new Dictionary<string, object> { { "ModuleId", moduleId } });
        }
    }
}