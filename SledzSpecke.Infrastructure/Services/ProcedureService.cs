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

        // Implementacje istniejących metod z dodaną walidacją
        public async Task<ProcedureExecution> AddProcedureAsync(ProcedureExecution procedure)
        {
            try
            {
                procedure.UserId = await _userService.GetCurrentUserIdAsync();
                procedure.CreatedAt = DateTime.UtcNow;
                await ValidateProcedureAsync(procedure);

                // Sprawdź, czy procedura pasuje do wymagań programu
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

        public async Task<List<ProcedureRequirement>> GetRequirementsForSpecializationAsync()
        {
            try
            {
                var user = await _userService.GetCurrentUserAsync();
                if (user?.CurrentSpecializationId == null)
                {
                    throw new NotFoundException("Current specialization not found");
                }

                return await _repository.GetRequirementsForSpecializationAsync(user.CurrentSpecializationId.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting procedure requirements");
                throw;
            }
        }

        public async Task<List<ProcedureRequirement>> GetRequirementsByStageAsync(string stage)
        {
            try
            {
                var user = await _userService.GetCurrentUserAsync();
                if (user?.CurrentSpecializationId == null)
                {
                    throw new NotFoundException("Current specialization not found");
                }

                return await _repository.GetRequirementsByStageAsync(user.CurrentSpecializationId.Value, stage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting procedure requirements by stage");
                throw;
            }
        }

        public async Task<List<ProcedureRequirement>> GetRequirementsByCategoryAsync(string category)
        {
            try
            {
                var user = await _userService.GetCurrentUserAsync();
                if (user?.CurrentSpecializationId == null)
                {
                    throw new NotFoundException("Current specialization not found");
                }

                return await _repository.GetRequirementsByCategoryAsync(user.CurrentSpecializationId.Value, category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting procedure requirements by category");
                throw;
            }
        }

        public async Task<Dictionary<string, (int Required, int Completed, int Assisted)>> GetProcedureProgressByCategoryAsync()
        {
            try
            {
                var userId = await _userService.GetCurrentUserIdAsync();
                var user = await _userService.GetCurrentUserAsync();
                if (user?.CurrentSpecializationId == null)
                {
                    throw new NotFoundException("Current specialization not found");
                }

                return await _repository.GetProcedureProgressByCategoryAsync(userId, user.CurrentSpecializationId.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting procedure progress by category");
                throw;
            }
        }

        public async Task<Dictionary<string, (int Required, int Completed, int Assisted)>> GetProcedureProgressByStageAsync()
        {
            try
            {
                var userId = await _userService.GetCurrentUserIdAsync();
                var user = await _userService.GetCurrentUserAsync();
                if (user?.CurrentSpecializationId == null)
                {
                    throw new NotFoundException("Current specialization not found");
                }

                return await _repository.GetProcedureProgressByStageAsync(userId, user.CurrentSpecializationId.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting procedure progress by stage");
                throw;
            }
        }

        public async Task<double> GetProcedureCompletionPercentageAsync()
        {
            try
            {
                var progress = await GetProcedureProgressByCategoryAsync();
                int totalRequired = 0;
                int totalCompleted = 0;

                foreach (var (_, stats) in progress)
                {
                    totalRequired += stats.Required;
                    totalCompleted += stats.Completed;
                }

                return totalRequired > 0 ? (double)totalCompleted / totalRequired : 1.0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating procedure completion percentage");
                throw;
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
                throw;
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
                throw;
            }
        }

        public async Task<bool> ValidateProcedureRequirementsAsync(ProcedureExecution procedure)
        {
            try
            {
                // Jeśli nie ma przypisanej kategorii lub etapu, nie ma co sprawdzać
                if (string.IsNullOrEmpty(procedure.Category) && string.IsNullOrEmpty(procedure.Stage))
                {
                    return true;
                }

                var user = await _userService.GetCurrentUserAsync();
                if (user?.CurrentSpecializationId == null)
                {
                    return true; // Nie można zwalidować, ale nie ma potrzeby rzucać wyjątkiem
                }

                var requirements = await GetRequirementsForSpecializationAsync();

                // Znajdź pasujące wymaganie
                var matchingRequirements = requirements
                    .Where(r => (string.IsNullOrEmpty(procedure.Category) || r.Category == procedure.Category) &&
                              (string.IsNullOrEmpty(procedure.Stage) || r.Stage == procedure.Stage))
                    .ToList();

                if (!matchingRequirements.Any())
                {
                    return true; // Nie znaleziono pasujących wymagań, ale to nie błąd
                }

                // Sprawdź czy procedura typu symulacja jest dozwolona
                if (procedure.IsSimulation)
                {
                    var allowsSimulation = matchingRequirements.Any(r => r.AllowSimulation);
                    if (!allowsSimulation)
                    {
                        throw new ValidationException("Simulation procedures are not allowed for this category/stage");
                    }
                    // Sprawdź limity symulacji (to wymagałoby dodatkowej logiki do określenia ilu procedur użytkownik używa jako symulacje)
                }

                // Sprawdź czy potrzebny nadzór
                if (matchingRequirements.Any(r => r.SupervisionRequired) && procedure.SupervisorId == null)
                {
                    throw new ValidationException("This procedure requires supervision");
                }

                // Jeśli wszystko przeszło, oznacz procedurę jako pasującą do odpowiedniego wymagania programu
                if (matchingRequirements.Count == 1)
                {
                    procedure.ProcedureRequirementId = matchingRequirements[0].Id;
                }

                return true;
            }
            catch (ValidationException)
            {
                throw; // Przekaż wyjątek walidacji
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating procedure requirements");
                throw;
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

            // Sprawdź czy procedura wymaga nadzoru
            if (procedure.ProcedureRequirementId.HasValue)
            {
                // FIX: Use proper repository method to get ProcedureRequirement
                var requirementId = procedure.ProcedureRequirementId.Value;
                var requirements = await GetRequirementsForSpecializationAsync();
                var requirement = requirements.FirstOrDefault(r => r.Id == requirementId);

                if (requirement != null && requirement.SupervisionRequired && procedure.SupervisorId == null)
                {
                    throw new ValidationException("This procedure requires supervision");
                }
            }
        }

        // Implementacje pozostałych istniejących metod pozostają bez zmian
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
    }
}
