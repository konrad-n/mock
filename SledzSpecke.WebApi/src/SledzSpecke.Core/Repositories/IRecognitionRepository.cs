using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.Repositories;

public interface IRecognitionRepository
{
    Task<Recognition?> GetByIdAsync(RecognitionId id);
    Task<IEnumerable<Recognition>> GetByUserIdAsync(UserId userId);
    Task<IEnumerable<Recognition>> GetBySpecializationIdAsync(SpecializationId specializationId);
    Task<IEnumerable<Recognition>> GetByUserAndSpecializationAsync(UserId userId, SpecializationId specializationId);
    Task<IEnumerable<Recognition>> GetApprovedRecognitionsAsync(UserId userId, SpecializationId specializationId);
    Task<int> GetTotalReductionDaysAsync(UserId userId, SpecializationId specializationId);
    Task<IEnumerable<Recognition>> GetByTypeAsync(RecognitionType type);
    Task AddAsync(Recognition recognition);
    Task UpdateAsync(Recognition recognition);
    Task DeleteAsync(RecognitionId id);
}