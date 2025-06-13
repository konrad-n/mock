using Microsoft.EntityFrameworkCore;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Infrastructure.DAL.Repositories;

internal sealed class SqlAbsenceRepository : IAbsenceRepository
{
    private readonly SledzSpeckeDbContext _context;
    private readonly DbSet<Absence> _absences;

    public SqlAbsenceRepository(SledzSpeckeDbContext context)
    {
        _context = context;
        _absences = context.Absences;
    }

    public async Task<Absence?> GetByIdAsync(AbsenceId id)
        => await _absences.SingleOrDefaultAsync(a => a.Id == id);

    public async Task<IEnumerable<Absence>> GetByUserIdAsync(UserId userId)
        => await _absences
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.StartDate)
            .ToListAsync();

    public async Task<IEnumerable<Absence>> GetBySpecializationIdAsync(SpecializationId specializationId)
        => await _absences
            .Where(a => a.SpecializationId == specializationId)
            .OrderByDescending(a => a.StartDate)
            .ToListAsync();

    public async Task<IEnumerable<Absence>> GetByUserAndSpecializationAsync(UserId userId, SpecializationId specializationId)
        => await _absences
            .Where(a => a.UserId == userId && a.SpecializationId == specializationId)
            .OrderByDescending(a => a.StartDate)
            .ToListAsync();

    public async Task<IEnumerable<Absence>> GetActiveAbsencesAsync(UserId userId)
    {
        var today = DateTime.UtcNow.Date;
        return await _absences
            .Where(a => a.UserId == userId && a.StartDate <= today && a.EndDate >= today && a.IsApproved)
            .ToListAsync();
    }

    public async Task<IEnumerable<Absence>> GetOverlappingAbsencesAsync(UserId userId, DateTime startDate, DateTime endDate)
        => await _absences
            .Where(a => a.UserId == userId && a.StartDate <= endDate && a.EndDate >= startDate)
            .ToListAsync();

    public async Task<bool> HasOverlappingAbsencesAsync(UserId userId, DateTime startDate, DateTime endDate, AbsenceId? excludeId = null)
    {
        var query = _absences
            .Where(a => a.UserId == userId && a.StartDate <= endDate && a.EndDate >= startDate);

        if (excludeId != null)
            query = query.Where(a => a.Id != excludeId);

        return await query.AnyAsync();
    }

    public async Task AddAsync(Absence absence)
    {
        await _absences.AddAsync(absence);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Absence absence)
    {
        _absences.Update(absence);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(AbsenceId id)
    {
        var absence = await _absences.FindAsync(id.Value);
        if (absence is not null)
        {
            _absences.Remove(absence);
            await _context.SaveChangesAsync();
        }
    }
}