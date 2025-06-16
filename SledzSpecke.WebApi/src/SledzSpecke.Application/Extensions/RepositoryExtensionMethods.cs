using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Repositories;

namespace SledzSpecke.Application.Extensions;

public static class RepositoryExtensionMethods
{
    public static async Task<IEnumerable<MedicalShift>> GetByInternshipIdAndDateRangeAsync(
        this IMedicalShiftRepository repository,
        int internshipId,
        DateTime startDate,
        DateTime endDate)
    {
        var allShifts = await repository.GetByInternshipIdAsync(internshipId);
        return allShifts.Where(s => s.Date >= startDate && s.Date <= endDate);
    }

    public static async Task<IEnumerable<ProcedureBase>> GetByInternshipIdAndDateAsync(
        this IProcedureRepository repository,
        int internshipId,
        DateTime date)
    {
        var allProcedures = await repository.GetByInternshipIdAsync(internshipId);
        return allProcedures.Where(p => p.Date.Date == date.Date);
    }

    public static async Task<IEnumerable<ProcedureBase>> GetRecentByInternshipIdAsync(
        this IProcedureRepository repository,
        int internshipId,
        int days)
    {
        var allProcedures = await repository.GetByInternshipIdAsync(internshipId);
        var cutoffDate = DateTime.UtcNow.AddDays(-days);
        return allProcedures.Where(p => p.Date >= cutoffDate).OrderByDescending(p => p.Date);
    }
}