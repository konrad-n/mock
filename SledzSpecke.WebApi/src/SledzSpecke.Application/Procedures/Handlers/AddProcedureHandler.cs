using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Commands;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Exceptions;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Procedures.Handlers;

public sealed class AddProcedureHandler : IResultCommandHandler<AddProcedure, int>
{
    private readonly IProcedureRepository _procedureRepository;
    private readonly IInternshipRepository _internshipRepository;
    private readonly ISpecializationRepository _specializationRepository;
    private readonly ISpecializationValidationService _validationService;
    private readonly IYearCalculationService _yearCalculationService;
    private readonly IUnitOfWork _unitOfWork;

    public AddProcedureHandler(
        IProcedureRepository procedureRepository,
        IInternshipRepository internshipRepository,
        ISpecializationRepository specializationRepository,
        ISpecializationValidationService validationService,
        IYearCalculationService yearCalculationService,
        IUnitOfWork unitOfWork)
    {
        _procedureRepository = procedureRepository;
        _internshipRepository = internshipRepository;
        _specializationRepository = specializationRepository;
        _validationService = validationService;
        _yearCalculationService = yearCalculationService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<int>> HandleAsync(AddProcedure command)
    {
        try
        {
            // Validate internship exists
            var internshipId = new InternshipId(command.InternshipId);
            var internship = await _internshipRepository.GetByIdAsync(internshipId);
            if (internship is null)
            {
                return Result.Failure<int>($"Internship with ID {command.InternshipId} not found.");
            }

            // Get specialization to determine SMK version
            var specialization = await _specializationRepository.GetByIdAsync(internship.SpecializationId);
            if (specialization is null)
            {
                return Result.Failure<int>($"Specialization with ID {internship.SpecializationId.Value} not found.");
            }

            // Validate procedure date is within internship period
            if (command.Date < internship.StartDate || command.Date > internship.EndDate)
            {
                return Result.Failure<int>("Procedure date must be within the internship period.");
            }

            // Validate status
            if (!Enum.TryParse<ProcedureStatus>(command.Status, out var procedureStatus))
            {
                return Result.Failure<int>($"Invalid procedure status: {command.Status}");
            }

            // Validate year based on SMK version
            int procedureYear = command.Year;
            if (specialization.SmkVersion == SmkVersion.Old)
            {
                var availableYears = _yearCalculationService.GetAvailableYears(specialization);
                // Year 0 means "unassigned" in MAUI
                if (procedureYear != 0 && !availableYears.Contains(procedureYear))
                {
                    return Result.Failure<int>($"Year must be 0 (unassigned) or between {availableYears.Min()} and {availableYears.Max()} for this specialization.");
                }
            }

            // Validate required fields for completed procedures
            if (procedureStatus == ProcedureStatus.Completed)
            {
                if (specialization.SmkVersion == SmkVersion.Old)
                {
                    if (string.IsNullOrWhiteSpace(command.PerformingPerson))
                    {
                        return Result.Failure<int>("Performing person is required for completed procedures in Old SMK.");
                    }
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(command.Supervisor))
                    {
                        return Result.Failure<int>("Supervisor is required for completed procedures in New SMK.");
                    }
                }
            }

            // Create the appropriate procedure type based on SMK version
            var procedureId = ProcedureId.New();
            ProcedureBase procedure;

            if (specialization.SmkVersion == SmkVersion.Old)
            {
                procedure = ProcedureOldSmk.Create(
                    procedureId,
                    internshipId,
                    command.Date,
                    procedureYear,
                    command.Code,
                    command.Location);

                var oldSmkProcedure = (ProcedureOldSmk)procedure;

                if (!string.IsNullOrWhiteSpace(command.ProcedureGroup))
                {
                    oldSmkProcedure.SetProcedureGroup(command.ProcedureGroup);
                }

                if (!string.IsNullOrWhiteSpace(command.AssistantData))
                {
                    oldSmkProcedure.SetAssistantData(command.AssistantData);
                }

                if (command.ProcedureRequirementId.HasValue)
                {
                    oldSmkProcedure.SetProcedureRequirement(command.ProcedureRequirementId.Value);
                }
            }
            else
            {
                var moduleId = command.ModuleId ?? internship.ModuleId?.Value;
                if (!moduleId.HasValue)
                {
                    return Result.Failure<int>("Module ID is required for New SMK procedures");
                }

                var procedureRequirementId = command.ProcedureRequirementId ?? 0;
                var procedureName = command.ProcedureName ?? command.Code;

                procedure = ProcedureNewSmk.Create(
                    procedureId,
                    internshipId,
                    command.Date,
                    command.Code,
                    command.Location,
                    new ModuleId(moduleId.Value),
                    procedureRequirementId,
                    procedureName);

                var newSmkProcedure = (ProcedureNewSmk)procedure;

                if (!string.IsNullOrWhiteSpace(command.Supervisor))
                {
                    newSmkProcedure.SetSupervisor(command.Supervisor);
                }
            }

            // Set additional details
            if (!string.IsNullOrWhiteSpace(command.OperatorCode) ||
                !string.IsNullOrWhiteSpace(command.PerformingPerson) ||
                !string.IsNullOrWhiteSpace(command.PatientInitials))
            {
                procedure.UpdateProcedureDetails(
                    command.OperatorCode,
                    command.PerformingPerson,
                    command.PatientInitials,
                    command.PatientGender);
            }

            // Set procedure status
            if (procedureStatus != ProcedureStatus.Pending)
            {
                procedure.ChangeStatus(procedureStatus);
            }

            // Validate using template service before saving
            var validationResult = await _validationService.ValidateProcedureAsync(procedure, specialization.Id.Value);
            if (!validationResult.IsValid)
            {
                return Result.Failure<int>($"Procedure validation failed: {string.Join(", ", validationResult.Errors)}");
            }

            // Save the procedure
            var procedureIdValue = await _procedureRepository.AddAsync(procedure);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success(procedureIdValue);
        }
        catch (Exception ex) when (ex is CustomException)
        {
            return Result.Failure<int>(ex.Message);
        }
        catch (Exception)
        {
            return Result.Failure<int>("An error occurred while adding the procedure.");
        }
    }
}