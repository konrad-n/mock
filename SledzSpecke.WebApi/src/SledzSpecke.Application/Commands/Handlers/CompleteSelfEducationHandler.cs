using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Exceptions;
using SledzSpecke.Core.Repositories;

namespace SledzSpecke.Application.Commands.Handlers;

internal sealed class CompleteSelfEducationHandler : ICommandHandler<CompleteSelfEducation>
{
    private readonly ISelfEducationRepository _selfEducationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContextService _userContextService;

    public CompleteSelfEducationHandler(
        ISelfEducationRepository selfEducationRepository,
        IUnitOfWork unitOfWork,
        IUserContextService userContextService)
    {
        _selfEducationRepository = selfEducationRepository;
        _unitOfWork = unitOfWork;
        _userContextService = userContextService;
    }

    public async Task HandleAsync(CompleteSelfEducation command)
    {
        var selfEducation = await _selfEducationRepository.GetByIdAsync(command.SelfEducationId);
        if (selfEducation is null)
        {
            throw new SelfEducationNotFoundException(command.SelfEducationId.Value);
        }

        var currentUserId = _userContextService.GetUserId();
        if (selfEducation.UserId.Value != (int)currentUserId)
        {
            throw new UnauthorizedAccessException("You can only complete your own self-education activities.");
        }

        selfEducation.Complete(command.CompletedAt ?? DateTime.UtcNow);
        
        if (!string.IsNullOrEmpty(command.CertificatePath))
        {
            selfEducation.SetCertificatePath(command.CertificatePath);
        }

        await _selfEducationRepository.UpdateAsync(selfEducation);
        await _unitOfWork.SaveChangesAsync();
    }
}