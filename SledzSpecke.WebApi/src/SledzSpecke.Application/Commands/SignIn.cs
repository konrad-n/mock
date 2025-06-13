using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;

namespace SledzSpecke.Application.Commands;

public record SignIn(string Username, string Password) : ICommand<JwtDto>;