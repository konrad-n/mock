using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.DTO;

public record UserDto(
    int Id,
    string Email,
    string FullName,
    SmkVersion SmkVersion,
    int SpecializationId,
    DateTime RegistrationDate
);