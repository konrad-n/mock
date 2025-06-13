using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.Repositories;

public interface IModuleRepository
{
    Task<Module?> GetByIdAsync(ModuleId id);
    Task<IEnumerable<Module>> GetBySpecializationIdAsync(SpecializationId specializationId);
    Task<IEnumerable<Module>> GetAllAsync();
    Task<ModuleId> AddAsync(Module module);
    Task UpdateAsync(Module module);
    Task DeleteAsync(ModuleId id);
}