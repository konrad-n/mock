using SledzSpecke.Core.Enums;

namespace SledzSpecke.Core.Entities;

public sealed class User
{
    public int UserId { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Name { get; set; }
    public string PhoneNumber { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string CorrespondenceAddress { get; set; }
    public DateTime RegistrationDate { get; set; }
    public bool NotificationsEnabled { get; set; }
    public bool EmailNotificationsEnabled { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public SmkVersion SmkVersion { get; set; }
    public SyncStatus SyncStatus { get; set; }

    // Private constructor for EF Core
    private User() { }

    // Factory method for creating a new user
    public static User Create(
        string email, 
        string password, 
        string name,
        string phoneNumber,
        DateTime dateOfBirth,
        string correspondenceAddress)
    {
        return new User
        {
            Email = email,
            Password = password,
            Name = name,
            PhoneNumber = phoneNumber,
            DateOfBirth = dateOfBirth,
            CorrespondenceAddress = correspondenceAddress,
            RegistrationDate = DateTime.UtcNow,
            NotificationsEnabled = true,
            EmailNotificationsEnabled = true,
            CreatedAt = DateTime.UtcNow,
            LastLoginAt = null,
            SmkVersion = SmkVersion.New,
            SyncStatus = SyncStatus.Unsynced
        };
    }

    // Factory method for creating a user with known ID (e.g., from database)
    public static User CreateWithId(
        int id,
        string email, 
        string password, 
        string name,
        string phoneNumber,
        DateTime dateOfBirth,
        string correspondenceAddress,
        DateTime registrationDate)
    {
        return new User
        {
            UserId = id,
            Email = email,
            Password = password,
            Name = name,
            PhoneNumber = phoneNumber,
            DateOfBirth = dateOfBirth,
            CorrespondenceAddress = correspondenceAddress,
            RegistrationDate = registrationDate,
            CreatedAt = registrationDate,
            NotificationsEnabled = true,
            EmailNotificationsEnabled = true,
            SmkVersion = SmkVersion.New,
            SyncStatus = SyncStatus.Unsynced
        };
    }

    public void ChangePassword(string newPassword)
    {
        if (string.IsNullOrEmpty(newPassword))
            throw new ArgumentNullException(nameof(newPassword));
            
        Password = newPassword;
    }

    public void UpdateProfile(
        string email, 
        string name,
        string phoneNumber,
        string correspondenceAddress)
    {
        if (string.IsNullOrEmpty(email))
            throw new ArgumentNullException(nameof(email));
        if (string.IsNullOrEmpty(name))
            throw new ArgumentNullException(nameof(name));
        if (string.IsNullOrEmpty(phoneNumber))
            throw new ArgumentNullException(nameof(phoneNumber));
        if (string.IsNullOrEmpty(correspondenceAddress))
            throw new ArgumentNullException(nameof(correspondenceAddress));
            
        Email = email;
        Name = name;
        PhoneNumber = phoneNumber;
        CorrespondenceAddress = correspondenceAddress;
    }

    public void UpdateNotificationPreferences(bool notificationsEnabled, bool emailNotificationsEnabled)
    {
        NotificationsEnabled = notificationsEnabled;
        EmailNotificationsEnabled = emailNotificationsEnabled;
    }

    public void RecordLogin()
    {
        LastLoginAt = DateTime.UtcNow;
    }
    
    // Business logic methods
    public bool IsProfileComplete()
    {
        return !string.IsNullOrEmpty(Email) && 
               !string.IsNullOrEmpty(Name) && 
               !string.IsNullOrEmpty(PhoneNumber);
    }
    
    public bool HasRecentActivity(int daysThreshold = 30)
    {
        return LastLoginAt.HasValue && 
               (DateTime.UtcNow - LastLoginAt.Value).TotalDays <= daysThreshold;
    }
}