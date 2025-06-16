using SledzSpecke.Core.ValueObjects;
using SledzSpecke.Core.Exceptions;

namespace SledzSpecke.Core.Entities;

public sealed class User
{
    public UserId Id { get; private set; }
    public Email Email { get; private set; }
    public Username Username { get; private set; }
    public HashedPassword Password { get; private set; }
    public FullName FullName { get; private set; }
    public SmkVersion SmkVersion { get; private set; }
    public SpecializationId SpecializationId { get; private set; }
    public DateTime RegistrationDate { get; private set; }
    public PhoneNumber? PhoneNumber { get; private set; }
    public DateTime? DateOfBirth { get; private set; }
    public UserBio? Bio { get; private set; }
    public FilePath? ProfilePicturePath { get; private set; }
    public Language? PreferredLanguage { get; private set; }
    public Theme? PreferredTheme { get; private set; }
    public bool NotificationsEnabled { get; private set; }
    public bool EmailNotificationsEnabled { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastLoginAt { get; private set; }

    // Private constructor for EF Core
    private User() { }

    // Factory method for creating a new user
    public static User Create(
        Email email, 
        Username username, 
        HashedPassword password, 
        FullName fullName,
        SmkVersion smkVersion, 
        SpecializationId specializationId)
    {
        return new User
        {
            Id = new UserId(0), // Will be set by repository
            Email = email,
            Username = username,
            Password = password,
            FullName = fullName,
            SmkVersion = smkVersion,
            SpecializationId = specializationId,
            RegistrationDate = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            NotificationsEnabled = true,
            EmailNotificationsEnabled = true
        };
    }

    // Factory method for creating a user with known ID (e.g., from database)
    public static User CreateWithId(
        UserId id,
        Email email, 
        Username username, 
        HashedPassword password, 
        FullName fullName,
        SmkVersion smkVersion, 
        SpecializationId specializationId,
        DateTime registrationDate)
    {
        return new User
        {
            Id = id,
            Email = email,
            Username = username,
            Password = password,
            FullName = fullName,
            SmkVersion = smkVersion,
            SpecializationId = specializationId,
            RegistrationDate = registrationDate,
            CreatedAt = registrationDate, // Use registration date as created date for existing users
            NotificationsEnabled = true,
            EmailNotificationsEnabled = true
        };
    }
    
    // Internal method for repository to set ID after creation
    internal void SetId(UserId id)
    {
        if (Id != null && Id.Value != 0)
            throw new InvalidOperationException("Cannot change ID once it's set");
        Id = id;
    }

    public void ChangePassword(HashedPassword newPassword)
    {
        if (newPassword == null)
            throw new ArgumentNullException(nameof(newPassword));
            
        Password = newPassword;
    }

    public void UpdateProfile(Email email, FullName fullName)
    {
        if (email == null)
            throw new ArgumentNullException(nameof(email));
        if (fullName == null)
            throw new ArgumentNullException(nameof(fullName));
            
        Email = email;
        FullName = fullName;
    }

    public void UpdateProfileDetails(PhoneNumber? phoneNumber, DateTime? dateOfBirth, UserBio? bio)
    {
        PhoneNumber = phoneNumber;
        DateOfBirth = dateOfBirth;
        Bio = bio;
    }

    public void UpdatePreferences(Language? language, Theme? theme, bool notificationsEnabled, bool emailNotificationsEnabled)
    {
        PreferredLanguage = language;
        PreferredTheme = theme;
        NotificationsEnabled = notificationsEnabled;
        EmailNotificationsEnabled = emailNotificationsEnabled;
    }

    public void SetProfilePicture(FilePath? path)
    {
        ProfilePicturePath = path;
    }

    public void RecordLogin()
    {
        LastLoginAt = DateTime.UtcNow;
    }
    
    // Business logic methods
    public bool CanAccessSpecialization(SpecializationId specializationId)
    {
        return SpecializationId == specializationId;
    }
    
    public bool IsProfileComplete()
    {
        return PhoneNumber != null && 
               DateOfBirth != null && 
               Bio != null && !Bio.IsEmpty;
    }
    
    public bool HasRecentActivity(int daysThreshold = 30)
    {
        return LastLoginAt.HasValue && 
               (DateTime.UtcNow - LastLoginAt.Value).TotalDays <= daysThreshold;
    }
}