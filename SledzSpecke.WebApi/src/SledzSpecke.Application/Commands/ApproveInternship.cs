using SledzSpecke.Application.Abstractions;

namespace SledzSpecke.Application.Commands;

public record ApproveInternship(
    int InternshipId,
    string ApproverName
) : ICommand;