using SledzSpecke.Application.Abstractions;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Commands;

public record CreateRecognition(
    SpecializationId SpecializationId,
    UserId UserId,
    RecognitionType Type,
    string Title,
    DateTime StartDate,
    DateTime EndDate,
    int DaysReduction,
    string? Description = null,
    string? Institution = null) : ICommand;