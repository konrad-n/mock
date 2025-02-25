using SledzSpecke.Core.Interfaces.Services;
using SledzSpecke.Core.Models.Domain;
using SledzSpecke.Infrastructure.Database.Repositories;
using System;
using System.Threading.Tasks;

namespace SledzSpecke.Infrastructure.Services
{
    public class SpecializationSyncService : ISpecializationSyncService
    {
        private readonly ISpecializationRepository _specializationRepository;
        
        public SpecializationSyncService(ISpecializationRepository specializationRepository)
        {
            _specializationRepository = specializationRepository;
        }
        
        public async Task<bool> UpdateSpecializationAsync(Specialization updatedSpecialization)
        {
            try
            {
                // Pobierz istniejącą specjalizację
                var existingSpecialization = await _specializationRepository.GetWithRequirementsAsync(updatedSpecialization.Id);
                if (existingSpecialization == null)
                {
                    return false;
                }
                
                // Zaktualizuj podstawowe dane
                existingSpecialization.Name = updatedSpecialization.Name;
                existingSpecialization.DurationInWeeks = updatedSpecialization.DurationInWeeks;
                existingSpecialization.ProgramVersion = updatedSpecialization.ProgramVersion;
                existingSpecialization.ApprovalDate = updatedSpecialization.ApprovalDate;
                existingSpecialization.MinimumDutyHours = updatedSpecialization.MinimumDutyHours;
                existingSpecialization.Requirements = updatedSpecialization.Requirements;
                existingSpecialization.Description = updatedSpecialization.Description;
                
                // Zapisz zaktualizowaną specjalizację
                await _specializationRepository.UpdateAsync(existingSpecialization);
                
                // Tutaj można dodać kod do aktualizacji związanych z nią wymagań
                // np. procedur, kursów, staży itp.
                
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        
        public async Task<bool> ImportSpecializationDataAsync(string jsonData)
        {
            try
            {
                // Deserializacja danych specjalizacji z JSON
                var specialization = System.Text.Json.JsonSerializer.Deserialize<Specialization>(jsonData);
                if (specialization == null)
                {
                    return false;
                }
                
                // Sprawdź, czy specjalizacja już istnieje
                var existingSpecialization = await _specializationRepository.GetByIdAsync(specialization.Id);
                if (existingSpecialization != null)
                {
                    // Aktualizuj istniejącą specjalizację
                    return await UpdateSpecializationAsync(specialization);
                }
                else
                {
                    // Dodaj nową specjalizację
                    await _specializationRepository.AddAsync(specialization);
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        
        public async Task<string> ExportSpecializationDataAsync(int specializationId)
        {
            try
            {
                // Pobierz specjalizację z wszystkimi powiązanymi danymi
                var specialization = await _specializationRepository.GetWithRequirementsAsync(specializationId);
                if (specialization == null)
                {
                    return string.Empty;
                }
                
                // Serializuj do JSON
                var jsonOptions = new System.Text.Json.JsonSerializerOptions
                {
                    WriteIndented = true
                };
                
                return System.Text.Json.JsonSerializer.Serialize(specialization, jsonOptions);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}
