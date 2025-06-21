using Microsoft.EntityFrameworkCore;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.Specifications;
using SledzSpecke.Core.ValueObjects;
using SledzSpecke.Application.Abstractions;

namespace SledzSpecke.Infrastructure.DAL.Repositories;

/// <summary>
/// Refactored SelfEducation repository using BaseRepository and Specifications
/// </summary>
internal sealed class RefactoredSqlSelfEducationRepository : BaseRepository<SelfEducation>, ISelfEducationRepository
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly SledzSpeckeDbContext _context;

    public RefactoredSqlSelfEducationRepository(
        SledzSpeckeDbContext context,
        IUnitOfWork unitOfWork) : base(context)
    {
        _context = context;
        _unitOfWork = unitOfWork;
    }

    public async Task<SelfEducation?> GetByIdAsync(SelfEducationId id)
    {
        var specification = new SelfEducationByIdSpecification(id);
        return await GetSingleBySpecificationAsync(specification);
    }

    public async Task<IEnumerable<SelfEducation>> GetByUserIdAsync(UserId userId)
    {
        // Join through Module to get user's self educations
        var selfEducations = await _context.SelfEducations
            .Include(se => se.Module)
            .Join(_context.Specializations,
                se => se.Module.SpecializationId,
                s => s.SpecializationId,
                (se, s) => new { SelfEducation = se, Specialization = s })
            .Where(x => x.Specialization.UserId == userId)
            .Select(x => x.SelfEducation)
            .OrderByDescending(se => se.Date)
            .ToListAsync();

        return selfEducations;
    }

    public async Task<IEnumerable<SelfEducation>> GetBySpecializationIdAsync(SpecializationId specializationId)
    {
        // Need to join through Module -> Specialization
        var selfEducations = await _context.SelfEducations
            .Include(se => se.Module)
            .Where(se => se.Module.SpecializationId == specializationId)
            .OrderByDescending(se => se.Date)
            .ToListAsync();

        return selfEducations;
    }

    public async Task<IEnumerable<SelfEducation>> GetByUserAndSpecializationAsync(UserId userId, SpecializationId specializationId)
    {
        // Since SelfEducation is linked through Module->Specialization->User, we need to use a query
        // We can't use simple specifications here due to the complex relationship
        var selfEducations = await _context.SelfEducations
            .Include(se => se.Module)
            .Where(se => se.Module.SpecializationId == specializationId)
            .Join(_context.Specializations,
                se => se.Module.SpecializationId,
                s => s.SpecializationId,
                (se, s) => new { SelfEducation = se, Specialization = s })
            .Where(x => x.Specialization.SpecializationId == specializationId && x.Specialization.UserId == userId)
            .Select(x => x.SelfEducation)
            .OrderByDescending(se => se.Date)
            .ToListAsync();
            
        return selfEducations;
    }

    public async Task<IEnumerable<SelfEducation>> GetByYearAsync(UserId userId, int year)
    {
        // Join through Module and Specialization to get user's self educations by year
        var selfEducations = await _context.SelfEducations
            .Include(se => se.Module)
            .Join(_context.Specializations,
                se => se.Module.SpecializationId,
                s => s.SpecializationId,
                (se, s) => new { SelfEducation = se, Specialization = s })
            .Where(x => x.Specialization.UserId == userId && x.SelfEducation.Date.Year == year)
            .Select(x => x.SelfEducation)
            .OrderByDescending(se => se.Date)
            .ToListAsync();

        return selfEducations;
    }

    public async Task<IEnumerable<SelfEducation>> GetByTypeAsync(SelfEducationType type)
    {
        var specification = new SelfEducationByTypeSpecification(type);
        return await GetBySpecificationAsync(specification,
            se => se.Date, false);
    }

    public async Task<IEnumerable<SelfEducation>> GetCompletedActivitiesAsync(UserId userId, SpecializationId specializationId)
    {
        // In the new model, all recorded activities are considered complete
        return await GetByUserAndSpecializationAsync(userId, specializationId);
    }

    public async Task<int> GetTotalCreditHoursAsync(UserId userId, SpecializationId specializationId)
    {
        var selfEducations = await GetByUserAndSpecializationAsync(userId, specializationId);
        return selfEducations.Sum(se => se.Hours);
    }

    public async Task<int> GetTotalQualityScoreAsync(UserId userId, SpecializationId specializationId)
    {
        var selfEducations = await GetByUserAndSpecializationAsync(userId, specializationId);
        return selfEducations.Sum(se => se.GetEducationPoints());
    }

    public async Task<IEnumerable<SelfEducation>> GetActivitiesWithCertificatesAsync(UserId userId)
    {
        // The new model doesn't track certificates
        // Return activities that might have certificates (workshops, conferences)
        var selfEducations = await _context.SelfEducations
            .Include(se => se.Module)
            .Join(_context.Specializations,
                se => se.Module.SpecializationId,
                s => s.SpecializationId,
                (se, s) => new { SelfEducation = se, Specialization = s })
            .Where(x => x.Specialization.UserId == userId && 
                        (x.SelfEducation.Type == SelfEducationType.Workshop || 
                         x.SelfEducation.Type == SelfEducationType.Conference))
            .Select(x => x.SelfEducation)
            .OrderByDescending(se => se.Date)
            .ToListAsync();

        return selfEducations;
    }

    public async Task AddAsync(SelfEducation selfEducation)
    {
        // ID generation should be handled by database or a dedicated service
        // For now, keeping the existing logic but moving it to a private method
        if (selfEducation.Id.Value == Guid.Empty)
        {
            await GenerateIdForEntity(selfEducation);
        }

        await AddAsync(selfEducation, default);
        // Note: SaveChangesAsync should be called by Unit of Work, not here
        // But keeping it for backward compatibility
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateAsync(SelfEducation selfEducation)
    {
        Update(selfEducation);
        // Note: SaveChangesAsync should be called by Unit of Work, not here
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(SelfEducationId id)
    {
        var selfEducation = await GetByIdAsync(id);
        if (selfEducation != null)
        {
            Remove(selfEducation);
            // Note: SaveChangesAsync should be called by Unit of Work, not here
            await _unitOfWork.SaveChangesAsync();
        }
    }

    // Additional methods using specifications
    public async Task<IEnumerable<SelfEducation>> GetByModuleAsync(ModuleId moduleId)
    {
        var specification = SelfEducationSpecificationExtensions.GetSelfEducationForModule(moduleId);
        return await GetBySpecificationAsync(specification,
            se => se.Date, false);
    }

    public async Task<IEnumerable<SelfEducation>> GetPublicationsForModuleAsync(ModuleId moduleId)
    {
        var specification = SelfEducationSpecificationExtensions.GetPublicationsForModule(moduleId);
        return await GetBySpecificationAsync(specification,
            se => se.Date, false);
    }

    public async Task<IEnumerable<SelfEducation>> GetPeerReviewedPublicationsAsync(ModuleId moduleId)
    {
        var specification = SelfEducationSpecificationExtensions.GetPeerReviewedPublicationsForModule(moduleId);
        return await GetBySpecificationAsync(specification);
    }

    public async Task<IEnumerable<SelfEducation>> GetSelfEducationInDateRangeAsync(ModuleId moduleId, DateTime startDate, DateTime endDate)
    {
        var specification = SelfEducationSpecificationExtensions.GetSelfEducationInDateRange(moduleId, startDate, endDate);
        return await GetBySpecificationAsync(specification);
    }

    public async Task<int> GetTotalHoursForModuleAsync(ModuleId moduleId)
    {
        var specification = SelfEducationSpecificationExtensions.GetSelfEducationForModule(moduleId);
        var activities = await GetBySpecificationAsync(specification);
        return activities.Sum(se => se.Hours);
    }

    public async Task<int> GetTotalPointsForModuleAsync(ModuleId moduleId)
    {
        var specification = SelfEducationSpecificationExtensions.GetSelfEducationForModule(moduleId);
        var activities = await GetBySpecificationAsync(specification);
        return activities.Sum(se => se.GetEducationPoints());
    }

    public async Task<bool> ExistsAsync(SelfEducationId id)
    {
        var specification = new SelfEducationByIdSpecification(id);
        return await CountBySpecificationAsync(specification) > 0;
    }

    public async Task<(IEnumerable<SelfEducation> Items, int TotalCount)> GetPagedSelfEducationAsync(
        ModuleId moduleId, 
        int pageNumber, 
        int pageSize)
    {
        var specification = SelfEducationSpecificationExtensions.GetSelfEducationForModule(moduleId);
        return await GetPagedAsync(specification, pageNumber, pageSize, 
            se => se.Date, false);
    }

    // Private helper methods
    private Task GenerateIdForEntity(SelfEducation selfEducation)
    {
        // This should ideally be in a separate ID generation service
        var newId = SelfEducationId.New();

        // Use reflection to set the ID since it's private
        var idProperty = selfEducation.GetType().GetProperty("Id");
        idProperty?.SetValue(selfEducation, newId);

        return Task.CompletedTask;
    }
}