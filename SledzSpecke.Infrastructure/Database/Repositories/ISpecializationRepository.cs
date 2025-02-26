using SledzSpecke.Core.Models.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SledzSpecke.Infrastructure.Database.Repositories
{
    public interface ISpecializationRepository : IBaseRepository<Specialization>
    {
        Task<Specialization> GetWithRequirementsAsync(int id);
    }

}
