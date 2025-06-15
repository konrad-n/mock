using Microsoft.EntityFrameworkCore;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Infrastructure.DAL.Repositories;

/// <summary>
/// Enhanced version of SqlInternshipRepository that follows Unit of Work pattern
/// by NOT calling SaveChangesAsync directly - that's the responsibility of UnitOfWork
/// </summary>
internal sealed class SqlInternshipRepositoryEnhanced : IInternshipRepository
{
    private readonly SledzSpeckeDbContext _context;
    private readonly DbSet<Internship> _internships;

    public SqlInternshipRepositoryEnhanced(SledzSpeckeDbContext context)
    {
        _context = context;
        _internships = context.Internships;
    }

    public async Task<Internship?> GetByIdAsync(InternshipId id)
        => await _internships
            .Include(i => i.MedicalShifts)
            .Include(i => i.Procedures)
            .SingleOrDefaultAsync(i => i.InternshipId == id);

    public async Task<Internship?> GetByIdWithShiftsAsync(InternshipId id)
        => await _internships
            .Include(i => i.MedicalShifts)
            .SingleOrDefaultAsync(i => i.InternshipId == id);

    public async Task<Internship?> GetByIdWithProceduresAsync(InternshipId id)
        => await _internships
            .Include(i => i.Procedures)
            .SingleOrDefaultAsync(i => i.InternshipId == id);

    public async Task<IEnumerable<Internship>> GetBySpecializationAsync(SpecializationId specializationId)
        => await _internships
            .Where(i => i.SpecializationId == specializationId)
            .Include(i => i.MedicalShifts)
            .Include(i => i.Procedures)
            .ToListAsync();

    public async Task<IEnumerable<Internship>> GetBySpecializationIdAsync(SpecializationId specializationId)
        => await GetBySpecializationAsync(specializationId);

    public async Task<IEnumerable<Internship>> GetByModuleIdAsync(ModuleId moduleId)
        => await GetByModuleAsync(moduleId);

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
        // Generate new ID if it's 0
        if (internship.InternshipId.Value == 0)
        {
            // Query raw database to get max ID
            var connection = _context.Database.GetDbConnection();
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT COALESCE(MAX(\"Id\"), 0) FROM \"Internships\"";
            var maxId = (int)(await command.ExecuteScalarAsync() ?? 0);

            var newId = new InternshipId(maxId + 1);

            // Use reflection to set the ID since it's private
            var idProperty = internship.GetType().GetProperty("InternshipId");
            idProperty?.SetValue(internship, newId);

            await connection.CloseAsync();
        }

        await _internships.AddAsync(internship);
        // NO SaveChangesAsync - let UnitOfWork handle it
    }

    public Task UpdateAsync(Internship internship)
    {
        _internships.Update(internship);
        // NO SaveChangesAsync - let UnitOfWork handle it
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(InternshipId id)
    {
        var internship = await _internships.FindAsync(id.Value);
        if (internship is not null)
        {
            _internships.Remove(internship);
            // NO SaveChangesAsync - let UnitOfWork handle it
        }
    }
}