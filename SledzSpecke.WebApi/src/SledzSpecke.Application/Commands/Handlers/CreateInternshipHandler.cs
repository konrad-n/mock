using SledzSpecke.Application.Abstractions;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Exceptions;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Commands.Handlers;

public sealed class CreateInternshipHandler : IResultCommandHandler<CreateInternship, int>
{
    private readonly IInternshipRepository _internshipRepository;
    private readonly ISpecializationRepository _specializationRepository;
    private readonly IModuleRepository _moduleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateInternshipHandler(
        IInternshipRepository internshipRepository,
        ISpecializationRepository specializationRepository,
        IModuleRepository moduleRepository,
        IUnitOfWork unitOfWork)
    {
        _internshipRepository = internshipRepository;
        _specializationRepository = specializationRepository;
        _moduleRepository = moduleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<int>> HandleAsync(CreateInternship command)
    {
        try
        {
            // Validate specialization exists
            var specialization = await _specializationRepository.GetByIdAsync(command.SpecializationId);
            if (specialization is null)
            {
                return Result.Failure<int>($"Specialization with ID {command.SpecializationId} not found.");
            }

            // Validate module exists if provided
            if (command.ModuleId.HasValue)
            {
                var module = await _moduleRepository.GetByIdAsync(command.ModuleId.Value);
                if (module is null)
                {
                    return Result.Failure<int>($"Module with ID {command.ModuleId.Value} not found.");
                }
                
                // Verify module belongs to the specialization
                if (module.SpecializationId != command.SpecializationId)
                {
                    return Result.Failure<int>("Module does not belong to the specified specialization.");
                }
            }

            // Validate date range
            if (command.StartDate >= command.EndDate)
            {
                return Result.Failure<int>("Start date must be before end date.");
            }

            // Create the internship
            var internshipId = InternshipId.New();
            var internship = Internship.Create(
                internshipId,
                command.SpecializationId,
                command.InstitutionName,
                command.DepartmentName,
                command.StartDate,
                command.EndDate);

            if (!string.IsNullOrWhiteSpace(command.SupervisorName))
            {
                internship.SetSupervisor(command.SupervisorName);
            }

            if (command.ModuleId.HasValue)
            {
                internship.AssignToModule(command.ModuleId.Value);
            }

            await _internshipRepository.AddAsync(internship);
            await _unitOfWork.SaveChangesAsync();
            
            return Result.Success(internship.Id.Value);
        }
        catch (Exception ex) when (ex is CustomException)
        {
            return Result.Failure<int>(ex.Message);
        }
        catch (Exception)
        {
            return Result.Failure<int>("An error occurred while creating the internship.");
        }
    }
}