using Microsoft.EntityFrameworkCore;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;
using SledzSpecke.Infrastructure.DAL.Specifications.EducationalActivity;

namespace SledzSpecke.Infrastructure.DAL.Repositories;

/// <summary>
/// Refactored repository for EducationalActivity using BaseRepository and Specification pattern
/// </summary>
internal sealed class RefactoredEducationalActivityRepository : BaseRepository<EducationalActivity>, IEducationalActivityRepository
{
    public RefactoredEducationalActivityRepository(SledzSpeckeDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<EducationalActivity?> GetByIdAsync(EducationalActivityId id)
    {
        var spec = new EducationalActivityByIdSpecification(id);
        return await GetSingleBySpecificationAsync(spec);
    }

    public async Task<IEnumerable<EducationalActivity>> GetBySpecializationIdAsync(SpecializationId specializationId)
    {
        var spec = new EducationalActivityBySpecializationSpecification(specializationId);
        return await GetBySpecificationAsync(spec, a => a.StartDate, ascending: false);
    }

    public async Task<IEnumerable<EducationalActivity>> GetByModuleIdAsync(ModuleId moduleId)
    {
        var spec = new EducationalActivityByModuleSpecification(moduleId);
        return await GetBySpecificationAsync(spec, a => a.StartDate, ascending: false);
    }

    public async Task<IEnumerable<EducationalActivity>> GetByTypeAsync(SpecializationId specializationId, EducationalActivityType type)
    {
        var spec = new EducationalActivityByTypeSpecification(specializationId, type);
        return await GetBySpecificationAsync(spec, a => a.StartDate, ascending: false);
    }

    public async Task<IEnumerable<EducationalActivity>> GetByDateRangeAsync(
        SpecializationId specializationId, 
        DateTime startDate, 
        DateTime endDate)
    {
        var spec = new EducationalActivityByDateRangeSpecification(specializationId, startDate, endDate);
        return await GetBySpecificationAsync(spec, a => a.StartDate, ascending: false);
    }

    public async Task AddAsync(EducationalActivity activity)
    {
        await base.AddAsync(activity);
    }

    public Task UpdateAsync(EducationalActivity activity)
    {
        base.Update(activity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(EducationalActivity activity)
    {
        base.Remove(activity);
        return Task.CompletedTask;
    }
}