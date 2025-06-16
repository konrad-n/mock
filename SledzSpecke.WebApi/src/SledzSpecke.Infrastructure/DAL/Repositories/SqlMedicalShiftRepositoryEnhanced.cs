using Microsoft.EntityFrameworkCore;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;
using System.Linq.Expressions;

namespace SledzSpecke.Infrastructure.DAL.Repositories;

/// <summary>
/// Enhanced version of SqlMedicalShiftRepository following MySpot patterns
/// This maintains backward compatibility while adding new features
/// </summary>
internal sealed class SqlMedicalShiftRepositoryEnhanced : IMedicalShiftRepository
{
    private readonly SledzSpeckeDbContext _context;
    private readonly DbSet<MedicalShift> _medicalShifts;

    public SqlMedicalShiftRepositoryEnhanced(SledzSpeckeDbContext context)
    {
        _context = context;
        _medicalShifts = context.MedicalShifts;
    }

    // Original interface implementations
    public async Task<MedicalShift?> GetByIdAsync(int id)
    {
        return await _medicalShifts
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id.Value == id);
    }

    public async Task<IEnumerable<MedicalShift>> GetByInternshipIdAsync(int internshipId)
    {
        return await _medicalShifts
            .AsNoTracking()
            .Where(s => s.InternshipId.Value == internshipId)
            .OrderByDescending(s => s.Date)
            .ToListAsync();
    }

    public async Task<IEnumerable<MedicalShift>> GetByUserIdAsync(int userId)
    {
        var internshipIds = await GetUserInternshipIds(userId);

        return await _medicalShifts
            .AsNoTracking()
            .Where(s => internshipIds.Contains(s.InternshipId))
            .OrderByDescending(s => s.Date)
            .ToListAsync();
    }

    public async Task<IEnumerable<MedicalShift>> GetByUserAsync(UserId userId)
    {
        return await GetByUserIdAsync(userId.Value);
    }

    public async Task<int> AddAsync(MedicalShift shift)
    {
        // Generate new ID if it's 0
        if (shift.Id.Value == 0)
        {
            // Query raw database to get max ID
            var connection = _context.Database.GetDbConnection();
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT COALESCE(MAX(\"Id\"), 0) FROM \"MedicalShifts\"";
            var maxId = (int)(await command.ExecuteScalarAsync() ?? 0);

            var newId = new MedicalShiftId(maxId + 1);

            // Use reflection to set the ID since it's private
            var idProperty = shift.GetType().GetProperty("Id");
            idProperty?.SetValue(shift, newId);
        }

        await _medicalShifts.AddAsync(shift);
        await _context.SaveChangesAsync();
        return shift.Id.Value;
    }

    public async Task UpdateAsync(MedicalShift shift)
    {
        _medicalShifts.Update(shift);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var shift = await GetByIdAsync(id);
        if (shift != null)
        {
            _medicalShifts.Remove(shift);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<MedicalShift>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, int userId)
    {
        var internshipIds = await GetUserInternshipIds(userId);

        return await _medicalShifts
            .AsNoTracking()
            .Where(s => internshipIds.Contains(s.InternshipId) &&
                       s.Date >= startDate &&
                       s.Date <= endDate)
            .OrderByDescending(s => s.Date)
            .ToListAsync();
    }

    public async Task<IEnumerable<MedicalShift>> GetAllAsync()
    {
        return await _medicalShifts
            .AsNoTracking()
            .OrderByDescending(s => s.Date)
            .ToListAsync();
    }

    public async Task<int> GetTotalHoursAsync(int internshipId)
    {
        var shifts = await _medicalShifts
            .AsNoTracking()
            .Where(s => s.InternshipId.Value == internshipId)
            .ToListAsync();

        return shifts.Sum(s => s.Hours) + shifts.Sum(s => s.Minutes) / 60;
    }

    // Enhanced methods following MySpot patterns
    public async Task<IEnumerable<MedicalShift>> GetBySpecificationAsync(
        ISpecification<MedicalShift> specification)
    {
        return await _medicalShifts
            .AsNoTracking()
            .Where(specification.ToExpression())
            .ToListAsync();
    }

    public async Task<MedicalShift?> GetSingleBySpecificationAsync(
        ISpecification<MedicalShift> specification)
    {
        return await _medicalShifts
            .AsNoTracking()
            .Where(specification.ToExpression())
            .SingleOrDefaultAsync();
    }

    public async Task<bool> ExistsAsync(Expression<Func<MedicalShift, bool>> predicate)
    {
        return await _medicalShifts.AnyAsync(predicate);
    }

    public async Task<int> CountAsync(Expression<Func<MedicalShift, bool>>? predicate = null)
    {
        return predicate is null
            ? await _medicalShifts.CountAsync()
            : await _medicalShifts.CountAsync(predicate);
    }

    public async Task<List<MedicalShift>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        Expression<Func<MedicalShift, bool>>? predicate = null,
        bool orderByDateDescending = true)
    {
        var query = _medicalShifts.AsNoTracking();

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        query = orderByDateDescending
            ? query.OrderByDescending(s => s.Date)
            : query.OrderBy(s => s.Date);

        return await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    // Private helper methods
    private async Task<List<InternshipId>> GetUserInternshipIds(int userId)
    {
        // TODO: User-Specialization relationship needs to be redesigned
        // The User entity no longer has SpecializationId property
        // This query needs to be refactored once the new relationship model is established
        /*
        return await _context.Internships
            .Join(_context.Specializations,
                i => i.SpecializationId,
                s => s.Id,
                (i, s) => new { Internship = i, Specialization = s })
            .Join(_context.Users,
                x => x.Specialization.Id,
                u => u.SpecializationId,
                (x, u) => new { x.Internship, User = u })
            .Where(x => x.User.Id.Value == userId)
            .Select(x => x.Internship.InternshipId)
            .ToListAsync();
        */
        return new List<InternshipId>(); // Temporary empty list
    }

    public async Task<IEnumerable<MedicalShift>> GetByUserIdAndDateRangeAsync(UserId userId, DateTime startDate, DateTime endDate)
    {
        var internshipIds = await GetUserInternshipIds(userId.Value);
        
        return await _medicalShifts
            .Where(s => s.Date >= startDate && s.Date <= endDate && internshipIds.Contains(s.InternshipId))
            .ToListAsync();
    }
}