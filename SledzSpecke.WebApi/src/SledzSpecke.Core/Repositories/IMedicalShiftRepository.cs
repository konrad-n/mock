using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.Repositories;

public interface IMedicalShiftRepository
{
    Task<MedicalShift?> GetByIdAsync(int id);
    Task<IEnumerable<MedicalShift>> GetByInternshipIdAsync(int internshipId);
    Task<IEnumerable<MedicalShift>> GetByUserIdAsync(int userId);
    Task<IEnumerable<MedicalShift>> GetByUserAsync(UserId userId);
    Task<IEnumerable<MedicalShift>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, int userId);
    Task<IEnumerable<MedicalShift>> GetByUserIdAndDateRangeAsync(UserId userId, DateTime startDate, DateTime endDate);
    Task<IEnumerable<MedicalShift>> GetAllAsync();
    Task<int> AddAsync(MedicalShift medicalShift);
    Task UpdateAsync(MedicalShift medicalShift);
    Task DeleteAsync(int id);
    Task<int> GetTotalHoursAsync(int internshipId);
}