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
        // For backward compatibility, we need to adapt the old command to new entity structure
        // Use ModuleId 1 as default (should be updated to get from specialization)
        var moduleId = new ModuleId(1);
        
        // Convert title to description for new structure
        var description = string.IsNullOrEmpty(command.Description) 
            ? command.Title 
            : $"{command.Title}: {command.Description}";
            
        // Use current date as the activity date
        var activityDate = DateTime.UtcNow.Date;
        
        // Convert credit hours to hours (assuming 1 credit = 1 hour)
        var hours = command.CreditHours;
        
        var selfEducation = SelfEducation.Create(
            SelfEducationId.New(),
            moduleId,
            command.Type,
            description,
            activityDate,
            hours);

        await _selfEducationRepository.AddAsync(selfEducation);
    }
}