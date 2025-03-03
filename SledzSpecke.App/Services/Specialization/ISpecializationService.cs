using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;

namespace SledzSpecke.App.Services.Specialization
{
    public interface ISpecializationService
    {
        // User & Specialization
        Task<User> GetCurrentUserAsync();

        Task<Models.Specialization> GetCurrentSpecializationAsync();

        Task<Module> GetCurrentModuleAsync();

        Task SetCurrentModuleAsync(int moduleId);

        // Specialization Program
        Task<SpecializationProgram> LoadSpecializationProgramAsync(string code, SmkVersion smkVersion);

        Task<List<SpecializationProgram>> GetAvailableSpecializationProgramsAsync();

        // Modules
        Task<List<Module>> GetModulesAsync(int specializationId);

        Task<bool> HasModulesAsync(int specializationId);

        // Internships
        Task<List<Internship>> GetInternshipsAsync(int? moduleId = null);

        Task<Internship> GetInternshipAsync(int internshipId);

        Task<Internship> GetCurrentInternshipAsync();

        Task<bool> AddInternshipAsync(Internship internship);

        Task<bool> UpdateInternshipAsync(Internship internship);

        Task<bool> DeleteInternshipAsync(int internshipId);

        // Medical Shifts
        Task<List<MedicalShift>> GetMedicalShiftsAsync(int? internshipId = null);

        Task<MedicalShift> GetMedicalShiftAsync(int shiftId);

        Task<bool> AddMedicalShiftAsync(MedicalShift shift);

        Task<bool> UpdateMedicalShiftAsync(MedicalShift shift);

        Task<bool> DeleteMedicalShiftAsync(int shiftId);

        // Procedures
        Task<List<Procedure>> GetProceduresAsync(string searchText = null, int? internshipId = null);

        Task<Procedure> GetProcedureAsync(int procedureId);

        Task<bool> AddProcedureAsync(Procedure procedure);

        Task<bool> UpdateProcedureAsync(Procedure procedure);

        Task<bool> DeleteProcedureAsync(int procedureId);

        // Courses
        Task<List<Course>> GetCoursesAsync(int? moduleId = null);

        Task<Course> GetCourseAsync(int courseId);

        Task<bool> AddCourseAsync(Course course);

        Task<bool> UpdateCourseAsync(Course course);

        Task<bool> DeleteCourseAsync(int courseId);

        // Self Education
        Task<List<SelfEducation>> GetSelfEducationItemsAsync(int? moduleId = null);

        Task<SelfEducation> GetSelfEducationAsync(int selfEducationId);

        Task<bool> AddSelfEducationAsync(SelfEducation selfEducation);

        Task<bool> UpdateSelfEducationAsync(SelfEducation selfEducation);

        Task<bool> DeleteSelfEducationAsync(int selfEducationId);

        // Publications
        Task<List<Publication>> GetPublicationsAsync(int? moduleId = null);

        Task<Publication> GetPublicationAsync(int publicationId);

        Task<bool> AddPublicationAsync(Publication publication);

        Task<bool> UpdatePublicationAsync(Publication publication);

        Task<bool> DeletePublicationAsync(int publicationId);

        // Educational Activities
        Task<List<EducationalActivity>> GetEducationalActivitiesAsync(int? moduleId = null);

        Task<EducationalActivity> GetEducationalActivityAsync(int activityId);

        Task<bool> AddEducationalActivityAsync(EducationalActivity activity);

        Task<bool> UpdateEducationalActivityAsync(EducationalActivity activity);

        Task<bool> DeleteEducationalActivityAsync(int activityId);

        // Absences
        Task<List<Absence>> GetAbsencesAsync();

        Task<Absence> GetAbsenceAsync(int absenceId);

        Task<bool> AddAbsenceAsync(Absence absence);

        Task<bool> UpdateAbsenceAsync(Absence absence);

        Task<bool> DeleteAbsenceAsync(int absenceId);

        // Recognitions
        Task<List<Models.Recognition>> GetRecognitionsAsync();

        Task<Models.Recognition> GetRecognitionAsync(int recognitionId);

        Task<bool> AddRecognitionAsync(Models.Recognition recognition);

        Task<bool> UpdateRecognitionAsync(Models.Recognition recognition);

        Task<bool> DeleteRecognitionAsync(int recognitionId);

        // Statistics & Progress
        Task<SpecializationStatistics> GetSpecializationStatisticsAsync();

        Task UpdateSpecializationProgressAsync(int specializationId);

        Task UpdateModuleProgressAsync(int moduleId);

        Task<DateTime> CalculateSpecializationEndDateAsync(int specializationId);

        // Counts for dashboard
        Task<int> GetShiftCountAsync(int? moduleId = null);

        Task<int> GetProcedureCountAsync(int? moduleId = null);

        Task<int> GetInternshipCountAsync(int? moduleId = null);

        Task<int> GetCourseCountAsync(int? moduleId = null);

        Task<int> GetSelfEducationCountAsync(int? moduleId = null);

        Task<int> GetPublicationCountAsync(int? moduleId = null);

        Task<int> GetEducationalActivityCountAsync(int? moduleId = null);

        Task<int> GetAbsenceCountAsync();

        Task<int> GetRecognitionCountAsync();
    }
}