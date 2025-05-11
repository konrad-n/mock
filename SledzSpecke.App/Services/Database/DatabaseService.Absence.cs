using SledzSpecke.App.Exceptions;
using SledzSpecke.App.Models;

namespace SledzSpecke.App.Services.Database
{
    public partial class DatabaseService
    {
        public async Task<Absence> GetAbsenceAsync(int id)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                var absence = await this.database.Table<Absence>().FirstOrDefaultAsync(a => a.AbsenceId == id);
                if (absence == null)
                {
                    throw new ResourceNotFoundException(
                        $"Absence with ID {id} not found",
                        $"Nie znaleziono nieobecności o ID {id}");
                }
                return absence;
            },
            new Dictionary<string, object> { { "AbsenceId", id } },
            $"Nie udało się pobrać nieobecności o ID {id}");
        }

        public async Task<List<Absence>> GetAbsencesAsync(int specializationId)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                return await this.database.Table<Absence>().Where(a => a.SpecializationId == specializationId).ToListAsync();
            },
            new Dictionary<string, object> { { "SpecializationId", specializationId } },
            $"Nie udało się pobrać listy nieobecności dla specjalizacji o ID {specializationId}");
        }

        public async Task<int> SaveAbsenceAsync(Absence absence)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (absence == null)
                {
                    throw new InvalidInputException(
                        "Absence cannot be null",
                        "Nieobecność nie może być pusta");
                }

                if (absence.AbsenceId != 0)
                {
                    return await this.database.UpdateAsync(absence);
                }
                else
                {
                    return await this.database.InsertAsync(absence);
                }
            },
            new Dictionary<string, object> { { "Absence", absence?.AbsenceId }, { "SpecializationId", absence?.SpecializationId } },
            "Nie udało się zapisać danych nieobecności");
        }

        public async Task<int> DeleteAbsenceAsync(Absence absence)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (absence == null)
                {
                    throw new InvalidInputException(
                        "Absence cannot be null",
                        "Nieobecność nie może być pusta");
                }

                return await this.database.DeleteAsync(absence);
            },
            new Dictionary<string, object> { { "Absence", absence?.AbsenceId } },
            "Nie udało się usunąć nieobecności");
        }
    }
}