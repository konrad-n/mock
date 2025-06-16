using SledzSpecke.Core.Exceptions;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.Entities;

public class Course
{
    public CourseId Id { get; private set; }
    public SpecializationId SpecializationId { get; private set; }
    public ModuleId? ModuleId { get; private set; }
    public Module Module { get; private set; }
    public CourseType CourseType { get; private set; }
    public string CourseName { get; private set; }
    public string? CourseNumber { get; private set; }
    public string InstitutionName { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public DateTime CompletionDate { get; private set; }
    public int DurationDays { get; private set; }
    public int DurationHours { get; private set; }
    public bool HasCertificate { get; private set; }
    public string? CertificateNumber { get; private set; }
    public string? CmkpCertificateNumber { get; private set; }
    public bool IsVerifiedByCmkp { get; private set; }
    public DateTime? CmkpVerificationDate { get; private set; }
    public string OrganizerName { get; private set; }
    public string? Location { get; private set; }
    public bool IsApproved { get; private set; }
    public DateTime? ApprovalDate { get; private set; }
    public string? ApproverName { get; private set; }
    public SyncStatus SyncStatus { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private Course(CourseId id, SpecializationId specializationId, CourseType courseType,
        string courseName, string organizerName, string institutionName, DateTime startDate, 
        DateTime endDate, int durationDays, int durationHours)
    {
        Id = id;
        SpecializationId = specializationId;
        CourseType = courseType;
        CourseName = courseName;
        OrganizerName = organizerName;
        InstitutionName = institutionName;
        StartDate = startDate;
        EndDate = endDate;
        CompletionDate = endDate; // Default to end date
        DurationDays = durationDays;
        DurationHours = durationHours;
        SyncStatus = SyncStatus.NotSynced;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public static Course Create(CourseId id, SpecializationId specializationId,
        CourseType courseType, string courseName, string organizerName, string institutionName,
        DateTime startDate, DateTime endDate, int durationDays, int durationHours)
    {
        if (string.IsNullOrWhiteSpace(courseName))
            throw new ArgumentException("Course name cannot be empty.", nameof(courseName));

        if (string.IsNullOrWhiteSpace(organizerName))
            throw new ArgumentException("Organizer name cannot be empty.", nameof(organizerName));
            
        if (string.IsNullOrWhiteSpace(institutionName))
            throw new ArgumentException("Institution name cannot be empty.", nameof(institutionName));

        if (startDate > endDate)
            throw new ArgumentException("Start date cannot be after end date.", nameof(startDate));
            
        if (durationDays < 0)
            throw new ArgumentException("Duration days cannot be negative.", nameof(durationDays));
            
        if (durationHours < 0)
            throw new ArgumentException("Duration hours cannot be negative.", nameof(durationHours));

        return new Course(id, specializationId, courseType, courseName, organizerName, 
                        institutionName, startDate, endDate, durationDays, durationHours);
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
        CourseNumber = courseNumber;
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
        if (string.IsNullOrWhiteSpace(certificateNumber))
            throw new ArgumentException("Certificate number cannot be empty.", nameof(certificateNumber));

        HasCertificate = true;
        CertificateNumber = certificateNumber;
        UpdatedAt = DateTime.UtcNow;

        // Automatically transition from Synced to Modified
        if (SyncStatus == SyncStatus.Synced)
        {
            SyncStatus = SyncStatus.Modified;
        }
    }
    
    public void SetCmkpCertificate(string cmkpCertificateNumber)
    {
        EnsureCanModify();
        if (string.IsNullOrWhiteSpace(cmkpCertificateNumber))
            throw new ArgumentException("CMKP certificate number cannot be empty.", nameof(cmkpCertificateNumber));

        CmkpCertificateNumber = cmkpCertificateNumber;
        HasCertificate = true;
        UpdatedAt = DateTime.UtcNow;

        // Automatically transition from Synced to Modified
        if (SyncStatus == SyncStatus.Synced)
        {
            SyncStatus = SyncStatus.Modified;
        }
    }
    
    public void VerifyByCmkp(DateTime verificationDate)
    {
        EnsureCanModify();
        if (string.IsNullOrWhiteSpace(CmkpCertificateNumber))
            throw new InvalidOperationException("Cannot verify course without CMKP certificate number.");
            
        IsVerifiedByCmkp = true;
        CmkpVerificationDate = verificationDate;
        UpdatedAt = DateTime.UtcNow;

        // Automatically transition from Synced to Modified
        if (SyncStatus == SyncStatus.Synced)
        {
            SyncStatus = SyncStatus.Modified;
        }
    }
    
    public void SetLocation(string location)
    {
        EnsureCanModify();
        Location = location;
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
        return CourseType == CourseType.Specialization || CourseType == CourseType.Certification;
    }
    
    public bool RequiresCmkpCertificate()
    {
        // All courses in SMK require CMKP certificate
        return true;
    }
    
    public bool IsValid()
    {
        // Course is valid if it has CMKP certificate and is verified
        return !string.IsNullOrWhiteSpace(CmkpCertificateNumber) && IsVerifiedByCmkp;
    }

    /// <summary>
    /// Ensures the course can be modified.
    /// Only approved courses cannot be modified (they are locked).
    /// Synced courses CAN be modified - they will automatically transition to Modified status.
    /// This is a key change from the original design where synced items were read-only.
    /// </summary>
    private void EnsureCanModify()
    {
        if (IsApproved)
            throw new InvalidOperationException("Cannot modify approved course.");

        // IMPORTANT: Design Decision
        // Previously, synced items could not be modified at all (threw CannotModifySyncedDataException).
        // Now, synced items CAN be modified - they automatically transition to Modified status.
        // This allows users to correct/update synced data while maintaining the audit trail.
        // Only APPROVED items are truly read-only.
    }
}