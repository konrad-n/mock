using SledzSpecke.App.Exceptions;
using SledzSpecke.App.Models;

namespace SledzSpecke.App.Services.Database
{
    public partial class DatabaseService
    {
        public async Task<SelfEducation> GetSelfEducationAsync(int id)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                var selfEducation = await this.database.Table<SelfEducation>().FirstOrDefaultAsync(s => s.SelfEducationId == id);
                if (selfEducation == null)
                {
                    throw new ResourceNotFoundException(
                        $"SelfEducation with ID {id} not found",
                        $"Nie znaleziono samokształcenia o ID {id}");
                }
                return selfEducation;
            },
            new Dictionary<string, object> { { "SelfEducationId", id } },
            $"Nie udało się pobrać samokształcenia o ID {id}");
        }

        public async Task<List<SelfEducation>> GetSelfEducationItemsAsync(int? specializationId = null, int? moduleId = null)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                var query = this.database.Table<SelfEducation>();

                if (specializationId.HasValue)
                {
                    query = query.Where(s => s.SpecializationId == specializationId);
                }

                if (moduleId.HasValue)
                {
                    query = query.Where(s => s.ModuleId == moduleId);
                }

                return await query.ToListAsync();
            },
            new Dictionary<string, object> { { "SpecializationId", specializationId }, { "ModuleId", moduleId } },
            "Nie udało się pobrać listy samokształceń");
        }

        public async Task<int> SaveSelfEducationAsync(SelfEducation selfEducation)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (selfEducation == null)
                {
                    throw new InvalidInputException(
                        "SelfEducation cannot be null",
                        "Samokształcenie nie może być puste");
                }

                if (selfEducation.SelfEducationId != 0)
                {
                    return await this.database.UpdateAsync(selfEducation);
                }
                else
                {
                    return await this.database.InsertAsync(selfEducation);
                }
            },
            new Dictionary<string, object> {
                { "SelfEducation", selfEducation?.SelfEducationId },
                { "SpecializationId", selfEducation?.SpecializationId },
                { "ModuleId", selfEducation?.ModuleId }
            },
            "Nie udało się zapisać danych samokształcenia");
        }

        public async Task<int> DeleteSelfEducationAsync(SelfEducation selfEducation)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (selfEducation == null)
                {
                    throw new InvalidInputException(
                        "SelfEducation cannot be null",
                        "Samokształcenie nie może być puste");
                }

                return await this.database.DeleteAsync(selfEducation);
            },
            new Dictionary<string, object> { { "SelfEducation", selfEducation?.SelfEducationId } },
            "Nie udało się usunąć samokształcenia");
        }
    }
}