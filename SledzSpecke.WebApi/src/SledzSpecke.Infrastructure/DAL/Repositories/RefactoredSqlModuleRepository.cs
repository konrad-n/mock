using Microsoft.EntityFrameworkCore;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.Specifications;
using SledzSpecke.Core.ValueObjects;
using SledzSpecke.Application.Abstractions;

namespace SledzSpecke.Infrastructure.DAL.Repositories;

/// <summary>
/// Refactored Module repository using BaseRepository and Specifications
/// </summary>
internal sealed class RefactoredSqlModuleRepository : BaseRepository<Module>, IModuleRepository
{
    private readonly IUnitOfWork _unitOfWork;

    public RefactoredSqlModuleRepository(
        SledzSpeckeDbContext context,
        IUnitOfWork unitOfWork) : base(context)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Module?> GetByIdAsync(ModuleId id)
    {
        var specification = new ModuleByIdSpecification(id);
        return await GetSingleBySpecificationAsync(specification);
    }

    public async Task<Module?> GetByIdAsync(int id)
    {
        var moduleId = new ModuleId(id);
        return await GetByIdAsync(moduleId);
    }

    public async Task<IEnumerable<Module>> GetBySpecializationIdAsync(SpecializationId specializationId)
    {
        var specification = new ModuleBySpecializationSpecification(specializationId);
        return await GetBySpecificationAsync(specification);
    }

    public async Task<IEnumerable<Module>> GetAllAsync()
    {
        return await GetAllAsync(default);
    }

    public async Task<ModuleId> AddAsync(Module module)
    {
        // ID generation should be handled by database or a dedicated service
        // For now, keeping the existing logic but moving it to a private method
        if (module.Id.Value == 0)
        {
            await GenerateIdForEntity(module);
        }

        await AddAsync(module, default);
        // Note: SaveChangesAsync should be called by Unit of Work, not here
        // But keeping it for backward compatibility
        await _unitOfWork.SaveChangesAsync();
        return module.Id;
    }

    public async Task UpdateAsync(Module module)
    {
        Update(module);
        // Note: SaveChangesAsync should be called by Unit of Work, not here
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(ModuleId id)
    {
        var module = await GetByIdAsync(id);
        if (module != null)
        {
            Remove(module);
            // Note: SaveChangesAsync should be called by Unit of Work, not here
            await _unitOfWork.SaveChangesAsync();
        }
    }

    // Additional methods using specifications
    public async Task<IEnumerable<Module>> GetBasicModulesAsync(SpecializationId specializationId)
    {
        var specification = ModuleSpecificationExtensions.GetBasicModulesForSpecialization(specializationId);
        return await GetBySpecificationAsync(specification);
    }

    public async Task<IEnumerable<Module>> GetSpecialistModulesAsync(SpecializationId specializationId)
    {
        var specification = ModuleSpecificationExtensions.GetSpecialistModulesForSpecialization(specializationId);
        return await GetBySpecificationAsync(specification);
    }

    public async Task<IEnumerable<Module>> GetModulesInDateRangeAsync(SpecializationId specializationId, DateTime startDate, DateTime endDate)
    {
        var specification = ModuleSpecificationExtensions.GetModulesInDateRange(specializationId, startDate, endDate);
        return await GetBySpecificationAsync(specification);
    }

    public async Task<bool> ExistsAsync(ModuleId id)
    {
        var specification = new ModuleByIdSpecification(id);
        return await CountBySpecificationAsync(specification) > 0;
    }

    public async Task<Module?> GetActiveModuleForSpecializationAsync(int specializationId)
    {
        var specId = new SpecializationId(specializationId);
        var modules = await GetBySpecializationIdAsync(specId);
        
        // Find the module that is in progress (not completed)
        return modules.FirstOrDefault(m => !m.IsCompleted());
    }

    // Private helper methods
    private async Task GenerateIdForEntity(Module module)
    {
        // This should ideally be in a separate ID generation service
        var connection = Context.Database.GetDbConnection();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = "SELECT COALESCE(MAX(\"Id\"), 0) FROM \"Modules\"";
        var maxId = (int)(await command.ExecuteScalarAsync() ?? 0);

        var newId = new ModuleId(maxId + 1);

        // Use reflection to set the ID since it's private
        var idProperty = module.GetType().GetProperty("Id");
        idProperty?.SetValue(module, newId);

        await connection.CloseAsync();
    }
}