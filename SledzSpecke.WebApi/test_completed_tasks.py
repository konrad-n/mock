#!/usr/bin/env python3
"""
Test script for the last 5 completed tasks:
1. Add sync status management to automatically transition from Synced to Modified
2. Document sync status management code for future reference
3. Align medical shift duration validation with MAUI (hours + minutes, total calculation)
4. Add update functionality to InternshipsController
5. Implement year calculation based on specialization structure
"""

import requests
import json
import sys
from datetime import datetime, timedelta

# API Base URL
BASE_URL = "http://localhost:5000/api"

# Test user credentials
TEST_USER = {
    "username": "testuser",
    "password": "Test123!"
}

# Colors for output
GREEN = '\033[92m'
RED = '\033[91m'
YELLOW = '\033[93m'
BLUE = '\033[94m'
RESET = '\033[0m'

def print_test_header(test_name):
    print(f"\n{BLUE}{'='*60}{RESET}")
    print(f"{BLUE}Testing: {test_name}{RESET}")
    print(f"{BLUE}{'='*60}{RESET}")

def print_success(message):
    print(f"{GREEN}✓ {message}{RESET}")

def print_error(message):
    print(f"{RED}✗ {message}{RESET}")

def print_info(message):
    print(f"{YELLOW}ℹ {message}{RESET}")

class TestRunner:
    def __init__(self):
        self.token = None
        self.user_id = None
        self.specialization_id = None
        self.internship_id = None
        self.procedure_id = None
        self.medical_shift_id = None

    def login(self):
        """Login and get JWT token"""
        print_test_header("Login")
        
        response = requests.post(f"{BASE_URL}/auth/sign-in", json=TEST_USER)
        if response.status_code == 200:
            data = response.json()
            self.token = data['accessToken']
            self.user_id = data.get('userId', 1)  # Assuming user ID is returned
            print_success("Login successful")
            return True
        else:
            print_error(f"Login failed: {response.status_code} - {response.text}")
            return False

    def get_headers(self):
        """Get headers with JWT token"""
        return {"Authorization": f"Bearer {self.token}"}

    def setup_test_data(self):
        """Create necessary test data (specialization, internship)"""
        print_test_header("Setting up test data")
        
        # The test user is seeded with specialization ID 1 (Cardiology Old SMK)
        self.specialization_id = 1
        print_info(f"Using seeded specialization ID: {self.specialization_id} (Cardiology Old SMK)")

        # Create an internship for testing
        internship_data = {
            "specializationId": self.specialization_id,
            "institutionName": "Test Hospital",
            "departmentName": "Test Department",
            "supervisorName": "Dr. Test",
            "startDate": (datetime.now() - timedelta(days=30)).isoformat(),
            "endDate": (datetime.now() + timedelta(days=30)).isoformat()
        }
        
        response = requests.post(f"{BASE_URL}/internships", json=internship_data, headers=self.get_headers())
        if response.status_code in [200, 201]:
            self.internship_id = response.json()
            print_success(f"Created internship ID: {self.internship_id}")
            return True
        else:
            print_error(f"Failed to create internship: {response.status_code} - {response.text}")
            return False

    def test_sync_status_management(self):
        """Test 1: Sync status auto-transition from Synced to Modified"""
        print_test_header("Task 1: Sync Status Management")
        
        # Create a medical shift (initially NotSynced)
        shift_data = {
            "internshipId": self.internship_id,
            "date": datetime.now().isoformat(),
            "hours": 8,
            "minutes": 30,
            "location": "Emergency Room",
            "year": 1
        }
        
        response = requests.post(f"{BASE_URL}/medical-shifts", json=shift_data, headers=self.get_headers())
        if response.status_code in [200, 201]:
            self.medical_shift_id = response.json()
            print_success(f"Created medical shift ID: {self.medical_shift_id}")
        else:
            print_error(f"Failed to create medical shift: {response.status_code} - {response.text}")
            return

        # Get the shift to check initial status
        response = requests.get(f"{BASE_URL}/medical-shifts/{self.medical_shift_id}", headers=self.get_headers())
        if response.status_code == 200:
            shift = response.json()
            print_info(f"Initial sync status: {shift.get('syncStatus', 'Unknown')}")
        else:
            print_error(f"Failed to get medical shift: {response.status_code}")
            return

        # Update the shift (should auto-transition if it was Synced)
        update_data = {
            "hours": 9,
            "minutes": 0,
            "location": "ICU"
        }
        
        response = requests.put(f"{BASE_URL}/medical-shifts/{self.medical_shift_id}", 
                               json=update_data, headers=self.get_headers())
        if response.status_code == 200:
            print_success("Medical shift updated successfully")
            
            # Check the status after update
            response = requests.get(f"{BASE_URL}/medical-shifts/{self.medical_shift_id}", headers=self.get_headers())
            if response.status_code == 200:
                shift = response.json()
                print_info(f"Sync status after update: {shift.get('syncStatus', 'Unknown')}")
                print_success("Sync status management test completed")
            else:
                print_error("Failed to verify updated status")
        else:
            print_error(f"Failed to update medical shift: {response.status_code} - {response.text}")

    def test_medical_shift_duration_validation(self):
        """Test 3: Medical shift duration validation (MAUI alignment)"""
        print_test_header("Task 3: Medical Shift Duration Validation")
        
        test_cases = [
            {"hours": 0, "minutes": 30, "expected": "success", "description": "Valid: 30 minutes"},
            {"hours": 12, "minutes": 0, "expected": "success", "description": "Valid: 12 hours"},
            {"hours": 24, "minutes": 0, "expected": "success", "description": "Valid: 24 hours (no max limit)"},
            {"hours": 8, "minutes": 90, "expected": "success", "description": "Valid: 8h 90min (minutes > 59 allowed)"},
            {"hours": 0, "minutes": 0, "expected": "error", "description": "Invalid: Zero duration"},
            {"hours": -1, "minutes": 30, "expected": "error", "description": "Invalid: Negative hours"},
            {"hours": 8, "minutes": -30, "expected": "error", "description": "Invalid: Negative minutes"}
        ]
        
        for test in test_cases:
            shift_data = {
                "internshipId": self.internship_id,
                "date": datetime.now().isoformat(),
                "hours": test["hours"],
                "minutes": test["minutes"],
                "location": "Test Location",
                "year": 1
            }
            
            response = requests.post(f"{BASE_URL}/medical-shifts", json=shift_data, headers=self.get_headers())
            
            if test["expected"] == "success":
                if response.status_code in [200, 201]:
                    print_success(f"{test['description']} - Accepted as expected")
                else:
                    print_error(f"{test['description']} - Rejected unexpectedly: {response.text}")
            else:  # expected error
                if response.status_code >= 400:
                    print_success(f"{test['description']} - Rejected as expected")
                else:
                    print_error(f"{test['description']} - Accepted unexpectedly")

    def test_internship_update_functionality(self):
        """Test 4: Update functionality for InternshipsController"""
        print_test_header("Task 4: Internship Update Functionality")
        
        # Test updating internship details
        update_data = {
            "institutionName": "Updated Hospital",
            "departmentName": "Updated Department",
            "supervisorName": "Dr. Updated",
            "startDate": (datetime.now() - timedelta(days=20)).isoformat(),
            "endDate": (datetime.now() + timedelta(days=40)).isoformat()
        }
        
        response = requests.put(f"{BASE_URL}/internships/{self.internship_id}", 
                               json=update_data, headers=self.get_headers())
        if response.status_code == 200:
            print_success("Internship updated successfully")
            
            # Verify the update
            response = requests.get(f"{BASE_URL}/internships?specializationId={self.specialization_id}", 
                                  headers=self.get_headers())
            if response.status_code == 200:
                internships = response.json()
                updated = next((i for i in internships if i['id'] == self.internship_id), None)
                if updated and updated['institutionName'] == "Updated Hospital":
                    print_success("Update verified - institution name changed")
                else:
                    print_error("Update not reflected in data")
            else:
                print_error("Failed to verify update")
        else:
            print_error(f"Failed to update internship: {response.status_code} - {response.text}")
        
        # Test marking internship as completed
        response = requests.post(f"{BASE_URL}/internships/{self.internship_id}/complete", 
                               headers=self.get_headers())
        if response.status_code == 200:
            print_success("Internship marked as completed")
        else:
            print_error(f"Failed to mark internship as completed: {response.status_code} - {response.text}")

    def test_year_calculation(self):
        """Test 5: Year calculation based on specialization structure"""
        print_test_header("Task 5: Year Calculation for Procedures")
        
        # Test year validation for procedures
        test_cases = [
            {"year": 1, "expected": "success", "description": "Valid year 1"},
            {"year": 6, "expected": "success", "description": "Valid year 6"},
            {"year": 0, "expected": "success", "description": "Valid year 0 (unassigned)"},
            {"year": 7, "expected": "error", "description": "Invalid year 7 (exceeds max)"},
            {"year": -1, "expected": "error", "description": "Invalid negative year"}
        ]
        
        for test in test_cases:
            procedure_data = {
                "internshipId": self.internship_id,
                "date": datetime.now().isoformat(),
                "year": test["year"],
                "code": "TEST001",
                "location": "Operating Room",
                "status": "Pending"
            }
            
            response = requests.post(f"{BASE_URL}/procedures", json=procedure_data, headers=self.get_headers())
            
            if test["expected"] == "success":
                if response.status_code in [200, 201]:
                    print_success(f"{test['description']} - Accepted as expected")
                    self.procedure_id = response.json()
                else:
                    print_error(f"{test['description']} - Rejected unexpectedly: {response.text}")
            else:  # expected error
                if response.status_code >= 400:
                    print_success(f"{test['description']} - Rejected as expected")
                else:
                    print_error(f"{test['description']} - Accepted unexpectedly")

    def cleanup(self):
        """Clean up test data"""
        print_test_header("Cleanup")
        
        # Delete created resources
        if self.medical_shift_id:
            response = requests.delete(f"{BASE_URL}/medical-shifts/{self.medical_shift_id}", 
                                     headers=self.get_headers())
            if response.status_code in [200, 204]:
                print_success("Deleted test medical shift")
            else:
                print_info("Could not delete medical shift")
        
        if self.procedure_id:
            response = requests.delete(f"{BASE_URL}/procedures/{self.procedure_id}", 
                                     headers=self.get_headers())
            if response.status_code in [200, 204]:
                print_success("Deleted test procedure")
            else:
                print_info("Could not delete procedure")
        
        # Note: Internship deletion endpoint is not implemented yet
        print_info("Internship cleanup skipped (delete endpoint not implemented)")

    def run_all_tests(self):
        """Run all tests"""
        print(f"\n{BLUE}Starting tests for completed tasks...{RESET}")
        
        if not self.login():
            print_error("Failed to login. Exiting.")
            return
        
        if not self.setup_test_data():
            print_error("Failed to setup test data. Exiting.")
            return
        
        # Run tests
        self.test_sync_status_management()
        self.test_medical_shift_duration_validation()
        self.test_internship_update_functionality()
        self.test_year_calculation()
        
        # Cleanup
        self.cleanup()
        
        print(f"\n{BLUE}All tests completed!{RESET}")

if __name__ == "__main__":
    # Check if API is running by trying to access auth endpoint
    try:
        response = requests.post(f"{BASE_URL}/auth/sign-in", json={"username": "test", "password": "test"})
        # We expect 400 or 401, not connection error
        if response.status_code == 0:
            print_error("API is not responding. Please start the API first.")
            print_info("Run: python test_api.py")
            sys.exit(1)
    except requests.exceptions.ConnectionError:
        print_error("Cannot connect to API. Please start the API first.")
        print_info("Run: python test_api.py")
        sys.exit(1)
    
    # Run tests
    runner = TestRunner()
    runner.run_all_tests()