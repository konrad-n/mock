#!/usr/bin/env python3
"""Test script for specialized SMK entities (ProcedureOldSmk, ProcedureNewSmk)"""

import json
import requests
import sys
from datetime import datetime, timedelta

BASE_URL = "http://localhost:5000/api"
AUTH_HEADERS = {}

def login():
    """Login and get auth token"""
    login_data = {
        "username": "testuser",
        "password": "Test123!"
    }
    
    response = requests.post(f"{BASE_URL}/auth/sign-in", json=login_data)
    if response.status_code == 200:
        token = response.json()["AccessToken"]
        AUTH_HEADERS["Authorization"] = f"Bearer {token}"
        print("✓ Login successful")
        return True
    else:
        print(f"✗ Login failed: {response.status_code}")
        print(f"  Response: {response.text}")
        return False

def get_specialization_info():
    """Get current user's specialization info"""
    response = requests.get(f"{BASE_URL}/specializations/current", headers=AUTH_HEADERS)
    if response.status_code == 200:
        return response.json()
    return None

def test_old_smk_procedures():
    """Test Old SMK procedure creation and validation"""
    print("\n=== Testing Old SMK Procedures ===")
    
    # Get specialization info
    spec_info = get_specialization_info()
    if not spec_info:
        print("✗ Could not get specialization info")
        return False
    
    smk_version = spec_info.get("smkVersion", "")
    print(f"Current SMK version: {smk_version}")
    
    if smk_version.lower() != "old":
        print("✓ Skipping Old SMK tests (user has New SMK)")
        return True
    
    # Create procedure with Old SMK specific fields
    procedure_data = {
        "internshipId": 1,
        "date": datetime.now().isoformat(),
        "code": "PROC001",
        "location": "Hospital Ward A",
        "operatorCode": "A",
        "performingPerson": "Dr. Johnson",
        "patientInitials": "JD",
        "patientGender": "M",
        "procedureRequirementId": 1,
        "procedureGroup": "Cardiac",
        "assistantData": "Nurse Smith",
        "internshipName": "Cardiology Rotation"
    }
    
    response = requests.post(f"{BASE_URL}/procedures", json=procedure_data, headers=AUTH_HEADERS)
    if response.status_code == 200:
        print("✓ Created Old SMK procedure with specific fields")
        procedure_id = response.json()["id"]
        
        # Verify the procedure has Old SMK fields
        response = requests.get(f"{BASE_URL}/procedures/{procedure_id}", headers=AUTH_HEADERS)
        if response.status_code == 200:
            procedure = response.json()
            
            # Check Old SMK specific fields
            if procedure.get("smkVersion") == "Old":
                print("✓ Procedure correctly identified as Old SMK")
            else:
                print("✗ Procedure SMK version mismatch")
                
            if procedure.get("procedureGroup") == "Cardiac":
                print("✓ Procedure group preserved")
            else:
                print("✗ Procedure group not preserved")
                
            if procedure.get("assistantData") == "Nurse Smith":
                print("✓ Assistant data preserved")
            else:
                print("✗ Assistant data not preserved")
    else:
        print(f"✗ Failed to create Old SMK procedure: {response.status_code}")
        print(f"  Response: {response.text}")
        return False
    
    # Test invalid operator code for Old SMK
    invalid_procedure = procedure_data.copy()
    invalid_procedure["operatorCode"] = "C"  # Invalid for Old SMK
    
    response = requests.post(f"{BASE_URL}/procedures", json=invalid_procedure, headers=AUTH_HEADERS)
    if response.status_code == 400:
        print("✓ Correctly rejected invalid operator code 'C' for Old SMK")
    else:
        print("✗ Should have rejected operator code 'C' for Old SMK")
    
    return True

def test_new_smk_procedures():
    """Test New SMK procedure creation and validation"""
    print("\n=== Testing New SMK Procedures ===")
    
    # Get specialization info
    spec_info = get_specialization_info()
    if not spec_info:
        print("✗ Could not get specialization info")
        return False
    
    smk_version = spec_info.get("smkVersion", "")
    current_module_id = spec_info.get("currentModuleId")
    
    print(f"Current SMK version: {smk_version}")
    print(f"Current module ID: {current_module_id}")
    
    if smk_version.lower() != "new":
        print("✓ Skipping New SMK tests (user has Old SMK)")
        return True
    
    if not current_module_id:
        print("✗ No current module set for New SMK")
        return False
    
    # Create procedure with New SMK specific fields (aggregated counts)
    procedure_data = {
        "internshipId": 1,
        "date": datetime.now().isoformat(),
        "code": "PROC001",
        "location": "Hospital Ward B",
        "moduleId": current_module_id,
        "procedureRequirementId": 1,
        "procedureName": "Basic Cardiac Procedure",
        "countA": 5,
        "countB": 3,
        "supervisor": "Dr. Williams",
        "institution": "City Hospital",
        "comments": "Multiple procedures performed during shift"
    }
    
    response = requests.post(f"{BASE_URL}/procedures", json=procedure_data, headers=AUTH_HEADERS)
    if response.status_code == 200:
        print("✓ Created New SMK procedure with aggregated counts")
        procedure_id = response.json()["id"]
        
        # Verify the procedure has New SMK fields
        response = requests.get(f"{BASE_URL}/procedures/{procedure_id}", headers=AUTH_HEADERS)
        if response.status_code == 200:
            procedure = response.json()
            
            # Check New SMK specific fields
            if procedure.get("smkVersion") == "New":
                print("✓ Procedure correctly identified as New SMK")
            else:
                print("✗ Procedure SMK version mismatch")
                
            if procedure.get("countA") == 5 and procedure.get("countB") == 3:
                print("✓ Aggregated counts preserved")
            else:
                print("✗ Aggregated counts not preserved")
                
            if procedure.get("supervisor") == "Dr. Williams":
                print("✓ Supervisor preserved")
            else:
                print("✗ Supervisor not preserved")
                
            if procedure.get("moduleId") == current_module_id:
                print("✓ Module ID correctly set")
            else:
                print("✗ Module ID not preserved")
    else:
        print(f"✗ Failed to create New SMK procedure: {response.status_code}")
        print(f"  Response: {response.text}")
        return False
    
    # Test procedure without counts (should fail)
    invalid_procedure = procedure_data.copy()
    invalid_procedure["countA"] = 0
    invalid_procedure["countB"] = 0
    
    response = requests.post(f"{BASE_URL}/procedures", json=invalid_procedure, headers=AUTH_HEADERS)
    if response.status_code == 400:
        print("✓ Correctly rejected procedure with zero counts")
    else:
        print("✗ Should have rejected procedure with zero counts")
    
    # Test updating counts
    if procedure_id:
        update_data = {
            "countA": 8,
            "countB": 5
        }
        response = requests.put(f"{BASE_URL}/procedures/{procedure_id}/counts", 
                              json=update_data, headers=AUTH_HEADERS)
        if response.status_code == 200:
            print("✓ Successfully updated procedure counts")
        else:
            print(f"✗ Failed to update counts: {response.status_code}")
    
    return True

def test_smk_version_validation():
    """Test that procedures validate correctly based on SMK version"""
    print("\n=== Testing SMK Version Validation ===")
    
    spec_info = get_specialization_info()
    if not spec_info:
        print("✗ Could not get specialization info")
        return False
    
    smk_version = spec_info.get("smkVersion", "")
    
    # Try to create a procedure with wrong SMK version fields
    if smk_version.lower() == "old":
        # Try to use New SMK fields on Old SMK
        procedure_data = {
            "internshipId": 1,
            "date": datetime.now().isoformat(),
            "code": "PROC001",
            "location": "Test Location",
            "countA": 5,  # New SMK field
            "countB": 3,  # New SMK field
            "moduleId": 1  # New SMK field
        }
        
        response = requests.post(f"{BASE_URL}/procedures", json=procedure_data, headers=AUTH_HEADERS)
        if response.status_code == 400:
            print("✓ Correctly rejected New SMK fields for Old SMK user")
        else:
            print("✗ Should have rejected New SMK fields for Old SMK user")
            
    else:  # New SMK
        # Try to use Old SMK fields on New SMK
        procedure_data = {
            "internshipId": 1,
            "date": datetime.now().isoformat(),
            "code": "PROC001",
            "location": "Test Location",
            "year": 3,  # Old SMK field
            "operatorCode": "A",  # Old SMK uses individual operator codes
            "patientInitials": "JD"  # Old SMK tracks individual patients
        }
        
        response = requests.post(f"{BASE_URL}/procedures", json=procedure_data, headers=AUTH_HEADERS)
        if response.status_code == 400:
            print("✓ Correctly rejected Old SMK fields for New SMK user")
        else:
            print("✗ Should have rejected Old SMK fields for New SMK user")
    
    return True

def main():
    """Run all SMK entity tests"""
    print("Starting SMK entity tests...")
    
    if not login():
        print("\nFailed to authenticate. Exiting.")
        sys.exit(1)
    
    all_passed = True
    
    # Run tests
    all_passed &= test_old_smk_procedures()
    all_passed &= test_new_smk_procedures()
    all_passed &= test_smk_version_validation()
    
    if all_passed:
        print("\n✅ All SMK entity tests passed!")
    else:
        print("\n❌ Some SMK entity tests failed!")
        sys.exit(1)

if __name__ == "__main__":
    main()