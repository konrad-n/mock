using SledzSpecke.App.Exceptions;
using SledzSpecke.App.Models;

namespace SledzSpecke.App.Services.Database
{
    public partial class DatabaseService
    {
        public async Task<Models.Recognition> GetRecognitionAsync(int id)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                var recognition = await this.database.Table<Models.Recognition>().FirstOrDefaultAsync(r => r.RecognitionId == id);
                if (recognition == null)
                {
                    throw new ResourceNotFoundException(
                        $"Recognition with ID {id} not found",
                        $"Nie znaleziono uznania o ID {id}");
                }
                return recognition;
            },
            new Dictionary<string, object> { { "RecognitionId", id } },
            $"Nie udało się pobrać uznania o ID {id}");
        }

        public async Task<List<Models.Recognition>> GetRecognitionsAsync(int specializationId)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                return await this.database.Table<Models.Recognition>().Where(r => r.SpecializationId == specializationId).ToListAsync();
            },
            new Dictionary<string, object> { { "SpecializationId", specializationId } },
            $"Nie udało się pobrać listy uznań dla specjalizacji o ID {specializationId}");
        }

        public async Task<int> SaveRecognitionAsync(Models.Recognition recognition)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (recognition == null)
                {
                    throw new InvalidInputException(
                        "Recognition cannot be null",
                        "Uznanie nie może być puste");
                }

                if (recognition.RecognitionId != 0)
                {
                    return await this.database.UpdateAsync(recognition);
                }
                else
                {
                    return await this.database.InsertAsync(recognition);
                }
            },
            new Dictionary<string, object> { { "Recognition", recognition?.RecognitionId }, { "SpecializationId", recognition?.SpecializationId } },
            "Nie udało się zapisać danych uznania");
        }

        public async Task<int> DeleteRecognitionAsync(Models.Recognition recognition)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (recognition == null)
                {
                    throw new InvalidInputException(
                        "Recognition cannot be null",
                        "Uznanie nie może być puste");
                }

                return await this.database.DeleteAsync(recognition);
            },
            new Dictionary<string, object> { { "Recognition", recognition?.RecognitionId } },
            "Nie udało się usunąć uznania");
        }
    }
}