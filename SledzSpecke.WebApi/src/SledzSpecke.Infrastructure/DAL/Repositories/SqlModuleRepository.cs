using Microsoft.EntityFrameworkCore;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Infrastructure.DAL.Repositories;

internal sealed class SqlModuleRepository : IModuleRepository
{
    private readonly SledzSpeckeDbContext _context;

    public SqlModuleRepository(SledzSpeckeDbContext context)
    {
        _context = context;
    }

    public async Task<Module?> GetByIdAsync(ModuleId id)
    {
        return await _context.Modules
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<Module?> GetByIdAsync(int id)
    {
        var moduleId = new ModuleId(id);
        return await GetByIdAsync(moduleId);
    }

    public async Task<IEnumerable<Module>> GetBySpecializationIdAsync(SpecializationId specializationId)
    {
        return await _context.Modules
            .Where(m => m.SpecializationId == specializationId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Module>> GetAllAsync()
    {
        return await _context.Modules.ToListAsync();
    }

    public async Task<ModuleId> AddAsync(Module module)
    {
        await _context.Modules.AddAsync(module);
        await _context.SaveChangesAsync();
        return module.Id;
    }

    public async Task UpdateAsync(Module module)
    {
        _context.Modules.Update(module);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(ModuleId id)
    {
        var module = await GetByIdAsync(id);
        if (module != null)
        {
            _context.Modules.Remove(module);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<Module?> GetActiveModuleForSpecializationAsync(int specializationId)
    {
        var specId = new SpecializationId(specializationId);
        var modules = await GetBySpecializationIdAsync(specId);
        
        // Find the module that is in progress (not completed)
        return modules.FirstOrDefault(m => !m.IsCompleted());
    }
}