using SledzSpecke.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SledzSpecke.App.Services
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