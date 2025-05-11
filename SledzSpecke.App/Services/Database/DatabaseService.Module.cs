using SledzSpecke.App.Exceptions;
using SledzSpecke.App.Models;

namespace SledzSpecke.App.Services.Database
{
    public partial class DatabaseService
    {
        public async Task<Module> GetModuleAsync(int id)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                var module = await this.database.Table<Module>().FirstOrDefaultAsync(m => m.ModuleId == id);
                if (module == null)
                {
                    throw new ResourceNotFoundException(
                        $"Module with ID {id} not found",
                        $"Nie znaleziono modułu o ID {id}");
                }
                return module;
            },
            new Dictionary<string, object> { { "ModuleId", id } },
            $"Nie udało się pobrać modułu o ID {id}");
        }

        public async Task<List<Module>> GetModulesAsync(int specializationId)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (_moduleCache.TryGetValue(specializationId, out var cachedModules))
                {
                    return cachedModules;
                }

                var modules = await database.Table<Module>()
                    .Where(m => m.SpecializationId == specializationId)
                    .ToListAsync();

                if (modules != null)
                {
                    _moduleCache[specializationId] = modules;
                }

                return modules ?? new List<Module>();
            },
            new Dictionary<string, object> { { "SpecializationId", specializationId } },
            $"Nie udało się pobrać listy modułów dla specjalizacji o ID {specializationId}");
        }

        public async Task<int> SaveModuleAsync(Module module)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (module == null)
                {
                    throw new InvalidInputException(
                        "Module cannot be null",
                        "Moduł nie może być pusty");
                }

                if (module.ModuleId != 0)
                {
                    return await this.database.UpdateAsync(module);
                }
                else
                {
                    return await this.database.InsertAsync(module);
                }
            },
            new Dictionary<string, object> { { "Module", module?.ModuleId }, { "SpecializationId", module?.SpecializationId } },
            "Nie udało się zapisać danych modułu");
        }

        public async Task<int> UpdateModuleAsync(Module module)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (module == null)
                {
                    throw new InvalidInputException(
                        "Module cannot be null",
                        "Moduł nie może być pusty");
                }

                // Clear cache after update
                if (_moduleCache.TryGetValue(module.SpecializationId, out _))
                {
                    _moduleCache.Remove(module.SpecializationId);
                }

                return await this.database.UpdateAsync(module);
            },
            new Dictionary<string, object> { { "Module", module?.ModuleId }, { "SpecializationId", module?.SpecializationId } },
            "Nie udało się zaktualizować danych modułu");
        }
    }
}