using Microsoft.Extensions.Logging;
using SledzSpecke.Core.Exceptions;
using SledzSpecke.Core.Interfaces.Services;
using SledzSpecke.Core.Models.Domain;
using SledzSpecke.Core.Models.Monitoring;
using SledzSpecke.Infrastructure.Database.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SledzSpecke.Infrastructure.Services
{
    public class ProcedureService : IProcedureService
    {
        private readonly IProcedureRepository _repository;
        private readonly IUserService _userService;
        private readonly ISpecializationRequirementsProvider _requirementsProvider;
        private readonly ILogger<ProcedureService> _logger;

        public ProcedureService(
            IProcedureRepository repository,
            IUserService userService,
            ISpecializationRequirementsProvider requirementsProvider,
            ILogger<ProcedureService> logger)
        {
            _repository = repository;
            _userService = userService;
            _requirementsProvider = requirementsProvider;
            _logger = logger;
        }

        public async Task<ProcedureExecution> AddProcedureAsync(ProcedureExecution procedure)
        {
            try
            {
                procedure.UserId = await _userService.GetCurrentUserIdAsync();
                procedure.CreatedAt = DateTime.UtcNow;
                await ValidateProcedureAsync(procedure);
                await ValidateProcedureRequirementsAsync(procedure);
                await _repository.AddAsync(procedure);

                return procedure;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding procedure");
                throw;
            }
        }

        public async Task<List<ProcedureExecution>> GetUserProceduresAsync()
        {
            try
            {
                var userId = await _userService.GetCurrentUserIdAsync();
                return await _repository.GetUserProceduresAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user procedures");
                throw;
            }
        }

        public async Task<ProcedureExecution> GetProcedureAsync(int id)
        {
            try
            {
                return await _repository.GetProcedureWithDetailsAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting procedure {ProcedureId}", id);
                throw;
            }
        }

        public async Task<bool> UpdateProcedureAsync(ProcedureExecution procedure)
        {
            try
            {
                var existingProcedure = await _repository.GetByIdAsync(procedure.Id);
                if (existingProcedure == null)
                {
                    throw new NotFoundException("Procedure not found");
                }

                var currentUserId = await _userService.GetCurrentUserIdAsync();
                if (existingProcedure.UserId != currentUserId)
                {
                    throw new UnauthorizedAccessException("Cannot update other user's procedure");
                }

                procedure.ModifiedAt = DateTime.UtcNow;
                await ValidateProcedureAsync(procedure);
                await ValidateProcedureRequirementsAsync(procedure);
                await _repository.UpdateAsync(procedure);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating procedure {ProcedureId}", procedure.Id);
                throw;
            }
        }

        public async Task<bool> DeleteProcedureAsync(int id)
        {
            try
            {
                var procedure = await _repository.GetByIdAsync(id)
                    ?? throw new NotFoundException("Procedure not found");

                var currentUserId = await _userService.GetCurrentUserIdAsync();
                if (procedure.UserId != currentUserId)
                {
                    throw new UnauthorizedAccessException("Cannot delete other user's procedure");
                }

                await _repository.DeleteAsync(id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting procedure {ProcedureId}", id);
                throw;
            }
        }

        public async Task<List<ProcedureRequirement>> GetRequirementsForSpecializationAsync()
        {
            try
            {
                var user = await _userService.GetCurrentUserAsync();
                if (user?.CurrentSpecializationId == null)
                {
                    return new List<ProcedureRequirement>();
                }

                return await _repository.GetRequirementsForSpecializationAsync(user.CurrentSpecializationId.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting procedure requirements");
                return new List<ProcedureRequirement>();
            }
        }

        public async Task<List<string>> GetAvailableCategoriesAsync()
        {
            try
            {
                var requirements = await GetRequirementsForSpecializationAsync();
                return requirements
                    .Select(r => r.Category)
                    .Where(c => !string.IsNullOrEmpty(c))
                    .Distinct()
                    .OrderBy(c => c)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting available categories");
                return new List<string>();
            }
        }

        public async Task<List<string>> GetAvailableStagesAsync()
        {
            try
            {
                var requirements = await GetRequirementsForSpecializationAsync();
                return requirements
                    .Select(r => r.Stage)
                    .Where(s => !string.IsNullOrEmpty(s))
                    .Distinct()
                    .OrderBy(s => s)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting available stages");
                return new List<string>();
            }
        }

        private async Task ValidateProcedureAsync(ProcedureExecution procedure)
        {
            if (string.IsNullOrWhiteSpace(procedure.Name))
            {
                throw new ValidationException("Procedure name is required");
            }

            if (string.IsNullOrWhiteSpace(procedure.Location))
            {
                throw new ValidationException("Procedure location is required");
            }

            if (procedure.ExecutionDate > DateTime.Today)
            {
                throw new ValidationException("Cannot add future procedures");
            }
        }

        public async Task<bool> ValidateProcedureRequirementsAsync(ProcedureExecution procedure)
        {
            try
            {
                if (string.IsNullOrEmpty(procedure.Category) && string.IsNullOrEmpty(procedure.Stage))
                {
                    return true;
                }

                var user = await _userService.GetCurrentUserAsync();

                if (user?.CurrentSpecializationId == null)
                {
                    return true;
                }

                var procedureRequirements =
                    _requirementsProvider.GetRequiredProceduresBySpecialization(user.CurrentSpecializationId.Value);
                var matchingRequirements = new List<Core.Models.Requirements.RequiredProcedure>();

                foreach (var category in procedureRequirements.Keys)
                {
                    if (string.IsNullOrEmpty(procedure.Category) || category == procedure.Category)
                    {
                        var procedures = procedureRequirements[category]
                            .Where(p => p.Name.Equals(procedure.Name, StringComparison.OrdinalIgnoreCase))
                            .ToList();

                        if (procedures.Any())
                        {
                            matchingRequirements.AddRange(procedures);
                        }
                    }
                }

                if (!matchingRequirements.Any())
                {
                    return true;
                }

                if (procedure.IsSimulation)
                {
                    var allowsSimulation = matchingRequirements.Any(r => r.AllowSimulation);
                    if (!allowsSimulation)
                    {
                        throw new ValidationException("Simulation procedures are not allowed for this procedure type");
                    }

                    var simulationLimitedProcedure = matchingRequirements.FirstOrDefault(r => r.SimulationLimit.HasValue);
                    if (simulationLimitedProcedure != null)
                    {
                        var userProcedures = await GetUserProceduresAsync();
                        var sameProcedures = userProcedures
                            .Where(p => p.Name.Equals(procedure.Name, StringComparison.OrdinalIgnoreCase))
                            .ToList();
                        var simulationCount = sameProcedures.Count(p => p.IsSimulation);
                        var totalRequired = simulationLimitedProcedure.RequiredCount;

                        if (totalRequired > 0)
                        {
                            var maxSimulations = (totalRequired * simulationLimitedProcedure.SimulationLimit.Value) / 100;
                            if (simulationCount + 1 > maxSimulations)
                            {
                                throw new ValidationException($"Simulation limit exceeded. Maximum allowed: {maxSimulations}");
                            }
                        }
                    }
                }

                var needsSupervision = matchingRequirements.Any(r => r.RequiredCount > 0)
                                       && procedure.Type == Core.Models.Enums.ProcedureType.Execution;

                if (needsSupervision && procedure.SupervisorId == null)
                {
                    throw new ValidationException("This procedure requires supervision");
                }

                if (matchingRequirements.Count == 1)
                {
                    var requirements = await GetRequirementsForSpecializationAsync();
                    var matchingDbRequirement = requirements.FirstOrDefault(r =>
                        r.Name.Equals(matchingRequirements[0].Name, StringComparison.OrdinalIgnoreCase) &&
                        r.Category == procedure.Category);

                    if (matchingDbRequirement != null)
                    {
                        procedure.ProcedureRequirementId = matchingDbRequirement.Id;
                    }
                }

                return true;
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating procedure requirements");
                throw;
            }
        }

        public async Task<double> GetProcedureCompletionPercentageAsync()
        {
            try
            {
                var user = await _userService.GetCurrentUserAsync();
                if (user?.CurrentSpecializationId == null)
                    return 0;

                var requiredProcedures =
                    _requirementsProvider.GetRequiredProceduresBySpecialization(user.CurrentSpecializationId.Value);
                var userProcedures = await GetUserProceduresAsync();
                var completedProcedures = MapToProcedureProgress(userProcedures);
                var verifier = new ProcedureMonitoring.ProgressVerification(requiredProcedures);
                var summary = verifier.GenerateProgressSummary(completedProcedures);

                return summary.OverallCompletionPercentage / 100.0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating procedure completion percentage");
                return 0;
            }
        }

        private Dictionary<string, ProcedureMonitoring.ProcedureProgress> MapToProcedureProgress(
            List<ProcedureExecution> procedures)
        {
            var result = new Dictionary<string, ProcedureMonitoring.ProcedureProgress>();

            foreach (var procedure in procedures)
            {
                if (!result.ContainsKey(procedure.Name))
                {
                    result[procedure.Name] = new ProcedureMonitoring.ProcedureProgress
                    {
                        ProcedureName = procedure.Name,
                        CompletedCount = 0,
                        AssistanceCount = 0,
                        SimulationCount = 0,
                        Executions = new List<ProcedureMonitoring.ProcedureExecution>()
                    };
                }

                var progress = result[procedure.Name];

                progress.Executions.Add(new ProcedureMonitoring.ProcedureExecution
                {
                    ExecutionDate = procedure.ExecutionDate,
                    Type = procedure.Type.ToString(),
                    Location = procedure.Location,
                    Notes = procedure.Notes
                });

                if (procedure.Type == Core.Models.Enums.ProcedureType.Execution)
                {
                    if (procedure.IsSimulation)
                        progress.SimulationCount++;
                    else
                        progress.CompletedCount++;
                }
                else
                {
                    progress.AssistanceCount++;
                }
            }

            return result;
        }
    }
}
