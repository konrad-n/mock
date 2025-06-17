using Microsoft.EntityFrameworkCore;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.Specifications;
using SledzSpecke.Core.ValueObjects;
using SledzSpecke.Application.Abstractions;

namespace SledzSpecke.Infrastructure.DAL.Repositories;

/// <summary>
/// Refactored Specialization repository using BaseRepository and Specifications
/// </summary>
internal sealed class RefactoredSqlSpecializationRepository : BaseRepository<Specialization>, ISpecializationRepository
{
    private readonly IUnitOfWork _unitOfWork;

    public RefactoredSqlSpecializationRepository(
        SledzSpeckeDbContext context,
        IUnitOfWork unitOfWork) : base(context)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Specialization?> GetByIdAsync(SpecializationId id)
    {
        // IMPORTANT: Do NOT use .Include(s => s.Modules) here!
        // The Modules property is explicitly ignored in SpecializationConfiguration
        // Using Include will cause: "The expression 's.Modules' is invalid inside an 'Include' operation"
        // The Modules collection is handled differently in the domain model
        var specification = new SpecializationByIdSpecification(id);
        return await GetSingleBySpecificationAsync(specification);
    }

    public async Task<IEnumerable<Specialization>> GetByUserIdAsync(UserId userId)
    {
        var specification = new SpecializationByUserSpecification(userId);
        return await GetBySpecificationAsync(specification);
    }

    public async Task<IEnumerable<Specialization>> GetAllAsync()
    {
        return await GetAllAsync(default);
    }

    public async Task<SpecializationId> AddAsync(Specialization specialization)
    {
        // ID generation should be handled by database or a dedicated service
        // For now, keeping the existing logic but moving it to a private method
        if (specialization.Id.Value <= 0)
        {
            await GenerateIdForEntity(specialization);
        }

        await AddAsync(specialization, default);
        // Note: SaveChangesAsync should be called by Unit of Work, not here
        // But keeping it for backward compatibility
        await _unitOfWork.SaveChangesAsync();
        return specialization.Id;
    }

    public async Task UpdateAsync(Specialization specialization)
    {
        Update(specialization);
        // Note: SaveChangesAsync should be called by Unit of Work, not here
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(SpecializationId id)
    {
        var specialization = await GetByIdAsync(id);
        if (specialization != null)
        {
            Remove(specialization);
            // Note: SaveChangesAsync should be called by Unit of Work, not here
            await _unitOfWork.SaveChangesAsync();
        }
    }

    // Additional methods using specifications
    public async Task<bool> UserHasSpecializationAsync(UserId userId, SpecializationId specializationId)
    {
        var specification = SpecializationSpecificationExtensions.GetSpecializationForUser(specializationId, userId);
        return await CountBySpecificationAsync(specification) > 0;
    }

    public async Task<bool> ExistsAsync(SpecializationId id)
    {
        var specification = new SpecializationByIdSpecification(id);
        return await CountBySpecificationAsync(specification) > 0;
    }

    public async Task<bool> HasActiveSpecializationAsync(UserId userId)
    {
        var specification = SpecializationSpecificationExtensions.GetActiveSpecializations(userId);
        return await CountBySpecificationAsync(specification) > 0;
    }

    public async Task<int> GetSpecializationCountForUserAsync(UserId userId)
    {
        var specification = new SpecializationByUserSpecification(userId);
        return await CountBySpecificationAsync(specification);
    }

    public async Task<(IEnumerable<Specialization> Items, int TotalCount)> GetPagedSpecializationsAsync(
        UserId userId, 
        int pageNumber, 
        int pageSize)
    {
        var specification = new SpecializationByUserSpecification(userId);
        return await GetPagedAsync(specification, pageNumber, pageSize, 
            orderBy: s => s.StartDate, ascending: false);
    }

    // Private helper methods
    private async Task GenerateIdForEntity(Specialization specialization)
    {
        // This should ideally be in a separate ID generation service
        var connection = Context.Database.GetDbConnection();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = "SELECT COALESCE(MAX(\"Id\"), 0) FROM \"Specializations\"";
        var maxId = (int)(await command.ExecuteScalarAsync() ?? 0);

        var newId = new SpecializationId(maxId + 1);

        // Use reflection to set the ID since it's private
        var idProperty = specialization.GetType().GetProperty("Id");
        idProperty?.SetValue(specialization, newId);

        await connection.CloseAsync();
    }
}