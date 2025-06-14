using Microsoft.EntityFrameworkCore;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Infrastructure.DAL.Repositories;

internal sealed class SqlUserRepository : IUserRepository
{
    private readonly SledzSpeckeDbContext _context;

    public SqlUserRepository(SledzSpeckeDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(UserId id)
        => await _context.Users.SingleOrDefaultAsync(x => x.Id == id);

    public async Task<User?> GetByUsernameAsync(Username username)
        => await _context.Users.SingleOrDefaultAsync(x => x.Username == username);

    public async Task<User?> GetByEmailAsync(Email email)
        => await _context.Users.SingleOrDefaultAsync(x => x.Email == email);

    public async Task<IEnumerable<User>> GetAllAsync()
        => await _context.Users.ToListAsync();

    public async Task<UserId> AddAsync(User user)
    {
        // Generate a new ID if not provided
        if (user.Id == null || user.Id.Value == 0)
        {
            // Get the next available ID
            var maxId = await _context.Users
                .Select(u => u.Id.Value)
                .DefaultIfEmpty(0)
                .MaxAsync();
            var newId = new UserId(maxId + 1);

            // Create a new user with the generated ID
            var newUser = new User(newId, user.Email, user.Username, user.Password,
                user.FullName, user.SmkVersion, user.SpecializationId, user.RegistrationDate);

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();
            return newUser.Id;
        }

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user.Id;
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(UserId id)
    {
        var user = await _context.Users.SingleOrDefaultAsync(x => x.Id == id);
        if (user is not null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsByUsernameAsync(Username username)
        => await _context.Users.AnyAsync(x => x.Username == username);

    public async Task<bool> ExistsByEmailAsync(Email email)
        => await _context.Users.AnyAsync(x => x.Email == email);
}