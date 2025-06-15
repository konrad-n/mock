# Plan Wdrożenia Monitoringu i Logowania dla SledzSpecke

## Analiza Obecnej Sytuacji

### Problem
- Ogólne komunikaty błędów bez szczegółów ("Wystąpił błąd podczas rejestracji")
- Brak wglądu w rzeczywiste przyczyny błędów
- Brak metryk aplikacji
- Logi tylko w plikach lokalnych

### Wymagania
- Rozwiązanie darmowe lub z darmowym tierem
- Możliwość hostowania na VPS (3.8GB RAM)
- Łatwa integracja z .NET/Serilog
- Strukturalne logowanie z możliwością wyszukiwania
- Monitorowanie wyjątków i błędów

## Rekomendowane Rozwiązanie: Seq + Ulepszone Logowanie

### Dlaczego Seq?
1. **Darmowy** dla pojedynczego użytkownika (wystarczające dla development/small production)
2. **Dedykowany dla .NET** - natywna integracja z Serilog
3. **Lekki** - działa dobrze na VPS z ograniczonymi zasobami
4. **Strukturalne logi** - łatwe wyszukiwanie po właściwościach
5. **Real-time dashboard** - natychmiastowy wgląd w błędy
6. **Alerty** - powiadomienia o krytycznych błędach

### Alternatywy Rozważone

| Rozwiązanie | Zalety | Wady | Decyzja |
|------------|---------|------|---------|
| **Prometheus + Grafana** | Darmowe, popularne, dobre dla metryk | Skomplikowana konfiguracja, ciężkie na zasoby | ❌ Za ciężkie na początek |
| **Application Insights** | 5GB/miesiąc za darmo, łatwa integracja | Vendor lock-in (Azure), limit danych | ❌ Ograniczenia w darmowym tier |
| **ELK Stack** | Potężne, elastyczne | Bardzo ciężkie, wymaga 8GB+ RAM | ❌ Za duże wymagania |
| **Loki + Grafana** | Lżejsze niż ELK | Nadal wymaga Grafany, bardziej dla Kubernetes | ❌ Overengineering |
| **Seq** | Dedykowane dla .NET, lekkie, darmowe | Tylko 1 użytkownik w darmowej wersji | ✅ Idealne dla nas |

## Plan Implementacji

### Faza 1: Ulepszone Logowanie Błędów (1-2 dni)

#### 1.1 Rozszerzenie Error Middleware
```csharp
// Dodanie szczegółowych informacji o błędach
public class EnhancedErrorHandlingMiddleware
{
    - Logowanie pełnego stack trace
    - Kontekst użytkownika (ID, email)
    - Request details (endpoint, parametry)
    - Correlation ID dla śledzenia
}
```

#### 1.2 Ulepszone Komunikaty Błędów
```csharp
// Zamiast ogólnego "Wystąpił błąd"
public class DetailedErrorResponse
{
    public string ErrorCode { get; set; }      // np. "USER_REGISTRATION_FAILED"
    public string UserMessage { get; set; }     // Przyjazny komunikat
    public string TechnicalMessage { get; set; } // Szczegóły dla deweloperów
    public string CorrelationId { get; set; }   // Do śledzenia w logach
    public Dictionary<string, object> Details { get; set; } // Dodatkowe info
}
```

#### 1.3 Walidacja i Błędy Biznesowe
- Specific error codes dla każdego typu błędu
- Mapowanie wyjątków domenowych na przyjazne komunikaty
- Walidacja FluentValidation z czytelnymi komunikatami

### Faza 2: Instalacja i Konfiguracja Seq (1 dzień)

#### 2.1 Instalacja Seq na VPS
```bash
# Instalacja jako Docker container (najłatwiejsze)
docker run \
  --name seq \
  -d \
  --restart unless-stopped \
  -e ACCEPT_EULA=Y \
  -p 5341:80 \
  -v /opt/seq/data:/data \
  datalust/seq:latest

# Lub natywna instalacja
wget https://datalust.co/download/seq/linux
chmod +x seq-linux
./seq-linux install
```

#### 2.2 Konfiguracja Nginx
```nginx
# Proxy dla Seq UI
location /seq/ {
    proxy_pass http://localhost:5341/;
    proxy_set_header Host $host;
    proxy_set_header X-Real-IP $remote_addr;
}
```

#### 2.3 Zabezpieczenie
- Ustawienie hasła administratora
- Ograniczenie dostępu po IP
- HTTPS dla interfejsu webowego

### Faza 3: Integracja z Aplikacją (1 dzień)

#### 3.1 Dodanie Seq Sink do Serilog
```csharp
// Program.cs
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithEnvironmentName()
    .Enrich.WithExceptionDetails() // Szczegóły wyjątków
    .WriteTo.Console()
    .WriteTo.File("logs/app-.log", rollingInterval: RollingInterval.Day)
    .WriteTo.Seq("http://localhost:5341") // Seq sink
    .CreateLogger();
```

#### 3.2 Strukturalne Logowanie
```csharp
// Przykład użycia
_logger.LogError(ex, "Registration failed for user {Email}. Validation errors: {@Errors}", 
    command.Email, 
    validationResult.Errors);

// Bogaty kontekst
using (_logger.BeginScope(new Dictionary<string, object>
{
    ["UserId"] = userId,
    ["CorrelationId"] = correlationId,
    ["RequestPath"] = context.Request.Path
}))
{
    // Wszystkie logi w tym bloku będą miały ten kontekst
}
```

#### 3.3 Health Checks z Logowaniem
```csharp
services.AddHealthChecks()
    .AddSeqHealthCheck("http://localhost:5341")
    .AddDbContextCheck<SledzSpeckeDbContext>();
```

### Faza 4: Monitoring i Alerty (1 dzień)

#### 4.1 Konfiguracja Alertów w Seq
- Alert na błędy krytyczne (Level = Error/Fatal)
- Alert na failed registrations
- Alert na niepowodzenia health checks
- Alert na wysokie response time

#### 4.2 Dashboard w Seq
- Widget: Błędy w ostatniej godzinie
- Widget: Top 5 najczęstszych błędów
- Widget: Failed operations by type
- Widget: Response time percentiles

#### 4.3 Retention Policy
```json
{
  "RetentionPolicy": {
    "MinimumAge": "7d",    // Zachowaj logi przez 7 dni
    "MaximumSize": "5GB"   // Max 5GB przestrzeni
  }
}
```

### Faza 5: Dokumentacja i Procedury (0.5 dnia)

#### 5.1 Runbook
- Jak sprawdzić przyczynę błędu używając Correlation ID
- Typowe błędy i ich rozwiązania
- Procedura eskalacji

#### 5.2 Przykładowe Zapytania Seq
```sql
-- Wszystkie błędy rejestracji
@Message like '%Registration failed%'

-- Błędy dla konkretnego użytkownika
Email = 'user@example.com' and @Level = 'Error'

-- Wolne requesty (>1s)
ResponseTime > 1000
```

## Koszty i Zasoby

### Zasoby VPS
- **Seq**: ~200-400MB RAM
- **Przestrzeń dyskowa**: 5-10GB dla logów
- **CPU**: Minimalne obciążenie

### Koszty
- **Seq**: Darmowy (1 użytkownik)
- **Dodatkowa przestrzeń**: Już mamy 77GB
- **Całkowity koszt**: 0 PLN

## Harmonogram

| Faza | Czas | Priorytet |
|------|------|-----------|
| Faza 1: Ulepszone logowanie | 1-2 dni | WYSOKI |
| Faza 2: Instalacja Seq | 1 dzień | WYSOKI |
| Faza 3: Integracja | 1 dzień | WYSOKI |
| Faza 4: Monitoring | 1 dzień | ŚREDNI |
| Faza 5: Dokumentacja | 0.5 dnia | NISKI |
| **RAZEM** | **4.5-5.5 dni** | |

## Natychmiastowe Działania

1. **Debugowanie problemu rejestracji**:
   ```bash
   # Sprawdź logi API
   sudo journalctl -u sledzspecke-api -n 100 --no-pager | grep -i error
   
   # Sprawdź logi aplikacji
   tail -f /var/log/sledzspecke/api-*.log | grep -i registration
   ```

2. **Tymczasowe rozwiązanie** - dodaj verbose logging:
   ```csharp
   // W SignUpHandler
   _logger.LogInformation("Starting registration for email: {Email}", command.Email);
   try 
   {
       // existing code
   }
   catch (Exception ex)
   {
       _logger.LogError(ex, "Registration failed for {Email}. Full error: {Error}", 
           command.Email, ex.ToString());
       throw;
   }
   ```

## Długoterminowa Wizja

1. **Metryki Aplikacji** (Przyszłość)
   - Prometheus dla metryk (requests/sec, response time)
   - Grafana dla wizualizacji
   - Custom metrics (użytkownicy online, operacje/dzień)

2. **Distributed Tracing** (Jeśli będzie mikrousługi)
   - Jaeger lub Zipkin
   - Correlation między serwisami

3. **APM** (Application Performance Monitoring)
   - Rozważyć w przyszłości płatne rozwiązanie

## Podsumowanie

Seq to idealne rozwiązanie dla naszego przypadku:
- ✅ Darmowe
- ✅ Lekkie na zasoby
- ✅ Dedykowane dla .NET
- ✅ Łatwe w implementacji
- ✅ Natychmiastowy wgląd w błędy

Po wdrożeniu będziemy mieli pełną widoczność błędów i możliwość szybkiego debugowania problemów takich jak ten z rejestracją.