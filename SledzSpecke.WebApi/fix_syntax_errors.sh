#!/bin/bash

echo "Fixing syntax errors in Application layer..."

# Go to the Application directory
cd /home/ubuntu/projects/mock/SledzSpecke.WebApi/src/SledzSpecke.Application

# Fix pattern: .FirstOrDefault(x => condition) - missing closing parenthesis
echo "Fixing FirstOrDefault calls..."
find . -name "*.cs" -type f -exec sed -i 's/\.FirstOrDefault(\([^)]*\)$/\.FirstOrDefault(\1))/g' {} +

# Fix pattern: .Where(x => condition) - missing closing parenthesis
echo "Fixing Where calls..."
find . -name "*.cs" -type f -exec sed -i 's/\.Where(\([^)]*\)$/\.Where(\1))/g' {} +

# Fix pattern: .SingleOrDefault(x => condition) - missing closing parenthesis
echo "Fixing SingleOrDefault calls..."
find . -name "*.cs" -type f -exec sed -i 's/\.SingleOrDefault(\([^)]*\)$/\.SingleOrDefault(\1))/g' {} +

# Fix pattern: .Any(x => condition) - missing closing parenthesis
echo "Fixing Any calls..."
find . -name "*.cs" -type f -exec sed -i 's/\.Any(\([^)]*\)$/\.Any(\1))/g' {} +

# Fix pattern: .All(x => condition) - missing closing parenthesis
echo "Fixing All calls..."
find . -name "*.cs" -type f -exec sed -i 's/\.All(\([^)]*\)$/\.All(\1))/g' {} +

# Fix pattern: if (condition without closing parenthesis
echo "Fixing if statements..."
find . -name "*.cs" -type f -exec sed -i 's/if (\([^)]*\)$/if (\1))/g' {} +

# Fix pattern: foreach (var item in collection without closing parenthesis
echo "Fixing foreach statements..."
find . -name "*.cs" -type f -exec sed -i 's/foreach (\([^)]*\)$/foreach (\1))/g' {} +

# Fix common specific patterns from the error messages
echo "Fixing specific syntax patterns..."

# Fix GetTemplateAsync calls with incomplete parameters
find . -name "*.cs" -type f -exec sed -i 's/GetTemplateAsync(specialization\.ProgramCode, specialization\.SmkVersion$/GetTemplateAsync(specialization.ProgramCode, specialization.SmkVersion.ToString().ToLowerInvariant())/g' {} +

# Fix incomplete method calls ending with just a comma
find . -name "*.cs" -type f -exec sed -i 's/,$/);/g' {} +

# Fix common pattern: missing closing parenthesis in LINQ queries with lambdas
find . -name "*.cs" -type f -exec sed -i 's/=> [^)]*$/&)/g' {} +

echo "Running build to check remaining errors..."
cd /home/ubuntu/projects/mock/SledzSpecke.WebApi
dotnet build 2>&1 | grep -c "error CS"