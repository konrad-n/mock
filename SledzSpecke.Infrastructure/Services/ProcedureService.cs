using Microsoft.Extensions.Logging;
using SledzSpecke.Core.Exceptions;
using SledzSpecke.Core.Interfaces.Services;
using SledzSpecke.Core.Models.Domain;
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
        private readonly ILogger<ProcedureService> _logger;

        public ProcedureService(
            IProcedureRepository repository,
            IUserService userService,
            ILogger<ProcedureService> logger)
        {
            _repository = repository;
            _userService = userService;
            _logger = logger;
        }

        public async Task<ProcedureExecution> AddProcedureAsync(ProcedureExecution procedure)
        {
            try
            {
                procedure.UserId = await _userService.GetCurrentUserIdAsync();
                procedure.CreatedAt = DateTime.UtcNow;
                await ValidateProcedureAsync(procedure);

                // Check if procedure meets program requirements
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
                var procedure = await _repository.GetByIdAsync(id);
                if (procedure == null)
                {
                    throw new NotFoundException("Procedure not found");
                }

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
                // If no category or stage is assigned, nothing to check
                if (string.IsNullOrEmpty(procedure.Category) && string.IsNullOrEmpty(procedure.Stage))
                {
                    return true;
                }

                var user = await _userService.GetCurrentUserAsync();
                if (user?.CurrentSpecializationId == null)
                {
                    return true;
                }

                var requirements = await GetRequirementsForSpecializationAsync();

                // Find matching requirements
                var matchingRequirements = requirements
                    .Where(r => (string.IsNullOrEmpty(procedure.Category) || r.Category == procedure.Category) &&
                              (string.IsNullOrEmpty(procedure.Stage) || r.Stage == procedure.Stage))
                    .ToList();

                if (!matchingRequirements.Any())
                {
                    return true;
                }

                // Check if simulation procedures are allowed
                if (procedure.IsSimulation)
                {
                    var allowsSimulation = matchingRequirements.Any(r => r.AllowSimulation);
                    if (!allowsSimulation)
                    {
                        throw new ValidationException("Simulation procedures are not allowed for this category/stage");
                    }
                }

                // Check if supervision is required
                if (matchingRequirements.Any(r => r.SupervisionRequired) && procedure.SupervisorId == null)
                {
                    throw new ValidationException("This procedure requires supervision");
                }

                // Mark procedure as meeting appropriate program requirement
                if (matchingRequirements.Count == 1)
                {
                    procedure.ProcedureRequirementId = matchingRequirements[0].Id;
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

        public async Task<Dictionary<string, (int Required, int Completed, int Assisted)>> GetProcedureProgressByCategoryAsync()
        {
            try
            {
                var user = await _userService.GetCurrentUserAsync();
                if (user?.CurrentSpecializationId == null)
                {
                    return new Dictionary<string, (int Required, int Completed, int Assisted)>();
                }

                return await _repository.GetProcedureProgressByCategoryAsync(user.Id, user.CurrentSpecializationId.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting procedure progress by category");
                return new Dictionary<string, (int Required, int Completed, int Assisted)>();
            }
        }

        public async Task<Dictionary<string, (int Required, int Completed, int Assisted)>> GetProcedureProgressByStageAsync()
        {
            try
            {
                var user = await _userService.GetCurrentUserAsync();
                if (user?.CurrentSpecializationId == null)
                {
                    return new Dictionary<string, (int Required, int Completed, int Assisted)>();
                }

                return await _repository.GetProcedureProgressByStageAsync(user.Id, user.CurrentSpecializationId.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting procedure progress by stage");
                return new Dictionary<string, (int Required, int Completed, int Assisted)>();
            }
        }

        public async Task<double> GetProcedureCompletionPercentageAsync()
        {
            try
            {
                var categoriesProgress = await GetProcedureProgressByCategoryAsync();
                if (categoriesProgress.Count == 0)
                    return 0;

                int totalRequired = 0;
                int totalCompleted = 0;

                foreach (var category in categoriesProgress)
                {
                    totalRequired += category.Value.Required;
                    totalCompleted += category.Value.Completed;
                }

                return totalRequired > 0 ? (double)totalCompleted / totalRequired : 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating procedure completion percentage");
                return 0;
            }
        }
    }
}
