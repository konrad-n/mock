#!/bin/bash

# API endpoints testing script
API_BASE="https://api.sledzspecke.pl"

# Color codes for output
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Arrays to store results
declare -A results_by_status

# Function to test endpoint
test_endpoint() {
    local method=$1
    local endpoint=$2
    local full_url="${API_BASE}${endpoint}"
    
    # For endpoints with parameters, use sample values
    if [[ $endpoint == *"{userId}"* ]]; then
        endpoint_test="${endpoint//\{userId\}/1}"
        full_url="${API_BASE}${endpoint_test}"
    elif [[ $endpoint == *"{specializationId}"* ]]; then
        endpoint_test="${endpoint//\{specializationId\}/1}"
        full_url="${API_BASE}${endpoint_test}"
    elif [[ $endpoint == *"{id}"* ]]; then
        endpoint_test="${endpoint//\{id\}/1}"
        full_url="${API_BASE}${endpoint_test}"
    elif [[ $endpoint == *"{courseId}"* ]]; then
        endpoint_test="${endpoint//\{courseId\}/1}"
        full_url="${API_BASE}${endpoint_test}"
    elif [[ $endpoint == *"{internshipId}"* ]]; then
        endpoint_test="${endpoint//\{internshipId\}/1}"
        full_url="${API_BASE}${endpoint_test}"
    elif [[ $endpoint == *"{shiftId}"* ]]; then
        endpoint_test="${endpoint//\{shiftId\}/1}"
        full_url="${API_BASE}${endpoint_test}"
    elif [[ $endpoint == *"{procedureId}"* ]]; then
        endpoint_test="${endpoint//\{procedureId\}/1}"
        full_url="${API_BASE}${endpoint_test}"
    elif [[ $endpoint == *"{fileId}"* ]]; then
        endpoint_test="${endpoint//\{fileId\}/1}"
        full_url="${API_BASE}${endpoint_test}"
    elif [[ $endpoint == *"{entityType}"* ]] && [[ $endpoint == *"{entityId}"* ]]; then
        endpoint_test="${endpoint//\{entityType\}/course}"
        endpoint_test="${endpoint_test//\{entityId\}/1}"
        full_url="${API_BASE}${endpoint_test}"
    elif [[ $endpoint == *"{specializationCode}"* ]] && [[ $endpoint == *"{smkVersion}"* ]]; then
        endpoint_test="${endpoint//\{specializationCode\}/ANEST}"
        endpoint_test="${endpoint_test//\{smkVersion\}/old}"
        endpoint_test="${endpoint_test//\{moduleId\}/1}"
        full_url="${API_BASE}${endpoint_test}"
    elif [[ $endpoint == *"{year}"* ]]; then
        endpoint_test="${endpoint//\{year\}/2024}"
        full_url="${API_BASE}${endpoint_test}"
    elif [[ $endpoint == *"{type}"* ]]; then
        endpoint_test="${endpoint//\{type\}/conference}"
        full_url="${API_BASE}${endpoint_test}"
    fi
    
    # Use appropriate request method
    if [[ $method == "get" ]]; then
        response=$(curl -s -o /dev/null -w "%{http_code}" -X GET "$full_url" -H "Accept: application/json")
    elif [[ $method == "post" ]] || [[ $method == "put" ]] || [[ $method == "delete" ]]; then
        # For non-GET methods, just check if endpoint exists (expect 401 or 400)
        response=$(curl -s -o /dev/null -w "%{http_code}" -X $method "$full_url" -H "Content-Type: application/json" -H "Accept: application/json" -d '{}')
    else
        response="000"
    fi
    
    # Store result
    results_by_status[$response]+="$method $(echo $endpoint | sed 's/{[^}]*}/\*/g')\n"
    
    # Print result
    if [[ $response == "200" ]] || [[ $response == "204" ]]; then
        echo -e "${GREEN}✓${NC} $method $endpoint → $response"
    elif [[ $response == "401" ]] || [[ $response == "403" ]]; then
        echo -e "${YELLOW}⚠${NC} $method $endpoint → $response (Auth required)"
    elif [[ $response == "400" ]] || [[ $response == "404" ]]; then
        echo -e "${YELLOW}!${NC} $method $endpoint → $response"
    else
        echo -e "${RED}✗${NC} $method $endpoint → $response"
    fi
}

echo "Testing SledzSpecke API Endpoints"
echo "================================="
echo ""

# Test each endpoint
while IFS= read -r line; do
    endpoint=$(echo $line | awk '{print $1}')
    method=$(echo $line | awk '{print $2}')
    test_endpoint "$method" "$endpoint"
done << EOF
/api/Absences post
/api/Absences/user/{userId} get
/api/Absences/{id} delete
/api/Absences/{id} put
/api/Absences/{id}/approve put
/api/Calculations/internship-days post
/api/Calculations/normalize-time post
/api/Calculations/required-shift-hours get
/api/Courses get
/api/Courses post
/api/Courses/{courseId} delete
/api/Courses/{courseId} get
/api/Courses/{courseId} put
/api/Courses/{courseId}/approve post
/api/Courses/{courseId}/complete post
/api/Dashboard/overview get
/api/Dashboard/progress/{specializationId} get
/api/Dashboard/statistics/{specializationId} get
/api/EducationalActivities post
/api/EducationalActivities/specialization/{specializationId} get
/api/EducationalActivities/specialization/{specializationId}/type/{type} get
/api/EducationalActivities/{id} delete
/api/EducationalActivities/{id} get
/api/EducationalActivities/{id} put
/api/Internships get
/api/Internships post
/api/Internships/{internshipId} delete
/api/Internships/{internshipId} get
/api/Internships/{internshipId} put
/api/Internships/{internshipId}/approve post
/api/Internships/{internshipId}/complete post
/api/Logs/errors get
/api/Logs/recent get
/api/Logs/stats get
/api/MedicalShifts get
/api/MedicalShifts post
/api/MedicalShifts/statistics get
/api/MedicalShifts/{shiftId} delete
/api/MedicalShifts/{shiftId} get
/api/MedicalShifts/{shiftId} put
/api/Modules/specialization/{specializationId} get
/api/Modules/switch put
/api/Procedures get
/api/Procedures post
/api/Procedures/statistics get
/api/Procedures/{procedureId} delete
/api/Procedures/{procedureId} get
/api/Procedures/{procedureId} put
/api/Publications post
/api/Publications/user/{userId} get
/api/Publications/user/{userId}/first-author get
/api/Publications/user/{userId}/peer-reviewed get
/api/Publications/user/{userId}/specialization/{specializationId}/impact-score get
/api/Publications/{id} delete
/api/Publications/{id} put
/api/Recognitions post
/api/Recognitions/user/{userId} get
/api/Recognitions/user/{userId}/specialization/{specializationId}/total-reduction get
/api/Recognitions/{id} delete
/api/Recognitions/{id} put
/api/Recognitions/{id}/approve put
/api/SelfEducation post
/api/SelfEducation/user/{userId} get
/api/SelfEducation/user/{userId}/specialization/{specializationId}/completed get
/api/SelfEducation/user/{userId}/specialization/{specializationId}/credit-hours get
/api/SelfEducation/user/{userId}/specialization/{specializationId}/quality-score get
/api/SelfEducation/user/{userId}/year/{year} get
/api/SelfEducation/{id} delete
/api/SelfEducation/{id} put
/api/SelfEducation/{id}/complete put
/api/Specializations/{specializationId} get
/api/Specializations/{specializationId}/statistics get
/api/Users get
/api/Users/{userId} get
/api/auth/sign-in post
/api/auth/sign-up post
/api/files/entity/{entityType}/{entityId} get
/api/files/upload post
/api/files/{fileId} delete
/api/files/{fileId}/download get
/api/specialization-templates get
/api/specialization-templates/{specializationCode}/{smkVersion} get
/api/specialization-templates/{specializationCode}/{smkVersion}/courses/{courseId} get
/api/specialization-templates/{specializationCode}/{smkVersion}/internships/{internshipId} get
/api/specialization-templates/{specializationCode}/{smkVersion}/modules/{moduleId} get
/api/specialization-templates/{specializationCode}/{smkVersion}/procedures/{procedureId} get
/api/specialization-templates/{specializationCode}/{smkVersion}/procedures/{procedureId}/validate post
/api/users/change-password put
/api/users/preferences put
/api/users/profile get
/api/users/profile put
/monitoring/dashboard get
/monitoring/health get
EOF

echo ""
echo "Summary by Status Code"
echo "====================="
echo ""

# Print summary
for status in "${!results_by_status[@]}"; do
    echo "Status $status:"
    echo -e "${results_by_status[$status]}"
done