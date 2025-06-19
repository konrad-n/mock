using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;
using SledzSpecke.Application.Queries;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.DomainServices;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Queries.Handlers;

public sealed class PreviewSmkExportHandler : IQueryHandler<PreviewSmkExport, SmkExportPreviewDto>
{
    private readonly IUserRepository _userRepository;
    private readonly ISpecializationRepository _specializationRepository;
    private readonly IModuleRepository _moduleRepository;
    private readonly IInternshipRepository _internshipRepository;
    private readonly IMedicalShiftRepository _medicalShiftRepository;
    private readonly IProcedureRepository _procedureRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly ISelfEducationRepository _selfEducationRepository;
    private readonly IAdditionalSelfEducationDaysRepository _additionalDaysRepository;
    private readonly ISmkComplianceValidator _smkValidator;
    private readonly IUserContextService _userContextService;

    public PreviewSmkExportHandler(
        IUserRepository userRepository,
        ISpecializationRepository specializationRepository,
        IModuleRepository moduleRepository,
        IInternshipRepository internshipRepository,
        IMedicalShiftRepository medicalShiftRepository,
        IProcedureRepository procedureRepository,
        ICourseRepository courseRepository,
        ISelfEducationRepository selfEducationRepository,
        IAdditionalSelfEducationDaysRepository additionalDaysRepository,
        ISmkComplianceValidator smkValidator,
        IUserContextService userContextService)
    {
        _userRepository = userRepository;
        _specializationRepository = specializationRepository;
        _moduleRepository = moduleRepository;
        _internshipRepository = internshipRepository;
        _medicalShiftRepository = medicalShiftRepository;
        _procedureRepository = procedureRepository;
        _courseRepository = courseRepository;
        _selfEducationRepository = selfEducationRepository;
        _additionalDaysRepository = additionalDaysRepository;
        _smkValidator = smkValidator;
        _userContextService = userContextService;
    }

    public async Task<SmkExportPreviewDto> HandleAsync(PreviewSmkExport query)
    {
        var userId = _userContextService.GetUserId();
        
        // Get specialization and verify ownership
        var specialization = await _specializationRepository.GetByIdAsync(query.SpecializationId);
        if (specialization is null || specialization.UserId != userId)
        {
            throw new UnauthorizedAccessException("Specialization not found or access denied");
        }
        
        // Get user data
        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null)
        {
            throw new InvalidOperationException("User not found");
        }
        
        // Get all related data
        var modules = await _moduleRepository.GetBySpecializationIdAsync(query.SpecializationId);
        var moduleIds = modules.Select(m => m.Id).ToList();
        
        var internships = await _internshipRepository.GetBySpecializationIdAsync(query.SpecializationId);
        var medicalShifts = 0;
        var procedures = 0;
        var courses = 0;
        var selfEducationDays = 0.0;
        
        // Count data for each module
        foreach (var moduleId in moduleIds)
        {
            // Get shifts for all internships in this module
            var moduleInternships = internships.Where(i => i.ModuleId == moduleId);
            foreach (var internship in moduleInternships)
            {
                var internshipShifts = await _medicalShiftRepository.GetByInternshipIdAsync(internship.Id);
                medicalShifts += internshipShifts.Count();
            }
            
            foreach (var internship in moduleInternships)
            {
                var internshipProcedures = await _procedureRepository.GetByInternshipIdAsync(internship.Id);
                procedures += internshipProcedures.Count();
            }
            
            var allCourses = await _courseRepository.GetBySpecializationIdAsync(query.SpecializationId);
            courses += allCourses.Count(c => c.ModuleId == moduleId);
            
            var allSelfEdu = await _selfEducationRepository.GetByUserIdAsync(userId);
            var moduleSelfEdu = allSelfEdu.Where(s => s.ModuleId == moduleId);
            selfEducationDays += moduleSelfEdu.Sum(s => s.Hours / 8.0);
            
            var moduleAdditionalDays = await _additionalDaysRepository.GetByModuleIdAsync(moduleId);
            selfEducationDays += moduleAdditionalDays.Sum(d => d.NumberOfDays);
        }
        
        // Quick validation check
        var validationWarnings = new List<string>();
        
        if (internships.Count() == 0)
        {
            validationWarnings.Add("Brak staży - wymagany co najmniej jeden staż");
        }
        
        if (medicalShifts == 0)
        {
            validationWarnings.Add("Brak dyżurów medycznych");
        }
        
        if (procedures == 0)
        {
            validationWarnings.Add("Brak wykonanych procedur");
        }
        
        if (courses == 0)
        {
            validationWarnings.Add("Brak ukończonych kursów");
        }
        
        if (selfEducationDays < 5)
        {
            validationWarnings.Add($"Niewystarczająca liczba dni samokształcenia: {selfEducationDays:F1} (minimum 5)");
        }
        
        return new SmkExportPreviewDto
        {
            SpecializationId = query.SpecializationId,
            UserName = $"{user.FirstName.Value} {user.LastName.Value}",
            SpecializationName = specialization.Name,
            SmkVersion = specialization.SmkVersion == SmkVersion.Old ? "old" : "new",
            TotalInternships = internships.Count(),
            TotalCourses = courses,
            TotalMedicalShifts = medicalShifts,
            TotalProcedures = procedures,
            TotalSelfEducationDays = (int)Math.Round(selfEducationDays),
            ValidationStatus = validationWarnings.Any() ? "Ostrzeżenia" : "OK",
            ValidationWarnings = validationWarnings
        };
    }
}