using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Commands;
using SledzSpecke.Core.Abstractions;

namespace SledzSpecke.Application.Validation;

public sealed class AddProcedureValidator : CommandValidatorBase<AddProcedure>
{
    protected override void ValidateCommand(AddProcedure command, ValidationResult result)
    {
        ValidatePositive(command.InternshipId, "InternshipId", result);
        
        ValidateRequired(command.Code, "Code", result);
        ValidateMaxLength(command.Code, 50, "Code", result);
        
        ValidateRequired(command.Location, "Location", result);
        ValidateMaxLength(command.Location, 200, "Location", result);
        
        if (command.Date == default)
        {
            result.AddError("Date", "Date is required");
        }
        
        ValidateRequired(command.Status, "Status", result);
        
        if (command.Year < 0 || command.Year > 10)
        {
            result.AddError("Year", "Year must be between 0 and 10");
        }
        
        // Optional fields validation
        if (!string.IsNullOrEmpty(command.OperatorCode))
        {
            ValidateMaxLength(command.OperatorCode, 50, "OperatorCode", result);
        }
        
        if (!string.IsNullOrEmpty(command.PerformingPerson))
        {
            ValidateMaxLength(command.PerformingPerson, 200, "PerformingPerson", result);
        }
        
        if (!string.IsNullOrEmpty(command.PatientInitials))
        {
            ValidateMaxLength(command.PatientInitials, 10, "PatientInitials", result);
        }
        
        if (!string.IsNullOrEmpty(command.Supervisor))
        {
            ValidateMaxLength(command.Supervisor, 200, "Supervisor", result);
        }
        
        if (!string.IsNullOrEmpty(command.ProcedureName))
        {
            ValidateMaxLength(command.ProcedureName, 300, "ProcedureName", result);
        }
        
        if (!string.IsNullOrEmpty(command.ProcedureGroup))
        {
            ValidateMaxLength(command.ProcedureGroup, 100, "ProcedureGroup", result);
        }
        
        if (!string.IsNullOrEmpty(command.AssistantData))
        {
            ValidateMaxLength(command.AssistantData, 500, "AssistantData", result);
        }
    }
}