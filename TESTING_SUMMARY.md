# SledzSpecke API Testing Summary

## Testing Date: June 19, 2025

### ✅ Successfully Tested

1. **API Health Check**
   - Endpoint: `/health`
   - Status: Working correctly
   - Returns: Health status with database connectivity

2. **User Login**
   - Endpoint: `/api/auth/sign-in`
   - Status: Working after fixes
   - Password format: SHA256 Base64 encoded
   - Working test credentials:
     - Email: `api.test@wum.edu.pl`
     - Password: `Test123`

3. **PWZ Validation Fix**
   - Fixed inconsistency between `SignUpValidator.cs` and `PwzNumber.cs`
   - Both now use the same algorithm: Last digit = (sum of first 6 digits * position) % 11

### ❌ Issues Found and Fixed

1. **PWZ Validation Mismatch**
   - **Problem**: Two different validation algorithms
   - **Solution**: Updated SignUpValidator to match PwzNumber.cs
   - **Status**: Fixed and deployed

2. **Password Format**
   - **Problem**: Test data expects BCrypt but system uses SHA256
   - **Solution**: Identified correct format and test password

3. **Registration Validation**
   - Multiple validation layers make registration difficult
   - PESEL validation is strict and requires valid checksums
   - PWZ validation was inconsistent (now fixed)

### ⚠️ Outstanding Issues

1. **JWT Token Validation**
   - **Error**: "The signature key was not found"
   - **Impact**: Cannot access authenticated endpoints
   - **Likely Cause**: JWT signing key not configured in production

2. **User-Specialization Relationship**
   - Table "UserSpecializations" doesn't exist
   - Need to create proper relationship between users and specializations

3. **Empty API Responses**
   - Many endpoints return empty responses
   - Could be due to authentication issues or missing data

### 📊 Test Results Summary

| Functionality | Status | Notes |
|--------------|--------|-------|
| Health Check | ✅ Pass | API is running |
| User Registration | ⚠️ Partial | Validation is very strict |
| User Login | ✅ Pass | Works with SHA256 passwords |
| Dashboard | ❌ Fail | JWT token issues |
| Medical Shifts CRUD | ❌ Fail | JWT token issues |
| Procedures CRUD | ❌ Fail | JWT token issues |

### 🔧 Required Fixes

1. **Immediate**
   - Configure JWT signing key in production
   - Create UserSpecializations table/relationship
   - Add proper error messages for validation failures

2. **Short-term**
   - Implement proper password hashing (BCrypt instead of SHA256)
   - Add integration tests to catch validation mismatches
   - Improve registration error messages

3. **Long-term**
   - Unify validation across all layers
   - Add comprehensive E2E tests
   - Implement proper API documentation

### 🗄️ Database Insights

1. **Password Storage**
   - Format: SHA256 hash encoded in Base64
   - Example: `2bX1jws4GYKTlxhloUB09Z66PoJZW+y+hq5R8dnx9l4=` = "Test123"

2. **Valid Test Data**
   - PESEL: `90010110019` (Male, born 1990-01-01)
   - PWZ: `1000001`, `1000002`, etc. (sequential with valid checksum)

3. **User Sequence**
   - Uses PostgreSQL sequence for user IDs
   - No auto-increment on Id column

### 📝 Testing Commands Used

```bash
# Health check
curl https://api.sledzspecke.pl/health

# Login
curl -X POST https://api.sledzspecke.pl/api/auth/sign-in \
  -H "Content-Type: application/json" \
  -d '{"Username": "api.test@wum.edu.pl", "Password": "Test123"}'

# Registration (with fixed validation)
curl -X POST https://api.sledzspecke.pl/api/auth/sign-up \
  -H "Content-Type: application/json" \
  -d '{
    "Email": "test@wum.edu.pl",
    "Password": "Test123456@",
    "FirstName": "Jan",
    "LastName": "Kowalski",
    "Pesel": "90010110019",
    "PwzNumber": "1000001",
    "PhoneNumber": "+48123456789",
    "DateOfBirth": "1990-01-01T00:00:00",
    "CorrespondenceAddress": {
      "Street": "Testowa",
      "HouseNumber": "1",
      "City": "Warszawa",
      "PostalCode": "00-001",
      "Province": "mazowieckie"
    }
  }'
```