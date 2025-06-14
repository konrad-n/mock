using SledzSpecke.Application.Abstractions;

namespace SledzSpecke.Application.Commands;

public record MarkInternshipAsCompleted(int InternshipId) : ICommand;