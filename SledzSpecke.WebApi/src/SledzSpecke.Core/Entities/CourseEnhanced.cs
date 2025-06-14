using SledzSpecke.Core.Exceptions;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.Entities;

/// <summary>
/// Enhanced Course entity using value objects to eliminate primitive obsession
/// </summary>
public class CourseEnhanced
{
    public CourseId Id { get; private set; }
    public SpecializationId SpecializationId { get; private set; }
    public ModuleId? ModuleId { get; private set; }
    public CourseType CourseType { get; private set; }
    public CourseName CourseName { get; private set; }
    public CourseNumber? CourseNumber { get; private set; }
    public InstitutionName InstitutionName { get; private set; }
    public DateTime CompletionDate { get; private set; }
    public bool HasCertificate { get; private set; }
    public CertificateNumber? CertificateNumber { get; private set; }
    public bool IsApproved { get; private set; }
    public DateTime? ApprovalDate { get; private set; }
    public PersonName? ApproverName { get; private set; }
    public SyncStatus SyncStatus { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private CourseEnhanced(
        CourseId id, 
        SpecializationId specializationId, 
        CourseType courseType,
        CourseName courseName, 
        InstitutionName institutionName, 
        DateTime completionDate)
    {
        Id = id;
        SpecializationId = specializationId;
        CourseType = courseType;
        CourseName = courseName;
        InstitutionName = institutionName;
        CompletionDate = completionDate;
        SyncStatus = SyncStatus.NotSynced;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public static CourseEnhanced Create(
        CourseId id, 
        SpecializationId specializationId,
        CourseType courseType, 
        string courseName, 
        string institutionName, 
        DateTime completionDate)
    {
        if (completionDate > DateTime.UtcNow.Date)
            throw new ArgumentException("Completion date cannot be in the future.", nameof(completionDate));

        // Value objects handle their own validation
        var courseNameVO = new CourseName(courseName);
        var institutionNameVO = new InstitutionName(institutionName);

        return new CourseEnhanced(id, specializationId, courseType, courseNameVO, institutionNameVO, completionDate);
    }

    public void AssignToModule(ModuleId moduleId)
    {
        EnsureCanModify();
        ModuleId = moduleId;
        UpdatedAt = DateTime.UtcNow;

        // Automatically transition from Synced to Modified
        if (SyncStatus == SyncStatus.Synced)
        {
            SyncStatus = SyncStatus.Modified;
        }
    }

    public void SetCourseNumber(string courseNumber)
    {
        EnsureCanModify();
        
        // Value object handles validation
        CourseNumber = new CourseNumber(courseNumber);
        UpdatedAt = DateTime.UtcNow;

        // Automatically transition from Synced to Modified
        if (SyncStatus == SyncStatus.Synced)
        {
            SyncStatus = SyncStatus.Modified;
        }
    }

    public void SetCertificate(string certificateNumber)
    {
        EnsureCanModify();

        // Value object handles validation
        HasCertificate = true;
        CertificateNumber = new CertificateNumber(certificateNumber);
        UpdatedAt = DateTime.UtcNow;

        // Automatically transition from Synced to Modified
        if (SyncStatus == SyncStatus.Synced)
        {
            SyncStatus = SyncStatus.Modified;
        }
    }

    public void RemoveCertificate()
    {
        EnsureCanModify();
        HasCertificate = false;
        CertificateNumber = null;
        UpdatedAt = DateTime.UtcNow;

        // Automatically transition from Synced to Modified
        if (SyncStatus == SyncStatus.Synced)
        {
            SyncStatus = SyncStatus.Modified;
        }
    }

    public void Approve(string approverName)
    {
        // Value object handles validation
        var approverNameVO = new PersonName(approverName);
        
        IsApproved = true;
        ApprovalDate = DateTime.UtcNow;
        ApproverName = approverNameVO;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Reject()
    {
        IsApproved = false;
        ApprovalDate = null;
        ApproverName = null;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateSyncStatus(SyncStatus syncStatus)
    {
        SyncStatus = syncStatus;
        UpdatedAt = DateTime.UtcNow;
    }

    // Method to update course details
    public void UpdateDetails(string? courseName, string? institutionName, DateTime? completionDate)
    {
        EnsureCanModify();

        if (courseName != null)
        {
            CourseName = new CourseName(courseName);
        }

        if (institutionName != null)
        {
            InstitutionName = new InstitutionName(institutionName);
        }

        if (completionDate.HasValue)
        {
            if (completionDate.Value > DateTime.UtcNow.Date)
                throw new ArgumentException("Completion date cannot be in the future.");
            
            CompletionDate = completionDate.Value;
        }

        UpdatedAt = DateTime.UtcNow;

        // Automatically transition from Synced to Modified
        if (SyncStatus == SyncStatus.Synced)
        {
            SyncStatus = SyncStatus.Modified;
        }
    }

    public bool CanBeDeleted()
    {
        return SyncStatus == SyncStatus.NotSynced || SyncStatus == SyncStatus.SyncFailed;
    }

    public bool IsMandatory()
    {
        return CourseType == CourseType.Mandatory || CourseType == CourseType.Attestation;
    }

    /// <summary>
    /// Ensures the course can be modified.
    /// Only approved courses cannot be modified (they are locked).
    /// Synced courses CAN be modified - they will automatically transition to Modified status.
    /// </summary>
    private void EnsureCanModify()
    {
        if (IsApproved)
            throw new InvalidOperationException("Cannot modify approved course.");
    }
}