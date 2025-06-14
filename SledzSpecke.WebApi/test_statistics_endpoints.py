#!/usr/bin/env python3
import requests
import json
import sys
from datetime import datetime, timezone

# Configuration
BASE_URL = "http://localhost:5000/api"
USERNAME = "testuser"
PASSWORD = "Test123!"

def authenticate():
    """Authenticate and get JWT token"""
    response = requests.post(f"{BASE_URL}/auth/sign-in", json={
        "username": USERNAME,
        "password": PASSWORD
    })
    
    if response.status_code != 200:
        print(f"❌ Authentication failed: {response.status_code}")
        print(response.text)
        sys.exit(1)
    
    token = response.json()["AccessToken"]
    return {"Authorization": f"Bearer {token}"}

def test_procedure_statistics(headers):
    """Test procedure statistics endpoint"""
    print("\n📊 Testing Procedure Statistics Endpoint")
    
    # Test without filters
    response = requests.get(f"{BASE_URL}/procedures/statistics", headers=headers)
    if response.status_code == 200:
        stats = response.json()
        print(f"✅ GET /procedures/statistics - Success")
        print(f"   Response: {json.dumps(stats, indent=2)}")
        # The API returns camelCase keys
        if 'requiredCountA' in stats:
            print(f"   Required: A={stats.get('requiredCountA', 0)}, B={stats.get('requiredCountB', 0)}")
            print(f"   Completed: A={stats.get('completedCountA', 0)}, B={stats.get('completedCountB', 0)}")
            print(f"   Approved: A={stats.get('approvedCountA', 0)}, B={stats.get('approvedCountB', 0)}")
            print(f"   Remaining: A={stats.get('remainingCountA', 0)}, B={stats.get('remainingCountB', 0)}")
    else:
        print(f"❌ GET /procedures/statistics failed: {response.status_code}")
        print(response.text)
    
    # Test with module filter
    response = requests.get(f"{BASE_URL}/procedures/statistics?moduleId=101", headers=headers)
    if response.status_code == 200:
        stats = response.json()
        print(f"\n✅ GET /procedures/statistics?moduleId=101 - Success")
        print(f"   Module 101 statistics retrieved")
    else:
        print(f"❌ GET /procedures/statistics?moduleId=101 failed: {response.status_code}")

def test_medical_shift_statistics(headers):
    """Test medical shift statistics endpoint"""
    print("\n📊 Testing Medical Shift Statistics Endpoint")
    
    # Test without filters
    response = requests.get(f"{BASE_URL}/medicalshifts/statistics", headers=headers)
    if response.status_code == 200:
        stats = response.json()
        print(f"✅ GET /medical-shifts/statistics - Success")
        print(f"   Response: {json.dumps(stats, indent=2)}")
        if 'totalHours' in stats:
            print(f"   Total: {stats.get('totalHours', 0)}h {stats.get('totalMinutes', 0)}m")
            print(f"   Approved: {stats.get('approvedHours', 0)}h {stats.get('approvedMinutes', 0)}m")
    else:
        print(f"❌ GET /medical-shifts/statistics failed: {response.status_code}")
        print(response.text)
    
    # Test with year filter (for Old SMK)
    response = requests.get(f"{BASE_URL}/medicalshifts/statistics?year=1", headers=headers)
    if response.status_code == 200:
        stats = response.json()
        print(f"\n✅ GET /medical-shifts/statistics?year=1 - Success")
        print(f"   Year 1 statistics retrieved")
    else:
        print(f"❌ GET /medical-shifts/statistics?year=1 failed: {response.status_code}")

def create_test_data(headers):
    """Create some test procedures and medical shifts"""
    print("\n🔧 Creating test data...")
    
    # Get user's internships first
    response = requests.get(f"{BASE_URL}/internships", headers=headers)
    if response.status_code != 200:
        print("❌ Failed to get internships")
        return
    
    internships = response.json()
    if not internships:
        print("❌ No internships found")
        return
    
    internship_id = internships[0]["id"]
    
    # Create a procedure
    procedure_data = {
        "internshipId": internship_id,
        "date": datetime.now(timezone.utc).isoformat(),
        "year": 1,
        "code": "TEST001",
        "location": "Test Hospital",
        "status": "Completed",
        "operatorCode": "OP",
        "performingPerson": "Dr. Test",
        "patientInitials": "JD",
        "patientGender": "M",
        "moduleId": 101,
        "procedureName": "Test Procedure",
        "countA": 1,
        "countB": 0,
        "supervisor": "Dr. Supervisor",
        "institution": "Test Institution",
        "comments": "Test comment"
    }
    
    response = requests.post(f"{BASE_URL}/procedures", json=procedure_data, headers=headers)
    if response.status_code == 201:
        print("✅ Created test procedure")
    else:
        print(f"❌ Failed to create procedure: {response.status_code}")
        print(response.text)
    
    # Create a medical shift
    shift_data = {
        "internshipId": internship_id,
        "date": datetime.now(timezone.utc).isoformat(),
        "hours": 8,
        "minutes": 30,
        "location": "Test Hospital",
        "year": 1
    }
    
    response = requests.post(f"{BASE_URL}/medical-shifts", json=shift_data, headers=headers)
    if response.status_code == 201:
        print("✅ Created test medical shift")
    else:
        print(f"❌ Failed to create medical shift: {response.status_code}")
        print(response.text)

def main():
    print("🧪 Testing Statistics Endpoints")
    print("=" * 50)
    
    # Authenticate
    headers = authenticate()
    print("✅ Authentication successful")
    
    # Create test data
    create_test_data(headers)
    
    # Test statistics endpoints
    test_procedure_statistics(headers)
    test_medical_shift_statistics(headers)
    
    print("\n✨ Statistics endpoint testing completed!")

if __name__ == "__main__":
    main()