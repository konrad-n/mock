#!/bin/bash

API_URL="http://localhost:5000/api"

echo "Testing Phase 2 GET endpoints..."
echo

echo "1. Testing Publications endpoint"
curl -s -X GET "$API_URL/publications/user/1?specializationId=1" | jq . || echo "No publications found"
echo

echo "2. Testing Absences endpoint"
curl -s -X GET "$API_URL/absences/user/1?specializationId=1" | jq . || echo "No absences found"
echo

echo "3. Testing Recognitions endpoint"
curl -s -X GET "$API_URL/recognitions/user/1?specializationId=1" | jq . || echo "No recognitions found"
echo

echo "4. Testing Self-Education endpoint"
curl -s -X GET "$API_URL/selfeducation/user/1?specializationId=1" | jq . || echo "No self-education found"
echo

echo "5. Testing Swagger UI availability"
curl -s -I "$API_URL/../swagger" | head -n 1
echo

echo "Test complete!"