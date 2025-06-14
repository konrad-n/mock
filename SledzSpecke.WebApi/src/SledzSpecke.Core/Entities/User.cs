using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.Entities;

public class User
{
    public UserId Id { get; private set; }
    public Email Email { get; private set; }
    public Username Username { get; private set; }
    public Password Password { get; private set; }
    public FullName FullName { get; private set; }
    public SmkVersion SmkVersion { get; private set; }
    public SpecializationId SpecializationId { get; private set; }
    public DateTime RegistrationDate { get; private set; }
    public string? PhoneNumber { get; private set; }
    public DateTime? DateOfBirth { get; private set; }
    public string? Bio { get; private set; }
    public string? ProfilePicturePath { get; private set; }
    public string? PreferredLanguage { get; private set; }
    public string? PreferredTheme { get; private set; }
    public bool NotificationsEnabled { get; private set; } = true;
    public bool EmailNotificationsEnabled { get; private set; } = true;
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastLoginAt { get; private set; }

    private User() { }

    public User(UserId id, Email email, Username username, Password password, FullName fullName,
        SmkVersion smkVersion, SpecializationId specializationId, DateTime registrationDate)
    {
        Id = id;
        Email = email;
        Username = username;
        Password = password;
        FullName = fullName;
        SmkVersion = smkVersion;
        SpecializationId = specializationId;
        RegistrationDate = registrationDate;
        CreatedAt = DateTime.UtcNow;
    }

    public User(Email email, Username username, Password password, FullName fullName,
        SmkVersion smkVersion, SpecializationId specializationId, DateTime registrationDate)
    {
        Email = email;
        Username = username;
        Password = password;
        FullName = fullName;
        SmkVersion = smkVersion;
        SpecializationId = specializationId;
        RegistrationDate = registrationDate;
        CreatedAt = DateTime.UtcNow;
    }

    public void ChangePassword(Password newPassword)
    {
        Password = newPassword;
    }

    public void UpdateProfile(Email email, FullName fullName)
    {
        Email = email;
        FullName = fullName;
    }

    public void UpdateProfileDetails(string? phoneNumber, DateTime? dateOfBirth, string? bio)
    {
        PhoneNumber = phoneNumber;
        DateOfBirth = dateOfBirth;
        Bio = bio;
    }

    public void UpdatePreferences(string? language, string? theme, bool notificationsEnabled, bool emailNotificationsEnabled)
    {
        PreferredLanguage = language;
        PreferredTheme = theme;
        NotificationsEnabled = notificationsEnabled;
        EmailNotificationsEnabled = emailNotificationsEnabled;
    }

    public void SetProfilePicturePath(string? path)
    {
        ProfilePicturePath = path;
    }

    public void UpdateLastLoginTime()
    {
        LastLoginAt = DateTime.UtcNow;
    }
}