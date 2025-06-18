using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Exceptions;
using SledzSpecke.Core.Repositories;

namespace SledzSpecke.Application.Commands.Handlers;

public sealed class UpdateRecognitionHandler : ICommandHandler<UpdateRecognition>
{
    private readonly IRecognitionRepository _recognitionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContextService _userContextService;

    public UpdateRecognitionHandler(
        IRecognitionRepository recognitionRepository,
        IUnitOfWork unitOfWork,
        IUserContextService userContextService)
    {
        _recognitionRepository = recognitionRepository;
        _unitOfWork = unitOfWork;
        _userContextService = userContextService;
    }

    public async Task HandleAsync(UpdateRecognition command)
    {
        await HandleAsync(command, CancellationToken.None);
    }
    
    public async Task HandleAsync(UpdateRecognition command, CancellationToken cancellationToken)
    {
        var recognition = await _recognitionRepository.GetByIdAsync(command.RecognitionId);
        if (recognition is null)
        {
            throw new RecognitionNotFoundException(command.RecognitionId.Value);
        }

        var currentUserId = _userContextService.GetUserId();
        if (recognition.UserId.Value != (int)currentUserId)
        {
            throw new UnauthorizedAccessException("You can only update your own recognitions.");
        }

        recognition.UpdateDetails(
            command.Type,
            command.Title,
            command.Description,
            command.Institution,
            command.StartDate,
            command.EndDate,
            command.DaysReduction);

        await _recognitionRepository.UpdateAsync(recognition);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}