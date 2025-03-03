using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;

namespace SledzSpecke.App.Services.Recognition
{
    public interface IRecognitionService
    {
        Task<List<Models.Recognition>> GetRecognitionsAsync(int specializationId);

        Task<Models.Recognition> GetRecognitionAsync(int recognitionId);

        Task<bool> AddRecognitionAsync(Models.Recognition recognition);

        Task<bool> UpdateRecognitionAsync(Models.Recognition recognition);

        Task<bool> DeleteRecognitionAsync(int recognitionId);

        Task<int> CalculateTotalReductionDaysAsync(int specializationId);

        Task<DateTime> ApplyReductionsToEndDateAsync(DateTime endDate, int specializationId);

        Task<List<RecognitionType>> GetAvailableRecognitionTypesAsync(int specializationId);
    }
}