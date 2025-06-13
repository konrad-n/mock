#!/usr/bin/env python3
"""
Automated API Testing Script for SledzSpecke Web API

This script tests all endpoints including:
- Authentication
- Procedures (Old and New SMK)
- Medical Shifts (Old and New SMK)
- Internships

Run with: python test_api.py
"""

import requests
import json
import sys
import subprocess
import time
import os
from datetime import datetime, timedelta
from typing import Optional, Dict, Any, List, Tuple
import argparse
import platform

# Configuration
API_BASE_URL = "http://localhost:5000/api"
TEST_USERNAME = "testuser"
TEST_PASSWORD = "Test123!"
API_PROJECT_PATH = os.path.dirname(os.path.abspath(__file__))

# Test Results Storage
test_results = {
    "total": 0,
    "passed": 0,
    "failed": 0,
    "skipped": 0,
    "details": []
}

# Color codes for output
class Colors:
    HEADER = '\033[95m'
    OKBLUE = '\033[94m'
    OKCYAN = '\033[96m'
    OKGREEN = '\033[92m'
    WARNING = '\033[93m'
    FAIL = '\033[91m'
    ENDC = '\033[0m'
    BOLD = '\033[1m'
    UNDERLINE = '\033[4m'

def print_header(text: str):
    """Print a formatted header"""
    print(f"\n{Colors.HEADER}{Colors.BOLD}{'='*60}{Colors.ENDC}")
    print(f"{Colors.HEADER}{Colors.BOLD}{text.center(60)}{Colors.ENDC}")
    print(f"{Colors.HEADER}{Colors.BOLD}{'='*60}{Colors.ENDC}\n")

def print_test_result(test_name: str, passed: bool, message: str = "", duration: float = 0):
    """Print a test result with color coding"""
    status = f"{Colors.OKGREEN}PASSED{Colors.ENDC}" if passed else f"{Colors.FAIL}FAILED{Colors.ENDC}"
    duration_str = f" ({duration:.2f}s)" if duration > 0 else ""
    print(f"  [{status}] {test_name}{duration_str}")
    if message:
        print(f"         {message}")

def print_summary():
    """Print test summary"""
    print_header("TEST SUMMARY")
    
    total = test_results["total"]
    passed = test_results["passed"]
    failed = test_results["failed"]
    skipped = test_results["skipped"]
    
    print(f"Total Tests: {total}")
    print(f"{Colors.OKGREEN}Passed: {passed}{Colors.ENDC}")
    if failed > 0:
        print(f"{Colors.FAIL}Failed: {failed}{Colors.ENDC}")
    if skipped > 0:
        print(f"{Colors.WARNING}Skipped: {skipped}{Colors.ENDC}")
    
    success_rate = (passed / total * 100) if total > 0 else 0
    print(f"\nSuccess Rate: {success_rate:.1f}%")
    
    if failed > 0:
        print(f"\n{Colors.FAIL}Failed Tests:{Colors.ENDC}")
        for result in test_results["details"]:
            if result["status"] == "failed":
                print(f"  - {result['test']}: {result['message']}")

def record_test(test_name: str, passed: bool, message: str = ""):
    """Record a test result"""
    test_results["total"] += 1
    if passed:
        test_results["passed"] += 1
        status = "passed"
    else:
        test_results["failed"] += 1
        status = "failed"
    
    test_results["details"].append({
        "test": test_name,
        "status": status,
        "message": message
    })

def check_api_health(url: str = API_BASE_URL) -> bool:
    """Check if the API is responding"""
    try:
        # Try to access the root endpoint or swagger
        response = requests.get(f"{url.replace('/api', '')}/swagger/index.html", timeout=5)
        return response.status_code in [200, 301, 302]
    except:
        return False

def start_api() -> subprocess.Popen:
    """Start the API if it's not running"""
    print("Starting API...")
    
    # Change to API directory
    api_dir = os.path.join(API_PROJECT_PATH, "src", "SledzSpecke.Api")
    
    # Determine the command based on the platform
    if platform.system() == "Windows":
        cmd = ["dotnet", "run"]
        shell = True
    else:
        cmd = ["dotnet", "run"]
        shell = False
    
    # Start the API process
    process = subprocess.Popen(
        cmd,
        cwd=api_dir,
        stdout=subprocess.PIPE,
        stderr=subprocess.PIPE,
        shell=shell
    )
    
    # Wait for API to start
    max_retries = 30
    for i in range(max_retries):
        time.sleep(2)
        if check_api_health():
            print(f"{Colors.OKGREEN}API started successfully!{Colors.ENDC}")
            return process
        print(f"Waiting for API to start... ({i+1}/{max_retries})")
    
    raise Exception("Failed to start API")

def create_test_user(token: Optional[str] = None) -> bool:
    """Create a test user if it doesn't exist"""
    try:
        # First try to sign up
        signup_data = {
            "username": TEST_USERNAME,
            "password": TEST_PASSWORD,
            "fullName": "Test User",
            "email": "test@example.com",
            "smkVersion": 1,  # OldSMK
            "specializationId": 1  # Cardiology Old SMK
        }
        
        response = requests.post(
            f"{API_BASE_URL}/auth/sign-up",
            json=signup_data
        )
        
        if response.status_code == 200:
            print(f"Test user created: {TEST_USERNAME}")
            return True
        elif response.status_code == 400 or response.status_code == 500:
            # User might already exist - check if we can sign in
            if "already in use" in response.text.lower():
                return True
            else:
                print(f"Failed to create test user: {response.status_code} - {response.text}")
                return False
        else:
            print(f"Failed to create test user: {response.status_code} - {response.text}")
            return False
    except Exception as e:
        print(f"Error creating test user: {e}")
        return False

def get_auth_token() -> Optional[str]:
    """Authenticate and get JWT token"""
    try:
        # Ensure test user exists
        create_test_user()
        
        # Sign in
        signin_data = {
            "username": TEST_USERNAME,
            "password": TEST_PASSWORD
        }
        
        print(f"Signing in with: {signin_data}")
        
        response = requests.post(
            f"{API_BASE_URL}/auth/sign-in",
            json=signin_data
        )
        
        if response.status_code == 200:
            token = response.json().get("accessToken")
            return token
        else:
            print(f"Authentication failed: {response.status_code} - {response.text}")
            return None
    except Exception as e:
        print(f"Authentication error: {e}")
        return None

def get_headers(token: str) -> Dict[str, str]:
    """Get headers with authorization"""
    return {
        "Authorization": f"Bearer {token}",
        "Content-Type": "application/json"
    }

def test_authentication():
    """Test authentication endpoints"""
    print_header("Testing Authentication")
    
    # Test sign-up
    start_time = time.time()
    try:
        signup_data = {
            "username": f"testuser_{int(time.time())}",
            "password": "Test123!",
            "fullName": "New Test User",
            "email": f"test_{int(time.time())}@example.com",
            "role": "user"
        }
        
        response = requests.post(
            f"{API_BASE_URL}/auth/sign-up",
            json=signup_data
        )
        
        passed = response.status_code == 200
        duration = time.time() - start_time
        print_test_result("Sign Up", passed, 
                         f"Status: {response.status_code}", duration)
        record_test("Authentication - Sign Up", passed, 
                   f"Status code: {response.status_code}")
    except Exception as e:
        print_test_result("Sign Up", False, str(e))
        record_test("Authentication - Sign Up", False, str(e))
    
    # Test sign-in
    start_time = time.time()
    try:
        token = get_auth_token()
        passed = token is not None
        duration = time.time() - start_time
        print_test_result("Sign In", passed, 
                         "Token received" if passed else "No token", duration)
        record_test("Authentication - Sign In", passed,
                   "Token received" if passed else "Authentication failed")
        return token
    except Exception as e:
        print_test_result("Sign In", False, str(e))
        record_test("Authentication - Sign In", False, str(e))
        return None

def test_internships(token: str):
    """Test internship endpoints"""
    print_header("Testing Internships")
    
    # Create an internship
    start_time = time.time()
    try:
        internship_data = {
            "specializationId": 1,
            "moduleId": 1,
            "institutionName": "Test Hospital",
            "departmentName": "Cardiology",
            "supervisorName": "Dr. Test",
            "startDate": datetime.now().isoformat(),
            "endDate": (datetime.now() + timedelta(days=30)).isoformat()
        }
        
        response = requests.post(
            f"{API_BASE_URL}/internships",
            json=internship_data,
            headers=get_headers(token)
        )
        
        passed = response.status_code in [200, 201]
        duration = time.time() - start_time
        internship_id = response.json() if passed else None
        
        print_test_result("Create Internship", passed,
                         f"Status: {response.status_code}, ID: {internship_id}", duration)
        record_test("Internships - Create", passed,
                   f"Status code: {response.status_code}")
        
        # Get internships
        start_time = time.time()
        response = requests.get(
            f"{API_BASE_URL}/internships?specializationId=1",
            headers=get_headers(token)
        )
        
        passed = response.status_code == 200
        duration = time.time() - start_time
        count = len(response.json()) if passed else 0
        
        print_test_result("Get Internships", passed,
                         f"Status: {response.status_code}, Count: {count}", duration)
        record_test("Internships - Get List", passed,
                   f"Retrieved {count} internships")
        
        return internship_id
    except Exception as e:
        print_test_result("Internship Tests", False, str(e))
        record_test("Internships - General", False, str(e))
        return None

def test_procedures(token: str, internship_id: int, smk_version: str = "new"):
    """Test procedure endpoints for both SMK versions"""
    print_header(f"Testing Procedures - {smk_version.upper()} SMK")
    
    # Create a procedure
    start_time = time.time()
    try:
        procedure_data = {
            "internshipId": internship_id,
            "date": datetime.now().isoformat(),
            "year": datetime.now().year,
            "code": "CARD-001" if smk_version == "new" else "P001",
            "location": "Test Hospital",
            "status": "completed",
            "operatorCode": "OP001",
            "performingPerson": "Dr. Test",
            "patientInitials": "JD",
            "patientGender": "M"
        }
        
        response = requests.post(
            f"{API_BASE_URL}/procedures",
            json=procedure_data,
            headers=get_headers(token)
        )
        
        passed = response.status_code in [200, 201]
        duration = time.time() - start_time
        procedure_id = response.json() if passed else None
        
        print_test_result(f"Create Procedure ({smk_version} SMK)", passed,
                         f"Status: {response.status_code}, ID: {procedure_id}", duration)
        record_test(f"Procedures {smk_version} SMK - Create", passed,
                   f"Status code: {response.status_code}")
        
        # Get procedures
        start_time = time.time()
        response = requests.get(
            f"{API_BASE_URL}/procedures?internshipId={internship_id}",
            headers=get_headers(token)
        )
        
        passed = response.status_code == 200
        duration = time.time() - start_time
        count = len(response.json()) if passed else 0
        
        print_test_result(f"Get Procedures ({smk_version} SMK)", passed,
                         f"Status: {response.status_code}, Count: {count}", duration)
        record_test(f"Procedures {smk_version} SMK - Get List", passed,
                   f"Retrieved {count} procedures")
        
        # Update procedure
        if procedure_id:
            start_time = time.time()
            update_data = {
                "status": "reviewed",
                "location": "Updated Hospital"
            }
            
            response = requests.put(
                f"{API_BASE_URL}/procedures/{procedure_id}",
                json=update_data,
                headers=get_headers(token)
            )
            
            passed = response.status_code in [200, 204]
            duration = time.time() - start_time
            
            print_test_result(f"Update Procedure ({smk_version} SMK)", passed,
                             f"Status: {response.status_code}", duration)
            record_test(f"Procedures {smk_version} SMK - Update", passed,
                       f"Status code: {response.status_code}")
            
            # Delete procedure
            start_time = time.time()
            response = requests.delete(
                f"{API_BASE_URL}/procedures/{procedure_id}",
                headers=get_headers(token)
            )
            
            passed = response.status_code in [200, 204]
            duration = time.time() - start_time
            
            print_test_result(f"Delete Procedure ({smk_version} SMK)", passed,
                             f"Status: {response.status_code}", duration)
            record_test(f"Procedures {smk_version} SMK - Delete", passed,
                       f"Status code: {response.status_code}")
    except Exception as e:
        print_test_result(f"Procedure Tests ({smk_version} SMK)", False, str(e))
        record_test(f"Procedures {smk_version} SMK - General", False, str(e))

def test_medical_shifts(token: str, internship_id: int, smk_version: str = "new"):
    """Test medical shift endpoints for both SMK versions"""
    print_header(f"Testing Medical Shifts - {smk_version.upper()} SMK")
    
    # Create a medical shift
    start_time = time.time()
    try:
        shift_data = {
            "internshipId": internship_id,
            "date": datetime.now().isoformat(),
            "hours": 8,
            "minutes": 30,
            "location": "Emergency Department",
            "year": datetime.now().year
        }
        
        response = requests.post(
            f"{API_BASE_URL}/medicalshifts",
            json=shift_data,
            headers=get_headers(token)
        )
        
        passed = response.status_code in [200, 201]
        duration = time.time() - start_time
        shift_id = response.json() if passed else None
        
        print_test_result(f"Create Medical Shift ({smk_version} SMK)", passed,
                         f"Status: {response.status_code}, ID: {shift_id}", duration)
        record_test(f"Medical Shifts {smk_version} SMK - Create", passed,
                   f"Status code: {response.status_code}")
        
        # Get medical shifts
        start_time = time.time()
        response = requests.get(
            f"{API_BASE_URL}/medicalshifts?internshipId={internship_id}",
            headers=get_headers(token)
        )
        
        passed = response.status_code == 200
        duration = time.time() - start_time
        count = len(response.json()) if passed else 0
        
        print_test_result(f"Get Medical Shifts ({smk_version} SMK)", passed,
                         f"Status: {response.status_code}, Count: {count}", duration)
        record_test(f"Medical Shifts {smk_version} SMK - Get List", passed,
                   f"Retrieved {count} shifts")
        
        # Update medical shift
        if shift_id:
            start_time = time.time()
            update_data = {
                "hours": 10,
                "minutes": 0,
                "location": "ICU"
            }
            
            response = requests.put(
                f"{API_BASE_URL}/medicalshifts/{shift_id}",
                json=update_data,
                headers=get_headers(token)
            )
            
            passed = response.status_code in [200, 204]
            duration = time.time() - start_time
            
            print_test_result(f"Update Medical Shift ({smk_version} SMK)", passed,
                             f"Status: {response.status_code}", duration)
            record_test(f"Medical Shifts {smk_version} SMK - Update", passed,
                       f"Status code: {response.status_code}")
            
            # Delete medical shift
            start_time = time.time()
            response = requests.delete(
                f"{API_BASE_URL}/medicalshifts/{shift_id}",
                headers=get_headers(token)
            )
            
            passed = response.status_code in [200, 204]
            duration = time.time() - start_time
            
            print_test_result(f"Delete Medical Shift ({smk_version} SMK)", passed,
                             f"Status: {response.status_code}", duration)
            record_test(f"Medical Shifts {smk_version} SMK - Delete", passed,
                       f"Status code: {response.status_code}")
    except Exception as e:
        print_test_result(f"Medical Shift Tests ({smk_version} SMK)", False, str(e))
        record_test(f"Medical Shifts {smk_version} SMK - General", False, str(e))

def main():
    """Main test execution"""
    global API_BASE_URL
    
    parser = argparse.ArgumentParser(description="Test SledzSpecke Web API")
    parser.add_argument("--url", default=API_BASE_URL, help="API base URL")
    parser.add_argument("--no-start", action="store_true", help="Don't start API if not running")
    args = parser.parse_args()
    
    API_BASE_URL = args.url
    
    print_header("SledzSpecke Web API Test Suite")
    print(f"API URL: {API_BASE_URL}")
    print(f"Test User: {TEST_USERNAME}")
    
    # Check if API is running
    api_process = None
    if not check_api_health():
        if args.no_start:
            print(f"{Colors.FAIL}API is not running! Use --no-start=false to auto-start.{Colors.ENDC}")
            sys.exit(1)
        else:
            try:
                api_process = start_api()
            except Exception as e:
                print(f"{Colors.FAIL}Failed to start API: {e}{Colors.ENDC}")
                sys.exit(1)
    else:
        print(f"{Colors.OKGREEN}API is already running!{Colors.ENDC}")
    
    try:
        # Run tests
        token = test_authentication()
        
        if token:
            # Create internship for testing
            internship_id = test_internships(token)
            
            if internship_id:
                # Test procedures for both SMK versions
                test_procedures(token, internship_id, "new")
                test_procedures(token, internship_id, "old")
                
                # Test medical shifts for both SMK versions
                test_medical_shifts(token, internship_id, "new")
                test_medical_shifts(token, internship_id, "old")
            else:
                print(f"{Colors.WARNING}Skipping procedure and medical shift tests (no internship){Colors.ENDC}")
                test_results["skipped"] += 8
        else:
            print(f"{Colors.WARNING}Skipping authenticated endpoint tests (no token){Colors.ENDC}")
            test_results["skipped"] += 10
        
        # Print summary
        print_summary()
        
        # Exit with appropriate code
        sys.exit(0 if test_results["failed"] == 0 else 1)
        
    finally:
        # Clean up API process if we started it
        if api_process:
            print("\nStopping API...")
            api_process.terminate()
            api_process.wait(timeout=10)

if __name__ == "__main__":
    main()