using SledzSpecke.Application.Abstractions;

namespace SledzSpecke.Application.Commands;

public record ChangePassword(
    string CurrentPassword,
    string NewPassword) : ICommand;