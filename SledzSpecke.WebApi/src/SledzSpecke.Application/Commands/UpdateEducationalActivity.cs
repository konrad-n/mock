using SledzSpecke.Application.Abstractions;

namespace SledzSpecke.Application.Commands;

public sealed record UpdateEducationalActivity(
    int Id,
    string Type,
    string Title,
    string? Description,
    DateTime StartDate,
    DateTime EndDate) : ICommand;