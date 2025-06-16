using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Queries;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.DomainServices;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Repositories;

namespace SledzSpecke.Application.Queries.Handlers;

public sealed class ExportSpecializationToXlsxHandler : IQueryHandler<ExportSpecializationToXlsx, byte[]>
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
    private readonly ISmkExcelGenerator _excelGenerator;
    private readonly IUserContextService _userContextService;

    public ExportSpecializationToXlsxHandler(
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
        ISmkExcelGenerator excelGenerator,
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
        _excelGenerator = excelGenerator;
        _userContextService = userContextService;
    }

    public async Task<byte[]> HandleAsync(ExportSpecializationToXlsx query)
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
        var medicalShifts = new List<MedicalShift>();
        var procedures = new List<ProcedureBase>();
        var courses = new List<Course>();
        var selfEducation = new List<SelfEducation>();
        var additionalDays = new List<AdditionalSelfEducationDays>();
        
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
            
            foreach (var internship in moduleInternships)
            {
                var internshipProcedures = await _procedureRepository.GetByInternshipIdAsync(internship.Id);
                procedures.AddRange(internshipProcedures);
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
            procedures,
            courses,
            selfEducation,
            additionalDays);
        
        // Create export data
        var exportData = new SmkExportData
        {
            User = user,
            Specialization = specialization,
            SmkVersion = specialization.SmkVersion,
            Modules = modules,
            Internships = internships,
            MedicalShifts = medicalShifts,
            Procedures = procedures,
            Courses = courses,
            SelfEducation = selfEducation,
            AdditionalDays = additionalDays,
            ValidationResult = validationResult.Value
        };
        
        // Generate Excel file
        return await _excelGenerator.GenerateSmkExportAsync(exportData);
    }
}