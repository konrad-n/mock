#!/bin/bash

# SledzSpecke Integration Tests - Automated Fix Script
# This script fixes common patterns that are causing compilation errors

echo "========================================="
echo "SledzSpecke Integration Tests Fix Script"
echo "========================================="
echo ""

# Color codes for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Base directory for tests
TEST_DIR="/home/ubuntu/projects/mock/SledzSpecke.WebApi/tests"

# Counter for changes
TOTAL_CHANGES=0

# Function to count changes
count_changes() {
    local changes=$1
    TOTAL_CHANGES=$((TOTAL_CHANGES + changes))
}

echo -e "${YELLOW}Step 1: Fixing SignIn command usage (Email → Username)${NC}"
echo "---------------------------------------------"
find "$TEST_DIR" -name "*.cs" -type f | while read file; do
    # Fix SignIn command constructor
    changes=$(grep -c "new SignIn(Email:" "$file" 2>/dev/null || echo 0)
    if [ $changes -gt 0 ]; then
        sed -i 's/new SignIn(Email:/new SignIn(Username:/g' "$file"
        echo -e "${GREEN}✓${NC} Fixed $changes occurrences in: $(basename "$file")"
        count_changes $changes
    fi
    
    # Fix SignIn parameter names
    changes=$(grep -c "SignIn(\s*Email:" "$file" 2>/dev/null || echo 0)
    if [ $changes -gt 0 ]; then
        sed -i 's/SignIn(\s*Email:/SignIn(Username:/g' "$file"
        count_changes $changes
    fi
done

echo ""
echo -e "${YELLOW}Step 2: Updating AccessTokenDto to JwtDto${NC}"
echo "---------------------------------------------"
find "$TEST_DIR" -name "*.cs" -type f | while read file; do
    changes=$(grep -c "AccessTokenDto" "$file" 2>/dev/null || echo 0)
    if [ $changes -gt 0 ]; then
        sed -i 's/AccessTokenDto/JwtDto/g' "$file"
        echo -e "${GREEN}✓${NC} Updated $changes references in: $(basename "$file")"
        count_changes $changes
    fi
done

echo ""
echo -e "${YELLOW}Step 3: Removing PatientType references${NC}"
echo "---------------------------------------------"
find "$TEST_DIR" -name "*.cs" -type f | while read file; do
    # Replace PatientType enum values with strings
    changes=$(grep -c "PatientType\." "$file" 2>/dev/null || echo 0)
    if [ $changes -gt 0 ]; then
        sed -i 's/PatientType\.Adult/"Adult"/g' "$file"
        sed -i 's/PatientType\.Child/"Child"/g' "$file"
        sed -i 's/PatientType\.Infant/"Infant"/g' "$file"
        sed -i 's/PatientType\.[A-Za-z]*/"Adult"/g' "$file"
        echo -e "${GREEN}✓${NC} Replaced $changes PatientType references in: $(basename "$file")"
        count_changes $changes
    fi
    
    # Remove PatientType type declarations
    sed -i 's/PatientType patientType/string patientType/g' "$file"
done

echo ""
echo -e "${YELLOW}Step 4: Fixing PatientGender references${NC}"
echo "---------------------------------------------"
find "$TEST_DIR" -name "*.cs" -type f | while read file; do
    changes=$(grep -c "PatientGender" "$file" 2>/dev/null || echo 0)
    if [ $changes -gt 0 ]; then
        sed -i 's/PatientGender\./Gender\./g' "$file"
        sed -i 's/PatientGender /Gender /g' "$file"
        echo -e "${GREEN}✓${NC} Fixed $changes PatientGender references in: $(basename "$file")"
        count_changes $changes
    fi
done

echo ""
echo -e "${YELLOW}Step 5: Updating Duration to ShiftDuration${NC}"
echo "---------------------------------------------"
find "$TEST_DIR" -name "*.cs" -type f | while read file; do
    changes=$(grep -c "new Duration(" "$file" 2>/dev/null || echo 0)
    if [ $changes -gt 0 ]; then
        sed -i 's/new Duration(/new ShiftDuration(/g' "$file"
        sed -i 's/Duration duration/ShiftDuration duration/g' "$file"
        sed -i 's/Duration\./ShiftDuration\./g' "$file"
        echo -e "${GREEN}✓${NC} Updated $changes Duration references in: $(basename "$file")"
        count_changes $changes
    fi
done

echo ""
echo -e "${YELLOW}Step 6: Fixing command parameter syntax${NC}"
echo "---------------------------------------------"
find "$TEST_DIR" -name "*.cs" -type f | while read file; do
    # Fix positional parameters to named parameters
    # AddMedicalShift pattern
    sed -i 's/new AddMedicalShift(\([0-9]*\), /new AddMedicalShift(InternshipId: \1, /g' "$file"
    
    # CreateInternship pattern
    sed -i 's/new CreateInternship(\([0-9]*\), /new CreateInternship(SpecializationId: \1, /g' "$file"
    
    # Other common patterns
    sed -i 's/new UpdateMedicalShift(\([0-9]*\), /new UpdateMedicalShift(ShiftId: \1, /g' "$file"
    sed -i 's/new DeleteMedicalShift(\([0-9]*\))/new DeleteMedicalShift(ShiftId: \1)/g' "$file"
done

echo ""
echo -e "${YELLOW}Step 7: Fixing ShiftType references${NC}"
echo "---------------------------------------------"
find "$TEST_DIR" -name "*.cs" -type f | while read file; do
    # Ensure ShiftType enum values are correct
    sed -i 's/ShiftType\.DayShift/ShiftType.Accompanying/g' "$file"
    sed -i 's/ShiftType\.NightShift/ShiftType.Independent/g' "$file"
done

echo ""
echo -e "${YELLOW}Step 8: Adding missing using statements${NC}"
echo "---------------------------------------------"
find "$TEST_DIR" -name "*.cs" -type f | while read file; do
    # Check if file uses JwtDto but doesn't have the using statement
    if grep -q "JwtDto" "$file" && ! grep -q "using SledzSpecke.Application.DTO;" "$file"; then
        # Add using statement after the last using statement
        sed -i '/^using /h;${g;s/$/\nusing SledzSpecke.Application.DTO;/;}' "$file"
        echo -e "${GREEN}✓${NC} Added DTO using statement to: $(basename "$file")"
    fi
done

echo ""
echo -e "${YELLOW}Step 9: Fixing SignUp commands to include Username${NC}"
echo "---------------------------------------------"
find "$TEST_DIR" -name "*.cs" -type f | while read file; do
    # Skip the TestDataFactoryExtensions file itself
    if [[ "$file" == *"TestDataFactoryExtensions.cs" ]]; then
        continue
    fi
    
    # Fix SignUp commands that are missing Username parameter
    # Pattern 1: SignUp with Email as first parameter but no Username
    if grep -q "new SignUp(Email:" "$file" && ! grep -q "Username:" "$file"; then
        # Extract email value and add Username based on it
        perl -i -pe 's/new SignUp\(\s*Email:\s*"([^"]+)"([^)]*)\)/
            my $email = $1;
            my $rest = $2;
            my $username = $email;
            $username =~ s/@.*//;
            "new SignUp(Email: \"$email\", Username: \"$username\"$rest)"/ge' "$file"
        echo -e "${GREEN}✓${NC} Fixed SignUp commands in: $(basename "$file")"
    fi
done

echo ""
echo -e "${YELLOW}Step 10: Creating TestDataFactory extensions${NC}"
echo "---------------------------------------------"

# Create an extended TestDataFactory if it doesn't have these methods
FACTORY_EXT_FILE="$TEST_DIR/SledzSpecke.Tests.Integration/Common/TestDataFactoryExtensions.cs"

if [ ! -f "$FACTORY_EXT_FILE" ]; then
cat > "$FACTORY_EXT_FILE" << 'EOF'
using SledzSpecke.Application.Commands;
using SledzSpecke.Application.DTO;
using SledzSpecke.Core.ValueObjects;
using System;

namespace SledzSpecke.Tests.Integration.Common;

public static class TestDataFactoryExtensions
{
    public static SignUp CreateSignUpCommand(
        string? email = null,
        string? username = null,
        string? password = null,
        string? firstName = null,
        string? lastName = null)
    {
        var generatedEmail = email ?? $"test{Guid.NewGuid():N}@example.com";
        return new SignUp(
            Email: generatedEmail,
            Username: username ?? generatedEmail.Split('@')[0],
            Password: password ?? "SecurePassword123!",
            FirstName: firstName ?? "Test",
            LastName: lastName ?? "User",
            Pesel: "90010123456",
            PwzNumber: "1234567",
            PhoneNumber: "+48123456789",
            DateOfBirth: new DateTime(1990, 1, 1),
            CorrespondenceAddress: new Commands.AddressDto(
                Street: "Test Street",
                HouseNumber: "1",
                ApartmentNumber: null,
                PostalCode: "00-001",
                City: "Warsaw",
                Province: "Mazowieckie"
            )
        );
    }
    
    public static SignIn CreateSignInCommand(string? username = null, string? password = null)
    {
        return new SignIn(
            Username: username ?? "testuser",
            Password: password ?? "TestPassword123!"
        );
    }
    
    public static AddMedicalShift CreateAddMedicalShiftCommand(
        int? internshipId = null,
        DateTime? date = null,
        int hours = 8,
        int minutes = 0,
        string? location = null,
        int year = 3)
    {
        return new AddMedicalShift(
            InternshipId: internshipId ?? 1,
            Date: date ?? DateTime.Today,
            Hours: hours,
            Minutes: minutes,
            Location: location ?? "Test Hospital",
            Year: year
        );
    }
}
EOF
echo -e "${GREEN}✓${NC} Created TestDataFactoryExtensions.cs"
fi

echo ""
echo "========================================="
echo -e "${GREEN}Fix script completed!${NC}"
echo "========================================="
echo -e "Total changes made: ${GREEN}$TOTAL_CHANGES${NC}"
echo ""
echo -e "${YELLOW}Next steps:${NC}"
echo "1. Run 'dotnet build' to check remaining compilation errors"
echo "2. Fix any SignUp commands manually (they need full parameter list)"
echo "3. Update specific test assertions as needed"
echo "4. Run 'dotnet test' to verify functionality"
echo ""
echo -e "${YELLOW}Tip:${NC} Use the TestDataFactoryExtensions for creating test commands!"