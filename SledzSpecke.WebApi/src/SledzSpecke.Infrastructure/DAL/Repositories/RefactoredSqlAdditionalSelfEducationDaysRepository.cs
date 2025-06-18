using Microsoft.EntityFrameworkCore;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.Specifications;
using SledzSpecke.Core.ValueObjects;
using SledzSpecke.Application.Abstractions;

namespace SledzSpecke.Infrastructure.DAL.Repositories;

/// <summary>
/// Refactored AdditionalSelfEducationDays repository using BaseRepository and Specifications
/// </summary>
internal sealed class RefactoredSqlAdditionalSelfEducationDaysRepository : BaseRepository<AdditionalSelfEducationDays>, IAdditionalSelfEducationDaysRepository
{
    private readonly IUnitOfWork _unitOfWork;

    public RefactoredSqlAdditionalSelfEducationDaysRepository(
        SledzSpeckeDbContext context,
        IUnitOfWork unitOfWork) : base(context)
    {
        _unitOfWork = unitOfWork;
    }

    public override async Task<AdditionalSelfEducationDays?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var specification = new AdditionalSelfEducationDaysByIdSpecification(id);
        return await GetSingleBySpecificationAsync(specification, cancellationToken);
    }

    public async Task<List<AdditionalSelfEducationDays>> GetByModuleIdAsync(int moduleId, CancellationToken cancellationToken = default)
    {
        var specification = AdditionalSelfEducationDaysSpecificationExtensions.GetDaysForModule(new ModuleId(moduleId));
        var result = await GetBySpecificationAsync(specification, 
            d => d.StartDate, 
            true, 
            cancellationToken);
        return result.ToList();
    }

    public async Task<List<AdditionalSelfEducationDays>> GetByInternshipIdAsync(int internshipId, CancellationToken cancellationToken = default)
    {
        var specification = AdditionalSelfEducationDaysSpecificationExtensions.GetDaysForInternship(new InternshipId(internshipId));
        var result = await GetBySpecificationAsync(specification,
            d => d.StartDate,
            true,
            cancellationToken);
        return result.ToList();
    }

    public async Task<List<AdditionalSelfEducationDays>> GetByYearAsync(int year, CancellationToken cancellationToken = default)
    {
        var specification = new AdditionalSelfEducationDaysInYearSpecification(year);
        var result = await GetBySpecificationAsync(specification,
            d => d.StartDate,
            true,
            cancellationToken);
        return result.ToList();
    }

    public async Task<int> GetTotalDaysInYearAsync(int moduleId, int year, CancellationToken cancellationToken = default)
    {
        // GetDaysInYear already returns the specification with year filter
        // We need to combine it with approved filter
        var specification = new AdditionalSelfEducationDaysByModuleSpecification(new ModuleId(moduleId))
            .And(new AdditionalSelfEducationDaysInYearSpecification(year))
            .And(new ApprovedAdditionalSelfEducationDaysSpecification());
            
        var educationDays = await GetBySpecificationAsync(specification, cancellationToken);

        // Calculate total days, accounting for events that span multiple years
        var totalDays = 0;
        foreach (var edu in educationDays)
        {
            var startDate = edu.StartDate.Year == year ? edu.StartDate : new DateTime(year, 1, 1);
            var endDate = edu.EndDate.Year == year ? edu.EndDate : new DateTime(year, 12, 31);
            
            if (startDate <= endDate)
            {
                totalDays += (endDate - startDate).Days + 1;
            }
        }

        return totalDays;
    }

    public override async Task AddAsync(AdditionalSelfEducationDays additionalSelfEducationDays, CancellationToken cancellationToken = default)
    {
        await base.AddAsync(additionalSelfEducationDays, cancellationToken);
        // Note: SaveChangesAsync should be called by Unit of Work, not here
        // But the interface doesn't return Task, so we need to save here for backward compatibility
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public Task UpdateAsync(AdditionalSelfEducationDays additionalSelfEducationDays, CancellationToken cancellationToken = default)
    {
        Update(additionalSelfEducationDays);
        // Note: SaveChangesAsync should be called by Unit of Work, not here
        return Task.CompletedTask;
    }

    public Task DeleteAsync(AdditionalSelfEducationDays additionalSelfEducationDays, CancellationToken cancellationToken = default)
    {
        Remove(additionalSelfEducationDays);
        // Note: SaveChangesAsync should be called by Unit of Work, not here
        return Task.CompletedTask;
    }

    // Additional methods using specifications
    public async Task<IEnumerable<AdditionalSelfEducationDays>> GetApprovedDaysForModuleAsync(ModuleId moduleId, CancellationToken cancellationToken = default)
    {
        var specification = AdditionalSelfEducationDaysSpecificationExtensions.GetApprovedDaysForModule(moduleId);
        return await GetBySpecificationAsync(specification, cancellationToken: cancellationToken);
    }

    public async Task<IEnumerable<AdditionalSelfEducationDays>> GetPendingDaysForModuleAsync(ModuleId moduleId, CancellationToken cancellationToken = default)
    {
        var specification = AdditionalSelfEducationDaysSpecificationExtensions.GetPendingDaysForModule(moduleId);
        return await GetBySpecificationAsync(specification, cancellationToken: cancellationToken);
    }

    public async Task<bool> HasOverlappingDaysAsync(ModuleId moduleId, DateTime startDate, DateTime endDate, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        var specification = AdditionalSelfEducationDaysSpecificationExtensions.GetOverlappingDays(moduleId, startDate, endDate, excludeId);
        return await CountBySpecificationAsync(specification, cancellationToken) > 0;
    }

    public async Task<int> GetTotalDaysForModuleAsync(ModuleId moduleId, CancellationToken cancellationToken = default)
    {
        var specification = AdditionalSelfEducationDaysSpecificationExtensions.GetApprovedDaysForModule(moduleId);
        var days = await GetBySpecificationAsync(specification, cancellationToken: cancellationToken);
        return days.Sum(d => d.NumberOfDays);
    }

    public async Task<(IEnumerable<AdditionalSelfEducationDays> Items, int TotalCount)> GetPagedDaysAsync(
        ModuleId moduleId, 
        int pageNumber, 
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var specification = AdditionalSelfEducationDaysSpecificationExtensions.GetDaysForModule(moduleId);
        return await GetPagedAsync(specification, pageNumber, pageSize, 
            orderBy: d => d.StartDate, ascending: false, cancellationToken: cancellationToken);
    }
}