using SledzSpecke.Application.Abstractions;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Commands;

public record DeleteAbsence(AbsenceId AbsenceId) : ICommand;