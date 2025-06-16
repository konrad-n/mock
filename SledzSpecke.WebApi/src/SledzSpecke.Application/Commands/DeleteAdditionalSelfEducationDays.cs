using SledzSpecke.Application.Abstractions;

namespace SledzSpecke.Application.Commands;

public record DeleteAdditionalSelfEducationDays(int Id) : ICommand;