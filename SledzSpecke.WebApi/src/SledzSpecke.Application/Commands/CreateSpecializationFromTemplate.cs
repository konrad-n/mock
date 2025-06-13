using SledzSpecke.Application.Abstractions;

namespace SledzSpecke.Application.Commands;

public record CreateSpecializationFromTemplate(
    string TemplateName,
    string SmkVersion,
    int UserId,
    DateTime StartDate
) : ICommand<int>;