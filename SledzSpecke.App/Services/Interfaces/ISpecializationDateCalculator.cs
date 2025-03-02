using SledzSpecke.App.Services.Implementations;

namespace SledzSpecke.App.Services.Interfaces
{
    public interface ISpecializationDateCalculator
    {
        Task<DateTime> CalculateExpectedEndDateAsync(int specializationId);
        Task<int> GetRemainingEducationDaysForYearAsync(int specializationId, int year);
        Task<List<SpecializationDateInfo>> GetImportantDatesAsync(int specializationId);
    }
}