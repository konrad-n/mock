using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Exceptions;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Commands.Handlers;

internal sealed class DeleteInternshipHandler : ICommandHandler<DeleteInternship>
{
    private readonly IInternshipRepository _internshipRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserContextService _userContextService;

    public DeleteInternshipHandler(
        IInternshipRepository internshipRepository,
        IUserRepository userRepository,
        IUserContextService userContextService)
    {
        _internshipRepository = internshipRepository;
        _userRepository = userRepository;
        _userContextService = userContextService;
    }

    public async Task HandleAsync(DeleteInternship command)
    {
        var internship = await _internshipRepository.GetByIdAsync(new InternshipId(command.InternshipId));
        if (internship is null)
        {
            throw new InternshipNotFoundException(command.InternshipId);
        }

        // Check if the internship belongs to the current user
        var userId = _userContextService.GetUserId();
        var user = await _userRepository.GetByIdAsync(new UserId(userId));
        if (user is null || user.SpecializationId != internship.SpecializationId)
        {
            throw new UnauthorizedAccessException("You can only delete your own internships.");
        }

        // Check if internship can be deleted
        if (!internship.CanBeDeleted())
        {
            throw new InvalidOperationException("Cannot delete synced or approved internship.");
        }

        await _internshipRepository.DeleteAsync(new InternshipId(command.InternshipId));
    }
}