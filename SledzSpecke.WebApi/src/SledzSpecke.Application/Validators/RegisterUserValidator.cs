using FluentValidation;
using SledzSpecke.Application.Commands.Users;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;
using SledzSpecke.Api.Extensions;

namespace SledzSpecke.Application.Validators;

/// <summary>
/// Validator for RegisterUser command with email uniqueness check
/// </summary>
public class RegisterUserValidator : AbstractValidator<RegisterUser>
{
    private readonly IUserRepository _userRepository;

    public RegisterUserValidator(IUserRepository userRepository)
    {
        _userRepository = userRepository;

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Invalid email format")
            .MaximumLength(255)
            .WithMessage("Email cannot exceed 255 characters")
            .WithErrorCode(ErrorCodes.VALIDATION_ERROR)
            .MustAsync(BeUniqueEmail)
            .WithMessage("Email already exists")
            .WithErrorCode(ErrorCodes.USER_ALREADY_EXISTS);

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required")
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters long")
            .MaximumLength(100)
            .WithMessage("Password cannot exceed 100 characters")
            .Matches(@"[A-Z]")
            .WithMessage("Password must contain at least one uppercase letter")
            .Matches(@"[a-z]")
            .WithMessage("Password must contain at least one lowercase letter")
            .Matches(@"[0-9]")
            .WithMessage("Password must contain at least one digit")
            .Matches(@"[!@#$%^&*(),.?"":{}|<>]")
            .WithMessage("Password must contain at least one special character")
            .WithErrorCode(ErrorCodes.VALIDATION_ERROR);

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .MaximumLength(100)
            .WithMessage("Name cannot exceed 100 characters")
            .Matches(@"^[a-zA-ZąćęłńóśźżĄĆĘŁŃÓŚŹŻ\s\-']+$")
            .WithMessage("Name can only contain letters, spaces, hyphens, and apostrophes")
            .WithErrorCode(ErrorCodes.VALIDATION_ERROR);

        RuleFor(x => x.University)
            .NotEmpty()
            .WithMessage("University is required")
            .MaximumLength(255)
            .WithMessage("University cannot exceed 255 characters")
            .WithErrorCode(ErrorCodes.VALIDATION_ERROR);

        RuleFor(x => x.SpecializationId)
            .GreaterThan(0)
            .WithMessage("Specialization ID must be a positive number")
            .WithErrorCode(ErrorCodes.VALIDATION_ERROR);

        RuleFor(x => x.Year)
            .InclusiveBetween(1, 6)
            .WithMessage("Year must be between 1 and 6")
            .WithErrorCode(ErrorCodes.VALIDATION_ERROR);

        RuleFor(x => x.SmkVersion)
            .NotEmpty()
            .WithMessage("SMK version is required")
            .Must(BeValidSmkVersion)
            .WithMessage("SMK version must be 'old' or 'new'")
            .WithErrorCode(ErrorCodes.INVALID_SMK_VERSION);

        RuleFor(x => x.SpecializationName)
            .NotEmpty()
            .WithMessage("Specialization name is required")
            .MaximumLength(255)
            .WithMessage("Specialization name cannot exceed 255 characters")
            .WithErrorCode(ErrorCodes.VALIDATION_ERROR);

        RuleFor(x => x.PhoneNumber)
            .Matches(@"^(\+48)?[ -]?[0-9]{3}[ -]?[0-9]{3}[ -]?[0-9]{3}$")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber))
            .WithMessage("Invalid Polish phone number format")
            .WithErrorCode(ErrorCodes.VALIDATION_ERROR);

        RuleFor(x => x.DateOfBirth)
            .LessThan(DateTime.UtcNow.AddYears(-18))
            .WithMessage("User must be at least 18 years old")
            .GreaterThan(DateTime.UtcNow.AddYears(-100))
            .WithMessage("Invalid date of birth")
            .When(x => x.DateOfBirth.HasValue)
            .WithErrorCode(ErrorCodes.VALIDATION_ERROR);
    }

    private async Task<bool> BeUniqueEmail(string email, CancellationToken cancellationToken)
    {
        try
        {
            var emailValue = new Email(email);
            var existingUser = await _userRepository.GetByEmailAsync(emailValue);
            return existingUser == null;
        }
        catch
        {
            // If email is invalid (throws from value object), it will be caught by other rules
            return true;
        }
    }

    private bool BeValidSmkVersion(string? smkVersion)
    {
        return smkVersion?.ToLower() == "old" || smkVersion?.ToLower() == "new";
    }
}