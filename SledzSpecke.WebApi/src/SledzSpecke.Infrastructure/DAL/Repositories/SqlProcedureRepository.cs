using Microsoft.EntityFrameworkCore;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Infrastructure.DAL.Repositories;

internal sealed class SqlProcedureRepository : IProcedureRepository
{
    private readonly SledzSpeckeDbContext _context;

    public SqlProcedureRepository(SledzSpeckeDbContext context)
    {
        _context = context;
    }

    public async Task<Procedure?> GetByIdAsync(ProcedureId id)
    {
        return await _context.Procedures
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Procedure>> GetByInternshipIdAsync(int internshipId)
    {
        return await _context.Procedures
            .Where(p => p.InternshipId.Value == internshipId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Procedure>> GetByUserIdAsync(int userId)
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

        return await _context.Procedures
            .Where(p => internshipIds.Contains(p.InternshipId))
            .ToListAsync();
    }

    public async Task<IEnumerable<Procedure>> GetByUserAsync(UserId userId)
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

        return await _context.Procedures
            .Where(p => internshipIds.Contains(p.InternshipId))
            .ToListAsync();
    }

    public async Task<IEnumerable<Procedure>> GetByCodeAsync(string code)
    {
        return await _context.Procedures
            .Where(p => p.Code == code)
            .ToListAsync();
    }

    public async Task<IEnumerable<Procedure>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, int userId)
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

        return await _context.Procedures
            .Where(p => p.Date >= startDate && p.Date <= endDate && internshipIds.Contains(p.InternshipId))
            .ToListAsync();
    }

    public async Task<IEnumerable<Procedure>> GetAllAsync()
    {
        return await _context.Procedures.ToListAsync();
    }

    public async Task<Dictionary<string, int>> GetProcedureCountsByCodeAsync(int internshipId)
    {
        var procedures = await GetByInternshipIdAsync(internshipId);
        return procedures
            .GroupBy(p => p.Code)
            .ToDictionary(g => g.Key, g => g.Count());
    }

    public async Task<int> AddAsync(Procedure procedure)
    {
        await _context.Procedures.AddAsync(procedure);
        await _context.SaveChangesAsync();
        return procedure.Id.Value;
    }

    public async Task UpdateAsync(Procedure procedure)
    {
        _context.Procedures.Update(procedure);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Procedure procedure)
    {
        _context.Procedures.Remove(procedure);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<int>> GetUserInternshipIdsAsync(int userId)
    {
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
            .Select(x => x.Internship.Id.Value)
            .ToListAsync();
    }
}