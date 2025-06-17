using Microsoft.EntityFrameworkCore;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.Specifications;
using SledzSpecke.Core.ValueObjects;
using SledzSpecke.Application.Abstractions;
using System.Linq;

namespace SledzSpecke.Infrastructure.DAL.Repositories;

/// <summary>
/// Refactored Recognition repository using BaseRepository and Specifications
/// </summary>
internal sealed class RefactoredSqlRecognitionRepository : BaseRepository<Recognition>, IRecognitionRepository
{
    private readonly IUnitOfWork _unitOfWork;

    public RefactoredSqlRecognitionRepository(
        SledzSpeckeDbContext context,
        IUnitOfWork unitOfWork) : base(context)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Recognition?> GetByIdAsync(RecognitionId id)
    {
        var specification = new RecognitionByIdSpecification(id);
        return await GetSingleBySpecificationAsync(specification);
    }

    public async Task<IEnumerable<Recognition>> GetByUserIdAsync(UserId userId)
    {
        var specification = new RecognitionByUserSpecification(userId);
        return await GetBySpecificationAsync(specification, 
            r => r.CreatedAt, false);
    }

    public async Task<IEnumerable<Recognition>> GetBySpecializationIdAsync(SpecializationId specializationId)
    {
        var specification = new RecognitionBySpecializationSpecification(specializationId);
        return await GetBySpecificationAsync(specification,
            r => r.CreatedAt, false);
    }

    public async Task<IEnumerable<Recognition>> GetByUserAndSpecializationAsync(UserId userId, SpecializationId specializationId)
    {
        var specification = RecognitionSpecificationExtensions.GetRecognitionsForUserAndSpecialization(userId, specializationId);
        return await GetBySpecificationAsync(specification,
            r => r.CreatedAt, false);
    }

    public async Task<IEnumerable<Recognition>> GetApprovedRecognitionsAsync(UserId userId, SpecializationId specializationId)
    {
        // Reconstruct the specification to use And() properly
        var specification = new RecognitionByUserSpecification(userId)
            .And(new RecognitionBySpecializationSpecification(specializationId))
            .And(new ApprovedRecognitionSpecification());
        return await GetBySpecificationAsync(specification,
            r => r.ApprovedAt ?? r.CreatedAt, false);
    }

    public async Task<int> GetTotalReductionDaysAsync(UserId userId, SpecializationId specializationId)
    {
        // Reconstruct the specification to use And() properly
        var specification = new RecognitionByUserSpecification(userId)
            .And(new RecognitionBySpecializationSpecification(specializationId))
            .And(new ApprovedRecognitionSpecification());
        var recognitions = await GetBySpecificationAsync(specification);
        
        // Sum in memory since EF Core might not translate custom methods
        return recognitions.Sum(r => r.DaysReduction);
    }

    public async Task<IEnumerable<Recognition>> GetByTypeAsync(RecognitionType type)
    {
        var specification = new RecognitionByTypeSpecification(type);
        return await GetBySpecificationAsync(specification,
            r => r.CreatedAt, false);
    }

    public async Task AddAsync(Recognition recognition)
    {
        // ID generation should be handled by database or a dedicated service
        // For now, keeping the existing logic but moving it to a private method
        if (recognition.Id.Value == Guid.Empty)
        {
            await GenerateIdForEntity(recognition);
        }

        await AddAsync(recognition, default);
        // Note: SaveChangesAsync should be called by Unit of Work, not here
        // But keeping it for backward compatibility
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateAsync(Recognition recognition)
    {
        Update(recognition);
        // Note: SaveChangesAsync should be called by Unit of Work, not here
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(RecognitionId id)
    {
        var recognition = await GetByIdAsync(id);
        if (recognition != null)
        {
            Remove(recognition);
            // Note: SaveChangesAsync should be called by Unit of Work, not here
            await _unitOfWork.SaveChangesAsync();
        }
    }

    // Additional methods using specifications
    public async Task<IEnumerable<Recognition>> GetPendingRecognitionsAsync(UserId userId)
    {
        var specification = RecognitionSpecificationExtensions.GetPendingRecognitionsForUser(userId);
        return await GetBySpecificationAsync(specification,
            r => r.CreatedAt, false);
    }

    public async Task<IEnumerable<Recognition>> GetRecognitionsWithReductionAsync(UserId userId)
    {
        var specification = RecognitionSpecificationExtensions.GetApprovedRecognitionsWithReduction(userId);
        return await GetBySpecificationAsync(specification,
            r => r.DaysReduction, false);
    }

    public async Task<bool> HasOverlappingRecognitionsAsync(UserId userId, DateTime startDate, DateTime endDate, RecognitionId? excludeId = null)
    {
        var specification = RecognitionSpecificationExtensions.GetOverlappingRecognitions(userId, startDate, endDate, excludeId);
        return await CountBySpecificationAsync(specification) > 0;
    }

    public async Task<IEnumerable<Recognition>> GetRecognitionsByTypeAsync(UserId userId, RecognitionType type)
    {
        var specification = RecognitionSpecificationExtensions.GetRecognitionsByTypeForUser(userId, type);
        return await GetBySpecificationAsync(specification);
    }

    public async Task<bool> ExistsAsync(RecognitionId id)
    {
        var specification = new RecognitionByIdSpecification(id);
        return await CountBySpecificationAsync(specification) > 0;
    }

    public async Task<int> GetRecognitionCountForUserAsync(UserId userId, SpecializationId specializationId)
    {
        var specification = RecognitionSpecificationExtensions.GetRecognitionsForUserAndSpecialization(userId, specializationId);
        return await CountBySpecificationAsync(specification);
    }

    public async Task<(IEnumerable<Recognition> Items, int TotalCount)> GetPagedRecognitionsAsync(
        UserId userId, 
        int pageNumber, 
        int pageSize)
    {
        var specification = new RecognitionByUserSpecification(userId);
        return await GetPagedAsync(specification, pageNumber, pageSize, 
            r => r.CreatedAt, false);
    }

    public async Task<IEnumerable<Recognition>> GetRecognitionsRequiringDocumentAsync()
    {
        var specification = RecognitionSpecificationExtensions.GetRecognitionsRequiringDocument();
        return await GetBySpecificationAsync(specification);
    }

    // Private helper methods
    private Task GenerateIdForEntity(Recognition recognition)
    {
        // This should ideally be in a separate ID generation service
        var newId = RecognitionId.New();

        // Use reflection to set the ID since it's private
        var idProperty = recognition.GetType().GetProperty("Id");
        idProperty?.SetValue(recognition, newId);

        return Task.CompletedTask;
    }
}