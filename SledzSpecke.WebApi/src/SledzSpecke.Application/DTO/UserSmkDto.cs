namespace SledzSpecke.Application.DTO;

public record UserSmkDto(
    int Id,
    string Email,
    string FirstName,
    string LastName,
    string PhoneNumber,
    DateTime DateOfBirth,
    AddressDto CorrespondenceAddress,
    DateTime RegistrationDate,
    DateTime? LastLoginAt,
    bool NotificationsEnabled,
    bool EmailNotificationsEnabled
);