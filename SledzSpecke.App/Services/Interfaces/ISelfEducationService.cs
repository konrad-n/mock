using SledzSpecke.Core.Models;

namespace SledzSpecke.App.Services.Interfaces
{
    public interface ISelfEducationService
    {
        Task<List<SelfEducation>> GetAllSelfEducationAsync();

        Task<SelfEducation> GetSelfEducationAsync(int id);

        Task SaveSelfEducationAsync(SelfEducation selfEducation);

        Task DeleteSelfEducationAsync(SelfEducation selfEducation);

        Task<int> GetTotalUsedDaysAsync();

        Task<Dictionary<int, int>> GetYearlyUsedDaysAsync();

        Task<int> GetYearlyAllowanceAsync();
    }
}
