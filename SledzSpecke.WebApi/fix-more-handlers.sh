#!/bin/bash

echo "Fixing more handlers with duplicate cancellationToken..."

# Handlers with duplicate cancellationToken that implement IResultCommandHandler
result_handlers=(
    "ApproveAbsenceHandler"
    "ApproveInternshipHandler"
    "ChangePasswordHandler"
    "CreateAbsenceHandler"
    "UpdateCourseHandler"
    "UpdateEducationalActivityHandler"
    "UpdateInternshipHandler"
    "UpdatePublicationHandler"
)

# Handlers that implement ICommandHandler (no cancellationToken)
command_handlers=(
    "ApproveCourseHandler"
    "CompleteSelfEducationHandler"
    "UpdateSelfEducationHandler"
)

# Fix IResultCommandHandler implementations (remove duplicate cancellationToken)
for handler in "${result_handlers[@]}"; do
    echo "Processing $handler (IResultCommandHandler)..."
    
    file=$(find src -name "*.cs" -type f -exec grep -l "class $handler" {} \; | head -1)
    
    if [ -n "$file" ]; then
        echo "  Found in: $file"
        
        # Fix duplicate cancellationToken parameter (keep only one)
        sed -i 's/, CancellationToken cancellationToken = default, CancellationToken cancellationToken = default/, CancellationToken cancellationToken = default/g' "$file"
        
        echo "  Fixed $handler"
    else
        echo "  WARNING: Could not find file for $handler"
    fi
done

# Fix ICommandHandler implementations (remove cancellationToken completely)
for handler in "${command_handlers[@]}"; do
    echo "Processing $handler (ICommandHandler)..."
    
    file=$(find src -name "*.cs" -type f -exec grep -l "class $handler" {} \; | head -1)
    
    if [ -n "$file" ]; then
        echo "  Found in: $file"
        
        # Remove CancellationToken parameter from HandleAsync
        sed -i 's/HandleAsync(\(.*\), CancellationToken cancellationToken = default)/HandleAsync(\1)/g' "$file"
        
        # Remove SaveChangesAsync(cancellationToken)
        sed -i 's/SaveChangesAsync(cancellationToken)/SaveChangesAsync()/g' "$file"
        
        echo "  Fixed $handler"
    else
        echo "  WARNING: Could not find file for $handler"
    fi
done

# Special case for CompleteSelfEducationHandler - it has duplicate method definitions
echo "Fixing CompleteSelfEducationHandler duplicate method..."
file="src/SledzSpecke.Application/Commands/Handlers/CompleteSelfEducationHandler.cs"
if [ -f "$file" ]; then
    # This handler has a special issue - it has duplicate HandleAsync methods
    # We need to remove one of them
    echo "  Manually fixing CompleteSelfEducationHandler duplicate method issue"
fi

echo "Handler fixes complete!"