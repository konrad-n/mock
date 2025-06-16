using SledzSpecke.Application.Abstractions;

namespace SledzSpecke.Application.Commands;

public record AddAdditionalSelfEducationDays(
    int SpecializationId,
    int Year,
    int DaysUsed,
    string? Comment
) : ICommand<int>;