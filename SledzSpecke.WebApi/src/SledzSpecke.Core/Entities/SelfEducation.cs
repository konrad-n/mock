using SledzSpecke.Core.Exceptions;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.Entities;

/// <summary>
/// Represents self-education activities tracked in the SMK system
/// </summary>
public class SelfEducation
{
    public SelfEducationId Id { get; private set; }
    public ModuleId ModuleId { get; private set; }
    public Module Module { get; private set; }
    
    // SMK Fields
    public SelfEducationType Type { get; private set; }
    public string Description { get; private set; }
    public DateTime Date { get; private set; }
    public int Hours { get; private set; }
    
    // For publications
    public string? PublicationTitle { get; private set; }
    public string? JournalName { get; private set; }
    public bool IsPeerReviewed { get; private set; }
    public PublicationRole? Role { get; private set; }
    
    // Common fields
    public SyncStatus SyncStatus { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public bool CanBeModified => SyncStatus != SyncStatus.Synced;
    public bool IsPublication => Type == SelfEducationType.Publication;

    private SelfEducation(SelfEducationId id, ModuleId moduleId, SelfEducationType type,
        string description, DateTime date, int hours)
    {
        Id = id;
        ModuleId = moduleId;
        Type = type;
        Description = description;
        Date = date;
        Hours = hours;
        SyncStatus = SyncStatus.NotSynced;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public static SelfEducation Create(SelfEducationId id, ModuleId moduleId, SelfEducationType type,
        string description, DateTime date, int hours)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be empty.", nameof(description));
            
        if (hours <= 0)
            throw new ArgumentException("Hours must be greater than zero.", nameof(hours));
            
        if (date > DateTime.UtcNow.Date)
            throw new ArgumentException("Self-education date cannot be in the future.", nameof(date));
            
        return new SelfEducation(id, moduleId, type, description, date, hours);
    }
    
    public static SelfEducation CreatePublication(SelfEducationId id, ModuleId moduleId,
        string description, DateTime date, int hours, string publicationTitle, 
        string journalName, bool isPeerReviewed, PublicationRole role)
    {
        if (string.IsNullOrWhiteSpace(publicationTitle))
            throw new ArgumentException("Publication title cannot be empty.", nameof(publicationTitle));
            
        if (string.IsNullOrWhiteSpace(journalName))
            throw new ArgumentException("Journal name cannot be empty.", nameof(journalName));
            
        var selfEducation = Create(id, moduleId, SelfEducationType.Publication, description, date, hours);
        selfEducation.SetPublicationDetails(publicationTitle, journalName, isPeerReviewed, role);
        
        return selfEducation;
    }

    public void SetPublicationDetails(string publicationTitle, string journalName, 
        bool isPeerReviewed, PublicationRole role)
    {
        EnsureCanModify();
        
        if (Type != SelfEducationType.Publication)
            throw new InvalidOperationException("Publication details can only be set for Publication type.");
            
        PublicationTitle = publicationTitle;
        JournalName = journalName;
        IsPeerReviewed = isPeerReviewed;
        Role = role;
        UpdatedAt = DateTime.UtcNow;
        
        // Automatically transition from Synced to Modified
        if (SyncStatus == SyncStatus.Synced)
        {
            SyncStatus = SyncStatus.Modified;
        }
    }

    public void UpdateDescription(string description)
    {
        EnsureCanModify();
        
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be empty.", nameof(description));
            
        Description = description;
        UpdatedAt = DateTime.UtcNow;
        
        // Automatically transition from Synced to Modified
        if (SyncStatus == SyncStatus.Synced)
        {
            SyncStatus = SyncStatus.Modified;
        }
    }

    public void UpdateHours(int hours)
    {
        EnsureCanModify();
        
        if (hours <= 0)
            throw new ArgumentException("Hours must be greater than zero.", nameof(hours));
            
        Hours = hours;
        UpdatedAt = DateTime.UtcNow;
        
        // Automatically transition from Synced to Modified
        if (SyncStatus == SyncStatus.Synced)
        {
            SyncStatus = SyncStatus.Modified;
        }
    }

    public void SetSyncStatus(SyncStatus status)
    {
        SyncStatus = status;
        UpdatedAt = DateTime.UtcNow;
    }

    public int GetEducationPoints()
    {
        // Different types might have different point values in SMK
        return Type switch
        {
            SelfEducationType.Publication when IsPeerReviewed => Hours * 3,
            SelfEducationType.Publication => Hours * 2,
            SelfEducationType.Conference => Hours * 2,
            SelfEducationType.Workshop => Hours * 2,
            _ => Hours
        };
    }

    private void EnsureCanModify()
    {
        // Similar to other entities, synced items can be modified
        // They will transition to Modified status
    }

}