using SledzSpecke.Core.Models;

namespace SledzSpecke.App.Services.Interfaces
{
    public interface IDutyShiftService
    {
        Task<List<DutyShift>> GetAllDutyShiftsAsync();

        Task<DutyShift> GetDutyShiftAsync(int id);

        Task SaveDutyShiftAsync(DutyShift dutyShift);

        Task DeleteDutyShiftAsync(DutyShift dutyShift);

        Task<double> GetTotalDutyHoursAsync();

        Task<Dictionary<string, double>> GetMonthlyDutyHoursAsync();

        Task<double> GetAverageWeeklyHoursAsync(int weeks = 4);
    }
}
