#!/bin/bash

echo "Reverting and fixing syntax errors systematically..."

cd /home/ubuntu/projects/mock/SledzSpecke.WebApi/src/SledzSpecke.Application

# First, let's identify the pattern of errors
echo "Analyzing error patterns..."

# Pattern 1: Constructor/method parameters have semicolons instead of commas
# Fix lines like: ILogger<SomeClass> logger);
echo "Fixing constructor and method parameter semicolons..."
find . -name "*.cs" -type f -exec sed -i 's/\([a-zA-Z0-9_<>]\+\) \([a-zA-Z0-9_]\+\));$/\1 \2,/g' {} +

# Pattern 2: Fix lines where commas were wrongly placed
# Fix lines like: = new SmkRequirementsDto);
echo "Fixing object initializer syntax..."
find . -name "*.cs" -type f -exec sed -i 's/);$/,/g' {} +

# Pattern 3: Fix the last parameter in constructors/methods (should end with )
echo "Fixing last parameters in methods..."
find . -name "*.cs" -type f -exec sed -i ':a;N;$!ba;s/,\n[[:space:]]*)/\n    )/g' {} +

# Pattern 4: Fix object initializers that should end with };
echo "Fixing object initializer endings..."
find . -name "*.cs" -type f -exec sed -i 's/}$/};/g' {} +

# Pattern 5: Fix method calls that have wrong parentheses
echo "Fixing method call syntax..."
find . -name "*.cs" -type f -exec sed -i 's/);)$/)/g' {} +

# Pattern 6: Fix property/field assignments
echo "Fixing property assignments..."
find . -name "*.cs" -type f -exec sed -i 's/= \([0-9]\+\));$/= \1,/g' {} +
find . -name "*.cs" -type f -exec sed -i 's/= "\([^"]*\)");$/= "\1",/g' {} +

# Pattern 7: Fix if statements with wrong parentheses
echo "Fixing if statement conditions..."
find . -name "*.cs" -type f -exec sed -i 's/if (\([^)]*\))$/if (\1)/g' {} +

# Pattern 8: Fix LINQ expressions
echo "Fixing LINQ expressions..."
find . -name "*.cs" -type f -exec sed -i 's/\.Where(\([^)]*\));)$/.Where(\1))/g' {} +
find . -name "*.cs" -type f -exec sed -i 's/\.FirstOrDefault(\([^)]*\));)$/.FirstOrDefault(\1))/g' {} +
find . -name "*.cs" -type f -exec sed -i 's/\.Any(\([^)]*\));)$/.Any(\1))/g' {} +

# Pattern 9: Fix lines that end with just )
echo "Fixing standalone closing parentheses..."
find . -name "*.cs" -type f -exec sed -i 's/^)$/        )/g' {} +

# Pattern 10: Fix the specific pattern in records
echo "Fixing record parameter patterns..."
find . -name "*.cs" -type f -exec perl -i -pe 's/^(\s+\w+.*?);\s*$/\1,/g if /^\s+\w+.*\)\s*;/' {} +

echo "Running build to check progress..."
cd /home/ubuntu/projects/mock/SledzSpecke.WebApi
dotnet build 2>&1 | grep -c "error CS"