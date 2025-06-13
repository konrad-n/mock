using SledzSpecke.Core.Exceptions;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.Entities;

public class Course
{
    public CourseId Id { get; private set; }
    public SpecializationId SpecializationId { get; private set; }
    public ModuleId? ModuleId { get; private set; }
    public CourseType CourseType { get; private set; }
    public string CourseName { get; private set; }
    public string? CourseNumber { get; private set; }
    public string InstitutionName { get; private set; }
    public DateTime CompletionDate { get; private set; }
    public bool HasCertificate { get; private set; }
    public string? CertificateNumber { get; private set; }
    public bool IsApproved { get; private set; }
    public DateTime? ApprovalDate { get; private set; }
    public string? ApproverName { get; private set; }
    public SyncStatus SyncStatus { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private Course(CourseId id, SpecializationId specializationId, CourseType courseType,
        string courseName, string institutionName, DateTime completionDate)
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

    public static Course Create(CourseId id, SpecializationId specializationId,
        CourseType courseType, string courseName, string institutionName, DateTime completionDate)
    {
        if (string.IsNullOrWhiteSpace(courseName))
            throw new ArgumentException("Course name cannot be empty.", nameof(courseName));
        
        if (string.IsNullOrWhiteSpace(institutionName))
            throw new ArgumentException("Institution name cannot be empty.", nameof(institutionName));
        
        if (completionDate > DateTime.UtcNow.Date)
            throw new ArgumentException("Completion date cannot be in the future.", nameof(completionDate));

        return new Course(id, specializationId, courseType, courseName, institutionName, completionDate);
    }

    public void AssignToModule(ModuleId moduleId)
    {
        EnsureCanModify();
        ModuleId = moduleId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetCourseNumber(string courseNumber)
    {
        EnsureCanModify();
        CourseNumber = courseNumber;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetCertificate(string certificateNumber)
    {
        EnsureCanModify();
        if (string.IsNullOrWhiteSpace(certificateNumber))
            throw new ArgumentException("Certificate number cannot be empty.", nameof(certificateNumber));
        
        HasCertificate = true;
        CertificateNumber = certificateNumber;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveCertificate()
    {
        EnsureCanModify();
        HasCertificate = false;
        CertificateNumber = null;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Approve(string approverName)
    {
        if (string.IsNullOrWhiteSpace(approverName))
            throw new ArgumentException("Approver name cannot be empty.", nameof(approverName));
        
        IsApproved = true;
        ApprovalDate = DateTime.UtcNow;
        ApproverName = approverName;
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

    public bool CanBeDeleted()
    {
        return SyncStatus == SyncStatus.NotSynced || SyncStatus == SyncStatus.SyncFailed;
    }

    public bool IsMandatory()
    {
        return CourseType == CourseType.Mandatory || CourseType == CourseType.Attestation;
    }

    private void EnsureCanModify()
    {
        if (SyncStatus == SyncStatus.Synced)
            throw new CannotModifySyncedDataException();
    }
}