using SledzSpecke.Application.Abstractions;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Commands.Handlers;

internal sealed class ApproveInternshipHandler : ICommandHandler<ApproveInternship>
{
    private readonly IInternshipRepository _internshipRepository;

    public ApproveInternshipHandler(IInternshipRepository internshipRepository)
    {
        _internshipRepository = internshipRepository;
    }

    public async Task HandleAsync(ApproveInternship command)
    {
        var internship = await _internshipRepository.GetByIdAsync(new InternshipId(command.InternshipId));
        if (internship is null)
        {
            throw new InvalidOperationException($"Internship with ID {command.InternshipId} not found.");
        }

        internship.Approve(command.ApproverName);
        await _internshipRepository.UpdateAsync(internship);
    }
}