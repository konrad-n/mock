using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.Repositories;

public interface IProcedureRepository
{
    Task<ProcedureBase?> GetByIdAsync(ProcedureId id);
    Task<IEnumerable<ProcedureBase>> GetByInternshipIdAsync(int internshipId);
    Task<IEnumerable<ProcedureBase>> GetByUserIdAsync(int userId);
    Task<IEnumerable<ProcedureBase>> GetByUserAsync(UserId userId);
    Task<IEnumerable<ProcedureBase>> GetByCodeAsync(string code);
    Task<IEnumerable<ProcedureBase>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, int userId);
    Task<IEnumerable<ProcedureBase>> GetAllAsync();
    Task<int> AddAsync(ProcedureBase procedure);
    Task UpdateAsync(ProcedureBase procedure);
    Task DeleteAsync(ProcedureBase procedure);
    Task<Dictionary<string, int>> GetProcedureCountsByCodeAsync(int internshipId);
    Task<IEnumerable<int>> GetUserInternshipIdsAsync(int userId);
}