# AI Developer Persona

You are a world-class senior software developer with deep expertise in:
- **.NET 9 / C# 13**: Advanced patterns, performance optimization, and clean architecture
- **Entity Framework Core**: Complex queries, migrations, and database design
- **Clean Architecture & DDD**: Aggregate roots, value objects, domain events
- **CQRS Pattern**: Command/query separation, handlers, and mediator pattern
- **React Native & Expo**: Cross-platform mobile development with TypeScript
- **PostgreSQL**: Advanced queries, indexing, and performance tuning
- **RESTful APIs**: Design, security, versioning, and documentation
- **Testing**: Unit, integration, and E2E testing strategies
- **DevOps**: CI/CD, containerization, monitoring, and deployment

You write production-ready code that is:
- **Clean**: Following SOLID principles and design patterns
- **Performant**: Optimized for speed and resource usage
- **Secure**: Following OWASP guidelines and security best practices
- **Maintainable**: Well-documented with clear intent
- **Testable**: With high code coverage and meaningful tests

You understand the medical domain context of this specialization tracking system and can make informed decisions about business logic implementation.

---

# SledzSpecke - Kompletna Dokumentacja Systemu

## Spis Treści
1. [Wprowadzenie](#wprowadzenie)
2. [Architektura Systemu](#architektura-systemu)
3. [Model Biznesowy](#model-biznesowy)
4. [API Endpoints](#api-endpoints)
5. [Autentykacja i Autoryzacja](#autentykacja-i-autoryzacja)
6. [Encje i Value Objects](#encje-i-value-objects)
7. [Wzorce i Praktyki](#wzorce-i-praktyki)
8. [Specyfikacja Eksportu Danych](#specyfikacja-eksportu-danych)
9. [Integracja z SMK](#integracja-z-smk)
10. [Frontend - Technologie i Architektura](#frontend-technologie-i-architektura)
11. [Deployment i Konfiguracja](#deployment-i-konfiguracja)
12. [Testing](#testing)
13. [Development Tips and Gotchas](#development-tips-and-gotchas)
14. [Development Optimization](#development-optimization)
15. [API Implementation Details](#api-implementation-details)
16. [Business Rules and Validation](#business-rules-and-validation)
17. [Sync Status Management](#sync-status-management)

---

## 1. Wprowadzenie

### Cel Aplikacji
SledzSpecke to aplikacja mobilna i webowa dla lekarzy do śledzenia postępu specjalizacji medycznej. Umożliwia:
- Bieżącą rejestrację wszystkich elementów wymaganych w programie specjalizacji
- Dokładne odwzorowanie struktury systemu SMK
- Automatyczne obliczanie postępów i terminów
- Eksport danych w formacie zgodnym z SMK

### Kluczowe Założenia
- Aplikacja tylko dla lekarzy (bez dostępu kierowników)
- Synchronizacja jednostronna: SledzSpecke → SMK
- Wsparcie dla starego i nowego SMK
- Jedna specjalizacja na użytkownika

---

## 2. Architektura Systemu

### Clean Architecture
```
┌─────────────────────────────────────────────────┐
│                  Prezentacja                    │
│            (API Controllers / UI)               │
├─────────────────────────────────────────────────┤
│                  Aplikacja                      │
│        (Commands, Queries, Handlers)            │
├─────────────────────────────────────────────────┤
│                    Domena                       │
│      (Entities, Value Objects, Rules)           │
├─────────────────────────────────────────────────┤
│               Infrastruktura                    │
│    (Database, External Services, Auth)          │
└─────────────────────────────────────────────────┘
```

### Struktura Projektu
```
SledzSpecke.WebApi/
├── src/
│   ├── SledzSpecke.Api/          # Kontrolery, middleware
│   ├── SledzSpecke.Application/  # CQRS, DTOs, serwisy
│   ├── SledzSpecke.Core/         # Encje, Value Objects
│   └── SledzSpecke.Infrastructure/ # EF Core, JWT, repos
└── tests/                        # Testy jednostkowe i integracyjne
```

### Wzorce Architektoniczne
- **CQRS** - separacja komend i zapytań
- **Repository Pattern** - abstrakcja dostępu do danych
- **Value Objects** - silne typowanie prymitywów
- **Result Pattern** - jawna obsługa błędów
- **Unit of Work** - transakcyjność operacji

---

## 3. Model Biznesowy

### Przepływ Specjalizacji
1. **Rejestracja** → wybór specjalizacji i wersji SMK
2. **Moduły** → podstawowy (2 lata) + specjalistyczny (3-4 lata)
3. **Realizacja** → staże, kursy, procedury, dyżury
4. **Dokumentacja** → samokształcenie, publikacje
5. **Eksport** → dane w formacie SMK

### Struktura Modułowa
```
Specjalizacja (np. Kardiologia)
├── Moduł Podstawowy (Choroby Wewnętrzne)
│   ├── Staże kierunkowe
│   ├── Kursy obowiązkowe
│   └── Procedury wymagane
└── Moduł Specjalistyczny
    ├── Staże specjalistyczne
    ├── Kursy zaawansowane
    └── Procedury specjalistyczne
```

### Kalkulacja Postępu
- **Staże**: 35% wagi całkowitej
- **Kursy**: 25% wagi całkowitej
- **Procedury**: 30% wagi całkowitej
- **Dyżury**: 10% wagi całkowitej

---

## 4. API Endpoints

### Autentykacja
```http
POST /api/auth/sign-up
POST /api/auth/sign-in
```

### Profil Użytkownika
```http
GET    /api/users/profile
PUT    /api/users/profile
PUT    /api/users/change-password
PUT    /api/users/preferences
```

### Dashboard i Postępy
```http
GET /api/dashboard/overview
GET /api/dashboard/progress/{specializationId}
GET /api/dashboard/statistics/{specializationId}
```

### Moduły
```http
GET /api/modules/specialization/{specializationId}
PUT /api/modules/switch
```

### Staże (Internships)
```http
GET    /api/internships
GET    /api/internships/{id}
POST   /api/internships
PUT    /api/internships/{id}
DELETE /api/internships/{id}
```

### Dyżury Medyczne (Medical Shifts)
```http
GET    /api/medical-shifts
GET    /api/medical-shifts/{id}
POST   /api/medical-shifts
PUT    /api/medical-shifts/{id}
DELETE /api/medical-shifts/{id}
GET    /api/medical-shifts/statistics
```

### Procedury (Procedures)
```http
GET    /api/procedures
GET    /api/procedures/{id}
POST   /api/procedures
PUT    /api/procedures/{id}
DELETE /api/procedures/{id}
GET    /api/procedures/statistics
```

### Kursy (Courses)
```http
GET    /api/courses
GET    /api/courses/{id}
POST   /api/courses
PUT    /api/courses/{id}
DELETE /api/courses/{id}
POST   /api/courses/{id}/complete
```

### Samokształcenie (Self Education)
```http
GET    /api/selfeducation
POST   /api/selfeducation
PUT    /api/selfeducation/{id}
DELETE /api/selfeducation/{id}
PUT    /api/selfeducation/{id}/complete
GET    /api/selfeducation/by-year
GET    /api/selfeducation/completed
GET    /api/selfeducation/credit-hours
GET    /api/selfeducation/quality-score
```

### Publikacje (Publications)
```http
GET    /api/publications
POST   /api/publications
PUT    /api/publications/{id}
DELETE /api/publications/{id}
GET    /api/publications/peer-reviewed
GET    /api/publications/first-author
GET    /api/publications/impact-score
```

### Nieobecności (Absences)
```http
GET    /api/absences
POST   /api/absences
PUT    /api/absences/{id}
DELETE /api/absences/{id}
PUT    /api/absences/{id}/approve
```

### Uznania (Recognitions)
```http
GET    /api/recognitions
POST   /api/recognitions
PUT    /api/recognitions/{id}
DELETE /api/recognitions/{id}
PUT    /api/recognitions/{id}/approve
GET    /api/recognitions/total-reduction
```

### Działalność Edukacyjna (Educational Activities)
```http
GET    /api/educationalactivities/specialization/{id}
GET    /api/educationalactivities/{id}
GET    /api/educationalactivities/specialization/{id}/type/{type}
POST   /api/educationalactivities
PUT    /api/educationalactivities/{id}
DELETE /api/educationalactivities/{id}
```

### Zarządzanie Plikami
```http
POST   /api/files/upload
GET    /api/files/{fileId}/download
GET    /api/files/entity/{entityType}/{entityId}
DELETE /api/files/{fileId}
```

### Kalkulacje
```http
GET  /api/calculations/internship-days
POST /api/calculations/normalize-time
GET  /api/calculations/required-shift-hours/{smkVersion}
```

---

## 5. Autentykacja i Autoryzacja

### JWT Token
- Typ: Bearer Token
- Czas życia: 7 dni (konfigurowalny)
- Refresh token: nie zaimplementowany (do rozważenia)

### Przykład użycia
```typescript
// Request
POST /api/auth/sign-in
{
  "email": "dr.kowalski@example.com",
  "password": "SecurePassword123!"
}

// Response
{
  "accessToken": "eyJhbGciOiJIUzI1NiIs...",
  "userId": 1001,
  "email": "dr.kowalski@example.com",
  "fullName": "Dr. Jan Kowalski"
}

// Użycie tokenu
GET /api/dashboard/overview
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
```

---

## 6. Encje i Value Objects

### Główne Encje

#### User
```csharp
public class User : AggregateRoot<UserId>
{
    public Email Email { get; private set; }
    public Username Username { get; private set; }
    public Password Password { get; private set; }
    public PersonName FullName { get; private set; }
    public SmkVersion SmkVersion { get; private set; }
    public SpecializationId SpecializationId { get; private set; }
    public PhoneNumber? PhoneNumber { get; private set; }
    public DateTime? DateOfBirth { get; private set; }
    public string? Bio { get; private set; }
}
```

#### Specialization
```csharp
public class Specialization
{
    public SpecializationId Id { get; private set; }
    public string Name { get; private set; }
    public string ProgramCode { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime PlannedEndDate { get; private set; }
    public int DurationYears { get; private set; }
    public SmkVersion SmkVersion { get; private set; }
    public ModuleId CurrentModuleId { get; private set; }
    public ICollection<Module> Modules { get; private set; }
}
```

#### MedicalShift
```csharp
public class MedicalShift
{
    public MedicalShiftId Id { get; private set; }
    public InternshipId InternshipId { get; private set; }
    public DateTime Date { get; private set; }
    public Duration Duration { get; private set; }
    public Location Location { get; private set; }
    public int Year { get; private set; }
    public SyncStatus SyncStatus { get; private set; }
}
```

### Value Objects

#### Email
- Format: RFC 5322
- Maksymalna długość: 100 znaków
- Walidacja: regex + obecność @

#### Duration
- Przechowuje: Hours (int) i Minutes (int)
- Walidacja: Hours >= 0, 0 <= Minutes <= 59
- Metody: ToTotalMinutes(), ToString()

#### SmkVersion
- Wartości: "old" lub "new"
- Immutable po utworzeniu

#### Location
- Maksymalna długość: 500 znaków
- Nie może być pusta

#### SyncStatus
- Wartości: NotSynced, Synced, Modified, Approved
- Automatyczne przejścia przy modyfikacji

---

## 7. Wzorce i Praktyki

### CQRS Pattern

#### Command
```csharp
public record AddMedicalShift(
    int InternshipId,
    DateTime Date,
    int Hours,
    int Minutes,
    string Location,
    int Year) : ICommand<int>;
```

#### Command Handler
```csharp
public class AddMedicalShiftHandler : ICommandHandler<AddMedicalShift, int>
{
    public async Task<int> HandleAsync(AddMedicalShift command)
    {
        // Walidacja
        // Logika biznesowa
        // Zapis do bazy
        // Zwrot ID
    }
}
```

#### Query
```csharp
public record GetUserMedicalShifts(
    int UserId,
    int? InternshipId,
    DateTime? StartDate,
    DateTime? EndDate) : IQuery<IEnumerable<MedicalShiftDto>>;
```

### Result Pattern
```csharp
public class Result<T>
{
    public bool IsSuccess { get; }
    public T Value { get; }
    public string Error { get; }
    
    public static Result<T> Success(T value) => new(true, value, null);
    public static Result<T> Failure(string error) => new(false, default, error);
}
```

### Repository Pattern
```csharp
public interface IMedicalShiftRepository
{
    Task<MedicalShift?> GetByIdAsync(MedicalShiftId id);
    Task<IEnumerable<MedicalShift>> GetByInternshipIdAsync(InternshipId id);
    Task AddAsync(MedicalShift shift);
    Task UpdateAsync(MedicalShift shift);
    Task DeleteAsync(MedicalShift shift);
}
```

---

## 8. Specyfikacja Eksportu Danych

### Format Eksportu dla SMK

#### Dyżury Medyczne
```json
{
  "liczbaGodzin": 12,
  "liczbaMinut": 30,
  "dataRealizacji": "15-03-2024",
  "miejsce": "Oddział Kardiologii",
  "nazwaStazu": "Staż podstawowy w zakresie chorób wewnętrznych",
  "rokSzkolenia": 1,
  "statusAkceptacji": "Wprowadzone"
}
```

#### Procedury/Zabiegi
```json
{
  "kodProcedury": "89.52",
  "nazwaProcedury": "Koronarografia",
  "dataWykonania": "20-03-2024",
  "miejsceWykonania": "Pracownia Hemodynamiki",
  "kodOperatora": "A",
  "inicjalyPacjenta": "JK",
  "plecPacjenta": "M",
  "nazwaStazu": "Staż kierunkowy w zakresie kardiologii interwencyjnej",
  "rokSzkolenia": 3,
  "statusAkceptacji": "Wprowadzone"
}
```

#### Staże
```json
{
  "nazwaStazu": "Staż kierunkowy w zakresie kardiologii dziecięcej",
  "miejsceOdbywania": "Szpital Uniwersytecki w Krakowie",
  "oddzial": "Oddział Kardiologii Dziecięcej",
  "kierownikStazu": "prof. dr hab. Jan Kowalski",
  "dataRozpoczecia": "01-09-2024",
  "dataZakonczenia": "30-11-2024",
  "liczbaDni": 91,
  "czyZrealizowany": false,
  "statusAkceptacji": "Wprowadzone"
}
```

#### Kursy Specjalizacyjne
```json
{
  "nazwaKursu": "Podstawy intensywnej terapii kardiologicznej",
  "numerKursu": "KS/001/2024",
  "organizator": "CMKP",
  "dataRozpoczecia": "15-04-2024",
  "dataZakonczenia": "19-04-2024",
  "czyUkonczony": true,
  "numerZaswiadczenia": "CMKP/2024/1234",
  "statusAkceptacji": "Wprowadzone"
}
```

### Generowanie Excel dla SMK
```csharp
public class SmkExcelExporter
{
    public byte[] ExportToExcel(ExportData data, SmkVersion version)
    {
        using var package = new ExcelPackage();
        
        // Arkusz: Dyżury
        var shiftSheet = package.Workbook.Worksheets.Add("Dyżury medyczne");
        shiftSheet.Cells["A1"].Value = "Data realizacji";
        shiftSheet.Cells["B1"].Value = "Liczba godzin";
        shiftSheet.Cells["C1"].Value = "Liczba minut";
        shiftSheet.Cells["D1"].Value = "Miejsce";
        // ... wypełnienie danymi
        
        // Arkusz: Procedury
        var procedureSheet = package.Workbook.Worksheets.Add("Procedury");
        // ... struktura zgodna z SMK
        
        return package.GetAsByteArray();
    }
}
```

---

## 9. Integracja z SMK

### Mapowanie Wersji SMK

#### Stary SMK
- Struktura lat: 1-6
- Brak modułów
- Inne kody procedur
- Wymaga ręcznego wprowadzania przez użytkownika

#### Nowy SMK
- Struktura modułowa
- Automatyczne przejścia między modułami
- Rozszerzone kody procedur
- Możliwość importu przez wtyczkę Chrome (przyszłość)

### Status Synchronizacji
```
NotSynced → (eksport) → Synced → (modyfikacja) → Modified
                            ↓
                        (zatwierdzenie w SMK)
                            ↓
                        Approved
```

---

## 10. Frontend - Technologie i Architektura

### React Native + Expo

#### Struktura Projektu
```
/SledzSpecke-Mobile
├── /src
│   ├── /components
│   │   ├── /forms
│   │   │   ├── ProcedureForm.tsx
│   │   │   ├── ShiftForm.tsx
│   │   │   └── InternshipForm.tsx
│   │   ├── /ui
│   │   │   ├── Button.tsx
│   │   │   ├── Card.tsx
│   │   │   └── DataTable.tsx
│   │   └── /charts
│   │       └── ProgressChart.tsx
│   ├── /screens
│   │   ├── /auth
│   │   │   ├── LoginScreen.tsx
│   │   │   └── RegisterScreen.tsx
│   │   ├── /dashboard
│   │   │   └── DashboardScreen.tsx
│   │   └── /eks
│   │       ├── ProcedureListScreen.tsx
│   │       └── ShiftListScreen.tsx
│   ├── /services
│   │   ├── api.ts
│   │   ├── auth.ts
│   │   └── export.ts
│   ├── /utils
│   │   ├── validators.ts
│   │   └── formatters.ts
│   └── /navigation
│       └── AppNavigator.tsx
├── /web
│   └── webpack.config.js
├── app.json
└── package.json
```

#### Kluczowe Komponenty

##### Uniwersalny Formularz
```typescript
interface FormFieldProps {
  label: string;
  value: string;
  onChangeText: (text: string) => void;
  error?: string;
  keyboardType?: 'default' | 'numeric' | 'email-address';
}

export const FormField: React.FC<FormFieldProps> = ({
  label,
  value,
  onChangeText,
  error,
  keyboardType = 'default'
}) => {
  return (
    <View style={styles.container}>
      <Text style={styles.label}>{label}</Text>
      <TextInput
        style={[styles.input, error && styles.inputError]}
        value={value}
        onChangeText={onChangeText}
        keyboardType={keyboardType}
      />
      {error && <Text style={styles.error}>{error}</Text>}
    </View>
  );
};
```

##### API Client
```typescript
class ApiClient {
  private baseURL: string;
  private token: string | null = null;

  constructor(baseURL: string) {
    this.baseURL = baseURL;
  }

  setAuthToken(token: string) {
    this.token = token;
  }

  async request<T>(
    endpoint: string,
    options: RequestInit = {}
  ): Promise<T> {
    const url = `${this.baseURL}${endpoint}`;
    const config: RequestInit = {
      ...options,
      headers: {
        'Content-Type': 'application/json',
        ...(this.token && { Authorization: `Bearer ${this.token}` }),
        ...options.headers,
      },
    };

    const response = await fetch(url, config);
    
    if (!response.ok) {
      throw new ApiError(response.status, await response.text());
    }

    return response.json();
  }

  // Metody dla każdego endpointa
  auth = {
    signIn: (data: SignInDto) => 
      this.request<AuthResponse>('/auth/sign-in', {
        method: 'POST',
        body: JSON.stringify(data),
      }),
    signUp: (data: SignUpDto) =>
      this.request<AuthResponse>('/auth/sign-up', {
        method: 'POST',
        body: JSON.stringify(data),
      }),
  };

  procedures = {
    getAll: () => this.request<Procedure[]>('/procedures'),
    create: (data: CreateProcedureDto) =>
      this.request<Procedure>('/procedures', {
        method: 'POST',
        body: JSON.stringify(data),
      }),
    // ... pozostałe metody
  };
}
```

### Biblioteki i Narzędzia

#### Podstawowe
- **expo**: ~49.0.0
- **react-native**: 0.73.0
- **react-navigation**: ^6.0.0
- **react-native-web**: ~0.19.0

#### UI i Formularze
- **react-native-paper**: ^5.0.0 (Material Design)
- **react-hook-form**: ^7.0.0
- **yup**: ^1.0.0 (walidacja)

#### Stan i Dane
- **@tanstack/react-query**: ^5.0.0
- **zustand**: ^4.0.0 (prosty state management)
- **async-storage**: ~1.19.0

#### Eksport i Pliki
- **react-native-pdf**: ^6.7.0
- **xlsx**: ^0.18.0
- **expo-file-system**: ~15.4.0
- **expo-sharing**: ~11.5.0

---

## 11. Deployment i Konfiguracja

### Backend (.NET API)

#### Konfiguracja produkcyjna
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=SledzSpecke;User Id=sa;Password=...;TrustServerCertificate=True"
  },
  "JwtSettings": {
    "Secret": "your-256-bit-secret-key-here",
    "Issuer": "SledzSpecke",
    "Audience": "SledzSpeckeUsers",
    "ExpiryInDays": 7
  },
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    }
  }
}
```

#### Deployment na VPS
```bash
# Build
dotnet publish -c Release -o ./publish

# Systemd service
sudo nano /etc/systemd/system/sledzspecke-api.service
```

```ini
[Unit]
Description=SledzSpecke Web API
After=network.target

[Service]
Type=simple
WorkingDirectory=/var/www/sledzspecke-api
ExecStart=/usr/bin/dotnet /var/www/sledzspecke-api/SledzSpecke.Api.dll
Restart=always
RestartSec=10
User=www-data
Environment="ASPNETCORE_ENVIRONMENT=Production"
Environment="ASPNETCORE_URLS=http://localhost:5000"

[Install]
WantedBy=multi-user.target
```

### Frontend (React Native Web)

#### Build dla Web
```bash
# Expo build
expo build:web

# Output w /web-build
```

#### Nginx Configuration
```nginx
server {
    listen 80;
    server_name sledzspecke.pl;
    root /var/www/sledzspecke-web/web-build;
    index index.html;

    location / {
        try_files $uri $uri/ /index.html;
    }

    location /api {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection 'upgrade';
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
    }
}
```

### Mobile Apps

#### Android (APK)
```bash
# EAS Build
eas build --platform android --profile production

# Lub lokalnie
expo build:android -t apk
```

#### iOS (IPA)
```bash
# Wymaga konta Apple Developer
eas build --platform ios --profile production
```

### Monitoring i Logi

#### Serilog Configuration
```csharp
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(
        path: "/var/log/sledzspecke/api-.log",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30)
    .CreateLogger();
```

#### Health Checks
```csharp
services.AddHealthChecks()
    .AddDbContextCheck<SledzSpeckeDbContext>()
    .AddUrlGroup(new Uri("https://smk2.ezdrowie.gov.pl"), "SMK API");

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
```

---

## 12. Testing

### SledzSpecke Web API Automated Testing

#### Features
- **Automatic API startup**: Checks if the API is running and starts it if needed
- **Comprehensive endpoint testing**: Tests all major endpoints including:
  - Authentication (sign-up, sign-in)
  - Internships (CRUD operations)
  - Procedures (CRUD operations for both Old and New SMK)
  - Medical Shifts (CRUD operations for both Old and New SMK)
- **Clear reporting**: Shows pass/fail status for each test with timing
- **Summary report**: Displays overall test statistics at the end
- **Cross-platform**: Works on Linux, macOS, and Windows

#### Prerequisites
- Python 3.x installed
- .NET SDK (for running the API)
- `requests` Python package (will be installed automatically if missing)

#### Usage

##### Linux/macOS
```bash
# Make the script executable (first time only)
chmod +x test_api.sh

# Run all tests
./test_api.sh

# Run tests with custom API URL
./test_api.sh --url http://localhost:5001/api

# Run tests without auto-starting the API
./test_api.sh --no-start
```

##### Windows
```powershell
# Run all tests
.\test_api.ps1

# Run tests with custom API URL
.\test_api.ps1 --url http://localhost:5001/api

# Run tests without auto-starting the API
.\test_api.ps1 --no-start
```

##### Direct Python execution
```bash
# Run all tests
python test_api.py

# Run tests with custom API URL
python test_api.py --url http://localhost:5001/api

# Run tests without auto-starting the API
python test_api.py --no-start
```

#### Test Coverage

1. **Authentication**
   - User registration (sign-up)
   - User login (sign-in)
   - JWT token retrieval

2. **Internships**
   - Create new internship
   - Retrieve internship list
   - Update internship (if implemented)
   - Delete internship (if implemented)

3. **Procedures (Both Old and New SMK)**
   - Create procedure with all required fields
   - Retrieve procedures by internship
   - Update procedure details
   - Delete procedure

4. **Medical Shifts (Both Old and New SMK)**
   - Create medical shift with duration
   - Retrieve shifts by internship
   - Update shift details
   - Delete shift

#### Test Output
- Real-time test execution status
- Duration for each test
- Color-coded pass/fail indicators
- Final summary with:
  - Total tests run
  - Number passed/failed/skipped
  - Success rate percentage
  - List of failed tests (if any)

#### Exit Codes
- `0`: All tests passed
- `1`: One or more tests failed

#### Customization
```python
API_BASE_URL = "http://localhost:5000/api"  # API endpoint
TEST_USERNAME = "testuser"                    # Test user credentials
TEST_PASSWORD = "Test123!"
```

#### Troubleshooting
1. **API won't start**: Ensure you have .NET SDK installed and the database is configured
2. **Authentication fails**: Check that the test user can be created (database is writable)
3. **Tests timeout**: Increase timeout values in the script or check API performance
4. **Python not found**: Install Python 3.x from python.org

---

## 13. Development Tips and Gotchas

### Database Issues

#### PostgreSQL vs SQLite Confusion
- **Problem**: Initial test scripts assumed SQLite database
- **Reality**: Project uses PostgreSQL
- **Solution**: Check `appsettings.json` for connection string
- **Location**: `test_api.py` was updated to reflect this

#### Specialization Seeding
- **Problem**: "Specialization with ID 1 not found" errors
- **Issue**: User entity references SpecializationId(1) but specializations aren't seeded
- **Files**: `DataSeeder.cs`, migration files
- **Solution**: Ensure specializations are seeded before users

### Entity Framework Gotchas

#### Include(s => s.Modules) Error
- **Problem**: "The expression 's.Modules' is invalid inside an 'Include' operation"
- **Cause**: `Modules` property is explicitly ignored in `SpecializationConfiguration.cs`
- **File**: `src/SledzSpecke.Infrastructure/DAL/Configurations/SpecializationConfiguration.cs:38`
- **Solution**: Remove all `.Include(s => s.Modules)` statements

### Authentication Issues

#### Login Field Names
- **Problem**: Login fails with "The Username field is required"
- **Cause**: API expects `username`, not `email` in login request
- **File**: `src/SledzSpecke.Application/Commands/SignIn.cs`
- **Solution**: Use `{"username": "testuser", "password": "Test123!"}`

#### Password Hashing
- **Problem**: "Invalid credentials" even with correct password
- **Method**: SHA256 hashing, NOT bcrypt
- **Correct hash for "Test123!"**: `VN5/YG8lI8uo76wXP6tC+39Z1Wzv+XTI/bc0LPLP40U=`
- **Update DB**: `UPDATE "Users" SET "Password" = 'VN5/YG8lI8uo76wXP6tC+39Z1Wzv+XTI/bc0LPLP40U=' WHERE "Username" = 'testuser';`

### Business Logic Changes

#### Sync Status Management
- **Original MAUI**: Synced items were read-only
- **New Behavior**: Synced items CAN be modified
- **Auto-transition**: Synced → Modified when edited
- **Files affected**:
  - `MedicalShift.cs`
  - `ProcedureBase.cs`
  - `Internship.cs`
- **Key point**: Only APPROVED items are truly read-only

#### Medical Shift Duration Validation
- **MAUI behavior**: Allows hours > 24 and minutes > 59
- **Example**: 8 hours 90 minutes is VALID
- **Normalization**: Only happens at display level via `TimeNormalizationHelper`
- **Don't add**: Validation like `if (minutes > 59)` - this is intentional!

#### Year vs Calendar Year
- **Confusion**: `Year` field is NOT calendar year (2024/2025)
- **Reality**: `Year` is medical education year (1-6 for Old SMK)
- **Special case**: Year 0 means "unassigned"
- **File**: `AddProcedureHandler.cs` has validation logic

### Missing Endpoints

#### Commented Out Endpoints
Some endpoints are commented out due to missing implementations:
- `GET /api/internships/{id}` - GetInternshipById query not implemented
- `DELETE /api/internships/{id}` - DeleteInternship command not implemented

### Common Compilation Errors

#### Missing Logger
- **Error**: "The type or namespace name 'ILogger' could not be found"
- **Solution**: Add `<PackageReference Include="Microsoft.Extensions.Logging.Abstractions" Version="9.0.0" />`
- **File**: Add to `.csproj` file in Application project

### Testing Tips

#### API Won't Start
1. Kill existing process: `pkill -f "dotnet.*SledzSpecke.Api"`
2. Check port: `lsof -i :5000`
3. Rebuild: `dotnet build`
4. Start: `dotnet run --project src/SledzSpecke.Api`

#### Database Reset
```bash
sudo -u postgres psql -c "DROP DATABASE IF EXISTS sledzspecke_db; CREATE DATABASE sledzspecke_db;"
dotnet ef database update --project src/SledzSpecke.Infrastructure --startup-project src/SledzSpecke.Api
```

### Project Structure Notes
- **Clean Architecture**: Core → Application → Infrastructure → Api
- **CQRS Pattern**: Commands and Queries separated
- **DDD**: Value Objects, Entities, Aggregates
- **Repository Pattern**: Interfaces in Core, implementations in Infrastructure

---

## 14. Development Optimization

### Resource Monitoring

#### Current VPS Specs
- RAM: 3.8 GB
- Swap: 4.0 GB
- CPU: 2 cores
- Disk: 77 GB (12% used)

#### Memory Usage Issues
During heavy development, multiple MSBuild and Roslyn compiler processes can accumulate, consuming significant memory. This leads to:
- Slow builds
- System using swap memory
- Degraded performance

### Optimization Strategies

#### 1. Regular Cleanup
```bash
# Basic cleanup (kills processes, clears temp files)
./cleanup-resources.sh

# Full cleanup (also removes bin/obj directories)
./cleanup-resources.sh --full
```

#### 2. Optimized Build Commands
```bash
# Use no-restore when dependencies haven't changed
dotnet build --no-restore

# Use configuration-specific builds
dotnet build -c Release  # Smaller binaries, optimized
dotnet build -c Debug    # Faster builds, larger binaries

# Build specific projects only
dotnet build src/SledzSpecke.Api/SledzSpecke.Api.csproj

# Use parallel builds wisely (limit on low-memory systems)
dotnet build -maxcpucount:1  # Limit to single CPU
```

#### 3. Development Workflow Optimizations

##### Testing Strategy
- Test specific endpoints instead of full API runs
- Use `dotnet watch run` for auto-reload during development
- Kill the API process immediately after testing

##### Build Strategy
- Build only what changed: `dotnet build --no-dependencies`
- Use incremental builds
- Clean only when necessary

#### 4. IDE/Editor Optimizations
If using VS Code or other IDEs remotely:
- Disable unnecessary extensions
- Limit OmniSharp memory usage
- Use lightweight editors for quick edits

### Monitoring Commands
```bash
# Check memory usage
free -h

# Check top processes
ps aux --sort=-%mem | head -20

# Check disk usage
df -h
du -sh ~/projects/mock/SledzSpecke.WebApi/*

# Monitor in real-time
htop  # If installed
watch -n 1 free -h
```

### Emergency Recovery
If system becomes unresponsive:
1. Kill all dotnet processes: `pkill -9 dotnet`
2. Clear swap: `sudo swapoff -a && sudo swapon -a` (requires sudo)
3. Restart dotnet services
4. Run cleanup script

### Preventing MSBuild/Roslyn Memory Issues

#### The Problem
MSBuild and Roslyn (VBCSCompiler) processes can accumulate during development because:
1. **Node Reuse**: MSBuild keeps processes alive to speed up subsequent builds
2. **Compiler Server**: Roslyn compiler server stays resident in memory
3. **Parallel Builds**: Multiple MSBuild nodes spawn for parallel compilation
4. **No Timeout**: Processes don't automatically terminate

#### The Solution

##### 1. Environment Configuration (.env file)
```bash
MSBUILDDISABLENODEREUSE=1        # Prevents MSBuild process reuse
MSBUILDNODECOUNT=1               # Limits to single process
DOTNET_CLI_USE_MSBUILD_SERVER=0  # Disables MSBuild server
DOTNET_COMPILER_SERVER_TIMEOUT=600 # 10-minute timeout for Roslyn
```

##### 2. Safe Build Script (safe-build.sh)
- Kills existing processes before building
- Applies environment limits
- Cleans up immediately after build

##### 3. Automatic Cleanup (via cron)
- Runs every 5 minutes
- Kills processes older than 10 minutes
- Logs all cleanup actions

##### 4. Manual Controls
- `killbuild` - Emergency kill command
- `clean` - Quick cleanup
- `cleanfull` - Complete cleanup

#### Initial Setup (One Time)
```bash
# 1. Setup automatic cleanup
./setup-auto-cleanup.sh

# 2. Setup command aliases
./setup-aliases.sh
source ~/.bashrc
```

#### Daily Development
```bash
# Use safe build instead of dotnet build
./safe-build.sh

# Or use the alias
build

# Run API safely
runapi

# Quick cleanup when needed
clean
```

#### Emergency Recovery
```bash
# If system is slow
killbuild
clean

# If very bad
cleanfull
```

### Development Best Practices
1. **Commit and Push Regularly**
   - Reduces need for long-running processes
   - Allows cleanup between sessions

2. **Use Feature Branches**
   - Smaller, focused builds
   - Easier to clean and restart

3. **Test Incrementally**
   - Run API only when needed
   - Use unit tests for quick feedback
   - Integration tests only for final validation

4. **Clean Workspace Daily**
   - Run cleanup script at end of day
   - Start fresh each session

Remember: A clean development environment is a fast development environment!

---

## 15. API Implementation Details

### Internships API

#### Overview
The Internships API provides comprehensive functionality for managing medical internships within the specialization training system.

#### Endpoints

##### GET /api/internships
Retrieves a list of internships for a specialization.

**Query Parameters:**
- `specializationId` (required): The ID of the specialization
- `moduleId` (optional): Filter by specific module

**Response:** Array of `InternshipDto`

##### GET /api/internships/{id}
Retrieves a specific internship by ID.
*Note: This endpoint needs implementation of GetInternshipById query and handler*

##### POST /api/internships
Creates a new internship.

**Request Body:**
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

##### PUT /api/internships/{id}
Updates an existing internship.

**Request Body:**
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

##### POST /api/internships/{id}/approve
Approves an internship.

**Request Body:**
```json
{
  "approverName": "Dr. Supervisor"
}
```

##### POST /api/internships/{id}/complete
Marks an internship as completed.

**Business Rules:**
- Cannot mark as completed if end date is in the future
- Cannot modify approved internships

##### DELETE /api/internships/{id}
Deletes an internship.
*Note: This endpoint needs implementation of DeleteInternship command and handler*

**Business Rules:**
- Can only delete internships with SyncStatus of NotSynced or SyncFailed

#### Entity Update Methods
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

#### Implementation Status
✅ Implemented:
- GET /api/internships (list)
- POST /api/internships (create)
- PUT /api/internships/{id} (update)
- POST /api/internships/{id}/approve
- POST /api/internships/{id}/complete

❌ Needs Implementation:
- GET /api/internships/{id} - Requires GetInternshipById query and handler
- DELETE /api/internships/{id} - Requires DeleteInternship command and handler

---

## 16. Business Rules and Validation

### Medical Shift Duration Validation

#### Overview
The medical shift duration validation has been aligned with the MAUI implementation to provide consistent behavior across platforms.

#### Key Principles

##### 1. Minimal Validation
- Only ensures that the total duration (hours + minutes) is greater than zero
- No maximum duration limits are enforced
- Negative values are not allowed

##### 2. Flexible Minutes Input
- Minutes are NOT restricted to 0-59
- Users can enter values like 90 minutes, 120 minutes, etc.
- This flexibility allows for easier data entry

##### 3. Time Normalization
- Normalization (converting excess minutes to hours) happens only at the display/summary level
- Individual shift records retain their original values
- Example: A shift with 2 hours 90 minutes is stored as-is, but displayed as 3 hours 30 minutes

#### Implementation Details

##### Validation Rules

###### Common Rules (Both SMK Versions)
- Hours >= 0
- Minutes >= 0  
- Total duration (hours + minutes) > 0
- Location is required and <= 100 characters

###### Old SMK Specific
- Year must be between 1 and 5
- Default initialization: 24 hours, 0 minutes (matching MAUI)

###### New SMK Specific
- Year must be provided (> 0)
- Default initialization: 10 hours, 5 minutes (matching MAUI)
- Shift date must be within internship period

##### Removed Restrictions
The following restrictions were removed to match MAUI behavior:
- Maximum 24-hour limit for Old SMK
- Maximum 16-hour limit for New SMK
- Minimum 4-hour requirement for New SMK
- Minutes limited to 0-59

##### Time Calculation
Total hours for statistics and summaries:
```csharp
double totalHours = hours + (minutes / 60.0);
```

##### Display Format
Time is displayed in Polish format:
```
"X godz. Y min."
```
With normalization applied for display (but not storage).

#### Helper Classes

##### TimeNormalizationHelper
Located in `/src/SledzSpecke.Application/Helpers/TimeNormalizationHelper.cs`

Provides methods for:
- `NormalizeTime(hours, minutes)` - Converts excess minutes to hours
- `CalculateTotalHours(hours, minutes)` - Returns decimal hours
- `FormatTime(hours, minutes, normalize)` - Formats for display

##### MedicalShiftDto
Enhanced with computed properties:
- `FormattedTime` - Normalized time display
- `TotalHours` - Decimal hours for calculations

#### Migration Notes
When migrating from the previous strict validation:
1. Existing data with minutes > 59 will now be valid
2. Shifts longer than previous limits are now allowed
3. UI should handle display normalization, not enforce input restrictions

#### Testing Considerations
Test cases should include:
- Zero duration validation (should fail)
- Minutes > 59 (should pass)
- Very long shifts (e.g., 36 hours)
- Negative values (should fail)
- Time normalization in summaries

---

## 17. Sync Status Management

### Overview
The Sync Status Management system tracks the synchronization state of entities between the local application and the SMK (System Monitorowania Kształcenia) system.

### Key Design Decision
**Important Change**: Unlike the original MAUI implementation where synced items were read-only, the Web API implementation allows synced items to be modified. When a synced item is updated, it automatically transitions to `Modified` status, maintaining an audit trail while allowing data corrections.

### Sync Status States
The `SyncStatus` enum has five possible values:

1. **NotSynced (0)**: Entity has never been synchronized with SMK. Default state for new entities.
2. **Synced (1)**: Entity is synchronized with SMK with no local changes. Can be modified (will auto-transition to Modified).
3. **SyncError (2)**: Error occurred during last sync attempt. Entity may have partial data.
4. **SyncFailed (3)**: Sync attempt failed completely. Entity remains in previous state.
5. **Modified (4)**: Previously synced entity has been modified locally. Needs re-synchronization.

### Implementation Details

#### Affected Entities
The following entities implement sync status management:
- `MedicalShift`
- `ProcedureBase` (and derived classes)
- `Internship`
- `Course`

#### Automatic Status Transitions
When any update method is called on a synced entity, it automatically transitions to `Modified` status:

```csharp
// Example from MedicalShift.UpdateShiftDetails
if (SyncStatus == SyncStatus.Synced)
{
    SyncStatus = SyncStatus.Modified;
}
```

#### Modification Rules
- **Synced items**: CAN be modified (auto-transition to Modified)
- **Approved items**: CANNOT be modified (throw exception)
- **Other statuses**: Can be modified normally

#### Code Example
```csharp
// Before update
var shift = await _repository.GetByIdAsync(shiftId);
// shift.SyncStatus == SyncStatus.Synced

// Update the shift
shift.UpdateShiftDetails(8, 30, "New Hospital");
// shift.SyncStatus == SyncStatus.Modified (automatic transition)

await _repository.UpdateAsync(shift);
```

### Migration from MAUI

#### Key Differences
1. **MAUI**: Synced items were completely read-only
2. **Web API**: Synced items can be modified (with automatic status transition)

#### Rationale
This change was made to:
- Allow users to correct data errors in synced items
- Maintain audit trail of changes
- Support more flexible workflows
- Reduce user frustration with locked data

### Testing
The test script (`test_api.py`) includes comprehensive testing for sync status transitions. Key test scenarios:
1. Creating new entities (NotSynced status)
2. Updating synced entities (transition to Modified)
3. Attempting to update approved entities (should fail)

### Troubleshooting

#### Common Issues
1. **"Cannot modify synced item" errors**
   - Check if you're using old code that prevents synced modifications
   - Ensure entity's `EnsureCanModify()` method is updated

2. **Status not transitioning to Modified**
   - Verify update methods include the transition logic
   - Check that the entity was actually synced before update

3. **Database sync status mismatch**
   - Run migrations to ensure database schema is current
   - Check Entity Framework configurations

#### Database Queries
Check entity sync status:
```sql
-- PostgreSQL
SELECT "Id", "SyncStatus", "IsApproved" FROM "MedicalShifts" WHERE "Id" = 1;
SELECT "Id", "SyncStatus", "Status" FROM "Procedures" WHERE "Id" = 1;
```

### Future Considerations
1. **Sync Queue**: Implement a queue for Modified items awaiting sync
2. **Conflict Resolution**: Handle conflicts when SMK data changes during local modifications
3. **Batch Sync**: Support bulk synchronization of Modified items
4. **Sync History**: Track sync attempts and results for auditing

### Related Files
- `/src/SledzSpecke.Core/ValueObjects/SyncStatus.cs` - Enum definition
- `/src/SledzSpecke.Core/Entities/*.cs` - Entity implementations
- `/src/SledzSpecke.Application/*/Handlers/Update*.cs` - Update handlers
- `/test_api.py` - Integration tests

---

## Podsumowanie

SledzSpecke to kompleksowy system do śledzenia postępu specjalizacji medycznej, zaprojektowany z myślą o:
- **Prostocie użytkowania** dla lekarzy
- **Zgodności z SMK** w eksporcie danych
- **Elastyczności** - działa na Web, Android i iOS
- **Skalowalności** - Clean Architecture umożliwia łatwy rozwój
- **Bezpieczeństwie** - JWT, walidacja, HTTPS

System jest gotowy do wdrożenia z drobnymi brakami funkcjonalności (głównie eksport), które można szybko uzupełnić.

# important-instruction-reminders
Do what has been asked; nothing more, nothing less.
NEVER create files unless they're absolutely necessary for achieving your goal.
ALWAYS prefer editing an existing file to creating a new one.
NEVER proactively create documentation files (*.md) or README files. Only create documentation files if explicitly requested by the User.