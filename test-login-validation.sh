#!/bin/bash

echo "Testing login validation with updated frontend..."
echo "================================================"

API_URL="https://api.sledzspecke.pl"

# Test 1: Login with username
echo -e "\n1. Testing login with username (should work):"
response=$(curl -X POST "${API_URL}/api/auth/sign-in" \
  -H "Content-Type: application/json" \
  -d '{"Username": "testuser", "Password": "Test123!"}' \
  -s)
echo "$response" | grep -q "AccessToken" && echo "✓ SUCCESS: Login with username works!" || echo "✗ FAILED: $response"

# Test 2: Login with email
echo -e "\n2. Testing login with email (should work):"
response=$(curl -X POST "${API_URL}/api/auth/sign-in" \
  -H "Content-Type: application/json" \
  -d '{"Username": "test@example.com", "Password": "Test123!"}' \
  -s)
echo "$response" | grep -q "AccessToken" && echo "✓ SUCCESS: Login with email works!" || echo "✗ FAILED: $response"

# Test 3: Invalid login
echo -e "\n3. Testing invalid login (should fail):"
response=$(curl -X POST "${API_URL}/api/auth/sign-in" \
  -H "Content-Type: application/json" \
  -d '{"Username": "invalid", "Password": "wrong"}' \
  -s)
echo "$response" | grep -q "AccessToken" && echo "✗ UNEXPECTED: Should have failed!" || echo "✓ SUCCESS: Invalid login properly rejected"

echo -e "\n================================================"
echo "Frontend validation has been updated to accept both email and username."
echo "Users can now log in using either their username or email address."