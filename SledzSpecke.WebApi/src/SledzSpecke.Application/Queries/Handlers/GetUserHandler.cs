using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;
using SledzSpecke.Application.Exceptions;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Queries.Handlers;

public sealed class GetUserHandler : IQueryHandler<GetUser, UserDto>
{
    private readonly IUserRepository _userRepository;

    public GetUserHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserDto> HandleAsync(GetUser query)
    {
        var userId = new UserId(query.UserId);
        var user = await _userRepository.GetByIdAsync(userId);

        if (user is null)
        {
            throw new UserNotFoundException(query.UserId);
        }

        return new UserDto(
            user.UserId,
            user.Email,
            user.Name,
            Core.ValueObjects.SmkVersion.Old, // SmkVersion - temporary default
            0, // SpecializationId - temporary
            user.RegistrationDate
        );
    }
}