using SledzSpecke.App.Exceptions;
using SledzSpecke.App.Models;

namespace SledzSpecke.App.Services.Database
{
    public partial class DatabaseService
    {
        public async Task<EducationalActivity> GetEducationalActivityAsync(int id)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                var activity = await this.database.Table<EducationalActivity>().FirstOrDefaultAsync(a => a.ActivityId == id);
                if (activity == null)
                {
                    throw new ResourceNotFoundException(
                        $"EducationalActivity with ID {id} not found",
                        $"Nie znaleziono aktywności edukacyjnej o ID {id}");
                }
                return activity;
            },
            new Dictionary<string, object> { { "ActivityId", id } },
            $"Nie udało się pobrać aktywności edukacyjnej o ID {id}");
        }

        public async Task<List<EducationalActivity>> GetEducationalActivitiesAsync(int? specializationId = null, int? moduleId = null)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                var query = this.database.Table<EducationalActivity>();

                if (specializationId.HasValue)
                {
                    query = query.Where(a => a.SpecializationId == specializationId);
                }

                if (moduleId.HasValue)
                {
                    query = query.Where(a => a.ModuleId == moduleId);
                }

                return await query.ToListAsync();
            },
            new Dictionary<string, object> { { "SpecializationId", specializationId }, { "ModuleId", moduleId } },
            "Nie udało się pobrać listy aktywności edukacyjnych");
        }

        public async Task<int> SaveEducationalActivityAsync(EducationalActivity activity)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (activity == null)
                {
                    throw new InvalidInputException(
                        "EducationalActivity cannot be null",
                        "Aktywność edukacyjna nie może być pusta");
                }

                if (activity.ActivityId != 0)
                {
                    return await this.database.UpdateAsync(activity);
                }
                else
                {
                    return await this.database.InsertAsync(activity);
                }
            },
            new Dictionary<string, object> {
                { "Activity", activity?.ActivityId },
                { "SpecializationId", activity?.SpecializationId },
                { "ModuleId", activity?.ModuleId }
            },
            "Nie udało się zapisać danych aktywności edukacyjnej");
        }

        public async Task<int> DeleteEducationalActivityAsync(EducationalActivity activity)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (activity == null)
                {
                    throw new InvalidInputException(
                        "EducationalActivity cannot be null",
                        "Aktywność edukacyjna nie może być pusta");
                }

                return await this.database.DeleteAsync(activity);
            },
            new Dictionary<string, object> { { "Activity", activity?.ActivityId } },
            "Nie udało się usunąć aktywności edukacyjnej");
        }
    }
}