#!/usr/bin/env python3
"""Integration tests for SMK version-specific procedure behaviors"""

import requests
import json
from datetime import datetime, timedelta, timezone

# API configuration
API_URL = "http://localhost:5000/api"
USERNAME = "testuser"
PASSWORD = "Test123!"

def get_auth_token():
    """Get authentication token"""
    response = requests.post(f"{API_URL}/auth/sign-in", json={
        "username": USERNAME,
        "password": PASSWORD
    })
    if response.status_code == 200:
        return response.json()['AccessToken']
    else:
        print(f"Authentication failed: {response.status_code}")
        print(response.text)
        return None

def create_test_internship(token):
    """Create a test internship"""
    headers = {"Authorization": f"Bearer {token}"}
    internship_data = {
        "startDate": datetime.now(timezone.utc).isoformat(),
        "endDate": (datetime.now(timezone.utc) + timedelta(days=30)).isoformat(),
        "name": "Test Internship for SMK Validation",
        "location": "Test Hospital",
        "moduleId": 1,
        "coordinatorName": "Dr. Test Coordinator",
        "coordinatorEmail": "test@hospital.com",
        "coordinatorPhone": "+48123456789",
        "departmentName": "Test Department",
        "institutionName": "Test Medical Institution"
    }
    
    response = requests.post(f"{API_URL}/internships", json=internship_data, headers=headers)
    if response.status_code == 201:
        return response.json()
    else:
        print(f"Failed to create internship: {response.status_code}")
        print(response.text)
        return None

def test_procedure_creation_variations(token, internship_id):
    """Test different procedure creation scenarios"""
    print("\n=== Testing Procedure Creation Variations ===")
    
    headers = {"Authorization": f"Bearer {token}"}
    test_cases = []
    
    # Test case 1: Basic procedure with minimum fields
    test_cases.append({
        "name": "Minimal Procedure",
        "data": {
            "internshipId": internship_id,
            "date": datetime.now(timezone.utc).isoformat(),
            "year": 1,
            "code": "MIN001",
            "location": "Hospital",
            "status": "Pending"
        },
        "expect_success": True
    })
    
    # Test case 2: Old SMK style procedure
    test_cases.append({
        "name": "Old SMK Style Procedure",
        "data": {
            "internshipId": internship_id,
            "date": datetime.now(timezone.utc).isoformat(),
            "year": 2,
            "code": "OLD001",
            "location": "Old Hospital",
            "status": "Completed",
            "operatorCode": "A",
            "performingPerson": "Dr. Old",
            "patientInitials": "JD",
            "patientGender": "M",
            "procedureRequirementId": 1,
            "procedureGroup": "Cardiac",
            "assistantData": "Assistant info",
            "internshipName": "Old SMK Internship"
        },
        "expect_success": True
    })
    
    # Test case 3: New SMK style procedure
    test_cases.append({
        "name": "New SMK Style Procedure",
        "data": {
            "internshipId": internship_id,
            "date": datetime.now(timezone.utc).isoformat(),
            "year": 1,
            "code": "NEW001",
            "location": "New Hospital",
            "status": "Pending",
            "moduleId": 1,
            "procedureName": "Advanced Procedure",
            "countA": 3,
            "countB": 2,
            "supervisor": "Prof. New",
            "institution": "Medical Center",
            "comments": "Multiple procedures"
        },
        "expect_success": True
    })
    
    # Test case 4: Mixed fields (both Old and New SMK)
    test_cases.append({
        "name": "Mixed SMK Fields",
        "data": {
            "internshipId": internship_id,
            "date": datetime.now(timezone.utc).isoformat(),
            "year": 1,
            "code": "MIX001",
            "location": "Mixed Hospital",
            "status": "Pending",
            # Old SMK fields
            "operatorCode": "B",
            "procedureGroup": "Surgery",
            # New SMK fields
            "countA": 1,
            "countB": 1,
            "moduleId": 1
        },
        "expect_success": True  # Should succeed as API accepts both
    })
    
    # Test case 5: Future date (should succeed now)
    test_cases.append({
        "name": "Future Date Procedure",
        "data": {
            "internshipId": internship_id,
            "date": (datetime.now(timezone.utc) + timedelta(days=90)).isoformat(),
            "year": 1,
            "code": "FUT001",
            "location": "Future Hospital",
            "status": "Pending"
        },
        "expect_success": True
    })
    
    # Run test cases
    results = []
    for test_case in test_cases:
        print(f"\nTesting: {test_case['name']}")
        response = requests.post(f"{API_URL}/procedures", json=test_case['data'], headers=headers)
        
        success = response.status_code == 201
        expected = test_case['expect_success']
        
        if success == expected:
            print(f"✅ PASS: Got expected result (success={success})")
            if success:
                procedure_id = response.json()
                print(f"   Created procedure ID: {procedure_id}")
                results.append(procedure_id)
        else:
            print(f"❌ FAIL: Expected success={expected}, got success={success}")
            print(f"   Status: {response.status_code}")
            print(f"   Response: {response.text}")
    
    return results

def test_procedure_retrieval(token, procedure_ids):
    """Test retrieving procedures and verify fields are preserved"""
    print("\n=== Testing Procedure Retrieval ===")
    
    headers = {"Authorization": f"Bearer {token}"}
    
    for proc_id in procedure_ids[:3]:  # Test first 3 procedures
        response = requests.get(f"{API_URL}/procedures/{proc_id}", headers=headers)
        if response.status_code == 200:
            data = response.json()
            print(f"\nProcedure {proc_id}:")
            print(f"  Code: {data.get('Code')}")
            print(f"  Status: {data.get('Status')}")
            print(f"  Year: {data.get('Year')}")
            
            # Check for Old SMK fields
            if data.get('ProcedureGroup'):
                print(f"  [Old SMK] ProcedureGroup: {data.get('ProcedureGroup')}")
            if data.get('OperatorCode'):
                print(f"  [Old SMK] OperatorCode: {data.get('OperatorCode')}")
                
            # Check for New SMK fields
            if data.get('CountA') is not None:
                print(f"  [New SMK] CountA: {data.get('CountA')}")
            if data.get('CountB') is not None:
                print(f"  [New SMK] CountB: {data.get('CountB')}")
            if data.get('ModuleId'):
                print(f"  [New SMK] ModuleId: {data.get('ModuleId')}")
        else:
            print(f"❌ Failed to retrieve procedure {proc_id}: {response.status_code}")

def test_medical_shift_behaviors(token, internship_id):
    """Test medical shift creation and statistics"""
    print("\n=== Testing Medical Shift Behaviors ===")
    
    headers = {"Authorization": f"Bearer {token}"}
    
    # Create shifts for different years
    shifts_created = []
    for year in [1, 2, 3]:
        shift_data = {
            "internshipId": internship_id,
            "date": (datetime.now(timezone.utc) + timedelta(days=year*5)).isoformat(),
            "hours": 8,
            "minutes": year * 15,  # Different minutes for each year
            "location": f"Hospital Year {year}",
            "year": year
        }
        
        response = requests.post(f"{API_URL}/medicalshifts", json=shift_data, headers=headers)
        if response.status_code == 201:
            shift_id = response.json()
            shifts_created.append((shift_id, year))
            print(f"✅ Created medical shift for year {year}, ID: {shift_id}")
        else:
            print(f"❌ Failed to create shift for year {year}: {response.text}")
    
    # Test year-based statistics
    print("\nTesting year-based statistics:")
    for year in [1, 2, 3]:
        response = requests.get(f"{API_URL}/medicalshifts/statistics?year={year}", headers=headers)
        if response.status_code == 200:
            stats = response.json()
            print(f"  Year {year}: {stats['TotalHours']}h {stats['TotalMinutes']}m")

def test_statistics_endpoints(token):
    """Test statistics endpoints work correctly"""
    print("\n=== Testing Statistics Endpoints ===")
    
    headers = {"Authorization": f"Bearer {token}"}
    
    # Test procedure statistics
    response = requests.get(f"{API_URL}/procedures/statistics", headers=headers)
    if response.status_code == 200:
        stats = response.json()
        print(f"✅ Procedure statistics:")
        print(f"   Completed: A={stats['CompletedCountA']}, B={stats['CompletedCountB']}")
        print(f"   Remaining: A={stats['RemainingCountA']}, B={stats['RemainingCountB']}")
    else:
        print(f"❌ Failed to get procedure statistics: {response.status_code}")
    
    # Test medical shift statistics
    response = requests.get(f"{API_URL}/medicalshifts/statistics", headers=headers)
    if response.status_code == 200:
        stats = response.json()
        print(f"✅ Medical shift statistics:")
        print(f"   Total: {stats['TotalHours']}h {stats['TotalMinutes']}m")
        print(f"   Approved: {stats['ApprovedHours']}h {stats['ApprovedMinutes']}m")
    else:
        print(f"❌ Failed to get medical shift statistics: {response.status_code}")

def main():
    print("=" * 60)
    print("SledzSpecke Web API Integration Tests")
    print("Testing SMK Version-Specific Behaviors")
    print("=" * 60)
    
    # Get authentication token
    token = get_auth_token()
    if not token:
        print("Failed to authenticate. Exiting.")
        return
    
    print(f"✅ Authenticated successfully")
    
    # Create test internship
    internship_id = create_test_internship(token)
    if not internship_id:
        print("Failed to create test internship. Exiting.")
        return
    
    print(f"✅ Created test internship ID: {internship_id}")
    
    # Run tests
    procedure_ids = test_procedure_creation_variations(token, internship_id)
    test_procedure_retrieval(token, procedure_ids)
    test_medical_shift_behaviors(token, internship_id)
    test_statistics_endpoints(token)
    
    print("\n" + "=" * 60)
    print("Integration testing completed!")
    print("=" * 60)

if __name__ == "__main__":
    main()