using SledzSpecke.Core.Models;

namespace SledzSpecke.App.Services.Interfaces
{
    public interface IDataManager
    {
        Task<Specialization> LoadSpecializationAsync();
        Task SaveSpecializationAsync(Specialization specialization);
        Task<bool> DeleteAllDataAsync();
        Task<List<SpecializationType>> GetAllSpecializationTypesAsync();
        Task<Specialization> InitializeSpecializationForUserAsync(int specializationTypeId, string username);
        string GetSpecializationName();
    }
}