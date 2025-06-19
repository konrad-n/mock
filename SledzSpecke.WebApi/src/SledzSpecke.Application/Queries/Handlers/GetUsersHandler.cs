using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;
using SledzSpecke.Core.Repositories;

namespace SledzSpecke.Application.Queries.Handlers;

public sealed class GetUsersHandler : IQueryHandler<GetUsers, IEnumerable<UserDto>>
{
    private readonly IUserRepository _userRepository;

    public GetUsersHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<UserDto>> HandleAsync(GetUsers query)
    {
        var users = await _userRepository.GetAllAsync();

        return users.Select(user => new UserDto(
            user.Id.Value,
            user.Email.Value,
            user.GetFullName(),
            Core.ValueObjects.SmkVersion.Old, // SmkVersion - temporary default
            0, // SpecializationId - temporary
            user.RegistrationDate
        ));
    }
}