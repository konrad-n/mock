using Microsoft.EntityFrameworkCore;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Infrastructure.DAL.Repositories;

internal sealed class SqlSpecializationRepository : ISpecializationRepository
{
    private readonly SledzSpeckeDbContext _context;

    public SqlSpecializationRepository(SledzSpeckeDbContext context)
    {
        _context = context;
    }

    public async Task<Specialization?> GetByIdAsync(SpecializationId id)
    {
        return await _context.Specializations
            .Include(s => s.Modules)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<IEnumerable<Specialization>> GetByUserIdAsync(UserId userId)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
            return Enumerable.Empty<Specialization>();

        return await _context.Specializations
            .Include(s => s.Modules)
            .Where(s => s.Id == user.SpecializationId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Specialization>> GetAllAsync()
    {
        return await _context.Specializations
            .Include(s => s.Modules)
            .ToListAsync();
    }

    public async Task<SpecializationId> AddAsync(Specialization specialization)
    {
        await _context.Specializations.AddAsync(specialization);
        await _context.SaveChangesAsync();
        return specialization.Id;
    }

    public async Task UpdateAsync(Specialization specialization)
    {
        _context.Specializations.Update(specialization);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(SpecializationId id)
    {
        var specialization = await GetByIdAsync(id);
        if (specialization != null)
        {
            _context.Specializations.Remove(specialization);
            await _context.SaveChangesAsync();
        }
    }
}