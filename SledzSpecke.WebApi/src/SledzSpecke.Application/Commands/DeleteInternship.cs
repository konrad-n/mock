using SledzSpecke.Application.Abstractions;

namespace SledzSpecke.Application.Commands;

public record DeleteInternship(int InternshipId) : ICommand;