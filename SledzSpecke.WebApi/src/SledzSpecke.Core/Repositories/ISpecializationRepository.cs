using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.Repositories;

public interface ISpecializationRepository
{
    Task<Specialization?> GetByIdAsync(SpecializationId id);
    Task<IEnumerable<Specialization>> GetByUserIdAsync(UserId userId);
    Task<IEnumerable<Specialization>> GetAllAsync();
    Task<SpecializationId> AddAsync(Specialization specialization);
    Task UpdateAsync(Specialization specialization);
    Task DeleteAsync(SpecializationId id);
}