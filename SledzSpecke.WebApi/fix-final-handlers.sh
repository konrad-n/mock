#!/bin/bash

echo "Fixing final batch of handlers..."

# Handlers with duplicate cancellationToken that implement IResultCommandHandler
result_handlers=(
    "SignUpHandler"
    "UpdateUserProfileHandler"
)

# Handlers that implement IQueryHandler (no cancellationToken)
query_handlers=(
    "GetUserSelfEducationHandler"
    "PreviewSmkExportHandler"
    "ValidateSpecializationForSmkHandler"
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

# Fix IQueryHandler implementations (remove cancellationToken completely)
for handler in "${query_handlers[@]}"; do
    echo "Processing $handler (IQueryHandler)..."
    
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

echo "Final handler fixes complete!"