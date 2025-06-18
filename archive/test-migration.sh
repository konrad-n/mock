#!/bin/bash

echo "Testing migration locally..."

# Use test database
export ConnectionStrings__DefaultConnection="Host=localhost;Database=sledzspecke_test;Username=www-data;Password=UDz2024#sDev!1"

# Apply migration
cd /home/ubuntu/projects/mock/SledzSpecke.WebApi
dotnet ef database update -p src/SledzSpecke.Infrastructure -s src/SledzSpecke.Api

echo "Migration test complete."