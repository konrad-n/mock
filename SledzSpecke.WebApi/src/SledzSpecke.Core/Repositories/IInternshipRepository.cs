using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.Repositories;

public interface IInternshipRepository
{
    Task<Internship?> GetByIdAsync(InternshipId id);
    Task<IEnumerable<Internship>> GetBySpecializationIdAsync(SpecializationId specializationId);
    Task<IEnumerable<Internship>> GetByModuleIdAsync(ModuleId moduleId);
    Task<IEnumerable<Internship>> GetByUserAndSpecializationAsync(UserId userId, SpecializationId specializationId);
    Task<IEnumerable<Internship>> GetByModuleAsync(ModuleId moduleId);
    Task<IEnumerable<Internship>> GetPendingApprovalAsync();
    Task AddAsync(Internship internship);
    Task UpdateAsync(Internship internship);
    Task DeleteAsync(InternshipId id);
}