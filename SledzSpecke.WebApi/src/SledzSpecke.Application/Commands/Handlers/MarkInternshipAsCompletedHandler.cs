using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Exceptions;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Commands.Handlers;

public class MarkInternshipAsCompletedHandler : ICommandHandler<MarkInternshipAsCompleted>
{
    private readonly IInternshipRepository _internshipRepository;
    private readonly IUserContextService _userContextService;
    private readonly IUserRepository _userRepository;

    public MarkInternshipAsCompletedHandler(
        IInternshipRepository internshipRepository,
        IUserContextService userContextService,
        IUserRepository userRepository)
    {
        _internshipRepository = internshipRepository;
        _userContextService = userContextService;
        _userRepository = userRepository;
    }

    public async Task HandleAsync(MarkInternshipAsCompleted command)
    {
        var internship = await _internshipRepository.GetByIdAsync(new InternshipId(command.InternshipId));
        if (internship == null)
        {
            throw new NotFoundException($"Internship with ID {command.InternshipId} not found.");
        }

        // Verify user has access to this internship
        var userId = _userContextService.GetUserId();
        var user = await _userRepository.GetByIdAsync(new UserId(userId));
        if (user == null || user.SpecializationId != internship.SpecializationId)
        {
            throw new UnauthorizedException("You are not authorized to modify this internship.");
        }

        // Check if internship can be marked as completed
        if (internship.IsApproved)
        {
            throw new BusinessRuleException("Cannot modify an approved internship.");
        }

        // Validate that the internship has ended or is ending today
        if (internship.EndDate > DateTime.UtcNow.Date)
        {
            throw new BusinessRuleException("Cannot mark an internship as completed before its end date.");
        }

        // Mark as completed
        internship.MarkAsCompleted();

        await _internshipRepository.UpdateAsync(internship);
    }
}