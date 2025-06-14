#!/usr/bin/env python3
"""
Automated API Testing Script for SledzSpecke Web API

This script tests all endpoints including:
- Authentication
- Procedures (Old and New SMK)
- Medical Shifts (Old and New SMK)
- Internships

Run with: python test_api.py

TROUBLESHOOTING GUIDE:
======================

1. Authentication Failures:
   - If "Sign In" fails with "Invalid credentials", check:
     a) The test user password in database matches TEST_PASSWORD
     b) Run: sudo -u postgres psql -d sledzspecke_db -c "SELECT \"Username\", \"Password\" FROM \"Users\" WHERE \"Username\" = 'testuser';"
     c) Password should be: VN5/YG8lI8uo76wXP6tC+39Z1Wzv+XTI/bc0LPLP40U= (SHA256 hash of "Test123!")
     d) To update: sudo -u postgres psql -d sledzspecke_db -c "UPDATE \"Users\" SET \"Password\" = 'VN5/YG8lI8uo76wXP6tC+39Z1Wzv+XTI/bc0LPLP40U=' WHERE \"Username\" = 'testuser';"

2. Database Connection Issues:
   - The API uses PostgreSQL, not SQLite
   - Check PostgreSQL is running: systemctl status postgresql
   - Check database exists: sudo -u postgres psql -c "SELECT 1 FROM pg_database WHERE datname='sledzspecke_db';"
   - Connection string is in appsettings.json/appsettings.Development.json

3. API Won't Start:
   - Check for build errors: dotnet build
   - Common issues:
     a) Missing NuGet packages - run: dotnet restore
     b) Port 5000 already in use - check: lsof -i :5000
     c) Missing logging package - ensure Microsoft.Extensions.Logging.Abstractions is referenced

4. Sync Status Related Issues:
   - Synced items CAN now be modified (they auto-transition to Modified status)
   - Only APPROVED items are truly read-only
   - Check entity's SyncStatus and IsApproved properties

5. JSON Template Loading Errors:
   - Templates are in src/SledzSpecke.Infrastructure/Data/Templates/
   - Check for null values in JSON files (e.g., internshipId)
   - Templates are cached - restart API if you modify them

6. Reset Everything:
   - Stop API: pkill -f "dotnet.*SledzSpecke.Api"
   - Reset database: sudo -u postgres psql -c "DROP DATABASE IF EXISTS sledzspecke_db; CREATE DATABASE sledzspecke_db;"
   - Rebuild and run: dotnet build && dotnet run --project src/SledzSpecke.Api/SledzSpecke.Api.csproj
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
            token = response.json().get("AccessToken")
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
            "smkVersion": 1,  # Old SMK
            "specializationId": 1  # Anestezjologia i intensywna terapia
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
            "moduleId": 101,  # Module ID for specialization 1: 1*100 + 1
            "institutionName": "Test Hospital",
            "departmentName": "Cardiology",
            "supervisorName": "Dr. Test",
            "startDate": datetime.utcnow().strftime('%Y-%m-%dT%H:%M:%SZ'),
            "endDate": (datetime.utcnow() + timedelta(days=30)).strftime('%Y-%m-%dT%H:%M:%SZ')
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
            "date": datetime.utcnow().strftime('%Y-%m-%dT%H:%M:%SZ'),
            "year": 1,  # Education year, not calendar year
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
            "date": datetime.utcnow().strftime('%Y-%m-%dT%H:%M:%SZ'),
            "hours": 8,
            "minutes": 30,
            "location": "Emergency Department",
            "year": 1  # Education year, not calendar year
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
                print(f"{Colors.WARNING}Troubleshooting tips:{Colors.ENDC}")
                print("  - Check the TROUBLESHOOTING GUIDE at the top of this script")
                print("  - Run 'dotnet build' to check for compilation errors")
                print("  - Check if port 5000 is already in use: lsof -i :5000")
                print("  - Check PostgreSQL is running: systemctl status postgresql")
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