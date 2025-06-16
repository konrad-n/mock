using SledzSpecke.Application.Abstractions;

namespace SledzSpecke.Application.Commands;

public record UpdateUserPreferences(
    bool NotificationsEnabled,
    bool EmailNotificationsEnabled) : ICommand;