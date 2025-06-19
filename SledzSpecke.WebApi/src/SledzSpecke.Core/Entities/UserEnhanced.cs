using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.Entities;

/// <summary>
/// Enhanced User entity using value objects to eliminate primitive obsession
/// </summary>
public class UserEnhanced
{
    public UserId Id { get; private set; }
    public Email Email { get; private set; }
    public Password Password { get; private set; }
    public FullName FullName { get; private set; }
    public SmkVersion SmkVersion { get; private set; }
    public SpecializationId SpecializationId { get; private set; }
    public DateTime RegistrationDate { get; private set; }
    public PhoneNumber? PhoneNumber { get; private set; }
    public DateTime? DateOfBirth { get; private set; }
    public UserBio Bio { get; private set; }
    public FilePath? ProfilePicturePath { get; private set; }
    public Language PreferredLanguage { get; private set; }
    public Theme PreferredTheme { get; private set; }
    public bool NotificationsEnabled { get; private set; }
    public bool EmailNotificationsEnabled { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastLoginAt { get; private set; }

    private UserEnhanced() 
    { 
        Bio = new UserBio(null);
        PreferredLanguage = Language.English;
        PreferredTheme = Theme.Light;
        NotificationsEnabled = true;
        EmailNotificationsEnabled = true;
    }

    public UserEnhanced(
        UserId id, 
        Email email, 
        Password password, 
        FullName fullName,
        SmkVersion smkVersion, 
        SpecializationId specializationId, 
        DateTime registrationDate)
    {
        Id = id;
        Email = email;
        Password = password;
        FullName = fullName;
        SmkVersion = smkVersion;
        SpecializationId = specializationId;
        RegistrationDate = registrationDate;
        Bio = new UserBio(null);
        PreferredLanguage = Language.English;
        PreferredTheme = Theme.Light;
        NotificationsEnabled = true;
        EmailNotificationsEnabled = true;
        CreatedAt = DateTime.UtcNow;
    }

    public UserEnhanced(
        Email email, 
        Password password, 
        FullName fullName,
        SmkVersion smkVersion, 
        SpecializationId specializationId, 
        DateTime registrationDate)
    {
        Email = email;
        Password = password;
        FullName = fullName;
        SmkVersion = smkVersion;
        SpecializationId = specializationId;
        RegistrationDate = registrationDate;
        Bio = new UserBio(null);
        PreferredLanguage = Language.English;
        PreferredTheme = Theme.Light;
        NotificationsEnabled = true;
        EmailNotificationsEnabled = true;
        CreatedAt = DateTime.UtcNow;
    }

    public void ChangePassword(Password newPassword)
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

    public void UpdateProfileDetails(string? phoneNumber, DateTime? dateOfBirth, string? bio)
    {
        PhoneNumber = phoneNumber != null ? new PhoneNumber(phoneNumber) : null;
        DateOfBirth = dateOfBirth;
        Bio = new UserBio(bio);
    }

    public void UpdatePreferences(string? language, string? theme, bool notificationsEnabled, bool emailNotificationsEnabled)
    {
        PreferredLanguage = language != null ? new Language(language) : Language.English;
        PreferredTheme = theme != null ? new Theme(theme) : Theme.Light;
        NotificationsEnabled = notificationsEnabled;
        EmailNotificationsEnabled = emailNotificationsEnabled;
    }

    public void SetProfilePicturePath(string? path)
    {
        ProfilePicturePath = path != null ? new FilePath(path) : null;
    }

    public void UpdateLastLoginTime()
    {
        LastLoginAt = DateTime.UtcNow;
    }

    public int GetAge()
    {
        if (!DateOfBirth.HasValue)
            return 0;

        var today = DateTime.UtcNow.Date;
        var age = today.Year - DateOfBirth.Value.Year;
        
        if (DateOfBirth.Value.Date > today.AddYears(-age))
            age--;

        return age;
    }

    public bool IsProfileComplete()
    {
        return PhoneNumber != null && 
               DateOfBirth.HasValue && 
               !Bio.IsEmpty && 
               ProfilePicturePath != null;
    }
}