using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Commands;
using SledzSpecke.Core.Abstractions;

namespace SledzSpecke.Application.Validation;

internal sealed class UploadFileValidator : IValidator<UploadFile>
{
    public Result Validate(UploadFile command)
    {
        if (command is null)
        {
            return Result.Failure("Command cannot be null.");
        }
        
        if (command.File is null)
        {
            return Result.Failure("File is required.");
        }
        
        if (command.File.Length == 0)
        {
            return Result.Failure("File cannot be empty.");
        }
        
        if (command.File.Length > 10 * 1024 * 1024) // 10 MB
        {
            return Result.Failure("File size cannot exceed 10 MB.");
        }
        
        if (string.IsNullOrWhiteSpace(command.EntityType))
        {
            return Result.Failure("Entity type is required.");
        }
        
        if (command.EntityId <= 0)
        {
            return Result.Failure("Entity ID must be a positive number.");
        }
        
        // Validate allowed entity types
        var allowedEntityTypes = new HashSet<string>
        {
            "Course", "Publication", "SelfEducation", "Recognition", 
            "User", "EducationalActivity", "Certificate"
        };
        
        if (!allowedEntityTypes.Contains(command.EntityType))
        {
            return Result.Failure($"Entity type '{command.EntityType}' is not allowed.");
        }
        
        return Result.Success();
    }
}