using SledzSpecke.Application.Abstractions;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Commands;

public record ApproveRecognition(RecognitionId RecognitionId, int ApprovedBy) : ICommand;