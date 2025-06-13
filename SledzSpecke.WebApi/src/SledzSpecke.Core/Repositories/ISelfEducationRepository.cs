using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.Repositories;

public interface ISelfEducationRepository
{
    Task<SelfEducation?> GetByIdAsync(SelfEducationId id);
    Task<IEnumerable<SelfEducation>> GetByUserIdAsync(UserId userId);
    Task<IEnumerable<SelfEducation>> GetBySpecializationIdAsync(SpecializationId specializationId);
    Task<IEnumerable<SelfEducation>> GetByUserAndSpecializationAsync(UserId userId, SpecializationId specializationId);
    Task<IEnumerable<SelfEducation>> GetByYearAsync(UserId userId, int year);
    Task<IEnumerable<SelfEducation>> GetByTypeAsync(SelfEducationType type);
    Task<IEnumerable<SelfEducation>> GetCompletedActivitiesAsync(UserId userId, SpecializationId specializationId);
    Task<int> GetTotalCreditHoursAsync(UserId userId, SpecializationId specializationId);
    Task<int> GetTotalQualityScoreAsync(UserId userId, SpecializationId specializationId);
    Task<IEnumerable<SelfEducation>> GetActivitiesWithCertificatesAsync(UserId userId);
    Task AddAsync(SelfEducation selfEducation);
    Task UpdateAsync(SelfEducation selfEducation);
    Task DeleteAsync(SelfEducationId id);
}