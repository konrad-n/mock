using Microsoft.EntityFrameworkCore;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Infrastructure.DAL.Repositories;

internal sealed class SqlPublicationRepository : IPublicationRepository
{
    private readonly SledzSpeckeDbContext _context;
    private readonly DbSet<Publication> _publications;

    public SqlPublicationRepository(SledzSpeckeDbContext context)
    {
        _context = context;
        _publications = context.Publications;
    }

    public async Task<Publication?> GetByIdAsync(PublicationId id)
        => await _publications.SingleOrDefaultAsync(p => p.Id == id);

    public async Task<IEnumerable<Publication>> GetByUserIdAsync(UserId userId)
        => await _publications
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.PublicationDate)
            .ToListAsync();

    public async Task<IEnumerable<Publication>> GetBySpecializationIdAsync(SpecializationId specializationId)
        => await _publications
            .Where(p => p.SpecializationId == specializationId)
            .OrderByDescending(p => p.PublicationDate)
            .ToListAsync();

    public async Task<IEnumerable<Publication>> GetByUserAndSpecializationAsync(UserId userId, SpecializationId specializationId)
        => await _publications
            .Where(p => p.UserId == userId && p.SpecializationId == specializationId)
            .OrderByDescending(p => p.PublicationDate)
            .ToListAsync();

    public async Task<IEnumerable<Publication>> GetByTypeAsync(PublicationType type)
        => await _publications
            .Where(p => p.Type == type)
            .OrderByDescending(p => p.PublicationDate)
            .ToListAsync();

    public async Task<IEnumerable<Publication>> GetPeerReviewedPublicationsAsync(UserId userId)
        => await _publications
            .Where(p => p.UserId == userId && p.IsPeerReviewed)
            .OrderByDescending(p => p.PublicationDate)
            .ToListAsync();

    public async Task<IEnumerable<Publication>> GetFirstAuthorPublicationsAsync(UserId userId)
        => await _publications
            .Where(p => p.UserId == userId && p.IsFirstAuthor)
            .OrderByDescending(p => p.PublicationDate)
            .ToListAsync();

    public async Task<IEnumerable<Publication>> GetRecentPublicationsAsync(UserId userId, int years = 5)
    {
        var cutoffDate = DateTime.UtcNow.AddYears(-years);
        return await _publications
            .Where(p => p.UserId == userId && p.PublicationDate >= cutoffDate)
            .OrderByDescending(p => p.PublicationDate)
            .ToListAsync();
    }

    public async Task<int> GetTotalImpactScoreAsync(UserId userId, SpecializationId specializationId)
        => await _publications
            .Where(p => p.UserId == userId && p.SpecializationId == specializationId)
            .SumAsync(p => p.CalculateImpactScore());

    public async Task<int> GetPublicationCountByTypeAsync(UserId userId, PublicationType type)
        => await _publications
            .CountAsync(p => p.UserId == userId && p.Type == type);

    public async Task AddAsync(Publication publication)
    {
        await _publications.AddAsync(publication);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Publication publication)
    {
        _publications.Update(publication);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(PublicationId id)
    {
        var publication = await _publications.FindAsync(id.Value);
        if (publication is not null)
        {
            _publications.Remove(publication);
            await _context.SaveChangesAsync();
        }
    }
}