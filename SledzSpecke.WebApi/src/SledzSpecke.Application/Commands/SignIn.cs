using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;

namespace SledzSpecke.Application.Commands;

public record SignIn(string Email, string Password) : ICommand<JwtDto>;