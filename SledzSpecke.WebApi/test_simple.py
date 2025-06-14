#!/usr/bin/env python3
import requests
import json
from datetime import datetime, timedelta

API_BASE_URL = "http://localhost:5000/api"

# Test sign-in
signin_response = requests.post(
    f"{API_BASE_URL}/auth/sign-in",
    json={"username": "testuser", "password": "Test123!"}
)

print(f"Sign-in status: {signin_response.status_code}")
print(f"Response: {signin_response.json()}")
if signin_response.status_code == 200:
    token = signin_response.json()["AccessToken"]
    print(f"Token received: {token[:50]}...")
    
    headers = {"Authorization": f"Bearer {token}"}
    
    # Create internship
    internship_data = {
        "specializationId": 1,
        "moduleId": 101,
        "institutionName": "Test Hospital",
        "departmentName": "Cardiology",
        "supervisorName": "Dr. Test",
        "startDate": datetime.utcnow().strftime('%Y-%m-%dT%H:%M:%SZ'),
        "endDate": (datetime.utcnow() + timedelta(days=30)).strftime('%Y-%m-%dT%H:%M:%SZ')
    }
    
    internship_response = requests.post(
        f"{API_BASE_URL}/internships",
        json=internship_data,
        headers=headers
    )
    print(f"\nInternship creation status: {internship_response.status_code}")
    
    if internship_response.status_code in [200, 201]:
        internship_id = internship_response.json()
        print(f"Internship ID: {internship_id}")
        
        # Create procedure
        # Use a date within the internship period (5 days after start)
        procedure_date = datetime.utcnow() + timedelta(days=5)
        procedure_data = {
            "internshipId": internship_id,
            "date": procedure_date.strftime('%Y-%m-%dT%H:%M:%SZ'),
            "year": 1,
            "code": "P001",
            "location": "Test Hospital",
            "status": "Pending",
            "operatorCode": "OP001",
            "performingPerson": "Dr. Test",
            "patientInitials": "JD",
            "patientGender": "M"
        }
        
        procedure_response = requests.post(
            f"{API_BASE_URL}/procedures",
            json=procedure_data,
            headers=headers
        )
        print(f"\nProcedure creation status: {procedure_response.status_code}")
        if procedure_response.status_code not in [200, 201]:
            print(f"Error: {procedure_response.text}")
        else:
            print(f"Procedure ID: {procedure_response.json()}")
            
        # Create medical shift
        # Use a date within the internship period (7 days after start)
        shift_date = datetime.utcnow() + timedelta(days=7)
        shift_data = {
            "internshipId": internship_id,
            "date": shift_date.strftime('%Y-%m-%dT%H:%M:%SZ'),
            "hours": 8,
            "minutes": 30,
            "location": "Emergency Department",
            "year": 1
        }
        
        shift_response = requests.post(
            f"{API_BASE_URL}/medicalshifts",
            json=shift_data,
            headers=headers
        )
        print(f"\nMedical shift creation status: {shift_response.status_code}")
        if shift_response.status_code not in [200, 201]:
            print(f"Error: {shift_response.text}")
        else:
            print(f"Medical shift ID: {shift_response.json()}")
else:
    print(f"Sign-in failed: {signin_response.text}")