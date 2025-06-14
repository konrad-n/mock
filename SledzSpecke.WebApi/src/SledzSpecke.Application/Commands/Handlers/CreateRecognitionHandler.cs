using SledzSpecke.Application.Abstractions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Commands.Handlers;

public sealed class CreateRecognitionHandler : ICommandHandler<CreateRecognition>
{
    private readonly IRecognitionRepository _recognitionRepository;

    public CreateRecognitionHandler(IRecognitionRepository recognitionRepository)
    {
        _recognitionRepository = recognitionRepository;
    }

    public async Task HandleAsync(CreateRecognition command)
    {
        var recognition = Recognition.Create(
            RecognitionId.New(),
            command.SpecializationId,
            command.UserId,
            command.Type,
            command.Title,
            command.StartDate,
            command.EndDate,
            command.DaysReduction);

        if (!string.IsNullOrEmpty(command.Description))
        {
            recognition.UpdateDetails(
                command.Type,
                command.Title,
                command.Description,
                command.Institution,
                command.StartDate,
                command.EndDate,
                command.DaysReduction);
        }

        await _recognitionRepository.AddAsync(recognition);
    }
}