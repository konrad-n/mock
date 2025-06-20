using Microsoft.EntityFrameworkCore;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;
using SledzSpecke.Application.Abstractions;

namespace SledzSpecke.Infrastructure.DAL.Repositories;

internal sealed class SqlProcedureRequirementRepository : BaseRepository<ProcedureRequirement>, IProcedureRequirementRepository
{
    private readonly IUnitOfWork _unitOfWork;

    public SqlProcedureRequirementRepository(
        SledzSpeckeDbContext context,
        IUnitOfWork unitOfWork) : base(context)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ProcedureRequirement?> GetByIdAsync(ProcedureRequirementId id)
    {
        return await DbSet
            .Include(pr => pr.Module)
            .Include(pr => pr.Realizations)
            .FirstOrDefaultAsync(pr => pr.Id == id);
    }

    public async Task<ProcedureRequirement?> GetByCodeAsync(ModuleId moduleId, string code)
    {
        return await DbSet
            .Include(pr => pr.Module)
            .FirstOrDefaultAsync(pr => pr.ModuleId == moduleId && pr.Code == code);
    }

    public async Task<IEnumerable<ProcedureRequirement>> GetByModuleIdAsync(ModuleId moduleId)
    {
        return await DbSet
            .Include(pr => pr.Module)
            .Where(pr => pr.ModuleId == moduleId)
            .OrderBy(pr => pr.DisplayOrder)
            .ThenBy(pr => pr.Code)
            .ToListAsync();
    }

    public async Task<IEnumerable<ProcedureRequirement>> GetBySpecializationIdAsync(int specializationId)
    {
        return await DbSet
            .Include(pr => pr.Module)
            .Where(pr => pr.Module.SpecializationId == new SpecializationId(specializationId))
            .OrderBy(pr => pr.Module.Type)
            .ThenBy(pr => pr.DisplayOrder)
            .ThenBy(pr => pr.Code)
            .ToListAsync();
    }

    public async Task<ProcedureRequirementId> AddAsync(ProcedureRequirement requirement)
    {
        var entry = await DbSet.AddAsync(requirement);
        await _unitOfWork.SaveChangesAsync();
        return entry.Entity.Id;
    }

    public async Task UpdateAsync(ProcedureRequirement requirement)
    {
        DbSet.Update(requirement);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(ProcedureRequirement requirement)
    {
        DbSet.Remove(requirement);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(ModuleId moduleId, string code)
    {
        return await DbSet.AnyAsync(pr => pr.ModuleId == moduleId && pr.Code == code);
    }
}