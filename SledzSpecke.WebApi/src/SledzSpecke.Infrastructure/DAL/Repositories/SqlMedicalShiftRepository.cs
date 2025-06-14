using Microsoft.EntityFrameworkCore;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Infrastructure.DAL.Repositories;

internal sealed class SqlMedicalShiftRepository : IMedicalShiftRepository
{
    private readonly SledzSpeckeDbContext _context;

    public SqlMedicalShiftRepository(SledzSpeckeDbContext context)
    {
        _context = context;
    }

    public async Task<MedicalShift?> GetByIdAsync(int id)
    {
        return await _context.MedicalShifts
            .FirstOrDefaultAsync(s => s.Id.Value == id);
    }

    public async Task<IEnumerable<MedicalShift>> GetByInternshipIdAsync(int internshipId)
    {
        return await _context.MedicalShifts
            .Where(s => s.InternshipId.Value == internshipId)
            .ToListAsync();
    }

    public async Task<IEnumerable<MedicalShift>> GetByUserIdAsync(int userId)
    {
        var internshipIds = await _context.Internships
            .Join(_context.Specializations,
                i => i.SpecializationId,
                s => s.Id,
                (i, s) => new { Internship = i, Specialization = s })
            .Join(_context.Users,
                x => x.Specialization.Id,
                u => u.SpecializationId,
                (x, u) => new { x.Internship, User = u })
            .Where(x => x.User.Id.Value == userId)
            .Select(x => x.Internship.Id)
            .ToListAsync();

        return await _context.MedicalShifts
            .Where(s => internshipIds.Contains(s.InternshipId))
            .ToListAsync();
    }

    public async Task<IEnumerable<MedicalShift>> GetByUserAsync(UserId userId)
    {
        var internshipIds = await _context.Internships
            .Join(_context.Specializations,
                i => i.SpecializationId,
                s => s.Id,
                (i, s) => new { Internship = i, Specialization = s })
            .Join(_context.Users,
                x => x.Specialization.Id,
                u => u.SpecializationId,
                (x, u) => new { x.Internship, User = u })
            .Where(x => x.User.Id == userId)
            .Select(x => x.Internship.Id)
            .ToListAsync();

        return await _context.MedicalShifts
            .Where(s => internshipIds.Contains(s.InternshipId))
            .ToListAsync();
    }

    public async Task<IEnumerable<MedicalShift>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, int userId)
    {
        var internshipIds = await _context.Internships
            .Join(_context.Specializations,
                i => i.SpecializationId,
                s => s.Id,
                (i, s) => new { Internship = i, Specialization = s })
            .Join(_context.Users,
                x => x.Specialization.Id,
                u => u.SpecializationId,
                (x, u) => new { x.Internship, User = u })
            .Where(x => x.User.Id.Value == userId)
            .Select(x => x.Internship.Id)
            .ToListAsync();

        return await _context.MedicalShifts
            .Where(s => s.Date >= startDate && s.Date <= endDate && internshipIds.Contains(s.InternshipId))
            .ToListAsync();
    }

    public async Task<IEnumerable<MedicalShift>> GetAllAsync()
    {
        return await _context.MedicalShifts.ToListAsync();
    }

    public async Task<int> GetTotalHoursAsync(int internshipId)
    {
        var shifts = await GetByInternshipIdAsync(internshipId);
        return shifts.Sum(s => s.Hours + (s.Minutes / 60));
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
            
            await connection.CloseAsync();
        }
        
        await _context.MedicalShifts.AddAsync(shift);
        await _context.SaveChangesAsync();
        return shift.Id.Value;
    }

    public async Task UpdateAsync(MedicalShift shift)
    {
        _context.MedicalShifts.Update(shift);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var shift = await GetByIdAsync(id);
        if (shift != null)
        {
            _context.MedicalShifts.Remove(shift);
            await _context.SaveChangesAsync();
        }
    }
}