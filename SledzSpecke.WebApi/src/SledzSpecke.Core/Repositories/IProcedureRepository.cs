using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.Repositories;

public interface IProcedureRepository
{
    Task<Procedure?> GetByIdAsync(ProcedureId id);
    Task<IEnumerable<Procedure>> GetByInternshipIdAsync(int internshipId);
    Task<IEnumerable<Procedure>> GetByUserIdAsync(int userId);
    Task<IEnumerable<Procedure>> GetByUserAsync(UserId userId);
    Task<IEnumerable<Procedure>> GetByCodeAsync(string code);
    Task<IEnumerable<Procedure>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, int userId);
    Task<IEnumerable<Procedure>> GetAllAsync();
    Task<int> AddAsync(Procedure procedure);
    Task UpdateAsync(Procedure procedure);
    Task DeleteAsync(Procedure procedure);
    Task<Dictionary<string, int>> GetProcedureCountsByCodeAsync(int internshipId);
    Task<IEnumerable<int>> GetUserInternshipIdsAsync(int userId);
}