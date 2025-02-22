using Microsoft.Extensions.Logging;
using SledzSpecke.Core.Exceptions;
using SledzSpecke.Core.Interfaces.Services;
using SledzSpecke.Core.Models.Domain;
using SledzSpecke.Infrastructure.Database.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace SledzSpecke.Infrastructure.Services
{
    // Infrastructure/Services/ProcedureService.cs
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

        public async Task<ProcedureExecution> AddProcedureAsync(ProcedureExecution procedure)
        {
            try
            {
                procedure.UserId = await _userService.GetCurrentUserIdAsync();
                procedure.CreatedAt = DateTime.UtcNow;

                await ValidateProcedureAsync(procedure);
                await _repository.AddAsync(procedure);
                return procedure;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding procedure");
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

        public async Task<Dictionary<string, int>> GetProcedureStatisticsAsync()
        {
            try
            {
                var userId = await _userService.GetCurrentUserIdAsync();
                return await _repository.GetProcedureStatsAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting procedure statistics");
                throw;
            }
        }
    }
}
