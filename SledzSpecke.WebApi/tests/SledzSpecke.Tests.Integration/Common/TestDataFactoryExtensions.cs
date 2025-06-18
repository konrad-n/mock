using SledzSpecke.Application.Commands;
using SledzSpecke.Application.DTO;
using SledzSpecke.Core.ValueObjects;
using System;
using AddressDto = SledzSpecke.Application.Commands.AddressDto;

namespace SledzSpecke.Tests.Integration.Common;

public static class TestDataFactoryExtensions
{
    public static SignUp CreateSignUpCommand(
        string? email = null,
        string? password = null,
        string? firstName = null,
        string? lastName = null)
    {
        return new SignUp(
            Email: email ?? $"test{Guid.NewGuid():N}@example.com",
            Password: password ?? "SecurePassword123!",
            FirstName: firstName ?? "Test",
            LastName: lastName ?? "User",
            Pesel: "90010123456",
            PwzNumber: "1234567",
            PhoneNumber: "+48123456789",
            DateOfBirth: new DateTime(1990, 1, 1),
            CorrespondenceAddress: new AddressDto(
                Street: "Test Street",
                HouseNumber: "1",
                ApartmentNumber: null,
                PostalCode: "00-001",
                City: "Warsaw",
                Province: "Mazowieckie"
            )
        );
    }
    
    public static SignIn CreateSignInCommand(string? username = null, string? password = null)
    {
        return new SignIn(
            Username: username ?? "testuser",
            Password: password ?? "TestPassword123!"
        );
    }
    
    public static AddMedicalShift CreateAddMedicalShiftCommand(
        int? internshipId = null,
        DateTime? date = null,
        int hours = 8,
        int minutes = 0,
        string? location = null,
        int year = 3)
    {
        return new AddMedicalShift(
            InternshipId: internshipId ?? 1,
            Date: date ?? DateTime.Today,
            Hours: hours,
            Minutes: minutes,
            Location: location ?? "Test Hospital",
            Year: year
        );
    }
}
