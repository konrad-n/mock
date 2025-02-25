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

        // Implementacja istniejących metod
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

        public async Task<Internship> GetInternshipAsync(int id)
        {
            try
            {
                return await _repository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting internship {InternshipId}", id);
                throw;
            }
        }

        public async Task<Internship> StartInternshipAsync(Internship internship)
        {
            try
            {
                internship.UserId = await _userService.GetCurrentUserIdAsync();
                internship.Status = Core.Models.Enums.InternshipStatus.InProgress;
                internship.StartDate = DateTime.Now;
                internship.CreatedAt = DateTime.UtcNow;
                
                await _repository.AddAsync(internship);
                return internship;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting internship");
                throw;
            }
        }

        public async Task<bool> CompleteInternshipAsync(int internshipId, InternshipDocument completion)
        {
            try
            {
                var internship = await _repository.GetByIdAsync(internshipId);
                if (internship == null)
                {
                    throw new NotFoundException("Internship not found");
                }

                var currentUserId = await _userService.GetCurrentUserIdAsync();
                if (internship.UserId != currentUserId)
                {
                    throw new UnauthorizedAccessException("Cannot complete other user's internship");
                }

                internship.Status = Core.Models.Enums.InternshipStatus.Completed;
                internship.IsCompleted = true;
                internship.CompletionDate = DateTime.Now;
                internship.ModifiedAt = DateTime.UtcNow;
                
                // Dodanie dokumentu potwierdzającego
                if (completion != null)
                {
                    completion.InternshipId = internshipId;
                    completion.CreatedAt = DateTime.UtcNow;
                    
                    // Tutaj można dodać kod do zapisu pliku dokumentu
                    // i aktualizacji w bazie danych
                }

                await _repository.UpdateAsync(internship);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing internship {InternshipId}", internshipId);
                throw;
            }
        }

        // Nowe metody
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

        public async Task<Dictionary<string, List<string>>> GetRequiredSkillsByInternshipAsync(int internshipDefinitionId)
        {
            try
            {
                return await _repository.GetRequiredSkillsByInternshipAsync(internshipDefinitionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting required skills by internship");
                throw;
            }
        }

        public async Task<Dictionary<string, Dictionary<string, int>>> GetRequiredProceduresByInternshipAsync(int internshipDefinitionId)
        {
            try
            {
                return await _repository.GetRequiredProceduresByInternshipAsync(internshipDefinitionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting required procedures by internship");
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

        public async Task<List<InternshipDefinition>> GetRecommendedInternshipsForCurrentYearAsync()
        {
            try
            {
                var user = await _userService.GetCurrentUserAsync();
                if (user?.CurrentSpecializationId == null)
                {
                    throw new NotFoundException("Current specialization not found");
                }
                
                // Oblicz aktualny rok specjalizacji na podstawie daty rozpoczęcia
                int currentYear = 1;
                if (user.SpecializationStartDate != default)
                {
                    var yearsInProgram = (DateTime.Today - user.SpecializationStartDate).Days / 365;
                    currentYear = Math.Max(1, Math.Min(6, yearsInProgram + 1)); // Zakładamy max 6 lat specjalizacji
                }
                
                var allInternships = await _repository.GetRequiredInternshipsAsync(user.CurrentSpecializationId.Value);
                return allInternships.Where(i => i.RecommendedYear == currentYear).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recommended internships for current year");
                throw;
            }
        }
    }
}
