using Microsoft.EntityFrameworkCore;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.Specifications;
using SledzSpecke.Core.ValueObjects;
using SledzSpecke.Application.Abstractions;

namespace SledzSpecke.Infrastructure.DAL.Repositories;

/// <summary>
/// Refactored Absence repository using BaseRepository and Specifications
/// </summary>
internal sealed class RefactoredSqlAbsenceRepository : BaseRepository<Absence>, IAbsenceRepository
{
    private readonly IUnitOfWork _unitOfWork;

    public RefactoredSqlAbsenceRepository(
        SledzSpeckeDbContext context,
        IUnitOfWork unitOfWork) : base(context)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Absence?> GetByIdAsync(AbsenceId id)
    {
        var specification = new AbsenceByIdSpecification(id);
        return await GetSingleBySpecificationAsync(specification);
    }

    public async Task<IEnumerable<Absence>> GetByUserIdAsync(UserId userId)
    {
        var specification = new AbsenceByUserSpecification(userId);
        return await GetBySpecificationAsync(specification);
    }

    public async Task<IEnumerable<Absence>> GetBySpecializationIdAsync(SpecializationId specializationId)
    {
        var specification = new AbsenceBySpecializationSpecification(specializationId);
        return await GetBySpecificationAsync(specification);
    }

    public async Task<IEnumerable<Absence>> GetByUserAndSpecializationAsync(UserId userId, SpecializationId specializationId)
    {
        var specification = new AbsenceByUserSpecification(userId)
            .And(new AbsenceBySpecializationSpecification(specializationId));
        return await GetBySpecificationAsync(specification);
    }

    public async Task<IEnumerable<Absence>> GetActiveAbsencesAsync(UserId userId)
    {
        var specification = AbsenceSpecificationExtensions.GetActiveAbsencesForUser(userId);
        return await GetBySpecificationAsync(specification);
    }

    public async Task<IEnumerable<Absence>> GetOverlappingAbsencesAsync(UserId userId, DateTime startDate, DateTime endDate)
    {
        var specification = AbsenceSpecificationExtensions.GetOverlappingAbsencesForUser(userId, startDate, endDate);
        return await GetBySpecificationAsync(specification);
    }

    public async Task<bool> HasOverlappingAbsencesAsync(UserId userId, DateTime startDate, DateTime endDate, AbsenceId? excludeId = null)
    {
        var specification = AbsenceSpecificationExtensions.GetOverlappingAbsencesForUser(userId, startDate, endDate, excludeId);
        return await CountBySpecificationAsync(specification) > 0;
    }

    public async Task AddAsync(Absence absence)
    {
        // ID generation should be handled by database or a dedicated service
        // For now, keeping the existing logic but moving it to a private method
        if (absence.Id.Value == Guid.Empty)
        {
            await GenerateIdForEntity(absence);
        }

        await AddAsync(absence, default);
        // Note: SaveChangesAsync should be called by Unit of Work, not here
        // But keeping it for backward compatibility
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateAsync(Absence absence)
    {
        Update(absence);
        // Note: SaveChangesAsync should be called by Unit of Work, not here
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(AbsenceId id)
    {
        var absence = await GetByIdAsync(id);
        if (absence != null)
        {
            Remove(absence);
            // Note: SaveChangesAsync should be called by Unit of Work, not here
            await _unitOfWork.SaveChangesAsync();
        }
    }

    // Additional methods using specifications
    public async Task<IEnumerable<Absence>> GetPendingAbsencesAsync(UserId userId)
    {
        var specification = new AbsenceByUserSpecification(userId)
            .And(AbsenceSpecificationExtensions.GetPendingAbsences());
        return await GetBySpecificationAsync(specification);
    }

    public async Task<IEnumerable<Absence>> GetAbsencesInDateRangeAsync(UserId userId, DateTime startDate, DateTime endDate)
    {
        var specification = new AbsenceByUserSpecification(userId)
            .And(AbsenceSpecificationExtensions.GetAbsencesInDateRange(startDate, endDate));
        return await GetBySpecificationAsync(specification);
    }

    public async Task<IEnumerable<Absence>> GetByDateRangeAsync(UserId userId, DateTime startDate, DateTime endDate)
    {
        // This method satisfies the IAbsenceRepository interface requirement
        return await GetAbsencesInDateRangeAsync(userId, startDate, endDate);
    }

    public async Task<IEnumerable<Absence>> GetByInternshipIdAsync(InternshipId internshipId)
    {
        // Since absences are not directly linked to internships, we need to:
        // 1. Get the internship details to find the user and date range
        // 2. Then get absences for that user within the internship period
        // This requires InternshipRepository access, which should be done at service level
        // For now, return empty collection - the actual logic should be in the service layer
        return Enumerable.Empty<Absence>();
    }

    public async Task<IEnumerable<Absence>> GetAbsencesByTypeAsync(UserId userId, AbsenceType type)
    {
        var specification = new AbsenceByUserSpecification(userId)
            .And(new AbsenceByTypeSpecification(type));
        return await GetBySpecificationAsync(specification);
    }

    public async Task<bool> ExistsAsync(AbsenceId id)
    {
        var specification = new AbsenceByIdSpecification(id);
        return await CountBySpecificationAsync(specification) > 0;
    }

    public async Task<int> GetAbsenceCountForUserAsync(UserId userId, SpecializationId specializationId)
    {
        var specification = new AbsenceByUserSpecification(userId)
            .And(new AbsenceBySpecializationSpecification(specializationId));
        return await CountBySpecificationAsync(specification);
    }

    public async Task<(IEnumerable<Absence> Items, int TotalCount)> GetPagedAbsencesAsync(
        UserId userId, 
        int pageNumber, 
        int pageSize)
    {
        var specification = new AbsenceByUserSpecification(userId);
        return await GetPagedAsync(specification, pageNumber, pageSize, 
            orderBy: a => a.StartDate, ascending: false);
    }

    // Private helper methods
    private Task GenerateIdForEntity(Absence absence)
    {
        // This should ideally be in a separate ID generation service
        var newId = AbsenceId.New();

        // Use reflection to set the ID since it's private
        var idProperty = absence.GetType().GetProperty("Id");
        idProperty?.SetValue(absence, newId);

        return Task.CompletedTask;
    }
}