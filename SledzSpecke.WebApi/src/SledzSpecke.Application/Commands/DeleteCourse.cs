using SledzSpecke.Application.Abstractions;

namespace SledzSpecke.Application.Commands;

public record DeleteCourse(int CourseId) : ICommand;