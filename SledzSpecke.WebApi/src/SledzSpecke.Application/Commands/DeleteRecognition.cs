using SledzSpecke.Application.Abstractions;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Commands;

public record DeleteRecognition(RecognitionId RecognitionId) : ICommand;