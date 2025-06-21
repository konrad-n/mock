#!/bin/bash

echo "Fixing interface method parameter syntax errors..."

cd /home/ubuntu/projects/mock/SledzSpecke.WebApi/src/SledzSpecke.Application

# Fix interface method parameters - replace semicolons with commas
# This pattern looks for lines that have semicolons after parameter types but before the closing parenthesis
find . -name "*.cs" -type f -exec sed -i 's/\([a-zA-Z0-9_>]\+\) \([a-zA-Z0-9_]\+\));$/\1 \2,/g' {} +

# Fix the last parameter in method signatures (should not have comma)
find . -name "*.cs" -type f -exec sed -i 's/,$/);/g' {} +

# Fix specific pattern where there are multiple semicolons in method parameters
find . -name "*.cs" -type f -exec sed -i 's/);[[:space:]]*$/,/g' {} +

# Fix the final closing parenthesis for method signatures
find . -name "*.cs" -type f -exec sed -i 's/,\([[:space:]]*CancellationToken cancellationToken = default\));$/,\1);/g' {} +

echo "Done fixing interface syntax errors"