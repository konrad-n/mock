using SledzSpecke.Application.Abstractions;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Commands;

public record UpdateRecognition(
    RecognitionId RecognitionId,
    RecognitionType Type,
    string Title,
    string? Description,
    string? Institution,
    DateTime StartDate,
    DateTime EndDate,
    int DaysReduction) : ICommand;