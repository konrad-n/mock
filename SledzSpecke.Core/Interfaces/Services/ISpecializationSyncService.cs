using SledzSpecke.Core.Models.Domain;
using System.Threading.Tasks;

namespace SledzSpecke.Core.Interfaces.Services
{
    public interface ISpecializationSyncService
    {
        Task<bool> UpdateSpecializationAsync(Specialization updatedSpecialization);
    }
}
