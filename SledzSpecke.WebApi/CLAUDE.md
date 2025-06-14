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

## Podsumowanie

SledzSpecke to kompleksowy system do śledzenia postępu specjalizacji medycznej, zaprojektowany z myślą o:
- **Prostocie użytkowania** dla lekarzy
- **Zgodności z SMK** w eksporcie danych
- **Elastyczności** - działa na Web, Android i iOS
- **Skalowalności** - Clean Architecture umożliwia łatwy rozwój
- **Bezpieczeństwie** - JWT, walidacja, HTTPS

System jest gotowy do wdrożenia z drobnymi brakami funkcjonalności (głównie eksport), które można szybko uzupełnić.