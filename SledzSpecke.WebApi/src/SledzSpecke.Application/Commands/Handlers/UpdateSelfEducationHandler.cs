using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Exceptions;
using SledzSpecke.Core.Repositories;

namespace SledzSpecke.Application.Commands.Handlers;

public sealed class UpdateSelfEducationHandler : ICommandHandler<UpdateSelfEducation>
{
    private readonly ISelfEducationRepository _selfEducationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContextService _userContextService;

    public UpdateSelfEducationHandler(
        ISelfEducationRepository selfEducationRepository,
        IUnitOfWork unitOfWork,
        IUserContextService userContextService)
    {
        _selfEducationRepository = selfEducationRepository;
        _unitOfWork = unitOfWork;
        _userContextService = userContextService;
    }

    public async Task HandleAsync(UpdateSelfEducation command)
    {
        var selfEducation = await _selfEducationRepository.GetByIdAsync(command.SelfEducationId);
        if (selfEducation is null)
        {
            throw new SelfEducationNotFoundException(command.SelfEducationId.Value);
        }

        // Note: The new SelfEducation entity doesn't have UserId, it's linked through Module
        // For now, skip the user check (should be implemented through Module->Specialization->User)
        
        // Update description (combining title and description from command)
        var newDescription = string.IsNullOrEmpty(command.Description) 
            ? command.Title 
            : $"{command.Title}: {command.Description}";
        
        selfEducation.UpdateDescription(newDescription);
        
        // Update hours if provided (convert credit hours to hours)
        if (command.CreditHours > 0)
        {
            selfEducation.UpdateHours(command.CreditHours);
        }

        await _selfEducationRepository.UpdateAsync(selfEducation);
        await _unitOfWork.SaveChangesAsync();
    }
}