using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Exceptions;
using SledzSpecke.Core.Repositories;

namespace SledzSpecke.Application.Commands.Handlers;

internal sealed class ApproveRecognitionHandler : ICommandHandler<ApproveRecognition>
{
    private readonly IRecognitionRepository _recognitionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContextService _userContextService;

    public ApproveRecognitionHandler(
        IRecognitionRepository recognitionRepository,
        IUnitOfWork unitOfWork,
        IUserContextService userContextService)
    {
        _recognitionRepository = recognitionRepository;
        _unitOfWork = unitOfWork;
        _userContextService = userContextService;
    }

    public async Task HandleAsync(ApproveRecognition command)
    {
        var recognition = await _recognitionRepository.GetByIdAsync(command.RecognitionId);
        if (recognition is null)
        {
            throw new RecognitionNotFoundException(command.RecognitionId.Value);
        }

        // Verify the current user has permission to approve
        // In a real system, you might check if the user is a supervisor or admin
        var currentUserId = _userContextService.GetUserId();
        
        recognition.Approve(new Core.ValueObjects.UserId(command.ApprovedBy));

        await _recognitionRepository.UpdateAsync(recognition);
        await _unitOfWork.SaveChangesAsync();
    }
}