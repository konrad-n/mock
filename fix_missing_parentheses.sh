#!/bin/bash

# Fix common syntax errors in Application layer

cd /home/ubuntu/projects/mock/SledzSpecke.WebApi

# Fix method calls ending with (, instead of );
find src/SledzSpecke.Application -name "*.cs" -exec sed -i 's/\(,$/);/g' {} \;

# Fix closing braces followed by semicolons
find src/SledzSpecke.Application -name "*.cs" -exec sed -i 's/\};$/}/g' {} \;

# Fix method declarations ending with comma
find src/SledzSpecke.Application -name "*.cs" -exec sed -i 's/\(async Task.*\)(,$/\1);/g' {} \;
find src/SledzSpecke.Application -name "*.cs" -exec sed -i 's/\(Task<.*>\s\+\w\+\)(,$/\1);/g' {} \;

# Fix if statements missing closing parenthesis
find src/SledzSpecke.Application -name "*.cs" -exec sed -i 's/if (\(.*\)$/if (\1)/g' {} \;

# Fix Where clauses missing closing parenthesis
find src/SledzSpecke.Application -name "*.cs" -exec sed -i 's/\.Where(\(.*\)$/\.Where(\1)/g' {} \;

# Fix foreach statements
find src/SledzSpecke.Application -name "*.cs" -exec sed -i 's/foreach (\(.*\)$/foreach (\1)/g' {} \;

echo "Fixed common syntax patterns. Running build to check progress..."
dotnet build 2>&1 | grep -c "error CS"