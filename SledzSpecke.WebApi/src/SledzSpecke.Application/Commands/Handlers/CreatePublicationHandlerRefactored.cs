using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Commands;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Exceptions;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Commands.Handlers;

public sealed class CreatePublicationHandlerRefactored : IResultCommandHandler<CreatePublication, int>
{
    private readonly IPublicationRepository _publicationRepository;
    private readonly ISpecializationRepository _specializationRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreatePublicationHandlerRefactored(
        IPublicationRepository publicationRepository,
        ISpecializationRepository specializationRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _publicationRepository = publicationRepository;
        _specializationRepository = specializationRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<int>> HandleAsync(CreatePublication command)
    {
        try
        {
            // Validate specialization exists
            var specialization = await _specializationRepository.GetByIdAsync(command.SpecializationId);
            if (specialization is null)
            {
                return Result.Failure<int>($"Specialization with ID {command.SpecializationId.Value} not found.");
            }

            // Validate user exists
            var user = await _userRepository.GetByIdAsync(command.UserId);
            if (user is null)
            {
                return Result.Failure<int>($"User with ID {command.UserId.Value} not found.");
            }

            // Verify user has access to this specialization
            if (user.SpecializationId.Value != command.SpecializationId.Value)
            {
                return Result.Failure<int>("You are not authorized to create publications for this specialization.");
            }

            // Validate publication date
            if (command.PublicationDate > DateTime.UtcNow)
            {
                return Result.Failure<int>("Publication date cannot be in the future.");
            }

            // Create the publication
            var publicationId = PublicationId.New();
            var publication = Publication.Create(
                publicationId,
                command.SpecializationId,
                command.UserId,
                command.Type,
                command.Title,
                command.PublicationDate);

            // Update basic details if provided
            if (!string.IsNullOrEmpty(command.Authors) || 
                !string.IsNullOrEmpty(command.Journal) || 
                !string.IsNullOrEmpty(command.Publisher))
            {
                publication.UpdateBasicDetails(
                    command.Type,
                    command.Title,
                    command.PublicationDate,
                    command.Authors,
                    command.Journal,
                    command.Publisher);
            }

            // Save the publication
            await _publicationRepository.AddAsync(publication);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success(publication.Id.Value);
        }
        catch (Exception ex) when (ex is CustomException)
        {
            return Result.Failure<int>(ex.Message);
        }
        catch (Exception)
        {
            return Result.Failure<int>("An error occurred while creating the publication.");
        }
    }
}