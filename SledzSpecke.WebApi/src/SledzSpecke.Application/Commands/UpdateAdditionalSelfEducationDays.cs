using SledzSpecke.Application.Abstractions;

namespace SledzSpecke.Application.Commands;

public record UpdateAdditionalSelfEducationDays(
    int Id,
    int DaysUsed,
    string? Comment
) : ICommand;