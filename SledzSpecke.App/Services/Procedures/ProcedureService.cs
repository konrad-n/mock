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

        public async Task<List<ProcedureRequirement>> GetAvailableProcedureRequirementsAsync(int? moduleId = null)
        {
            if (!moduleId.HasValue)
            {
                return new List<ProcedureRequirement>();
            }

            var module = await this.databaseService.GetModuleAsync(moduleId.Value);
            if (module == null || string.IsNullOrEmpty(module.Structure))
            {
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
                return new List<ProcedureRequirement>();
            }

            return moduleStructure.Procedures;
        }

        public async Task<List<RealizedProcedureOldSMK>> GetOldSMKProceduresAsync(int? moduleId = null)
        {
            var user = await this.authService.GetCurrentUserAsync();
            if (user == null)
            {
                return new List<RealizedProcedureOldSMK>();
            }
            if (moduleId.HasValue)
            {
                var module = await this.databaseService.GetModuleAsync(moduleId.Value);
                if (module != null)
                {
                    int startYear = module.Type == ModuleType.Basic ? 1 : 3;
                    int endYear = module.Type == ModuleType.Basic ? 2 : 6;
                    var sql = "SELECT * FROM RealizedProcedureOldSMK WHERE SpecializationId = ? AND Year BETWEEN ? AND ?";
                    return await this.databaseService.QueryAsync<RealizedProcedureOldSMK>(sql, user.SpecializationId, startYear, endYear);
                }
            }
            var defaultSql = "SELECT * FROM RealizedProcedureOldSMK WHERE SpecializationId = ?";
            return await this.databaseService.QueryAsync<RealizedProcedureOldSMK>(defaultSql, user.SpecializationId);
        }

        public async Task<RealizedProcedureOldSMK> GetOldSMKProcedureAsync(int procedureId)
        {
            var sql = "SELECT * FROM RealizedProcedureOldSMK WHERE ProcedureId = ?";
            var results = await this.databaseService.QueryAsync<RealizedProcedureOldSMK>(sql, procedureId);
            return results.FirstOrDefault();
        }

        public async Task<bool> SaveOldSMKProcedureAsync(RealizedProcedureOldSMK procedure)
        {
            if (procedure.SpecializationId <= 0)
            {
                var user = await this.authService.GetCurrentUserAsync();
                if (user == null)
                {
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
            }
            else
            {
                result = await this.databaseService.UpdateAsync(procedure);
            }

            return result > 0;
        }

        public async Task<bool> DeleteOldSMKProcedureAsync(int procedureId)
        {
            var procedure = await this.GetOldSMKProcedureAsync(procedureId);
            if (procedure == null)
            {
                return false;
            }

            if (procedure.SyncStatus == SyncStatus.Synced)
            {
                return false;
            }

            var sql = "DELETE FROM RealizedProcedureOldSMK WHERE ProcedureId = ?";
            int result = await this.databaseService.ExecuteAsync(sql, procedureId);

            return result > 0;
        }

        public async Task<List<RealizedProcedureNewSMK>> GetNewSMKProceduresAsync(int? moduleId = null, int? requirementId = null)
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

            return procedures;
        }

        public async Task<RealizedProcedureNewSMK> GetNewSMKProcedureAsync(int procedureId)
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

            return procedure;
        }

        public async Task<bool> SaveNewSMKProcedureAsync(RealizedProcedureNewSMK procedure)
        {
            if (procedure.SpecializationId <= 0)
            {
                var user = await this.authService.GetCurrentUserAsync();
                if (user == null)
                {
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
            }
            else
            {
                result = await this.databaseService.UpdateAsync(procedure);
            }

            return result > 0;
        }

        public async Task<bool> DeleteNewSMKProcedureAsync(int procedureId)
        {
            var procedure = await this.GetNewSMKProcedureAsync(procedureId);
            if (procedure == null)
            {
                return false;
            }

            if (procedure.SyncStatus == SyncStatus.Synced)
            {
                return false;
            }

            var sql = "DELETE FROM RealizedProcedureNewSMK WHERE ProcedureId = ?";
            int result = await this.databaseService.ExecuteAsync(sql, procedureId);

            return result > 0;
        }

        public async Task<ProcedureSummary> GetProcedureSummaryAsync(int? moduleId = null, int? requirementId = null)
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

            return summary;
        }

        public async Task<List<RealizedProcedureOldSMK>> GetOldSMKProceduresAsync(int? moduleId = null, int? year = null, int? requirementId = null)
        {
            var user = await this.authService.GetCurrentUserAsync();
            if (user == null)
            {
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
            return await this.databaseService.QueryAsync<RealizedProcedureOldSMK>(sql, parameters.ToArray());
        }

        public async Task<(int completed, int total)> GetProcedureStatisticsForModuleAsync(int moduleId)
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

            return (completedCount, totalRequired);
        }
    }
}