using Microsoft.EntityFrameworkCore;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Infrastructure.DAL.Repositories;

internal sealed class EducationalActivityRepository : IEducationalActivityRepository
{
    private readonly SledzSpeckeDbContext _dbContext;

    public EducationalActivityRepository(SledzSpeckeDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<EducationalActivity?> GetByIdAsync(EducationalActivityId id)
        => await _dbContext.EducationalActivities
            .FirstOrDefaultAsync(x => x.Id == id);

    public async Task<IEnumerable<EducationalActivity>> GetBySpecializationIdAsync(SpecializationId specializationId)
        => await _dbContext.EducationalActivities
            .Where(x => x.SpecializationId == specializationId)
            .OrderByDescending(x => x.StartDate)
            .ToListAsync();

    public async Task<IEnumerable<EducationalActivity>> GetByModuleIdAsync(ModuleId moduleId)
        => await _dbContext.EducationalActivities
            .Where(x => x.ModuleId == moduleId)
            .OrderByDescending(x => x.StartDate)
            .ToListAsync();

    public async Task<IEnumerable<EducationalActivity>> GetByTypeAsync(SpecializationId specializationId, EducationalActivityType type)
        => await _dbContext.EducationalActivities
            .Where(x => x.SpecializationId == specializationId && x.Type == type)
            .OrderByDescending(x => x.StartDate)
            .ToListAsync();

    public async Task<IEnumerable<EducationalActivity>> GetByDateRangeAsync(
        SpecializationId specializationId, 
        DateTime startDate, 
        DateTime endDate)
        => await _dbContext.EducationalActivities
            .Where(x => x.SpecializationId == specializationId 
                && x.StartDate >= startDate 
                && x.EndDate <= endDate)
            .OrderByDescending(x => x.StartDate)
            .ToListAsync();

    public async Task AddAsync(EducationalActivity activity)
    {
        await _dbContext.EducationalActivities.AddAsync(activity);
    }

    public Task UpdateAsync(EducationalActivity activity)
    {
        _dbContext.EducationalActivities.Update(activity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(EducationalActivity activity)
    {
        _dbContext.EducationalActivities.Remove(activity);
        return Task.CompletedTask;
    }
}