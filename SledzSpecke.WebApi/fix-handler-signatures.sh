#!/bin/bash

# Fix handlers that implement ICommandHandler or IQueryHandler by removing CancellationToken parameter

echo "Fixing handler signatures..."

# List of handlers that need CancellationToken removed (based on the error messages)
handlers=(
    "AddProcedureHandler"
    "GetProcedureByIdHandler"
    "GetProcedureStatisticsHandler"
    "GetUserProceduresHandler"
    "UpdateProcedureHandler"
    "GetAdditionalSelfEducationDaysByIdHandler"
    "GetAdditionalSelfEducationDaysByModuleHandler"
    "ExportSpecializationToXlsxHandler"
    "GetCompletedSelfEducationHandler"
    "GetEducationalActivitiesByTypeHandler"
    "GetEducationalActivitiesHandler"
    "CreateInternshipHandler"
    "GetEducationalActivityByIdHandler"
    "CreateRecognitionHandler"
    "CreateSelfEducationHandler"
    "GetInternshipsHandler"
    "GetSelfEducationByYearHandler"
    "GetSmkRequirementsHandler"
    "GetTotalCreditHoursHandler"
    "GetTotalQualityScoreHandler"
    "DeleteSelfEducationHandler"
)

# Find and fix each handler
for handler in "${handlers[@]}"; do
    echo "Processing $handler..."
    
    # Find the file containing the handler
    file=$(find src -name "*.cs" -type f -exec grep -l "class $handler" {} \; | head -1)
    
    if [ -n "$file" ]; then
        echo "  Found in: $file"
        
        # Remove CancellationToken parameter from HandleAsync method
        # This matches HandleAsync with CancellationToken and removes the parameter
        sed -i 's/HandleAsync(\(.*\), CancellationToken cancellationToken = default)/HandleAsync(\1)/g' "$file"
        
        # Also handle cases where there might be extra spaces
        sed -i 's/HandleAsync(\(.*\),\s*CancellationToken cancellationToken = default)/HandleAsync(\1)/g' "$file"
        
        # Remove any SaveChangesAsync calls with cancellationToken parameter
        sed -i 's/SaveChangesAsync(cancellationToken)/SaveChangesAsync()/g' "$file"
        
        echo "  Fixed $handler"
    else
        echo "  WARNING: Could not find file for $handler"
    fi
done

# Special case for AddAdditionalSelfEducationDaysHandler - it needs different handling
echo "Fixing AddAdditionalSelfEducationDaysHandler..."
file="src/SledzSpecke.Application/Commands/Handlers/AdditionalSelfEducationDays/AddAdditionalSelfEducationDaysHandler.cs"
if [ -f "$file" ]; then
    # Remove the CancellationToken parameter and keep the existing logic
    sed -i 's/HandleAsync(AddAdditionalSelfEducationDays command, CancellationToken cancellationToken = default)/HandleAsync(AddAdditionalSelfEducationDays command)/g' "$file"
    sed -i 's/SaveChangesAsync(cancellationToken)/SaveChangesAsync()/g' "$file"
    echo "  Fixed AddAdditionalSelfEducationDaysHandler"
fi

echo "Handler signature fixes complete!"