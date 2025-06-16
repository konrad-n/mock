namespace SledzSpecke.Application.DTO;

public class UserProfileDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public string Pesel { get; set; } = string.Empty;
    public string PwzNumber { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public AddressDto CorrespondenceAddress { get; set; } = new();
    public UserPreferencesDto Preferences { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
}

public class UserPreferencesDto
{
    public bool NotificationsEnabled { get; set; } = true;
    public bool EmailNotificationsEnabled { get; set; } = true;
}

public class AddressDto
{
    public string Street { get; set; } = string.Empty;
    public string HouseNumber { get; set; } = string.Empty;
    public string? ApartmentNumber { get; set; }
    public string PostalCode { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Province { get; set; } = string.Empty;
}