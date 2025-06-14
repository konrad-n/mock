using SledzSpecke.Application.Abstractions;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Commands;

public record ApproveAbsence(AbsenceId AbsenceId, int ApprovedBy) : ICommand;