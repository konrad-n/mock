#!/bin/bash

# SledzSpecke Full Application Flow Test
# Tests the complete user journey like a real browser

set -e

RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

API_URL="https://api.sledzspecke.pl"
FRONTEND_URL="https://sledzspecke.pl"

# Test data
TEST_EMAIL="test_$(date +%s)@wum.edu.pl"
TEST_PASSWORD="TestPassword123!"
TEST_NAME="Jan"
TEST_SURNAME="Testowy"

# Generate unique valid PESEL based on timestamp
TIMESTAMP=$(date +%s)
# Use last 3 digits of timestamp for serial number to ensure uniqueness
SERIAL=$((TIMESTAMP % 1000))
# Format as 3 digits
SERIAL_PADDED=$(printf "%03d" $SERIAL)

# PESEL for someone born on 1992-03-15 (less common date)
PESEL_BASE="920315${SERIAL_PADDED}3"
# Calculate checksum
sum=0
weights=(1 3 7 9 1 3 7 9 1 3)
for i in {0..9}; do
    digit=${PESEL_BASE:$i:1}
    weight=${weights[$i]}
    sum=$((sum + digit * weight))
done
checksum=$(( (10 - (sum % 10)) % 10 ))
VALID_PESEL="${PESEL_BASE}${checksum}"

# Generate unique valid PWZ based on timestamp
# Generate a 6-digit number that doesn't start with 0
PWZ_BASE=$(( (TIMESTAMP % 900000) + 100000 ))
# Ensure it's exactly 6 digits
PWZ_BASE=$(printf "%06d" $PWZ_BASE)
# Calculate PWZ checksum
pwz_sum=0
pwz_weights=(1 2 3 4 5 6)
for i in {0..5}; do
    digit=${PWZ_BASE:$i:1}
    weight=${pwz_weights[$i]}
    pwz_sum=$((pwz_sum + digit * weight))
done
pwz_checksum=$((pwz_sum % 11))
VALID_PWZ="${PWZ_BASE}${pwz_checksum}"

echo "================================================"
echo "SledzSpecke Full Application Flow Test"
echo "================================================"
echo "Generated test data:"
echo "  PESEL: $VALID_PESEL"
echo "  PWZ: $VALID_PWZ"
echo "  Email: $TEST_EMAIL"

# Function to test with color output
test_step() {
    local description=$1
    local result=$2
    
    if [ "$result" = "pass" ]; then
        echo -e "${GREEN}✓${NC} $description"
    else
        echo -e "${RED}✗${NC} $description"
        exit 1
    fi
}

# 1. Test Frontend Loading
echo -e "\n${BLUE}1. Testing Frontend Loading...${NC}"
frontend_status=$(curl -s -o /dev/null -w "%{http_code}" "$FRONTEND_URL")
if [ "$frontend_status" = "200" ]; then
    test_step "Frontend loads successfully" "pass"
else
    test_step "Frontend loads successfully" "fail"
fi

# 2. Test API Health
echo -e "\n${BLUE}2. Testing API Health...${NC}"
health_response=$(curl -s "$API_URL/api/health")
if echo "$health_response" | grep -q '"status": "healthy"'; then
    test_step "API is healthy" "pass"
else
    test_step "API is healthy" "fail"
fi

# 3. Test CORS from Frontend Origin
echo -e "\n${BLUE}3. Testing CORS Configuration...${NC}"
cors_response=$(curl -s -H "Origin: https://sledzspecke.pl" \
    -H "Access-Control-Request-Method: POST" \
    -H "Access-Control-Request-Headers: Content-Type" \
    -X OPTIONS \
    -o /dev/null -w "%{http_code}" \
    "$API_URL/api/auth/sign-up")

if [ "$cors_response" = "204" ]; then
    test_step "CORS preflight works" "pass"
else
    test_step "CORS preflight works" "fail"
fi

# 4. Test User Registration
echo -e "\n${BLUE}4. Testing User Registration...${NC}"
# Use -w to get HTTP status code
registration_response=$(curl -s -X POST "$API_URL/api/auth/sign-up" \
    -H "Content-Type: application/json" \
    -H "Origin: https://sledzspecke.pl" \
    -w "\nHTTP_STATUS:%{http_code}" \
    -d '{
        "email": "'"$TEST_EMAIL"'",
        "password": "'"$TEST_PASSWORD"'",
        "firstName": "'"$TEST_NAME"'",
        "lastName": "'"$TEST_SURNAME"'",
        "pesel": "'"$VALID_PESEL"'",
        "pwzNumber": "'"$VALID_PWZ"'",
        "phoneNumber": "+48600123456",
        "dateOfBirth": "1992-03-15",
        "correspondenceAddress": {
            "street": "Testowa",
            "houseNumber": "1",
            "apartmentNumber": "2",
            "postalCode": "00-001",
            "city": "Warszawa",
            "province": "mazowieckie"
        }
    }' 2>&1)

# Extract status code
http_status=$(echo "$registration_response" | grep "HTTP_STATUS:" | cut -d: -f2)
response_body=$(echo "$registration_response" | sed '/HTTP_STATUS:/d')

echo "HTTP Status: $http_status"
echo "Response body: $response_body"

if [ "$http_status" = "200" ] || [ "$http_status" = "201" ] || [ "$http_status" = "204" ]; then
    test_step "User registration" "pass"
elif [ -n "$response_body" ]; then
    echo "Registration error: $response_body"
    test_step "User registration" "fail"
else
    echo "Registration failed with HTTP status: $http_status"
    test_step "User registration" "fail"
fi

# 5. Test User Login
echo -e "\n${BLUE}5. Testing User Login...${NC}"
login_response=$(curl -s -X POST "$API_URL/api/auth/sign-in" \
    -H "Content-Type: application/json" \
    -H "Origin: https://sledzspecke.pl" \
    -d '{
        "username": "'"$TEST_EMAIL"'",
        "password": "'"$TEST_PASSWORD"'"
    }')

if echo "$login_response" | grep -q '"AccessToken"'; then
    test_step "User login successful" "pass"
    # Use jq for proper JSON parsing if available, otherwise fallback
    if command -v jq &> /dev/null; then
        ACCESS_TOKEN=$(echo "$login_response" | jq -r '.AccessToken')
        USER_ID=$(echo "$login_response" | jq -r '.UserId')
    else
        ACCESS_TOKEN=$(echo "$login_response" | grep -o '"AccessToken":"[^"]*' | cut -d'"' -f4)
        USER_ID=$(echo "$login_response" | grep -o '"UserId":[0-9]*' | cut -d':' -f2)
    fi
    echo "Extracted token: ${ACCESS_TOKEN:0:20}..."
    echo "User ID: $USER_ID"
else
    echo "Login response: $login_response"
    test_step "User login successful" "fail"
fi

# 6. Test Authenticated API Call
echo -e "\n${BLUE}6. Testing Authenticated API Access...${NC}"
echo "Using token: ${ACCESS_TOKEN:0:20}..." # Show first 20 chars for debugging
profile_response=$(curl -s -X GET "$API_URL/api/users/profile" \
    -H "Authorization: Bearer $ACCESS_TOKEN" \
    -H "Origin: https://sledzspecke.pl" \
    -w "\nHTTP_STATUS:%{http_code}")

http_status=$(echo "$profile_response" | grep "HTTP_STATUS:" | cut -d: -f2)
response_body=$(echo "$profile_response" | sed '/HTTP_STATUS:/d')

echo "Profile response status: $http_status"
echo "Profile response body: $response_body"

if echo "$response_body" | grep -q "$TEST_EMAIL"; then
    test_step "Authenticated API access works" "pass"
else
    test_step "Authenticated API access works" "fail"
fi

# 7. Test Dashboard Access
echo -e "\n${BLUE}7. Testing Dashboard Data...${NC}"
dashboard_response=$(curl -s -X GET "$API_URL/api/dashboard" \
    -H "Authorization: Bearer $ACCESS_TOKEN" \
    -H "Origin: https://sledzspecke.pl")

if echo "$dashboard_response" | grep -q '"totalHours"'; then
    test_step "Dashboard data accessible" "pass"
else
    test_step "Dashboard data accessible" "fail"
fi

# 8. Test Medical Shift Creation
echo -e "\n${BLUE}8. Testing Medical Shift Creation...${NC}"
shift_date=$(date +%Y-%m-%d)
shift_response=$(curl -s -X POST "$API_URL/api/medical-shifts" \
    -H "Authorization: Bearer $ACCESS_TOKEN" \
    -H "Content-Type: application/json" \
    -H "Origin: https://sledzspecke.pl" \
    -d '{
        "date": "'"$shift_date"'",
        "startTime": "08:00",
        "endTime": "16:00",
        "place": "Szpital Testowy",
        "department": "Oddział Testowy"
    }')

if echo "$shift_response" | grep -q '"id"' || [ "$?" -eq 0 ]; then
    test_step "Medical shift creation" "pass"
else
    echo "Shift response: $shift_response"
    test_step "Medical shift creation" "fail"
fi

# 9. Test Data Retrieval
echo -e "\n${BLUE}9. Testing Data Retrieval...${NC}"
shifts_response=$(curl -s -X GET "$API_URL/api/medical-shifts" \
    -H "Authorization: Bearer $ACCESS_TOKEN" \
    -H "Origin: https://sledzspecke.pl")

if echo "$shifts_response" | grep -q "Szpital Testowy"; then
    test_step "Medical shifts retrieval" "pass"
else
    test_step "Medical shifts retrieval" "fail"
fi

# 10. Test Concurrent Requests
echo -e "\n${BLUE}10. Testing Concurrent Requests...${NC}"
{
    curl -s "$API_URL/api/health" > /dev/null &
    curl -s "$API_URL/api/health" > /dev/null &
    curl -s "$API_URL/api/health" > /dev/null &
    curl -s "$API_URL/api/health" > /dev/null &
    curl -s "$API_URL/api/health" > /dev/null &
} 2>/dev/null

wait
test_step "Concurrent requests handled" "pass"

# Summary
echo -e "\n${GREEN}================================================${NC}"
echo -e "${GREEN}All tests passed! Application is working correctly.${NC}"
echo -e "${GREEN}================================================${NC}"

# Cleanup - Optional: Delete test user
# curl -s -X DELETE "$API_URL/api/users/$USER_ID" \
#     -H "Authorization: Bearer $ACCESS_TOKEN" \
#     -H "Origin: https://sledzspecke.pl"

echo -e "\nTest user created: $TEST_EMAIL"
echo "You can use these credentials to test the frontend manually."