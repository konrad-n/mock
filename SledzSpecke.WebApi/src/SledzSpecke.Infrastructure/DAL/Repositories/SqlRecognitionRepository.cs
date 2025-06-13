using Microsoft.EntityFrameworkCore;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Infrastructure.DAL.Repositories;

internal sealed class SqlRecognitionRepository : IRecognitionRepository
{
    private readonly SledzSpeckeDbContext _context;
    private readonly DbSet<Recognition> _recognitions;

    public SqlRecognitionRepository(SledzSpeckeDbContext context)
    {
        _context = context;
        _recognitions = context.Recognitions;
    }

    public async Task<Recognition?> GetByIdAsync(RecognitionId id)
        => await _recognitions.SingleOrDefaultAsync(r => r.Id == id);

    public async Task<IEnumerable<Recognition>> GetByUserIdAsync(UserId userId)
        => await _recognitions
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

    public async Task<IEnumerable<Recognition>> GetBySpecializationIdAsync(SpecializationId specializationId)
        => await _recognitions
            .Where(r => r.SpecializationId == specializationId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

    public async Task<IEnumerable<Recognition>> GetByUserAndSpecializationAsync(UserId userId, SpecializationId specializationId)
        => await _recognitions
            .Where(r => r.UserId == userId && r.SpecializationId == specializationId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

    public async Task<IEnumerable<Recognition>> GetApprovedRecognitionsAsync(UserId userId, SpecializationId specializationId)
        => await _recognitions
            .Where(r => r.UserId == userId && r.SpecializationId == specializationId && r.IsApproved)
            .OrderByDescending(r => r.ApprovedAt)
            .ToListAsync();

    public async Task<int> GetTotalReductionDaysAsync(UserId userId, SpecializationId specializationId)
        => await _recognitions
            .Where(r => r.UserId == userId && r.SpecializationId == specializationId && r.IsApproved)
            .SumAsync(r => r.DaysReduction);

    public async Task<IEnumerable<Recognition>> GetByTypeAsync(RecognitionType type)
        => await _recognitions
            .Where(r => r.Type == type)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

    public async Task AddAsync(Recognition recognition)
    {
        await _recognitions.AddAsync(recognition);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Recognition recognition)
    {
        _recognitions.Update(recognition);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(RecognitionId id)
    {
        var recognition = await _recognitions.FindAsync(id.Value);
        if (recognition is not null)
        {
            _recognitions.Remove(recognition);
            await _context.SaveChangesAsync();
        }
    }
}