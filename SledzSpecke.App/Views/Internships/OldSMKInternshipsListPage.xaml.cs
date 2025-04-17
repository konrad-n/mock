using SledzSpecke.App.Models;
using SledzSpecke.App.Services.Database;
using SledzSpecke.App.ViewModels.Internships;

namespace SledzSpecke.App.Views.Internships
{
    public partial class OldSMKInternshipsListPage : ContentPage
    {
        private readonly OldSMKInternshipsListViewModel viewModel;

        public OldSMKInternshipsListPage(OldSMKInternshipsListViewModel viewModel)
        {
            this.InitializeComponent();
            this.viewModel = viewModel;
            this.BindingContext = this.viewModel;
        }

        protected override void OnNavigatedTo(NavigatedToEventArgs args)
        {
            base.OnNavigatedTo(args);

            Shell.SetBackButtonBehavior(this, new BackButtonBehavior
            {
                Command = new Command(async () =>
                {
                    await Shell.Current.GoToAsync("///dashboard");
                })
            });
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Debugowanie - sprawdź, czy realizacje są w bazie danych
            var dbService = IPlatformApplication.Current.Services.GetRequiredService<SledzSpecke.App.Services.Database.IDatabaseService>();

            try
            {
                // Sprawdź, czy tabela istnieje
                bool tableExists = await dbService.TableExists("RealizedInternshipOldSMK");
                System.Diagnostics.Debug.WriteLine($"Tabela RealizedInternshipOldSMK istnieje: {tableExists}");

                if (!tableExists)
                {
                    System.Diagnostics.Debug.WriteLine("Tworzenie tabeli RealizedInternshipOldSMK...");
                    await dbService.ExecuteAsync(@"
                CREATE TABLE IF NOT EXISTS RealizedInternshipOldSMK (
                    RealizedInternshipId INTEGER PRIMARY KEY AUTOINCREMENT,
                    SpecializationId INTEGER,
                    InternshipName TEXT,
                    InstitutionName TEXT,
                    DepartmentName TEXT,
                    StartDate TEXT,
                    EndDate TEXT,
                    DaysCount INTEGER,
                    IsCompleted INTEGER,
                    IsApproved INTEGER,
                    Year INTEGER,
                    RequiresApproval INTEGER,
                    SupervisorName TEXT,
                    SyncStatus INTEGER,
                    AdditionalFields TEXT
                )");

                    // Odświeżenie tabeli
                    tableExists = await dbService.TableExists("RealizedInternshipOldSMK");
                    System.Diagnostics.Debug.WriteLine($"Tabela utworzona: {tableExists}");
                }

                // Sprawdź liczbę wierszy
                int rowCount = await dbService.GetTableRowCount("RealizedInternshipOldSMK");
                System.Diagnostics.Debug.WriteLine($"Liczba realizacji w bazie: {rowCount}");

                // Sprawdź staże i ich ID
                var internships = await dbService.QueryAsync<SledzSpecke.App.Models.Internship>("SELECT * FROM Internship");

                System.Diagnostics.Debug.WriteLine("DOSTĘPNE STAŻE:");
                foreach (var i in internships)
                {
                    System.Diagnostics.Debug.WriteLine($"ID: {i.InternshipId}, Nazwa: {i.InternshipName}");
                }

                // Sprawdź istniejące realizacje
                var realizacje = await dbService.QueryAsync<SledzSpecke.App.Models.RealizedInternshipOldSMK>("SELECT * FROM RealizedInternshipOldSMK");

                System.Diagnostics.Debug.WriteLine("ISTNIEJĄCE REALIZACJE:");
                foreach (var r in realizacje)
                {
                    System.Diagnostics.Debug.WriteLine($"ID: {r.RealizedInternshipId}, Nazwa: {r.InternshipName}, Dni: {r.DaysCount}");
                }

                // Napraw istniejące realizacje
                await dbService.FixRealizedInternshipNames();

                // Jeśli nie ma wierszy, dodaj testową realizację
                if (rowCount == 0)
                {
                    var user = await IPlatformApplication.Current.Services.GetService<SledzSpecke.App.Services.Authentication.IAuthService>().GetCurrentUserAsync();
                    if (user != null)
                    {
                        var specialization = await dbService.GetSpecializationAsync(user.SpecializationId);
                        if (specialization != null)
                        {
                            // Znajdź pierwsze wymaganie stażu
                            var firstInternship = internships.FirstOrDefault(i => i.InternshipId < 0);
                            string internshipName = "Staż podstawowy w zakresie chorób wewnętrznych";

                            if (firstInternship != null)
                            {
                                internshipName = firstInternship.InternshipName;
                            }

                            // Dodaj testową realizację bezpośrednio do bazy
                            try
                            {
                                await dbService.ExecuteAsync(@"
                            INSERT INTO RealizedInternshipOldSMK 
                            (SpecializationId, InternshipName, InstitutionName, DepartmentName, StartDate, EndDate, DaysCount, IsCompleted, IsApproved, Year, RequiresApproval, SupervisorName, SyncStatus)
                            VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)",
                                    specialization.SpecializationId,
                                    internshipName, // Dokładna nazwa stażu
                                    "Test Instytucja",
                                    "Test Oddział",
                                    DateTime.Today.AddDays(-30).ToString("o"),
                                    DateTime.Today.ToString("o"),
                                    30,
                                    false,
                                    false,
                                    1, // Rok 1
                                    false,
                                    "Test Kierownik",
                                    0); // SyncStatus.NotSynced

                                System.Diagnostics.Debug.WriteLine("Dodano testową realizację stażu");
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine($"Błąd podczas dodawania testowej realizacji: {ex.Message}");
                            }

                            // Sprawdź czy dodano
                            rowCount = await dbService.GetTableRowCount("RealizedInternshipOldSMK");
                            System.Diagnostics.Debug.WriteLine($"Liczba realizacji po dodaniu: {rowCount}");

                            if (rowCount > 0)
                            {
                                var testResults = await dbService.QueryAsync<RealizedInternshipOldSMK>("SELECT * FROM RealizedInternshipOldSMK");
                                foreach (var r in testResults)
                                {
                                    System.Diagnostics.Debug.WriteLine($"Realizacja: {r.InternshipName}, Dni: {r.DaysCount}, Rok: {r.Year}, SpecId: {r.SpecializationId}");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas debugowania: {ex.Message}");
            }

            // Ważne - odśwież widok po potencjalnym dodaniu realizacji
            this.viewModel.RefreshCommand.Execute(null);
        }

        private async void OnEditButtonClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.BindingContext is RealizedInternshipOldSMK realization)
            {
                var navigationParameter = new Dictionary<string, object>
                {
                    { "RealizedInternshipId", realization.RealizedInternshipId.ToString() },
                    { "Year", realization.Year.ToString() }
                };

                // Znajdź wymaganie stażowe o tej samej nazwie
                var specializationService = IPlatformApplication.Current.Services.GetRequiredService<SledzSpecke.App.Services.Specialization.ISpecializationService>();
                var internships = await specializationService.GetInternshipsAsync(null);
                var requirement = internships.FirstOrDefault(i => i.InternshipName == realization.InternshipName);

                if (requirement != null)
                {
                    navigationParameter.Add("InternshipRequirementId", requirement.InternshipId.ToString());
                }
                else
                {
                    // Jeśli nie znaleziono, wyświetl komunikat
                    await this.DisplayAlert("Uwaga",
                        "Nie znaleziono wymagania stażowego dla tej realizacji. Edycja może być niekompletna.",
                        "OK");

                    // Użyj pierwszego dostępnego wymagania
                    if (internships.Count > 0)
                    {
                        navigationParameter.Add("InternshipRequirementId", internships[0].InternshipId.ToString());
                    }
                    else
                    {
                        await this.DisplayAlert("Błąd",
                            "Brak wymagań stażowych. Nie można edytować realizacji.",
                            "OK");
                        return;
                    }
                }

                await Shell.Current.GoToAsync("//AddEditRealizedInternship", navigationParameter);
            }
        }

        private async void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.BindingContext is RealizedInternshipOldSMK realization)
            {
                bool confirm = await this.DisplayAlert(
                    "Potwierdzenie",
                    "Czy na pewno chcesz usunąć tę realizację stażu?",
                    "Tak",
                    "Nie");

                if (confirm)
                {
                    var specializationService = IPlatformApplication.Current.Services.GetRequiredService<SledzSpecke.App.Services.Specialization.ISpecializationService>();
                    bool success = await specializationService.DeleteRealizedInternshipOldSMKAsync(realization.RealizedInternshipId);

                    if (success)
                    {
                        await this.DisplayAlert("Sukces", "Realizacja stażu została usunięta.", "OK");
                        this.viewModel.RefreshCommand.Execute(null);
                    }
                    else
                    {
                        await this.DisplayAlert("Błąd", "Nie udało się usunąć realizacji stażu.", "OK");
                    }
                }
            }
        }
    }
}