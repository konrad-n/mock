#!/bin/bash

# Fix handlers to add CancellationToken parameter
echo "🔧 Fixing handlers to add CancellationToken parameter..."

# Find all handler files
find src/SledzSpecke.Application -name "*Handler.cs" -type f | while read -r file; do
    echo "Processing: $file"
    
    # Fix HandleAsync methods that don't have CancellationToken
    sed -i 's/Task<\([^>]*\)> HandleAsync(\([^)]*\))$/Task<\1> HandleAsync(\2, CancellationToken cancellationToken = default)/g' "$file"
    
    # Fix HandleAsync methods with single parameter
    sed -i 's/HandleAsync(\([^,)]*\))$/HandleAsync(\1, CancellationToken cancellationToken = default)/g' "$file"
    
    # Fix method calls that don't pass cancellationToken
    sed -i 's/\.HandleAsync(\([^)]*\));/.HandleAsync(\1, cancellationToken);/g' "$file"
done

echo "✅ Handler signatures fixed!"