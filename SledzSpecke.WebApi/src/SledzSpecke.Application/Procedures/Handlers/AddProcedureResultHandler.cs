using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Commands;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Exceptions;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Procedures.Handlers;

public sealed class AddProcedureResultHandler : IResultCommandHandler<AddProcedure, int>
{
    private readonly IProcedureRepository _procedureRepository;
    private readonly IInternshipRepository _internshipRepository;
    private readonly ISpecializationRepository _specializationRepository;
    private readonly ISpecializationValidationService _validationService;
    private readonly IYearCalculationService _yearCalculationService;
    private readonly IUnitOfWork _unitOfWork;

    public AddProcedureResultHandler(
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
            var internship = await _internshipRepository.GetByIdAsync(command.InternshipId);
            if (internship is null)
            {
                return Result<int>.Failure($"Internship with ID {command.InternshipId} not found.", "INTERNSHIP_NOT_FOUND");
            }

            // Get specialization to determine SMK version
            var specialization = await _specializationRepository.GetByIdAsync(internship.SpecializationId);
            if (specialization is null)
            {
                return Result<int>.Failure($"Specialization with ID {internship.SpecializationId} not found.", "SPECIALIZATION_NOT_FOUND");
            }

            // Validate procedure code
            if (string.IsNullOrWhiteSpace(command.Code))
            {
                return Result<int>.Failure("Procedure code cannot be empty.", "INVALID_PROCEDURE_CODE");
            }

            // Validate location
            if (string.IsNullOrWhiteSpace(command.Location))
            {
                return Result<int>.Failure("Location cannot be empty.", "INVALID_LOCATION");
            }

            // Validate date within internship period
            if (command.Date < internship.StartDate || command.Date > internship.EndDate)
            {
                return Result<int>.Failure("Procedure date must be within the internship period.", "DATE_OUT_OF_RANGE");
            }

            // Validate status
            if (!Enum.TryParse<ProcedureStatus>(command.Status, out var procedureStatus))
            {
                return Result<int>.Failure($"Invalid procedure status: {command.Status}", "INVALID_STATUS");
            }

            // Create procedure based on SMK version using domain methods
            var procedureId = ProcedureId.New();
            Result result;
            ProcedureBase? createdProcedure = null;

            if (specialization.SmkVersion == SmkVersion.Old)
            {
                // Validate year for Old SMK
                int procedureYear = command.Year;
                var availableYears = _yearCalculationService.GetAvailableYears(specialization);
                
                if (procedureYear != 0 && !availableYears.Contains(procedureYear))
                {
                    return Result<int>.Failure($"Year must be 0 (unassigned) or between {availableYears.Min()} and {availableYears.Max()} for this specialization.", "INVALID_YEAR");
                }

                // Use domain method to add procedure
                var executionType = Enum.Parse<ProcedureExecutionType>(command.ExecutionType);
                var addResult = internship.AddProcedureOldSmk(
                    procedureId,
                    command.Date,
                    procedureYear,
                    command.Code,
                    command.Name,
                    command.Location,
                    executionType,
                    command.SupervisorName);

                if (addResult.IsFailure)
                {
                    return Result<int>.Failure(addResult.Error, addResult.ErrorCode);
                }

                createdProcedure = addResult.Value;
                
                // Set additional Old SMK specific fields
                var oldSmkProcedure = (ProcedureOldSmk)createdProcedure;
                
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
                // For New SMK
                var moduleId = command.ModuleId ?? internship.ModuleId?.Value;
                if (!moduleId.HasValue)
                {
                    return Result<int>.Failure("Module ID is required for New SMK procedures.", "MODULE_ID_REQUIRED");
                }

                var procedureRequirementId = command.ProcedureRequirementId ?? 0;
                var procedureName = command.ProcedureName ?? command.Code;

                // Use domain method to add procedure
                var executionType = Enum.Parse<ProcedureExecutionType>(command.ExecutionType);
                var addResult = internship.AddProcedureNewSmk(
                    procedureId,
                    command.Date,
                    command.Code,
                    command.Location,
                    new ModuleId(moduleId.Value),
                    procedureRequirementId,
                    procedureName,
                    executionType,
                    command.SupervisorName);

                if (addResult.IsFailure)
                {
                    return Result<int>.Failure(addResult.Error, addResult.ErrorCode);
                }

                createdProcedure = addResult.Value;
                
                // Set additional New SMK specific fields
                var newSmkProcedure = (ProcedureNewSmk)createdProcedure;
                
                if (!string.IsNullOrWhiteSpace(command.Supervisor))
                {
                    newSmkProcedure.SetSupervisor(command.Supervisor);
                }
            }

            // Set common procedure details
            if (!string.IsNullOrWhiteSpace(command.PerformingPerson) ||
                !string.IsNullOrWhiteSpace(command.PatientInfo) ||
                !string.IsNullOrWhiteSpace(command.PatientInitials))
            {
                var executionType = Enum.Parse<ProcedureExecutionType>(command.ExecutionType);
                createdProcedure.UpdateProcedureDetails(
                    executionType,
                    command.PerformingPerson,
                    command.PatientInfo,
                    command.PatientInitials,
                    command.PatientGender ?? ' ');
            }
            
            // Set supervisor PWZ if provided
            if (!string.IsNullOrEmpty(command.SupervisorPwz))
            {
                createdProcedure.SetSupervisorPwz(command.SupervisorPwz);
            }

            // Validate before changing status to completed
            if (procedureStatus == ProcedureStatus.Completed)
            {
                if (specialization.SmkVersion == SmkVersion.Old && string.IsNullOrWhiteSpace(command.PerformingPerson))
                {
                    return Result<int>.Failure("Performing person is required for completed procedures in Old SMK.", "PERFORMING_PERSON_REQUIRED");
                }
                else if (specialization.SmkVersion == SmkVersion.New && string.IsNullOrWhiteSpace(command.Supervisor))
                {
                    return Result<int>.Failure("Supervisor is required for completed procedures in New SMK.", "SUPERVISOR_REQUIRED");
                }
            }

            // Set procedure status
            if (procedureStatus != ProcedureStatus.Pending)
            {
                createdProcedure.ChangeStatus(procedureStatus);
            }

            // Validate using template service
            var validationResult = await _validationService.ValidateProcedureAsync(createdProcedure, specialization.Id.Value);
            if (!validationResult.IsValid)
            {
                return Result<int>.Failure($"Procedure validation failed: {string.Join(", ", validationResult.Errors)}", "VALIDATION_FAILED");
            }

            // Update internship (already modified by domain method)
            await _internshipRepository.UpdateAsync(internship);
            
            // Save the procedure separately (since it's not automatically saved)
            var procedureIdValue = await _procedureRepository.AddAsync(createdProcedure);
            
            // Save changes
            await _unitOfWork.SaveChangesAsync();

            return Result<int>.Success(procedureIdValue);
        }
        catch (InvalidProcedureCodeException ex)
        {
            return Result<int>.Failure(ex.Message, "INVALID_PROCEDURE_CODE");
        }
        catch (Exception ex)
        {
            return Result<int>.Failure($"An error occurred while adding the procedure: {ex.Message}", "PROCEDURE_ADD_FAILED");
        }
    }
}