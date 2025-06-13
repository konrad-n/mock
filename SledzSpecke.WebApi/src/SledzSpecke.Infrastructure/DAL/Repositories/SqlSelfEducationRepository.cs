using Microsoft.EntityFrameworkCore;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Infrastructure.DAL.Repositories;

internal sealed class SqlSelfEducationRepository : ISelfEducationRepository
{
    private readonly SledzSpeckeDbContext _context;
    private readonly DbSet<SelfEducation> _selfEducations;

    public SqlSelfEducationRepository(SledzSpeckeDbContext context)
    {
        _context = context;
        _selfEducations = context.SelfEducations;
    }

    public async Task<SelfEducation?> GetByIdAsync(SelfEducationId id)
        => await _selfEducations.SingleOrDefaultAsync(se => se.Id == id);

    public async Task<IEnumerable<SelfEducation>> GetByUserIdAsync(UserId userId)
        => await _selfEducations
            .Where(se => se.UserId == userId)
            .OrderByDescending(se => se.Year)
            .ThenByDescending(se => se.CreatedAt)
            .ToListAsync();

    public async Task<IEnumerable<SelfEducation>> GetBySpecializationIdAsync(SpecializationId specializationId)
        => await _selfEducations
            .Where(se => se.SpecializationId == specializationId)
            .OrderByDescending(se => se.Year)
            .ThenByDescending(se => se.CreatedAt)
            .ToListAsync();

    public async Task<IEnumerable<SelfEducation>> GetByUserAndSpecializationAsync(UserId userId, SpecializationId specializationId)
        => await _selfEducations
            .Where(se => se.UserId == userId && se.SpecializationId == specializationId)
            .OrderByDescending(se => se.Year)
            .ThenByDescending(se => se.CreatedAt)
            .ToListAsync();

    public async Task<IEnumerable<SelfEducation>> GetByYearAsync(UserId userId, int year)
        => await _selfEducations
            .Where(se => se.UserId == userId && se.Year == year)
            .OrderByDescending(se => se.CreatedAt)
            .ToListAsync();

    public async Task<IEnumerable<SelfEducation>> GetByTypeAsync(SelfEducationType type)
        => await _selfEducations
            .Where(se => se.Type == type)
            .OrderByDescending(se => se.Year)
            .ThenByDescending(se => se.CreatedAt)
            .ToListAsync();

    public async Task<IEnumerable<SelfEducation>> GetCompletedActivitiesAsync(UserId userId, SpecializationId specializationId)
        => await _selfEducations
            .Where(se => se.UserId == userId && se.SpecializationId == specializationId && se.IsCompleted)
            .OrderByDescending(se => se.CompletedAt)
            .ToListAsync();

    public async Task<int> GetTotalCreditHoursAsync(UserId userId, SpecializationId specializationId)
        => await _selfEducations
            .Where(se => se.UserId == userId && se.SpecializationId == specializationId && se.IsCompleted)
            .SumAsync(se => se.CreditHours);

    public async Task<int> GetTotalQualityScoreAsync(UserId userId, SpecializationId specializationId)
        => await _selfEducations
            .Where(se => se.UserId == userId && se.SpecializationId == specializationId)
            .SumAsync(se => se.CalculateQualityScore());

    public async Task<IEnumerable<SelfEducation>> GetActivitiesWithCertificatesAsync(UserId userId)
        => await _selfEducations
            .Where(se => se.UserId == userId && se.HasCertificate)
            .OrderByDescending(se => se.CompletedAt)
            .ToListAsync();

    public async Task AddAsync(SelfEducation selfEducation)
    {
        await _selfEducations.AddAsync(selfEducation);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(SelfEducation selfEducation)
    {
        _selfEducations.Update(selfEducation);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(SelfEducationId id)
    {
        var selfEducation = await _selfEducations.FindAsync(id.Value);
        if (selfEducation is not null)
        {
            _selfEducations.Remove(selfEducation);
            await _context.SaveChangesAsync();
        }
    }
}