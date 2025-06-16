using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.Repositories;

public interface IUserRepository : IGenericRepository<User, UserId>
{
    Task<User?> GetByEmailAsync(Email email);
    Task<IEnumerable<User>> GetAllAsync();
    Task<UserId> AddAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(UserId id);
    Task<bool> ExistsByEmailAsync(Email email);
}