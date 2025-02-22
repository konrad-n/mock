using SledzSpecke.Core.Models.Domain;
using SledzSpecke.Infrastructure.Database.Context;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SledzSpecke.Infrastructure.Database.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(IApplicationDbContext context) : base(context) { }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> GetByPWZAsync(string pwz)
        {
            return await FirstOrDefaultAsync(u => u.PWZ == pwz);
        }

        public async Task<List<User>> GetSupervisedUsersAsync(int supervisorId)
        {
            return await WhereAsync(u => u.SupervisorId == supervisorId);
        }

        Task<List<User>> IBaseRepository<User>.GetAllAsync()
        {
            throw new System.NotImplementedException();
        }
    }

    // Podobnie zaimplementuj pozostałe repozytoria
}
