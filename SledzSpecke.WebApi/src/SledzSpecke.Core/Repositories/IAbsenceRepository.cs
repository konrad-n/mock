using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.Repositories;

public interface IAbsenceRepository
{
    Task<Absence?> GetByIdAsync(AbsenceId id);
    Task<IEnumerable<Absence>> GetByUserIdAsync(UserId userId);
    Task<IEnumerable<Absence>> GetBySpecializationIdAsync(SpecializationId specializationId);
    Task<IEnumerable<Absence>> GetByUserAndSpecializationAsync(UserId userId, SpecializationId specializationId);
    Task<IEnumerable<Absence>> GetActiveAbsencesAsync(UserId userId);
    Task<IEnumerable<Absence>> GetOverlappingAbsencesAsync(UserId userId, DateTime startDate, DateTime endDate);
    Task<bool> HasOverlappingAbsencesAsync(UserId userId, DateTime startDate, DateTime endDate, AbsenceId? excludeId = null);
    Task AddAsync(Absence absence);
    Task UpdateAsync(Absence absence);
    Task DeleteAsync(AbsenceId id);
}