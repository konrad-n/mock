using Microsoft.EntityFrameworkCore;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.Specifications;
using SledzSpecke.Core.ValueObjects;
using SledzSpecke.Application.Abstractions;

namespace SledzSpecke.Infrastructure.DAL.Repositories;

/// <summary>
/// Refactored Publication repository using BaseRepository and Specifications
/// </summary>
internal sealed class RefactoredSqlPublicationRepository : BaseRepository<Publication>, IPublicationRepository
{
    private readonly IUnitOfWork _unitOfWork;

    public RefactoredSqlPublicationRepository(
        SledzSpeckeDbContext context,
        IUnitOfWork unitOfWork) : base(context)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Publication?> GetByIdAsync(PublicationId id)
    {
        var specification = new PublicationByIdSpecification(id);
        return await GetSingleBySpecificationAsync(specification);
    }

    public async Task<IEnumerable<Publication>> GetByUserIdAsync(UserId userId)
    {
        var specification = new PublicationByUserSpecification(userId);
        return await GetBySpecificationAsync(specification);
    }

    public async Task<IEnumerable<Publication>> GetBySpecializationIdAsync(SpecializationId specializationId)
    {
        var specification = new PublicationBySpecializationSpecification(specializationId);
        return await GetBySpecificationAsync(specification);
    }

    public async Task<IEnumerable<Publication>> GetByUserAndSpecializationAsync(UserId userId, SpecializationId specializationId)
    {
        var specification = new PublicationByUserSpecification(userId)
            .And(new PublicationBySpecializationSpecification(specializationId));
        return await GetBySpecificationAsync(specification);
    }

    public async Task<IEnumerable<Publication>> GetByTypeAsync(PublicationType type)
    {
        var specification = new PublicationByTypeSpecification(type);
        return await GetBySpecificationAsync(specification);
    }

    public async Task<IEnumerable<Publication>> GetPeerReviewedPublicationsAsync(UserId userId)
    {
        var specification = PublicationSpecificationExtensions.GetPeerReviewedPublications(userId);
        return await GetBySpecificationAsync(specification);
    }

    public async Task<IEnumerable<Publication>> GetFirstAuthorPublicationsAsync(UserId userId)
    {
        var specification = PublicationSpecificationExtensions.GetFirstAuthorPublications(userId);
        return await GetBySpecificationAsync(specification);
    }

    public async Task<IEnumerable<Publication>> GetRecentPublicationsAsync(UserId userId, int years = 5)
    {
        var specification = PublicationSpecificationExtensions.GetRecentPublications(userId, years);
        return await GetBySpecificationAsync(specification);
    }

    public async Task<int> GetTotalImpactScoreAsync(UserId userId, SpecializationId specializationId)
    {
        var specification = new PublicationByUserSpecification(userId)
            .And(new PublicationBySpecializationSpecification(specializationId));
        var publications = await GetBySpecificationAsync(specification);
        
        // Sum the impact scores in memory since EF Core might not translate CalculateImpactScore()
        return publications.Sum(p => p.CalculateImpactScore());
    }

    public async Task<int> GetPublicationCountByTypeAsync(UserId userId, PublicationType type)
    {
        var specification = new PublicationByUserSpecification(userId)
            .And(new PublicationByTypeSpecification(type));
        return await CountBySpecificationAsync(specification);
    }

    public async Task AddAsync(Publication publication)
    {
        // ID generation should be handled by database or a dedicated service
        // For now, keeping the existing logic but moving it to a private method
        if (publication.Id.Value == 0)
        {
            await GenerateIdForEntity(publication);
        }

        await AddAsync(publication, default);
        // Note: SaveChangesAsync should be called by Unit of Work, not here
        // But keeping it for backward compatibility
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateAsync(Publication publication)
    {
        Update(publication);
        // Note: SaveChangesAsync should be called by Unit of Work, not here
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(PublicationId id)
    {
        var publication = await GetByIdAsync(id);
        if (publication != null)
        {
            Remove(publication);
            // Note: SaveChangesAsync should be called by Unit of Work, not here
            await _unitOfWork.SaveChangesAsync();
        }
    }

    // Additional methods using specifications
    public async Task<IEnumerable<Publication>> GetHighImpactPublicationsAsync(UserId userId, decimal minImpactFactor)
    {
        var specification = PublicationSpecificationExtensions.GetHighImpactPublications(userId, minImpactFactor);
        return await GetBySpecificationAsync(specification);
    }

    public async Task<IEnumerable<Publication>> GetPublicationsInJournalAsync(string journalName)
    {
        var specification = new PublicationByJournalSpecification(journalName);
        return await GetBySpecificationAsync(specification);
    }

    public async Task<IEnumerable<Publication>> GetPublicationsWithCoAuthorsAsync(UserId userId)
    {
        var specification = new PublicationByUserSpecification(userId)
            .And(new PublicationWithCoAuthorsSpecification());
        return await GetBySpecificationAsync(specification);
    }

    public async Task<bool> ExistsAsync(PublicationId id)
    {
        var specification = new PublicationByIdSpecification(id);
        return await CountBySpecificationAsync(specification) > 0;
    }

    public async Task<Dictionary<PublicationType, int>> GetPublicationCountsByTypeAsync(UserId userId)
    {
        var result = new Dictionary<PublicationType, int>();
        
        foreach (PublicationType type in Enum.GetValues<PublicationType>())
        {
            var count = await GetPublicationCountByTypeAsync(userId, type);
            result[type] = count;
        }
        
        return result;
    }

    public async Task<(IEnumerable<Publication> Items, int TotalCount)> GetPagedPublicationsAsync(
        UserId userId, 
        int pageNumber, 
        int pageSize)
    {
        var specification = new PublicationByUserSpecification(userId);
        return await GetPagedAsync(specification, pageNumber, pageSize, 
            orderBy: p => p.PublicationDate, ascending: false);
    }

    // Private helper methods
    private async Task GenerateIdForEntity(Publication publication)
    {
        // This should ideally be in a separate ID generation service
        var connection = Context.Database.GetDbConnection();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = "SELECT COALESCE(MAX(\"Id\"), 0) FROM \"Publications\"";
        var maxId = (int)(await command.ExecuteScalarAsync() ?? 0);

        var newId = new PublicationId(maxId + 1);

        // Use reflection to set the ID since it's private
        var idProperty = publication.GetType().GetProperty("Id");
        idProperty?.SetValue(publication, newId);

        await connection.CloseAsync();
    }
}