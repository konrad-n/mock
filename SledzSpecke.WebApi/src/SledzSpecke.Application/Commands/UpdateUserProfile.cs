using SledzSpecke.Application.Abstractions;

namespace SledzSpecke.Application.Commands;

public record UpdateUserProfile(
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    AddressDto CorrespondenceAddress) : ICommand;