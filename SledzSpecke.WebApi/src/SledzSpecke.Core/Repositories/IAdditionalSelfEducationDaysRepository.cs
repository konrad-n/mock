using SledzSpecke.Core.Entities;

namespace SledzSpecke.Core.Repositories;

public interface IAdditionalSelfEducationDaysRepository
{
    Task<AdditionalSelfEducationDays?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<AdditionalSelfEducationDays>> GetByModuleIdAsync(int moduleId, CancellationToken cancellationToken = default);
    Task<List<AdditionalSelfEducationDays>> GetByInternshipIdAsync(int internshipId, CancellationToken cancellationToken = default);
    Task<List<AdditionalSelfEducationDays>> GetByYearAsync(int year, CancellationToken cancellationToken = default);
    Task<int> GetTotalDaysInYearAsync(int moduleId, int year, CancellationToken cancellationToken = default);
    Task AddAsync(AdditionalSelfEducationDays additionalSelfEducationDays, CancellationToken cancellationToken = default);
    Task UpdateAsync(AdditionalSelfEducationDays additionalSelfEducationDays, CancellationToken cancellationToken = default);
    Task DeleteAsync(AdditionalSelfEducationDays additionalSelfEducationDays, CancellationToken cancellationToken = default);
}