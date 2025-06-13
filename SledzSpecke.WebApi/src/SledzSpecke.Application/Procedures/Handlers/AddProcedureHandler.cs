using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Commands;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Exceptions;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Procedures.Handlers;

internal sealed class AddProcedureHandler : ICommandHandler<AddProcedure, int>
{
    private readonly IProcedureRepository _procedureRepository;
    private readonly IInternshipRepository _internshipRepository;
    private readonly ISpecializationRepository _specializationRepository;
    private readonly ISpecializationValidationService _validationService;

    public AddProcedureHandler(
        IProcedureRepository procedureRepository,
        IInternshipRepository internshipRepository,
        ISpecializationRepository specializationRepository,
        ISpecializationValidationService validationService)
    {
        _procedureRepository = procedureRepository;
        _internshipRepository = internshipRepository;
        _specializationRepository = specializationRepository;
        _validationService = validationService;
    }

    public async Task<int> HandleAsync(AddProcedure command)
    {
        // Validate internship exists
        var internship = await _internshipRepository.GetByIdAsync(command.InternshipId);
        if (internship is null)
        {
            throw new InvalidOperationException($"Internship with ID {command.InternshipId} not found.");
        }

        // Get specialization to determine SMK version
        var specialization = await _specializationRepository.GetByIdAsync(internship.SpecializationId);
        if (specialization is null)
        {
            throw new InvalidOperationException($"Specialization with ID {internship.SpecializationId} not found.");
        }

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

        // Validate date
        if (command.Date > DateTime.UtcNow.Date)
        {
            throw new ArgumentException("Procedure date cannot be in the future.", nameof(command.Date));
        }

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
        
        // Create the procedure using the generic Procedure class
        var procedure = Procedure.Create(
            procedureId,
            internship.Id,
            command.Date,
            command.Code,
            command.Location,
            specialization.SmkVersion);

        // Set additional fields
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

        // SMK version-specific validation
        if (specialization.SmkVersion == SmkVersion.Old)
        {
            // For Old SMK, performing person is required for completed procedures
            if (procedureStatus == ProcedureStatus.Completed && string.IsNullOrWhiteSpace(command.PerformingPerson))
            {
                throw new InvalidOperationException("Performing person is required for completed procedures in Old SMK.");
            }
        }
        else // New SMK
        {
            // Check if operator code is required for New SMK procedures
            bool requiresOperatorCode = command.Code.StartsWith("A") || 
                                      command.Code.Contains("OPER") || 
                                      command.Code.Contains("SURG");
            
            if (requiresOperatorCode && string.IsNullOrWhiteSpace(command.OperatorCode))
            {
                throw new InvalidOperationException("Operator code is required for this procedure type in New SMK.");
            }
            
            // For New SMK, supervisor would be required for completed procedures
            // but since it's not in the command, we'll skip this validation for now
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