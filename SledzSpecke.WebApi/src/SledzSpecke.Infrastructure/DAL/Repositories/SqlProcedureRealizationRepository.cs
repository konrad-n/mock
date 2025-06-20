using Microsoft.EntityFrameworkCore;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;
using SledzSpecke.Application.Abstractions;

namespace SledzSpecke.Infrastructure.DAL.Repositories;

internal sealed class SqlProcedureRealizationRepository : BaseRepository<ProcedureRealization>, IProcedureRealizationRepository
{
    private readonly IUnitOfWork _unitOfWork;

    public SqlProcedureRealizationRepository(
        SledzSpeckeDbContext context,
        IUnitOfWork unitOfWork) : base(context)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ProcedureRealization?> GetByIdAsync(ProcedureRealizationId id)
    {
        return await DbSet
            .Include(pr => pr.Requirement)
                .ThenInclude(req => req.Module)
            .Include(pr => pr.User)
            .FirstOrDefaultAsync(pr => pr.Id == id);
    }

    public async Task<IEnumerable<ProcedureRealization>> GetByUserIdAsync(UserId userId)
    {
        return await DbSet
            .Include(pr => pr.Requirement)
                .ThenInclude(req => req.Module)
            .Where(pr => pr.UserId == userId)
            .OrderByDescending(pr => pr.Date)
            .ToListAsync();
    }

    public async Task<IEnumerable<ProcedureRealization>> GetByRequirementIdAsync(ProcedureRequirementId requirementId)
    {
        return await DbSet
            .Include(pr => pr.User)
            .Where(pr => pr.RequirementId == requirementId)
            .OrderByDescending(pr => pr.Date)
            .ToListAsync();
    }

    public async Task<IEnumerable<ProcedureRealization>> GetByUserAndRequirementAsync(UserId userId, ProcedureRequirementId requirementId)
    {
        return await DbSet
            .Where(pr => pr.UserId == userId && pr.RequirementId == requirementId)
            .OrderByDescending(pr => pr.Date)
            .ToListAsync();
    }

    public async Task<IEnumerable<ProcedureRealization>> GetByUserAndModuleAsync(UserId userId, ModuleId moduleId)
    {
        return await DbSet
            .Include(pr => pr.Requirement)
            .Where(pr => pr.UserId == userId && pr.Requirement.ModuleId == moduleId)
            .OrderByDescending(pr => pr.Date)
            .ToListAsync();
    }

    public async Task<ProcedureRealizationId> AddAsync(ProcedureRealization realization)
    {
        var entry = await DbSet.AddAsync(realization);
        await _unitOfWork.SaveChangesAsync();
        return entry.Entity.Id;
    }

    public async Task UpdateAsync(ProcedureRealization realization)
    {
        DbSet.Update(realization);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(ProcedureRealization realization)
    {
        DbSet.Remove(realization);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<int> CountByUserAndRequirementAsync(UserId userId, ProcedureRequirementId requirementId, ProcedureRole? role = null)
    {
        var query = DbSet.Where(pr => pr.UserId == userId && pr.RequirementId == requirementId);
        
        if (role.HasValue)
        {
            query = query.Where(pr => pr.Role == role.Value);
        }

        return await query.CountAsync();
    }

    public async Task<Dictionary<ProcedureRequirementId, int>> CountByUserAndModuleGroupedAsync(UserId userId, ModuleId moduleId, ProcedureRole? role = null)
    {
        var query = DbSet
            .Include(pr => pr.Requirement)
            .Where(pr => pr.UserId == userId && pr.Requirement.ModuleId == moduleId);

        if (role.HasValue)
        {
            query = query.Where(pr => pr.Role == role.Value);
        }

        var results = await query
            .GroupBy(pr => pr.RequirementId)
            .Select(g => new { RequirementId = g.Key, Count = g.Count() })
            .ToListAsync();

        return results.ToDictionary(r => r.RequirementId, r => r.Count);
    }
}