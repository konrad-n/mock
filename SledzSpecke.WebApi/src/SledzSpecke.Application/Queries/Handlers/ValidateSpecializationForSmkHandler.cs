using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;
using SledzSpecke.Application.Queries;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.DomainServices;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Queries.Handlers;

public sealed class ValidateSpecializationForSmkHandler : IQueryHandler<ValidateSpecializationForSmk, SmkValidationResultDto>
{
    private readonly ISpecializationRepository _specializationRepository;
    private readonly IModuleRepository _moduleRepository;
    private readonly IInternshipRepository _internshipRepository;
    private readonly IMedicalShiftRepository _medicalShiftRepository;
    private readonly IProcedureRealizationRepository _procedureRealizationRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly ISelfEducationRepository _selfEducationRepository;
    private readonly IAdditionalSelfEducationDaysRepository _additionalDaysRepository;
    private readonly ISmkComplianceValidator _smkValidator;
    private readonly IUserContextService _userContextService;

    public ValidateSpecializationForSmkHandler(
        ISpecializationRepository specializationRepository,
        IModuleRepository moduleRepository,
        IInternshipRepository internshipRepository,
        IMedicalShiftRepository medicalShiftRepository,
        IProcedureRealizationRepository procedureRealizationRepository,
        ICourseRepository courseRepository,
        ISelfEducationRepository selfEducationRepository,
        IAdditionalSelfEducationDaysRepository additionalDaysRepository,
        ISmkComplianceValidator smkValidator,
        IUserContextService userContextService)
    {
        _specializationRepository = specializationRepository;
        _moduleRepository = moduleRepository;
        _internshipRepository = internshipRepository;
        _medicalShiftRepository = medicalShiftRepository;
        _procedureRealizationRepository = procedureRealizationRepository;
        _courseRepository = courseRepository;
        _selfEducationRepository = selfEducationRepository;
        _additionalDaysRepository = additionalDaysRepository;
        _smkValidator = smkValidator;
        _userContextService = userContextService;
    }

    public async Task<SmkValidationResultDto> HandleAsync(ValidateSpecializationForSmk query)
    {
        var userId = _userContextService.GetUserId();
        
        // Get specialization and verify ownership
        var specialization = await _specializationRepository.GetByIdAsync(query.SpecializationId);
        if (specialization is null || specialization.UserId != userId)
        {
            throw new UnauthorizedAccessException("Specialization not found or access denied");
        }
        
        // Get all related data
        var modules = await _moduleRepository.GetBySpecializationIdAsync(query.SpecializationId);
        var moduleIds = modules.Select(m => m.Id).ToList();
        
        var internships = await _internshipRepository.GetBySpecializationIdAsync(query.SpecializationId);
        var medicalShifts = new List<MedicalShift>();
        var procedures = new List<ProcedureRealization>();
        var courses = new List<Course>();
        var selfEducation = new List<SelfEducation>();
        var additionalDays = new List<Core.Entities.AdditionalSelfEducationDays>();
        
        // Get all procedures for the user (procedures are user-based, not internship-based)
        var userProcedures = await _procedureRealizationRepository.GetByUserIdAsync(new UserId(userId));
        procedures.AddRange(userProcedures);
        
        // Get data for each module
        foreach (var moduleId in moduleIds)
        {
            // Get shifts for all internships in this module
            var moduleInternships = internships.Where(i => i.ModuleId == moduleId);
            foreach (var internship in moduleInternships)
            {
                var internshipShifts = await _medicalShiftRepository.GetByInternshipIdAsync(internship.Id);
                medicalShifts.AddRange(internshipShifts);
            }
            
            
            var moduleCourses = await _courseRepository.GetBySpecializationIdAsync(query.SpecializationId);
            courses.AddRange(moduleCourses.Where(c => c.ModuleId == moduleId));
            
            var moduleSelfEdu = await _selfEducationRepository.GetByUserIdAsync(userId);
            selfEducation.AddRange(moduleSelfEdu.Where(s => s.ModuleId == moduleId));
            
            var moduleAdditionalDays = await _additionalDaysRepository.GetByModuleIdAsync(moduleId);
            additionalDays.AddRange(moduleAdditionalDays);
        }
        
        // Validate the data
        var validationResult = await _smkValidator.ValidateSpecializationAsync(
            specialization,
            modules,
            internships,
            medicalShifts,
            procedures, // Now using ProcedureRealization
            courses,
            selfEducation,
            additionalDays);
        
        if (validationResult.IsFailure)
        {
            return new SmkValidationResultDto
            {
                SpecializationId = query.SpecializationId,
                SmkVersion = specialization.SmkVersion == SmkVersion.Old ? "old" : "new",
                IsValid = false,
                ValidationDate = DateTime.UtcNow,
                TotalErrors = 1,
                UserDataErrors = new List<string> { validationResult.Error }
            };
        }
        
        var result = validationResult.Value;
        
        return new SmkValidationResultDto
        {
            SpecializationId = query.SpecializationId,
            SmkVersion = specialization.SmkVersion == SmkVersion.Old ? "old" : "new",
            IsValid = result.IsValid,
            ValidationDate = result.ValidationDate,
            TotalErrors = result.TotalErrors,
            UserDataErrors = result.UserDataErrors,
            MedicalShiftErrors = result.MedicalShiftErrors,
            ProcedureErrors = result.ProcedureErrors,
            ModuleErrors = result.ModuleErrors,
            ModuleValidations = result.ModuleValidations.Select(m => new ModuleValidationInfo
            {
                ModuleId = m.ModuleId,
                ModuleName = m.ModuleName,
                IsValid = m.IsValid,
                Errors = m.ValidationErrors,
                Warnings = m.Warnings
            }).ToList()
        };
    }
}