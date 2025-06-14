# Internships API Documentation

## Overview

The Internships API provides comprehensive functionality for managing medical internships within the specialization training system.

## Endpoints

### GET /api/internships
Retrieves a list of internships for a specialization.

**Query Parameters:**
- `specializationId` (required): The ID of the specialization
- `moduleId` (optional): Filter by specific module

**Response:** Array of `InternshipDto`

### GET /api/internships/{id}
Retrieves a specific internship by ID.
*Note: This endpoint needs implementation of GetInternshipById query and handler*

**Parameters:**
- `id`: The internship ID

**Response:** `InternshipDto`

### POST /api/internships
Creates a new internship.

**Request Body:** `CreateInternship`
```json
{
  "specializationId": 1,
  "moduleId": 1,
  "institutionName": "City Hospital",
  "departmentName": "Cardiology Department",
  "supervisorName": "Dr. Smith",
  "startDate": "2024-01-01",
  "endDate": "2024-02-01"
}
```

**Response:** ID of created internship

### PUT /api/internships/{id}
Updates an existing internship.

**Parameters:**
- `id`: The internship ID

**Request Body:** `UpdateInternshipRequest`
```json
{
  "institutionName": "New Hospital Name",
  "departmentName": "New Department",
  "supervisorName": "Dr. Johnson",
  "startDate": "2024-01-01",
  "endDate": "2024-02-15",
  "moduleId": 2
}
```

All fields are optional - only provided fields will be updated.

**Business Rules:**
- Cannot update approved internships
- Dates must maintain valid range (end > start)
- Automatically transitions sync status from Synced to Modified

### POST /api/internships/{id}/approve
Approves an internship.

**Parameters:**
- `id`: The internship ID

**Request Body:**
```json
{
  "approverName": "Dr. Supervisor"
}
```

### POST /api/internships/{id}/complete
Marks an internship as completed.

**Parameters:**
- `id`: The internship ID

**Business Rules:**
- Cannot mark as completed if end date is in the future
- Cannot modify approved internships

### DELETE /api/internships/{id}
Deletes an internship.
*Note: This endpoint needs implementation of DeleteInternship command and handler*

**Parameters:**
- `id`: The internship ID

**Business Rules:**
- Can only delete internships with SyncStatus of NotSynced or SyncFailed

## Entity Update Methods

The Internship entity supports the following update operations:

1. **AssignToModule(ModuleId)** - Assigns internship to a module
2. **SetSupervisor(string)** - Sets the supervisor name
3. **UpdateInstitution(string, string)** - Updates institution and department names
4. **UpdateDates(DateTime, DateTime)** - Updates start and end dates
5. **MarkAsCompleted()** - Marks the internship as completed
6. **Approve(string)** - Approves the internship

All update methods:
- Check if the entity can be modified (not approved)
- Automatically transition SyncStatus from Synced to Modified
- Update the UpdatedAt timestamp

## Implementation Status

✅ Implemented:
- GET /api/internships (list)
- POST /api/internships (create)
- PUT /api/internships/{id} (update)
- POST /api/internships/{id}/approve
- POST /api/internships/{id}/complete

❌ Needs Implementation:
- GET /api/internships/{id} - Requires GetInternshipById query and handler
- DELETE /api/internships/{id} - Requires DeleteInternship command and handler

## Security

All endpoints require authentication (JWT bearer token) and verify that the user has access to the internship through their specialization assignment.