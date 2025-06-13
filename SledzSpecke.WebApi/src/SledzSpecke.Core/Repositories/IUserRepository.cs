using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(UserId id);
    Task<User?> GetByUsernameAsync(Username username);
    Task<User?> GetByEmailAsync(Email email);
    Task<IEnumerable<User>> GetAllAsync();
    Task<UserId> AddAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(UserId id);
    Task<bool> ExistsByUsernameAsync(Username username);
    Task<bool> ExistsByEmailAsync(Email email);
}