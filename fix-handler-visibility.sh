#!/bin/bash

# Find all files with internal sealed class handlers and make them public
find /home/ubuntu/projects/mock/SledzSpecke.WebApi/src/SledzSpecke.Application -name "*.cs" -type f | while read file; do
  # Skip .old files and decorators
  if [[ "$file" == *.old ]] || [[ "$file" == *Decorator* ]]; then
    continue
  fi
  
  # Replace internal sealed class with public sealed class for handlers
  sed -i 's/^internal sealed class \(.*Handler\)/public sealed class \1/g' "$file"
done

echo "Handler visibility fix completed"