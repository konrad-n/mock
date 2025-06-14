namespace SledzSpecke.Application.DTO;

public class UserProfileDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string SmkVersion { get; set; } = string.Empty;
    public int SpecializationId { get; set; }
    public string SpecializationName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Bio { get; set; }
    public string? ProfilePicturePath { get; set; }
    public UserPreferencesDto Preferences { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
}

public class UserPreferencesDto
{
    public string Language { get; set; } = "en";
    public string Theme { get; set; } = "light";
    public bool NotificationsEnabled { get; set; } = true;
    public bool EmailNotificationsEnabled { get; set; } = true;
}