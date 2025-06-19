using SledzSpecke.Application.Abstractions;

namespace SledzSpecke.Application.Commands;

public record SignUp(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string PhoneNumber,
    DateTime DateOfBirth,
    AddressDto CorrespondenceAddress,
    int SpecializationTemplateId,
    string SmkVersion
) : ICommand;

public record AddressDto(
    string Street,
    string HouseNumber,
    string? ApartmentNumber,
    string PostalCode,
    string City,
    string Province
);