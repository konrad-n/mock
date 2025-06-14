using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.Repositories;

public interface IEducationalActivityRepository
{
    Task<EducationalActivity?> GetByIdAsync(EducationalActivityId id);
    Task<IEnumerable<EducationalActivity>> GetBySpecializationIdAsync(SpecializationId specializationId);
    Task<IEnumerable<EducationalActivity>> GetByModuleIdAsync(ModuleId moduleId);
    Task<IEnumerable<EducationalActivity>> GetByTypeAsync(SpecializationId specializationId, EducationalActivityType type);
    Task<IEnumerable<EducationalActivity>> GetByDateRangeAsync(SpecializationId specializationId, DateTime startDate, DateTime endDate);
    Task AddAsync(EducationalActivity activity);
    Task UpdateAsync(EducationalActivity activity);
    Task DeleteAsync(EducationalActivity activity);
}