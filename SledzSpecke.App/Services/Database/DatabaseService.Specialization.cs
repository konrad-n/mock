using SledzSpecke.App.Exceptions;
using SledzSpecke.App.Models;

namespace SledzSpecke.App.Services.Database
{
    public partial class DatabaseService
    {
        public async Task<Models.Specialization> GetSpecializationAsync(int id)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (_specializationCache.TryGetValue(id, out var cachedSpecialization))
                {
                    return cachedSpecialization;
                }

                var specialization = await database.Table<Models.Specialization>()
                    .FirstOrDefaultAsync(s => s.SpecializationId == id);

                if (specialization != null)
                {
                    _specializationCache[id] = specialization;
                }

                return specialization ?? new Models.Specialization();
            },
            new Dictionary<string, object> { { "SpecializationId", id } },
            $"Nie udało się pobrać specjalizacji o ID {id}");
        }

        public async Task<Models.Specialization> GetSpecializationWithModulesAsync(int id)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                var specialization = await GetSpecializationAsync(id);
                if (specialization != null)
                {
                    specialization.Modules = await GetModulesAsync(specialization.SpecializationId);
                }
                return specialization;
            },
            new Dictionary<string, object> { { "SpecializationId", id } },
            $"Nie udało się pobrać specjalizacji z modułami o ID {id}");
        }

        public async Task<int> SaveSpecializationAsync(Models.Specialization specialization)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (specialization == null)
                {
                    throw new InvalidInputException(
                        "Specialization cannot be null",
                        "Specjalizacja nie może być pusta");
                }

                if (specialization.SpecializationId != 0)
                {
                    await this.database.UpdateAsync(specialization);

                    // Clear cache after update
                    if (_specializationCache.ContainsKey(specialization.SpecializationId))
                    {
                        _specializationCache.Remove(specialization.SpecializationId);
                    }

                    return specialization.SpecializationId;
                }
                else
                {
                    await this.database.InsertAsync(specialization);
                    var lastId = await this.database.ExecuteScalarAsync<int>("SELECT last_insert_rowid()");
                    specialization.SpecializationId = lastId;
                    return lastId;
                }
            },
            new Dictionary<string, object> { { "Specialization", specialization?.SpecializationId } },
            "Nie udało się zapisać danych specjalizacji");
        }

        public async Task<List<Models.Specialization>> GetAllSpecializationsAsync()
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                return await this.database.Table<Models.Specialization>().ToListAsync();
            }, null, "Nie udało się pobrać listy specjalizacji");
        }

        public async Task<int> UpdateSpecializationAsync(Models.Specialization specialization)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (specialization == null)
                {
                    throw new InvalidInputException(
                        "Specialization cannot be null",
                        "Specjalizacja nie może być pusta");
                }

                var result = await this.database.UpdateAsync(specialization);

                // Clear cache after update
                if (_specializationCache.ContainsKey(specialization.SpecializationId))
                {
                    _specializationCache.Remove(specialization.SpecializationId);
                }

                return result;
            },
            new Dictionary<string, object> { { "Specialization", specialization?.SpecializationId } },
            "Nie udało się zaktualizować danych specjalizacji");
        }
    }
}