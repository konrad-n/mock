#!/bin/bash

echo "Comprehensive syntax fix for Application layer..."

cd /home/ubuntu/projects/mock/SledzSpecke.WebApi/src/SledzSpecke.Application

# Fix 1: Remove trailing commas that were added incorrectly
echo "Fixing trailing commas..."
find . -name "*.cs" -type f -exec sed -i 's/,$/);/g' {} +

# Fix 2: Fix method parameters that have closing parenthesis on wrong line
echo "Fixing method parameter lists..."
find . -name "*.cs" -type f -exec sed -i ':a;N;$!ba;s/,\n[[:space:]]*)/)/g' {} +

# Fix 3: Fix object initializers
echo "Fixing object initializers..."
find . -name "*.cs" -type f -exec sed -i 's/= new(),/= new();/g' {} +

# Fix 4: Fix lambda expressions that have extra parenthesis
echo "Fixing lambda expressions..."
find . -name "*.cs" -type f -exec sed -i 's/))$/)/g' {} +

# Fix 5: Fix method calls that are missing closing parenthesis
echo "Fixing method calls..."
find . -name "*.cs" -type f -exec sed -i 's/GetByIdAsync(.*[^)]$/&)/g' {} +

# Fix 6: Fix ToList() calls
echo "Fixing ToList() calls..."
find . -name "*.cs" -type f -exec sed -i 's/\.ToList(,$/\.ToList();/g' {} +

# Fix 7: Fix Where/FirstOrDefault/Any/All calls
echo "Fixing LINQ calls..."
find . -name "*.cs" -type f -exec sed -i 's/\.Where(.*[^)]$/&)/g' {} +
find . -name "*.cs" -type f -exec sed -i 's/\.FirstOrDefault(.*[^)]$/&)/g' {} +
find . -name "*.cs" -type f -exec sed -i 's/\.Any(.*[^)]$/&)/g' {} +
find . -name "*.cs" -type f -exec sed -i 's/\.All(.*[^)]$/&)/g' {} +

# Fix 8: Fix if statements
echo "Fixing if statements..."
find . -name "*.cs" -type f -exec sed -i 's/if (.*[^)]$/&)/g' {} +

# Fix 9: Fix logging statements
echo "Fixing logging statements..."
find . -name "*.cs" -type f -exec sed -i 's/_logger\.Log\(Information\|Error\|Warning\|Debug\)(.*,$/&);/g' {} +

# Fix 10: Fix specific patterns from the errors
echo "Fixing specific patterns..."
# Fix lines ending with just a comma
find . -name "*.cs" -type f -exec sed -i 's/^\([[:space:]]*\)\(.*\),$/\1\2);/g' {} +

# Fix empty parentheses followed by comma
find . -name "*.cs" -type f -exec sed -i 's/(,$/();/g' {} +

# Fix double closing parentheses
find . -name "*.cs" -type f -exec sed -i 's/)))/)/g' {} +
find . -name "*.cs" -type f -exec sed -i 's/))/)/g' {} +

echo "Counting remaining errors..."
cd /home/ubuntu/projects/mock/SledzSpecke.WebApi
dotnet build 2>&1 | grep -c "error CS"