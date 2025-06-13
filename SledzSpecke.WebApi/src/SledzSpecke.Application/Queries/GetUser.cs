using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;

namespace SledzSpecke.Application.Queries;

public record GetUser(int UserId) : IQuery<UserDto>;