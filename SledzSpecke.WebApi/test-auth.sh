#!/bin/bash

# Test SignUp
echo "Testing SignUp..."
curl -X POST http://localhost:5000/api/auth/sign-up \
  -H "Content-Type: application/json" \
  -d '{
    "email": "newtest@example.com",
    "username": "newtestuser",
    "password": "Test123!",
    "fullName": "New Test User",
    "smkVersion": "new",
    "specializationId": 1
  }' | jq .

echo -e "\n\nTesting SignIn..."
# Test SignIn
curl -X POST http://localhost:5000/api/auth/sign-in \
  -H "Content-Type: application/json" \
  -d '{
    "username": "newtestuser",
    "password": "Test123!"
  }' | jq .