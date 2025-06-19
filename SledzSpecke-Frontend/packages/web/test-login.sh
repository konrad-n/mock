#!/bin/bash

echo "Testing login with test@example.com..."

# Build the JSON payload
payload=$(printf '{"Username":"testuser","Password":"Test123%s"}' '!')
echo "Request payload: $payload"

# Use printf to properly escape the JSON
response=$(curl -s -X POST http://localhost:5000/api/auth/sign-in \
  -H "Content-Type: application/json" \
  -d "$payload")

if echo "$response" | grep -q "AccessToken"; then
  echo "✅ Login successful!"
  TOKEN=$(echo "$response" | jq -r '.AccessToken')
  echo "Token obtained: ${TOKEN:0:20}..."
  
  echo -e "\nTesting authenticated endpoints..."
  
  # Test MedicalShifts
  echo -e "\nTesting /api/MedicalShifts:"
  response=$(curl -s -w "\n---STATUS:%{http_code}---" \
    -H "Authorization: Bearer $TOKEN" \
    http://localhost:5000/api/MedicalShifts)
  status=$(echo "$response" | grep -oP '(?<=---STATUS:)\d+(?=---)')
  body=$(echo "$response" | sed 's/---STATUS:[0-9]*---//')
  echo "Status: $status"
  if [ "$status" != "200" ]; then
    echo "Response: $body" | jq 2>/dev/null || echo "$body"
  fi
  
  # Test Procedures
  echo -e "\nTesting /api/Procedures:"
  response=$(curl -s -w "\n---STATUS:%{http_code}---" \
    -H "Authorization: Bearer $TOKEN" \
    http://localhost:5000/api/Procedures)
  status=$(echo "$response" | grep -oP '(?<=---STATUS:)\d+(?=---)')
  body=$(echo "$response" | sed 's/---STATUS:[0-9]*---//')
  echo "Status: $status"
  if [ "$status" != "200" ]; then
    echo "Response: $body" | jq 2>/dev/null || echo "$body"
  fi
  
  # Test Internships
  echo -e "\nTesting /api/Internships:"
  response=$(curl -s -w "\n---STATUS:%{http_code}---" \
    -H "Authorization: Bearer $TOKEN" \
    http://localhost:5000/api/Internships)
  status=$(echo "$response" | grep -oP '(?<=---STATUS:)\d+(?=---)')
  body=$(echo "$response" | sed 's/---STATUS:[0-9]*---//')
  echo "Status: $status"
  if [ "$status" != "200" ]; then
    echo "Response: $body" | jq 2>/dev/null || echo "$body"
  fi
  
else
  echo "❌ Login failed!"
  echo "$response" | jq
fi