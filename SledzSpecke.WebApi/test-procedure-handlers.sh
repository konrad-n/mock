#!/bin/bash

# Start the API in the background
echo "Starting API..."
ASPNETCORE_ENVIRONMENT=Development dotnet run --project src/SledzSpecke.Api --urls http://localhost:5001 &
API_PID=$!

# Wait for API to start
echo "Waiting for API to start..."
sleep 10

# Test the procedures endpoint
echo -e "\n\nTesting procedures endpoint..."
curl -X GET http://localhost:5001/api/procedures?internshipId=1 \
  -H "Authorization: Bearer test-token" \
  -v

# Kill the API
echo -e "\n\nStopping API..."
kill $API_PID