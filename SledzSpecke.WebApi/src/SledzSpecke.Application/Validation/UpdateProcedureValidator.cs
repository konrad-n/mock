using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Commands;
using SledzSpecke.Core.Abstractions;

namespace SledzSpecke.Application.Validation;

public sealed class UpdateProcedureValidator : CommandValidatorBase<UpdateProcedure>
{
    protected override void ValidateCommand(UpdateProcedure command, ValidationResult result)
    {
        ValidatePositive(command.ProcedureId, "ProcedureId", result);
        
        // Status is optional but if provided must be valid
        if (!string.IsNullOrEmpty(command.Status))
        {
            var validStatuses = new[] { "Pending", "Completed", "Cancelled" };
            if (!validStatuses.Contains(command.Status))
            {
                result.AddError("Status", $"Status must be one of: {string.Join(", ", validStatuses)}");
            }
        }
        
        // Optional fields validation
        if (!string.IsNullOrEmpty(command.ExecutionType))
        {
            var validTypes = new[] { "CodeA", "CodeB" };
            if (!validTypes.Contains(command.ExecutionType))
            {
                result.AddError("ExecutionType", $"ExecutionType must be one of: {string.Join(", ", validTypes)}");
            }
        }
        
        if (!string.IsNullOrEmpty(command.PerformingPerson))
        {
            ValidateMaxLength(command.PerformingPerson, 200, "PerformingPerson", result);
        }
        
        if (!string.IsNullOrEmpty(command.PatientInitials))
        {
            ValidateMaxLength(command.PatientInitials, 10, "PatientInitials", result);
        }
        
        if (command.PatientGender.HasValue)
        {
            var validGenders = new[] { 'M', 'F', 'O' };
            if (!validGenders.Contains(command.PatientGender.Value))
            {
                result.AddError("PatientGender", "Patient gender must be M, F, or O");
            }
        }
    }
}