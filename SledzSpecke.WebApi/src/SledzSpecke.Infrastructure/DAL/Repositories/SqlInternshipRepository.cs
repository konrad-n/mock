using Microsoft.EntityFrameworkCore;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Infrastructure.DAL.Repositories;

internal sealed class SqlInternshipRepository : IInternshipRepository
{
    private readonly SledzSpeckeDbContext _context;
    private readonly DbSet<Internship> _internships;

    public SqlInternshipRepository(SledzSpeckeDbContext context)
    {
        _context = context;
        _internships = context.Internships;
    }

    public async Task<Internship?> GetByIdAsync(InternshipId id)
        => await _internships
            .Include(i => i.MedicalShifts)
            .Include(i => i.Procedures)
            .SingleOrDefaultAsync(i => i.Id == id);

    public async Task<IEnumerable<Internship>> GetBySpecializationIdAsync(SpecializationId specializationId)
        => await _internships
            .Where(i => i.SpecializationId == specializationId)
            .Include(i => i.MedicalShifts)
            .Include(i => i.Procedures)
            .ToListAsync();

    public async Task<IEnumerable<Internship>> GetByModuleIdAsync(ModuleId moduleId)
        => await _internships
            .Where(i => i.ModuleId == moduleId)
            .Include(i => i.MedicalShifts)
            .Include(i => i.Procedures)
            .ToListAsync();

    public async Task<IEnumerable<Internship>> GetByUserAndSpecializationAsync(UserId userId, SpecializationId specializationId)
        => await _internships
            .Where(i => i.SpecializationId == specializationId)
            .Include(i => i.MedicalShifts)
            .Include(i => i.Procedures)
            .ToListAsync();

    public async Task<IEnumerable<Internship>> GetByModuleAsync(ModuleId moduleId)
        => await _internships
            .Where(i => i.ModuleId == moduleId)
            .Include(i => i.MedicalShifts)
            .Include(i => i.Procedures)
            .ToListAsync();

    public async Task<IEnumerable<Internship>> GetPendingApprovalAsync()
        => await _internships
            .Where(i => i.IsCompleted && !i.IsApproved)
            .Include(i => i.MedicalShifts)
            .Include(i => i.Procedures)
            .ToListAsync();

    public async Task AddAsync(Internship internship)
    {
        await _internships.AddAsync(internship);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Internship internship)
    {
        _internships.Update(internship);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(InternshipId id)
    {
        var internship = await _internships.FindAsync(id.Value);
        if (internship is not null)
        {
            _internships.Remove(internship);
            await _context.SaveChangesAsync();
        }
    }
}