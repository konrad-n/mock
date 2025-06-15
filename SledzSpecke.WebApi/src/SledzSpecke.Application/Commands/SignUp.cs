using SledzSpecke.Application.Abstractions;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Commands;

public record SignUp(
    string Email,
    string Username,
    string Password,
    string FullName,
    string SmkVersion,
    int SpecializationId
) : ICommand;