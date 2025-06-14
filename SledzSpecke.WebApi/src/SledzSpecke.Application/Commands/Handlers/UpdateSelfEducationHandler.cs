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

        var currentUserId = _userContextService.GetUserId();
        if (selfEducation.UserId.Value != (int)currentUserId)
        {
            throw new UnauthorizedAccessException("You can only update your own self-education activities.");
        }

        selfEducation.UpdateDetails(
            command.Title,
            command.Description,
            command.Provider,
            command.CreditHours,
            command.StartDate,
            command.EndDate,
            command.DurationHours);

        await _selfEducationRepository.UpdateAsync(selfEducation);
        await _unitOfWork.SaveChangesAsync();
    }
}