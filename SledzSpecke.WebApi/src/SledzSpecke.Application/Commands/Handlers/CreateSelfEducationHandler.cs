using SledzSpecke.Application.Abstractions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Commands.Handlers;

public sealed class CreateSelfEducationHandler : ICommandHandler<CreateSelfEducation>
{
    private readonly ISelfEducationRepository _selfEducationRepository;

    public CreateSelfEducationHandler(ISelfEducationRepository selfEducationRepository)
    {
        _selfEducationRepository = selfEducationRepository;
    }

    public async Task HandleAsync(CreateSelfEducation command)
    {
        var selfEducation = SelfEducation.Create(
            SelfEducationId.New(),
            command.SpecializationId,
            command.UserId,
            command.Type,
            command.Year,
            command.Title,
            command.CreditHours);

        if (!string.IsNullOrEmpty(command.Description) || !string.IsNullOrEmpty(command.Provider))
        {
            selfEducation.UpdateBasicDetails(
                command.Type,
                command.Year,
                command.Title,
                command.Description,
                command.Provider,
                null); // publisher
        }

        await _selfEducationRepository.AddAsync(selfEducation);
    }
}