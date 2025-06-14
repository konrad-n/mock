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

    public async Task<ProcedureBase?> GetByIdAsync(ProcedureId id)
    {
        return await _context.Procedures
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<ProcedureBase>> GetByInternshipIdAsync(int internshipId)
    {
        return await _context.Procedures
            .Where(p => p.InternshipId.Value == internshipId)
            .ToListAsync();
    }

    public async Task<IEnumerable<ProcedureBase>> GetByUserIdAsync(int userId)
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

    public async Task<IEnumerable<ProcedureBase>> GetByUserAsync(UserId userId)
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

    public async Task<IEnumerable<ProcedureBase>> GetByCodeAsync(string code)
    {
        return await _context.Procedures
            .Where(p => p.Code == code)
            .ToListAsync();
    }

    public async Task<IEnumerable<ProcedureBase>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, int userId)
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

    public async Task<IEnumerable<ProcedureBase>> GetAllAsync()
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

    public async Task<int> AddAsync(ProcedureBase procedure)
    {
        // Generate new ID if it's 0
        if (procedure.Id.Value == 0)
        {
            // Query raw database to get max ID
            var connection = _context.Database.GetDbConnection();
            await connection.OpenAsync();
            
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT COALESCE(MAX(\"Id\"), 0) FROM \"Procedures\"";
            var maxId = (int)(await command.ExecuteScalarAsync() ?? 0);
            
            var newId = new ProcedureId(maxId + 1);
            
            // Use reflection to set the ID since it's private
            var idProperty = procedure.GetType().GetProperty("Id");
            idProperty?.SetValue(procedure, newId);
            
            await connection.CloseAsync();
        }
        
        await _context.Procedures.AddAsync(procedure);
        await _context.SaveChangesAsync();
        return procedure.Id.Value;
    }

    public async Task UpdateAsync(ProcedureBase procedure)
    {
        _context.Procedures.Update(procedure);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(ProcedureBase procedure)
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