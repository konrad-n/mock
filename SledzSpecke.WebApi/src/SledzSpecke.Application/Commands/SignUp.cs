using SledzSpecke.Application.Abstractions;

namespace SledzSpecke.Application.Commands;

public record SignUp(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string Pesel,
    string PwzNumber,
    string PhoneNumber,
    DateTime DateOfBirth,
    AddressDto CorrespondenceAddress
) : ICommand;

public record AddressDto(
    string Street,
    string HouseNumber,
    string? ApartmentNumber,
    string PostalCode,
    string City,
    string Province
);