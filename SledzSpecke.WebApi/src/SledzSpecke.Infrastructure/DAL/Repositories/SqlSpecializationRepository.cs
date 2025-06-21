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
        // IMPORTANT: Do NOT use .Include(s => s.Modules) here!
        // The Modules property is explicitly ignored in SpecializationConfiguration
        // Using Include will cause: "The expression 's.Modules' is invalid inside an 'Include' operation"
        // The Modules collection is handled differently in the domain model
        return await _context.Specializations
            .FirstOrDefaultAsync(s => s.SpecializationId == id.Value);
    }

    public async Task<IEnumerable<Specialization>> GetByUserIdAsync(UserId userId)
    {
        return await _context.Specializations
            .Where(s => s.UserId == userId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Specialization>> GetAllAsync()
    {
        return await _context.Specializations
            .ToListAsync();
    }

    public async Task<SpecializationId> AddAsync(Specialization specialization)
    {
        await _context.Specializations.AddAsync(specialization);
        await _context.SaveChangesAsync();
        return new SpecializationId(specialization.SpecializationId);
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