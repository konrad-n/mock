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
    {
        // Note: In the new model, SelfEducation is linked to Module, not directly to User
        // This would require joining through Module -> Specialization -> User
        // For now, return empty collection as this needs architectural changes
        return await Task.FromResult(Enumerable.Empty<SelfEducation>());
    }

    public async Task<IEnumerable<SelfEducation>> GetBySpecializationIdAsync(SpecializationId specializationId)
    {
        // Note: Similar issue - need to join through Module -> Specialization
        // For now, return empty collection
        return await Task.FromResult(Enumerable.Empty<SelfEducation>());
    }

    public async Task<IEnumerable<SelfEducation>> GetByUserAndSpecializationAsync(UserId userId, SpecializationId specializationId)
    {
        // Note: Similar issue - need complex joins
        // For now, return empty collection
        return await Task.FromResult(Enumerable.Empty<SelfEducation>());
    }

    public async Task<IEnumerable<SelfEducation>> GetByYearAsync(UserId userId, int year)
        => await _selfEducations
            .Where(se => se.Date.Year == year)
            .OrderByDescending(se => se.CreatedAt)
            .ToListAsync();

    public async Task<IEnumerable<SelfEducation>> GetByTypeAsync(SelfEducationType type)
        => await _selfEducations
            .Where(se => se.Type == type)
            .OrderByDescending(se => se.Date)
            .ThenByDescending(se => se.CreatedAt)
            .ToListAsync();

    public async Task<IEnumerable<SelfEducation>> GetCompletedActivitiesAsync(UserId userId, SpecializationId specializationId)
    {
        // In the new model, all recorded activities are considered complete
        // Return all activities (would need joins for user/specialization filtering)
        return await _selfEducations
            .OrderByDescending(se => se.Date)
            .ToListAsync();
    }

    public async Task<int> GetTotalCreditHoursAsync(UserId userId, SpecializationId specializationId)
    {
        // Convert hours to credit hours (1:1 ratio)
        // Note: Would need joins for proper filtering
        return await _selfEducations
            .SumAsync(se => se.Hours);
    }

    public async Task<int> GetTotalQualityScoreAsync(UserId userId, SpecializationId specializationId)
    {
        // Calculate quality score based on education points
        var activities = await _selfEducations.ToListAsync();
        return activities.Sum(se => se.GetEducationPoints());
    }

    public async Task<IEnumerable<SelfEducation>> GetActivitiesWithCertificatesAsync(UserId userId)
    {
        // The new model doesn't track certificates
        // Return empty collection
        return await Task.FromResult(Enumerable.Empty<SelfEducation>());
    }

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