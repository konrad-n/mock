using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Exceptions;
using SledzSpecke.Core.Repositories;

namespace SledzSpecke.Application.Commands.Handlers;

internal sealed class DeleteRecognitionHandler : ICommandHandler<DeleteRecognition>
{
    private readonly IRecognitionRepository _recognitionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContextService _userContextService;

    public DeleteRecognitionHandler(
        IRecognitionRepository recognitionRepository,
        IUnitOfWork unitOfWork,
        IUserContextService userContextService)
    {
        _recognitionRepository = recognitionRepository;
        _unitOfWork = unitOfWork;
        _userContextService = userContextService;
    }

    public async Task HandleAsync(DeleteRecognition command)
    {
        var recognition = await _recognitionRepository.GetByIdAsync(command.RecognitionId);
        if (recognition is null)
        {
            throw new RecognitionNotFoundException(command.RecognitionId.Value);
        }

        var currentUserId = _userContextService.GetUserId();
        if (recognition.UserId.Value != (int)currentUserId)
        {
            throw new UnauthorizedAccessException("You can only delete your own recognitions.");
        }

        if (recognition.IsApproved)
        {
            throw new InvalidOperationException("Cannot delete an approved recognition.");
        }

        await _recognitionRepository.DeleteAsync(recognition.Id);
        await _unitOfWork.SaveChangesAsync();
    }
}