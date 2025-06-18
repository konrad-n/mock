# SledzSpecke Polish Medical Context Examples

## Overview

This document provides Polish medical context examples for the implemented patterns, based on SMK (System Monitorowania Kształcenia) requirements.

## 1. Saga Pattern - Real SMK Workflows

### Monthly Report Submission Saga (Miesięczne Sprawozdanie)

```csharp
public class SMKMiesieczneSprawozdanieSaga : SagaBase<SMKMiesieczneSprawozdanieData>
{
    public SMKMiesieczneSprawozdanieSaga() : base("SMKMiesieczneSprawozdanie")
    {
        // Kroki procesu miesięcznego sprawozdania
        AddStep(new WeryfikacjaDyzurowStep());          // Weryfikacja dyżurów
        AddStep(new WeryfikacjaProcedurStep());         // Weryfikacja procedur
        AddStep(new WeryfikacjaKursowStep());           // Weryfikacja kursów
        AddStep(new ObliczenieCzasuPracyStep());        // Obliczenie czasu pracy
        AddStep(new GenerowanieRaportuStep());          // Generowanie raportu
        AddStep(new PowiadomienieKierownikaStep());     // Powiadomienie kierownika
        AddStep(new WyslanieDoSMKStep());               // Wysłanie do systemu SMK
    }
}

public class WeryfikacjaDyzurowStep : ISagaStep<SMKMiesieczneSprawozdanieData>
{
    public string Name => "WeryfikacjaDyżurów";
    
    public async Task<Result> ExecuteAsync(SMKMiesieczneSprawozdanieData data, CancellationToken cancellationToken)
    {
        // Weryfikacja zgodnie z wymaganiami SMK:
        // - Minimum 160 godzin miesięcznie
        // - Maximum 48 godzin tygodniowo
        // - Dyżury tylko w akredytowanych placówkach
        
        var totalHours = data.Dyzury.Sum(d => d.LiczbaGodzin);
        
        if (totalHours < 160)
        {
            return Result.Failure($"Niewystarczająca liczba godzin: {totalHours}/160");
        }
        
        // Sprawdzenie limitów tygodniowych
        var weeklyHours = data.Dyzury
            .GroupBy(d => CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
                d.Data, CalendarWeekRule.FirstDay, DayOfWeek.Monday))
            .Select(g => g.Sum(d => d.LiczbaGodzin));
            
        if (weeklyHours.Any(h => h > 48))
        {
            return Result.Failure("Przekroczono limit 48 godzin tygodniowo");
        }
        
        return Result.Success();
    }
}
```

### Specialization Application Saga (Wniosek o Rozpoczęcie Specjalizacji)

```csharp
public class WniosekSpecjalizacjaSaga : SagaBase<WniosekSpecjalizacjaData>
{
    public WniosekSpecjalizacjaSaga() : base("WniosekSpecjalizacja")
    {
        AddStep(new WeryfikacjaLEPStep());               // Weryfikacja LEP
        AddStep(new WeryfikacjaStazuStep());             // Weryfikacja stażu
        AddStep(new SprawdzenieMiejscStep());            // Sprawdzenie wolnych miejsc
        AddStep(new GenerowanieWnioskuStep());           // Generowanie wniosku
        AddStep(new PodpisElektronicznyStep());          // Podpis elektroniczny
        AddStep(new WyslanieDoWojewodztwaStep());        // Wysłanie do urzędu wojewódzkiego
        AddStep(new OczekiwanieNaDecyzjeStep());         // Oczekiwanie na decyzję
    }
}

public class WeryfikacjaLEPStep : ISagaStep<WniosekSpecjalizacjaData>
{
    public async Task<Result> ExecuteAsync(WniosekSpecjalizacjaData data, CancellationToken cancellationToken)
    {
        // Weryfikacja Lekarskiego Egzaminu Państwowego
        if (!data.CzyZdanyLEP)
        {
            return Result.Failure("Brak zdanego LEP - wymagany do rozpoczęcia specjalizacji");
        }
        
        if (data.DataZdaniaLEP > DateTime.Now.AddYears(-5))
        {
            return Result.Failure("LEP zdany ponad 5 lat temu - wymagana ponowna weryfikacja");
        }
        
        return Result.Success();
    }
}
```

## 2. Enhanced Pipeline - Medical Shift Processing

### Polish Medical Shift Validation Pipeline

```csharp
public class DyzurMedycznyPipelineFactory : IPipelineFactory
{
    public Func<MessageContext, Task> CreatePipeline(string messageType)
    {
        var builder = new PipelineBuilder<MessageContext>();
        
        if (messageType == "DyżurMedyczny")
        {
            builder
                // Walidacja podstawowa
                .UseStep<WalidacjaDyzuruStep>()
                
                // Weryfikacja akredytacji placówki
                .UseStep<WeryfikacjaAkredytacjiStep>()
                
                // Sprawdzenie limitów
                .Use(async (context, next) =>
                {
                    var dyzur = context.GetPayload<DyzurMedyczny>();
                    
                    // Sprawdzenie czy placówka ma akredytację dla danej specjalizacji
                    if (!await SprawdzAkredytacje(dyzur.Placowka, dyzur.Specjalizacja))
                    {
                        context.ErrorMessage = "Placówka nie posiada akredytacji dla tej specjalizacji";
                        return;
                    }
                    
                    await next();
                })
                
                // Obliczenie punktów edukacyjnych
                .UseStep<ObliczeniePunktowEdukacyjnychStep>()
                
                // Zapis do bazy
                .UseStep<ZapisDyzuruStep>()
                
                // Powiadomienie kierownika
                .UseStep<PowiadomienieKierownikaStep>();
        }
        
        return builder.Build();
    }
}

public class WeryfikacjaAkredytacjiStep : IPipelineStep<MessageContext>
{
    private readonly IAkredytacjaService _akredytacjaService;
    
    public async Task ExecuteAsync(MessageContext context, Func<Task> next)
    {
        var dyzur = context.GetPayload<DodajDyzurCommand>();
        
        // Lista akredytowanych szpitali w Polsce
        var akredytowanePlacowki = new[]
        {
            "Centrum Onkologii - Instytut im. Marii Skłodowskiej-Curie",
            "Instytut Kardiologii im. Prymasa Tysiąclecia",
            "Szpital Uniwersytecki w Krakowie",
            "Wojewódzki Szpital Specjalistyczny im. M. Kopernika w Łodzi",
            // ... więcej placówek
        };
        
        if (!akredytowanePlacowki.Contains(dyzur.NazwaPlacowki))
        {
            context.ErrorMessage = $"Placówka '{dyzur.NazwaPlacowki}' nie posiada akredytacji SMK";
            context.Log($"Odrzucono dyżur - brak akredytacji placówki");
            return; // Przerwanie pipeline
        }
        
        await next();
    }
}
```

## 3. Audit Trail - Polish Medical Compliance

### Medical Procedure Audit (Audyt Procedur Medycznych)

```csharp
public class ProceduraMedycznaAuditService
{
    public async Task<AuditLog> LogujWykonanieProcedury(
        ProceduraMedyczna procedura,
        string wykonawca,
        string pacjentPESEL)
    {
        var auditLog = AuditLog.Create(
            entityType: "ProceduraMedyczna",
            entityId: procedura.Id.ToString(),
            action: "Wykonanie",
            userId: wykonawca,
            oldValues: null,
            newValues: new
            {
                KodProcedury = procedura.KodICD9,
                NazwaProcedury = procedura.Nazwa,
                DataWykonania = DateTime.Now,
                MiejsceWykonania = procedura.Szpital,
                Specjalizacja = procedura.Specjalizacja,
                // Zgodność z RODO - nie zapisujemy pełnego PESEL
                PacjentHash = HashPESEL(pacjentPESEL)
            }
        );
        
        return auditLog;
    }
    
    // Przykładowe kody procedur zgodne z klasyfikacją ICD-9
    private readonly Dictionary<string, string> procedury = new()
    {
        ["89.52"] = "Elektrokardiogram",
        ["88.72"] = "Echokardiografia",
        ["93.36"] = "Rehabilitacja kardiologiczna",
        ["37.22"] = "Cewnikowanie serca",
        ["36.06"] = "Angioplastyka wieńcowa"
    };
}
```

### Specialization Progress Audit (Audyt Postępu Specjalizacji)

```csharp
public class PostepSpecjalizacjiAuditHandler : INotificationHandler<PostepSpecjalizacjiZmieniony>
{
    public async Task Handle(PostepSpecjalizacjiZmieniony notification, CancellationToken cancellationToken)
    {
        var audit = new
        {
            LekarzId = notification.LekarzId,
            Specjalizacja = notification.Specjalizacja,
            RokSpecjalizacji = notification.RokSpecjalizacji,
            ModulPoprzedni = notification.PoprzedniModul,
            ModulObecny = notification.ObecnyModul,
            DataZmiany = DateTime.Now,
            
            // Wymagania SMK
            WymaganeProcedury = notification.WymaganeProcedury,
            ZrealizowaneProcedury = notification.ZrealizowaneProcedury,
            ProcentRealizacji = notification.ProcentRealizacji,
            
            // Kursy i szkolenia
            UkończoneKursy = notification.UkończoneKursy,
            WymaganeKursy = notification.WymaganeKursy
        };
        
        await _auditRepository.LogAsync("PostępSpecjalizacji", audit);
    }
}
```

## 4. Event Sourcing - Medical Education History

### Event-Sourced Internship (Staż Specjalizacyjny)

```csharp
public class StazSpecjalizacyjnyEventSourced : EventSourcedAggregate
{
    public int LekarzId { get; private set; }
    public string Specjalizacja { get; private set; }
    public DateTime DataRozpoczecia { get; private set; }
    public string StatusStazu { get; private set; }
    public List<ModulSpecjalizacyjny> Moduly { get; private set; }
    
    // Wydarzenia związane ze stażem
    public static StazSpecjalizacyjnyEventSourced RozpocznijStaz(
        int lekarzId,
        string specjalizacja,
        string jednostkaSzkolaca)
    {
        var staz = new StazSpecjalizacyjnyEventSourced();
        staz.RaiseEvent(new StazRozpoczetyEvent(
            Guid.NewGuid(),
            lekarzId,
            specjalizacja,
            jednostkaSzkolaca,
            DateTime.Now
        ));
        return staz;
    }
    
    public Result DodajZrealizowanyModul(string nazwaModulu, int liczbaGodzin)
    {
        // Weryfikacja zgodnie z programem specjalizacji
        var wymagania = PobierzWymaganiaModulu(Specjalizacja, nazwaModulu);
        
        if (liczbaGodzin < wymagania.MinimalnaLiczbaGodzin)
        {
            return Result.Failure($"Niewystarczająca liczba godzin: {liczbaGodzin}/{wymagania.MinimalnaLiczbaGodzin}");
        }
        
        RaiseEvent(new ModulZrealizowanyEvent(
            Id,
            nazwaModulu,
            liczbaGodzin,
            DateTime.Now
        ));
        
        return Result.Success();
    }
    
    private void Apply(StazRozpoczetyEvent @event)
    {
        Id = @event.StazId;
        LekarzId = @event.LekarzId;
        Specjalizacja = @event.Specjalizacja;
        DataRozpoczecia = @event.DataRozpoczecia;
        StatusStazu = "W trakcie";
        Moduly = new List<ModulSpecjalizacyjny>();
    }
    
    private void Apply(ModulZrealizowanyEvent @event)
    {
        Moduly.Add(new ModulSpecjalizacyjny
        {
            Nazwa = @event.NazwaModulu,
            LiczbaGodzin = @event.LiczbaGodzin,
            DataRealizacji = @event.DataRealizacji
        });
    }
}
```

### Medical Procedure Event Stream

```csharp
public class ProceduraMedycznaEventStream
{
    // Przykład strumienia zdarzeń dla procedury kardiologicznej
    public async Task<List<IDomainEvent>> PobierzHistorieProcedury(Guid proceduraId)
    {
        return new List<IDomainEvent>
        {
            new ProceduraZaplanowanaEvent(proceduraId, "37.22", "Cewnikowanie serca", DateTime.Now.AddDays(-7)),
            new PacjentPrzygotowanyEvent(proceduraId, "Badania przedoperacyjne wykonane"),
            new ProceduraRozpoczetaEvent(proceduraId, "Dr Jan Kowalski", "Sala operacyjna 3"),
            new EtapProcedurZakonczonyEvent(proceduraId, "Dostęp naczyniowy uzyskany"),
            new EtapProcedurZakonczonyEvent(proceduraId, "Cewnik wprowadzony do tętnicy wieńcowej"),
            new WynikBadaniaZarejestrowanyEvent(proceduraId, "Zwężenie 80% w LAD"),
            new ProceduraZakonczonaEvent(proceduraId, "Sukces", TimeSpan.FromMinutes(45)),
            new RaportProcedurWygenerowanyEvent(proceduraId, "Zalecana angioplastyka")
        };
    }
}
```

## 5. Integration Examples

### Complete Monthly SMK Report Workflow

```csharp
public class KompletnyProcesRaportowaniaSMK
{
    private readonly ISagaOrchestrator<SMKMiesieczneSprawozdanieSaga, SMKMiesieczneSprawozdanieData> _sagaOrchestrator;
    private readonly IPipelineFactory _pipelineFactory;
    private readonly IEventStore _eventStore;
    
    public async Task<Result> GenerujMiesieczneSprawozdanie(
        int lekarzId,
        int rok,
        int miesiac)
    {
        // 1. Przygotowanie danych
        var dane = new SMKMiesieczneSprawozdanieData
        {
            LekarzId = lekarzId,
            Rok = rok,
            Miesiac = miesiac
        };
        
        // 2. Uruchomienie sagi
        var sagaResult = await _sagaOrchestrator.StartAsync(dane, CancellationToken.None);
        
        if (!sagaResult.IsSuccess)
            return Result.Failure("Nie udało się rozpocząć procesu sprawozdania");
        
        // 3. Przetwarzanie dyżurów przez pipeline
        foreach (var dyzur in dane.Dyzury)
        {
            var context = new MessageContext
            {
                MessageType = "DyżurMedyczny",
                Payload = dyzur
            };
            
            var pipeline = _pipelineFactory.CreatePipeline("DyżurMedyczny");
            await pipeline(context);
        }
        
        // 4. Zapisanie w event store dla audytu
        var staz = await _eventStore.GetAggregateAsync<StazSpecjalizacyjnyEventSourced>(dane.StazId);
        staz.DodajMiesieczneSprawozdanie(rok, miesiac, dane.Dyzury.Count, dane.Procedury.Count);
        await _eventStore.SaveAggregateAsync(staz);
        
        return Result.Success();
    }
}
```

### Polish Medical Specializations

```csharp
public static class PolskieSpecjalizacjeMedyczne
{
    public static readonly Dictionary<string, SpecjalizacjaInfo> Specjalizacje = new()
    {
        ["kardiologia"] = new SpecjalizacjaInfo
        {
            Nazwa = "Kardiologia",
            CzasTrwania = 6, // lat
            WymaganeModuly = new[]
            {
                "Moduł podstawowy",
                "Moduł kardiologii inwazyjnej",
                "Moduł elektrofizjologii",
                "Moduł kardiologii dziecięcej"
            },
            MinimalnaLiczbaProcedur = new Dictionary<string, int>
            {
                ["EKG"] = 1000,
                ["ECHO"] = 500,
                ["Koronarografia"] = 100,
                ["Angioplastyka"] = 50
            }
        },
        ["chirurgia_ogolna"] = new SpecjalizacjaInfo
        {
            Nazwa = "Chirurgia ogólna",
            CzasTrwania = 6,
            WymaganeModuly = new[]
            {
                "Moduł podstawowy",
                "Moduł chirurgii jamy brzusznej",
                "Moduł chirurgii endokrynologicznej",
                "Moduł chirurgii onkologicznej"
            }
        }
        // ... więcej specjalizacji
    };
}
```

## Compliance Notes

1. **RODO Compliance**: Nie przechowujemy pełnych numerów PESEL
2. **NFZ Requirements**: Wszystkie procedury muszą mieć kody ICD-9/ICD-10
3. **SMK Integration**: Raporty muszą być zgodne z formatem XML wymaganym przez SMK
4. **Akredytacja**: Tylko zatwierdzone placówki mogą być miejscem odbywania stażu