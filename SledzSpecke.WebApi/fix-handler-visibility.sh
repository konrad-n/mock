#!/bin/bash

# Fix handler visibility to make them public
echo "🔧 Fixing handler visibility (making them public)..."

# Find all handler files
find src/SledzSpecke.Application -name "*Handler.cs" -type f | while read -r file; do
    echo "Processing: $file"
    
    # Change internal class to public sealed class
    sed -i 's/internal class \([A-Za-z0-9]*Handler\)/public sealed class \1/g' "$file"
    
    # Change internal sealed class to public sealed class
    sed -i 's/internal sealed class \([A-Za-z0-9]*Handler\)/public sealed class \1/g' "$file"
    
    # Change class without modifier to public sealed class
    sed -i 's/^class \([A-Za-z0-9]*Handler\)/public sealed class \1/g' "$file"
    
    # Fix any remaining non-public handlers
    sed -i 's/^\s*class \([A-Za-z0-9]*Handler\)/public sealed class \1/g' "$file"
done

echo "✅ Handler visibility fixed!"