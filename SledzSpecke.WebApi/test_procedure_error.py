#!/usr/bin/env python3
import requests
import json
from datetime import datetime, timedelta

API_BASE_URL = "http://localhost:5000/api"

# Get auth token
auth_response = requests.post(
    f"{API_BASE_URL}/sign-in",
    json={"username": "testuser", "password": "Test123!"}
)
token = auth_response.json()["accessToken"]
headers = {"Authorization": f"Bearer {token}"}

# Create internship first
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
internship_id = internship_response.json()
print(f"Created internship ID: {internship_id}")

# Try to create procedure
procedure_data = {
    "internshipId": internship_id,
    "date": datetime.utcnow().strftime('%Y-%m-%dT%H:%M:%SZ'),
    "year": datetime.now().year,
    "code": "CARD-001",
    "location": "Test Hospital",
    "status": "completed",
    "operatorCode": "OP001",
    "performingPerson": "Dr. Test",
    "patientInitials": "JD",
    "patientGender": "M",
    "supervisor": "Dr. Supervisor"  # Required for new SMK
}

print("\nSending procedure data:")
print(json.dumps(procedure_data, indent=2))

response = requests.post(
    f"{API_BASE_URL}/procedures",
    json=procedure_data,
    headers=headers
)

print(f"\nStatus code: {response.status_code}")
print(f"Response: {response.text}")