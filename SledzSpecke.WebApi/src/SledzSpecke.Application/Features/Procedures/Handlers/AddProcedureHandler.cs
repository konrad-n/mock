using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Commands;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Exceptions;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Features.Procedures.Handlers;

public sealed class AddProcedureHandler : ICommandHandler<AddProcedure, int>
{
    private readonly IProcedureRepository _procedureRepository;
    private readonly IInternshipRepository _internshipRepository;
    private readonly ISpecializationRepository _specializationRepository;
    private readonly ISpecializationValidationService _validationService;
    private readonly IYearCalculationService _yearCalculationService;

    public AddProcedureHandler(
        IProcedureRepository procedureRepository,
        IInternshipRepository internshipRepository,
        ISpecializationRepository specializationRepository,
        ISpecializationValidationService validationService,
        IYearCalculationService yearCalculationService)
    {
        _procedureRepository = procedureRepository;
        _internshipRepository = internshipRepository;
        _specializationRepository = specializationRepository;
        _validationService = validationService;
        _yearCalculationService = yearCalculationService;
    }

    public async Task<int> HandleAsync(AddProcedure command)
    {
        Console.WriteLine($"[DEBUG] AddProcedureHandler: Processing procedure for internship {command.InternshipId}");

        // Validate internship exists
        var internship = await _internshipRepository.GetByIdAsync(command.InternshipId);
        if (internship is null)
        {
            Console.WriteLine($"[ERROR] Internship with ID {command.InternshipId} not found");
            throw new InvalidOperationException($"Internship with ID {command.InternshipId} not found.");
        }
        Console.WriteLine($"[DEBUG] Found internship: {internship.InternshipId.Value}, Specialization: {internship.SpecializationId.Value}");

        // Get specialization to determine SMK version
        var specialization = await _specializationRepository.GetByIdAsync(internship.SpecializationId);
        if (specialization is null)
        {
            Console.WriteLine($"[ERROR] Specialization with ID {internship.SpecializationId} not found");
            throw new InvalidOperationException($"Specialization with ID {internship.SpecializationId} not found.");
        }
        Console.WriteLine($"[DEBUG] Specialization: {specialization.ProgramCode}, SMK Version: {specialization.SmkVersion}");

        // Validate procedure code
        if (string.IsNullOrWhiteSpace(command.Code))
        {
            throw new InvalidProcedureCodeException(command.Code ?? string.Empty);
        }

        // Validate location
        if (string.IsNullOrWhiteSpace(command.Location))
        {
            throw new ArgumentException("Location cannot be empty.", nameof(command.Location));
        }

        // No future date validation - MAUI app allows future dates

        if (command.Date < internship.StartDate || command.Date > internship.EndDate)
        {
            throw new ArgumentException("Procedure date must be within the internship period.", nameof(command.Date));
        }

        // Validate status
        if (!Enum.TryParse<ProcedureStatus>(command.Status, out var procedureStatus))
        {
            throw new ArgumentException($"Invalid procedure status: {command.Status}", nameof(command.Status));
        }

        // Create procedure based on SMK version
        var procedureId = ProcedureId.New();

        // Validate year based on specialization
        int procedureYear = command.Year;
        if (specialization.SmkVersion == SmkVersion.Old)
        {
            // For Old SMK, validate the year is within available years
            var availableYears = _yearCalculationService.GetAvailableYears(specialization);

            // AI HINT: Year 0 is special - it means "unassigned" in MAUI
            // This is different from the procedure date's calendar year!
            // The Year field represents the medical education year (1-6), not 2024/2025
            if (procedureYear != 0 && !availableYears.Contains(procedureYear))
            {
                throw new ArgumentException($"Year must be 0 (unassigned) or between {availableYears.Min()} and {availableYears.Max()} for this specialization.");
            }
        }
        else
        {
            // For New SMK, year should typically be 0 (not used)
            if (procedureYear != 0)
            {
                // Log warning but allow it for backward compatibility
                // In production, you might want to log this
            }
        }

        // Create the appropriate procedure type based on SMK version
        Console.WriteLine($"[DEBUG] Creating procedure: SMK={specialization.SmkVersion}, Year={procedureYear}, Code={command.Code}");

        ProcedureBase procedure;
        try
        {
            if (specialization.SmkVersion == SmkVersion.Old)
            {
                Console.WriteLine($"[DEBUG] Creating ProcedureOldSmk");
                var moduleId = command.ModuleId ?? internship.ModuleId?.Value ?? 1; // Default to 1 if not specified
                var executionType = Enum.Parse<ProcedureExecutionType>(command.ExecutionType);
                
                procedure = ProcedureOldSmk.Create(
                    procedureId,
                    new ModuleId(moduleId),
                    internship.Id,
                    command.Date,
                    procedureYear,
                    command.Code,
                    command.Name,
                    command.Location,
                    executionType,
                    command.SupervisorName);
            }
            else
            {
                Console.WriteLine($"[DEBUG] Creating ProcedureNewSmk");
                // For New SMK, we need moduleId, procedureRequirementId and procedureName
                var moduleId = command.ModuleId ?? internship.ModuleId?.Value ??
                    throw new InvalidOperationException("Module ID is required for New SMK procedures");
                var procedureRequirementId = command.ProcedureRequirementId ?? 0; // Will be validated later
                var procedureName = command.ProcedureName ?? command.Name ?? command.Code; // Use name or code as fallback
                var executionType = Enum.Parse<ProcedureExecutionType>(command.ExecutionType);

                procedure = ProcedureNewSmk.Create(
                    procedureId,
                    new ModuleId(moduleId),
                    internship.Id,
                    command.Date,
                    command.Code,
                    procedureName,
                    command.Location,
                    executionType,
                    command.SupervisorName,
                    procedureRequirementId);
            }
            Console.WriteLine($"[DEBUG] Procedure entity created successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Failed to create procedure entity: {ex.GetType().Name}: {ex.Message}");
            throw;
        }

        // Set additional fields
        if (!string.IsNullOrWhiteSpace(command.PerformingPerson) ||
            !string.IsNullOrWhiteSpace(command.PatientInfo) ||
            !string.IsNullOrWhiteSpace(command.PatientInitials))
        {
            var executionType = Enum.Parse<ProcedureExecutionType>(command.ExecutionType);
            procedure.UpdateProcedureDetails(
                executionType,
                command.PerformingPerson,
                command.PatientInfo,
                command.PatientInitials,
                command.PatientGender ?? ' ');
        }
        
        // Set supervisor PWZ if provided
        if (!string.IsNullOrEmpty(command.SupervisorPwz))
        {
            procedure.SetSupervisorPwz(command.SupervisorPwz);
        }

        // Set SMK-specific fields
        if (specialization.SmkVersion == SmkVersion.Old)
        {
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
            var newSmkProcedure = (ProcedureNewSmk)procedure;

            if (!string.IsNullOrWhiteSpace(command.Supervisor))
            {
                newSmkProcedure.SetSupervisor(command.Supervisor);
            }

            // Module ID is already set during creation, so we don't need to set it again
        }

        // SMK version-specific validation for completed procedures
        if (procedureStatus == ProcedureStatus.Completed)
        {
            if (specialization.SmkVersion == SmkVersion.Old)
            {
                // For Old SMK, performing person is required for completed procedures
                if (string.IsNullOrWhiteSpace(command.PerformingPerson))
                {
                    throw new InvalidOperationException("Performing person is required for completed procedures in Old SMK.");
                }
            }
            else // New SMK
            {
                // For New SMK, supervisor is required for completed procedures
                if (string.IsNullOrWhiteSpace(command.Supervisor))
                {
                    throw new InvalidOperationException("Supervisor is required for completed procedures in New SMK.");
                }
            }
        }

        // Set procedure status
        if (procedureStatus != ProcedureStatus.Pending)
        {
            procedure.ChangeStatus(procedureStatus);
        }

        // Additional validation based on status
        if (procedureStatus == ProcedureStatus.Completed)
        {
            try
            {
                procedure.ValidateSmkSpecificRules();
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException($"Cannot add completed procedure: {ex.Message}", ex);
            }
        }

        // Validate using template service before saving
        var validationResult = await _validationService.ValidateProcedureAsync(procedure, specialization.Id.Value);
        if (!validationResult.IsValid)
        {
            throw new InvalidOperationException($"Procedure validation failed: {string.Join(", ", validationResult.Errors)}");
        }

        // Log warnings if any
        if (validationResult.Warnings.Any())
        {
            // In a real app, you might want to log these warnings
            // For now, we'll just continue
        }

        // Save the procedure
        var procedureIdValue = await _procedureRepository.AddAsync(procedure);

        return procedureIdValue;
    }
}