using Microsoft.Extensions.Logging;
using SledzSpecke.Core.Exceptions;
using SledzSpecke.Core.Interfaces.Services;
using SledzSpecke.Core.Models.Domain;
using SledzSpecke.Infrastructure.Database.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SledzSpecke.Infrastructure.Services
{
    public class InternshipService : IInternshipService
    {
        private readonly IInternshipRepository _repository;
        private readonly IUserService _userService;
        private readonly ILogger<InternshipService> _logger;

        public InternshipService(
            IInternshipRepository repository,
            IUserService userService,
            ILogger<InternshipService> logger)
        {
            _repository = repository;
            _userService = userService;
            _logger = logger;
        }

        public async Task<List<Internship>> GetUserInternshipsAsync()
        {
            try
            {
                var userId = await _userService.GetCurrentUserIdAsync();
                return await _repository.GetUserInternshipsAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user internships");
                throw;
            }
        }

        public async Task<List<InternshipDefinition>> GetRequiredInternshipsAsync()
        {
            try
            {
                var user = await _userService.GetCurrentUserAsync();
                if (user?.CurrentSpecializationId == null)
                {
                    throw new NotFoundException("Current specialization not found");
                }
                
                return await _repository.GetRequiredInternshipsAsync(user.CurrentSpecializationId.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting required internships");
                throw;
            }
        }

        public async Task<List<InternshipModule>> GetModulesForInternshipAsync(int internshipDefinitionId)
        {
            try
            {
                return await _repository.GetModulesForInternshipAsync(internshipDefinitionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting internship modules");
                throw;
            }
        }

        public async Task<double> GetInternshipProgressAsync()
        {
            try
            {
                var userId = await _userService.GetCurrentUserIdAsync();
                var user = await _userService.GetCurrentUserAsync();
                if (user?.CurrentSpecializationId == null)
                {
                    throw new NotFoundException("Current specialization not found");
                }
                
                return await _repository.GetInternshipProgressAsync(userId, user.CurrentSpecializationId.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting internship progress");
                throw;
            }
        }

        public async Task<Dictionary<string, (int Required, int Completed)>> GetInternshipProgressByYearAsync()
        {
            try
            {
                var userId = await _userService.GetCurrentUserIdAsync();
                var user = await _userService.GetCurrentUserAsync();
                if (user?.CurrentSpecializationId == null)
                {
                    throw new NotFoundException("Current specialization not found");
                }
                
                return await _repository.GetInternshipProgressByYearAsync(userId, user.CurrentSpecializationId.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting internship progress by year");
                throw;
            }
        }
    }
}
