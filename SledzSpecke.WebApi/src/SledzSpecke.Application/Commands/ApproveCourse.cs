using SledzSpecke.Application.Abstractions;

namespace SledzSpecke.Application.Commands;

public record ApproveCourse(
    int CourseId,
    string ApproverName
) : ICommand;