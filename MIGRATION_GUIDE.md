# SledzSpecke Migration Guide

This guide addresses the issues discovered during testing on June 19, 2025.

## 1. JWT Configuration Fix (URGENT)

### Problem
JWT tokens fail with "The signature key was not found" error.

### Solution
Add JWT configuration to `appsettings.Production.json`:

```json
{
  "Auth": {
    "JwtKey": "your-very-secure-random-key-at-least-32-characters-long",
    "JwtIssuer": "sledzspecke-api",
    "JwtAudience": "sledzspecke-users",
    "JwtExpiry": "01:00:00"
  }
}
```

Generate a secure key:
```bash
openssl rand -base64 32
```

## 2. Password Hashing Migration

### Current State
- Production uses SHA256 with Base64 encoding
- Test data expects BCrypt format
- PasswordManager supports both formats

### Migration Steps

1. **Update PasswordManager** to use BCrypt for new passwords:
```csharp
public string Secure(string password)
{
    return BCrypt.Net.BCrypt.HashPassword(password, 10);
}
```

2. **Keep legacy support** in Verify method:
```csharp
public bool Verify(string password, string securedPassword)
{
    // Try BCrypt first
    if (securedPassword.StartsWith("$2"))
        return BCrypt.Net.BCrypt.Verify(password, securedPassword);
    
    // Fallback to SHA256
    return VerifyLegacySha256(password, securedPassword);
}
```

3. **Gradual migration** - rehash on login:
```csharp
// In SignInHandler after successful login with SHA256
if (!user.Password.Value.StartsWith("$2"))
{
    var newHash = _passwordManager.Secure(command.Password);
    user.UpdatePassword(new HashedPassword(newHash));
    await _unitOfWork.SaveChangesAsync();
}
```

## 3. Create User-Specialization Relationship

### Database Migration
```sql
CREATE TABLE "UserSpecializations" (
    "UserId" INTEGER NOT NULL,
    "SpecializationId" INTEGER NOT NULL,
    "StartDate" TIMESTAMP WITH TIME ZONE NOT NULL,
    "PlannedEndDate" TIMESTAMP WITH TIME ZONE NOT NULL,
    "CurrentModuleId" INTEGER,
    "IsActive" BOOLEAN NOT NULL DEFAULT true,
    "Status" VARCHAR(50) NOT NULL,
    "CompletionPercentage" INTEGER NOT NULL DEFAULT 0,
    CONSTRAINT "PK_UserSpecializations" PRIMARY KEY ("UserId", "SpecializationId"),
    CONSTRAINT "FK_UserSpecializations_Users" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id"),
    CONSTRAINT "FK_UserSpecializations_Specializations" FOREIGN KEY ("SpecializationId") REFERENCES "Specializations" ("Id"),
    CONSTRAINT "FK_UserSpecializations_Modules" FOREIGN KEY ("CurrentModuleId") REFERENCES "Modules" ("Id")
);
```

### Entity Framework Migration
```bash
dotnet ef migrations add AddUserSpecializations -p src/SledzSpecke.Infrastructure -s src/SledzSpecke.Api
dotnet ef database update -p src/SledzSpecke.Infrastructure -s src/SledzSpecke.Api
```

## 4. Fix Data Seeder

Update `DataSeeder.cs` to use consistent password format:

```csharp
var testUser = User.CreateWithId(
    new UserId(1),
    new Email("test@example.com"),
    new HashedPassword("2bX1jws4GYKTlxhloUB09Z66PoJZW+y+hq5R8dnx9l4="), // SHA256 of "Test123"
    // ... rest of user data
);
```

Or better, use PasswordManager:
```csharp
var passwordHash = _passwordManager.Secure("Test123");
var testUser = User.CreateWithId(
    // ...
    new HashedPassword(passwordHash),
    // ...
);
```

## 5. Validation Improvements

### Make Error Messages More Helpful
Update validation messages in SignUpValidator:

```csharp
RuleFor(x => x.Pesel)
    .NotEmpty().WithMessage("PESEL is required")
    .Length(11).WithMessage("PESEL must be exactly 11 digits")
    .Matches(@"^\d{11}$").WithMessage("PESEL must contain only digits")
    .Must(BeValidPesel).WithMessage("Invalid PESEL checksum. Example valid PESEL: 90010110019")
    .WithErrorCode("INVALID_PESEL");

RuleFor(x => x.PwzNumber)
    .NotEmpty().WithMessage("PWZ number is required")
    .Length(7).WithMessage("PWZ number must be exactly 7 digits")
    .Matches(@"^\d{7}$").WithMessage("PWZ number must contain only digits")
    .Must(BeValidPwz).WithMessage("Invalid PWZ checksum. Valid examples: 1000001, 2000023, 3000003")
    .WithErrorCode("INVALID_PWZ");
```

## 6. Testing Improvements

### Add Integration Tests for Validation
Create tests that verify validation consistency:

```csharp
[Fact]
public async Task SignUp_WithValidPwz_ShouldSucceed()
{
    // Arrange
    var validPwzNumbers = new[] { "1000001", "2000023", "3000003" };
    
    foreach (var pwz in validPwzNumbers)
    {
        var command = new SignUp { PwzNumber = pwz, /* other fields */ };
        
        // Act
        var result = await _handler.HandleAsync(command);
        
        // Assert
        result.IsSuccess.Should().BeTrue($"PWZ {pwz} should be valid");
    }
}
```

## 7. Quick Fixes Script

Create a script to apply all fixes at once:

```bash
#!/bin/bash
# fix-sledzspecke.sh

echo "Applying SledzSpecke fixes..."

# 1. Stop the service
sudo systemctl stop sledzspecke-api

# 2. Backup database
sudo -u postgres pg_dump sledzspecke_db > backup_$(date +%Y%m%d_%H%M%S).sql

# 3. Apply database migration
dotnet ef database update -p src/SledzSpecke.Infrastructure -s src/SledzSpecke.Api

# 4. Update configuration
# (Manual step - add JWT key to appsettings.Production.json)

# 5. Deploy new code
sudo dotnet publish src/SledzSpecke.Api -c Release -o /var/www/sledzspecke-api

# 6. Start the service
sudo systemctl start sledzspecke-api

echo "Fixes applied. Please manually update appsettings.Production.json with JWT key."
```

## Priority Order

1. **URGENT**: Fix JWT configuration (blocks all authenticated endpoints)
2. **HIGH**: Create UserSpecializations table (blocks user functionality)
3. **MEDIUM**: Update validation error messages (improves UX)
4. **LOW**: Migrate to BCrypt (security improvement)
5. **LOW**: Fix data seeder (development convenience)