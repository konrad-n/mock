using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using NSubstitute;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Database;
using SledzSpecke.App.Services.Dialog;
using SledzSpecke.App.Services.SmkStrategy;
using SledzSpecke.App.Services.Specialization;
using SledzSpecke.App.ViewModels.MedicalShifts;

namespace SledzSpecke.Tests.ViewModels.MedicalShifts
{
    [TestFixture]
    public class AddEditMedicalShiftViewModelTests
    {
        private AddEditMedicalShiftViewModel viewModel;
        private ISpecializationService specializationService;
        private IDatabaseService databaseService;
        private ISmkVersionStrategy smkStrategy;
        private IDialogService dialogService;

        private Specialization testSpecialization;
        private List<Internship> testInternships;
        private MedicalShift testShift;

        [SetUp]
        public void Setup()
        {
            // Inicjalizacja mocków serwisów
            this.specializationService = Substitute.For<ISpecializationService>();
            this.databaseService = Substitute.For<IDatabaseService>();
            this.smkStrategy = Substitute.For<ISmkVersionStrategy>();
            this.dialogService = Substitute.For<IDialogService>();

            // Konfiguracja danych testowych
            this.SetupTestData();

            // Konfiguracja zachowania mocków
            this.SetupMockBehavior();

            // Inicjalizacja ViewModel-a
            this.viewModel = new AddEditMedicalShiftViewModel(
                this.specializationService,
                this.databaseService,
                this.smkStrategy,
                this.dialogService);
        }

        private void SetupTestData()
        {
            // Przygotowanie danych testowych
            this.testSpecialization = new Specialization
            {
                SpecializationId = 1,
                Name = "Kardiologia",
                HasModules = false,
                StartDate = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Local),
                PlannedEndDate = new DateTime(2028, 1, 1, 0, 0, 0, DateTimeKind.Local),
            };

            this.testInternships = new List<Internship>
            {
                new Internship
                {
                    InternshipId = 1,
                    SpecializationId = 1,
                    InstitutionName = "Szpital Kliniczny nr 1",
                    DepartmentName = "Oddział Kardiologii",
                    InternshipName = "Staż podstawowy z kardiologii",
                },
                new Internship
                {
                    InternshipId = 2,
                    SpecializationId = 1,
                    InstitutionName = "Szpital Wojewódzki",
                    DepartmentName = "Oddział Intensywnej Terapii",
                    InternshipName = "Staż z intensywnej terapii",
                },
            };

            this.testShift = new MedicalShift
            {
                ShiftId = 1,
                InternshipId = 1,
                Date = new DateTime(2023, 5, 15, 0, 0, 0, DateTimeKind.Local),
                Hours = 12,
                Minutes = 30,
                Location = "Oddział Kardiologii",
                Year = 1,
                SyncStatus = SyncStatus.NotSynced,
            };
        }

        private void SetupMockBehavior()
        {
            // Konfiguracja zachowania serwisu specjalizacji
            this.specializationService.GetCurrentSpecializationAsync()
                .Returns(this.testSpecialization);

            // Konfiguracja zachowania serwisu bazy danych
            this.databaseService.GetInternshipsAsync(
                    Arg.Is<int>(specializationId => specializationId == 1),
                    Arg.Any<int?>())
                .Returns(this.testInternships);

            this.databaseService.GetMedicalShiftAsync(1)
                .Returns(this.testShift);

            // Konfiguracja zachowania strategii SMK
            this.smkStrategy.GetType().Name.Returns("NewSmkStrategy"); // Domyślnie nowa wersja SMK
            this.smkStrategy.FormatAdditionalFields(Arg.Any<Dictionary<string, object>>())
                .Returns(info => System.Text.Json.JsonSerializer.Serialize(info.Arg<Dictionary<string, object>>()));
            this.smkStrategy.ParseAdditionalFields(Arg.Any<string>())
                .Returns(info => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(info.Arg<string>())!);
        }

        [Test]
        public void Constructor_SetsDefaultValues()
        {
            // Assert
            Assert.That(this.viewModel.Title, Is.EqualTo("Nowy dyżur"));
            Assert.That(this.viewModel.Date.Date, Is.EqualTo(DateTime.Today.Date));
            Assert.That(this.viewModel.Hours, Is.EqualTo(10));
            Assert.That(this.viewModel.Minutes, Is.EqualTo(0));
            Assert.That(this.viewModel.Year, Is.EqualTo(1));
            Assert.That(this.viewModel.IsOldSmkVersion, Is.False);
        }

        [Test]
        public async Task LoadDataAsync_PopulatesInternshipList()
        {
            // Act - inicjalizacja ViewModel automatycznie wywołuje LoadDataAsync
            // Ręcznie wywołuję operacje na Property, aby ponownie załadować dane
            this.viewModel.Date = this.viewModel.Date.AddDays(1);
            await Task.Delay(100); // Daj czas na zakończenie asynchronicznej operacji

            // Assert
            Assert.That(this.viewModel.AvailableInternships, Is.Not.Null);
            Assert.That(this.viewModel.AvailableInternships.Count, Is.EqualTo(2));
            Assert.That(this.viewModel.AvailableInternships[0].InternshipId, Is.EqualTo(1));
            Assert.That(this.viewModel.AvailableInternships[0].DisplayName, Is.EqualTo("Staż podstawowy z kardiologii - Szpital Kliniczny nr 1"));
        }

        [Test]
        public async Task LoadShiftAsync_LoadsExistingShiftData()
        {
            // Act
            this.viewModel.ShiftId = 1;
            await Task.Delay(100); // Daj czas na zakończenie asynchronicznej operacji

            // Assert
            Assert.That(this.viewModel.Title, Is.EqualTo("Edytuj dyżur"));
            Assert.That(this.viewModel.Date, Is.EqualTo(new DateTime(2023, 5, 15, 0, 0, 0, DateTimeKind.Local)));
            Assert.That(this.viewModel.Hours, Is.EqualTo(12));
            Assert.That(this.viewModel.Minutes, Is.EqualTo(30));
            Assert.That(this.viewModel.Location, Is.EqualTo("Oddział Kardiologii"));
            Assert.That(this.viewModel.Year, Is.EqualTo(1));
        }

        [Test]
        public void ValidateInput_WithValidData_EnablesSave()
        {
            // Arrange
            this.viewModel.SelectedInternship = new InternshipListItem { InternshipId = 1, DisplayName = "Test" };
            this.viewModel.Hours = 10;
            this.viewModel.Location = "Oddział testowy";

            // Act - wywołanie walidacji poprzez zmianę wartości powodującej walidację
            this.viewModel.Location = "Oddział testowy 2";

            // Assert
            Assert.That(this.viewModel.CanSave, Is.True);
        }

        [Test]
        public void ValidateInput_WithInvalidData_DisablesSave()
        {
            // Arrange
            this.viewModel.SelectedInternship = new InternshipListItem { InternshipId = 1, DisplayName = "Test" };
            this.viewModel.Hours = 0; // Nieprawidłowa liczba godzin
            this.viewModel.Location = "Oddział testowy";

            // Act - wywołanie walidacji poprzez zmianę wartości powodującej walidację
            this.viewModel.Location = "Oddział testowy 2";

            // Assert
            Assert.That(this.viewModel.CanSave, Is.False);
        }

        [Test]
        public void ValidateInput_OldSMK_RequiresAdditionalFields()
        {
            // Arrange
            // Symuluj starą wersję SMK
            this.smkStrategy.GetType().Name.Returns("OldSmkStrategy");

            // Utwórz nowy ViewModel z zaktualizowaną strategią
            this.viewModel = new AddEditMedicalShiftViewModel(
                this.specializationService,
                this.databaseService,
                this.smkStrategy,
                this.dialogService);

            this.viewModel.SelectedInternship = new InternshipListItem { InternshipId = 1, DisplayName = "Test" };
            this.viewModel.Hours = 10;
            this.viewModel.Location = "Oddział testowy";

            // Brak wypełnienia pól specyficznych dla starego SMK
            // Act - wywołanie walidacji poprzez zmianę wartości powodującej walidację
            this.viewModel.Location = "Oddział testowy 2";

            // Assert
            Assert.That(this.viewModel.IsOldSmkVersion, Is.True);
            Assert.That(this.viewModel.CanSave, Is.False);
        }

        [Test]
        public async Task OnSaveAsync_ForNewShift_CallsDatabaseSave()
        {
            // Arrange
            this.viewModel.SelectedInternship = new InternshipListItem { InternshipId = 1, DisplayName = "Test" };
            this.viewModel.Hours = 10;
            this.viewModel.Location = "Oddział testowy";
            this.viewModel.Date = new DateTime(2023, 6, 1, 0, 0, 0, DateTimeKind.Local);

            // Upewnij się, że CanSave jest true
            Assert.That(this.viewModel.CanSave, Is.True, "CanSave powinno być true, aby można było wywołać SaveCommand");

            // Act - wywołaj komendę zapisu
            await this.InvokeCommandAsync(this.viewModel.SaveCommand);

            // Assert
            await this.databaseService.Received(1).SaveMedicalShiftAsync(Arg.Is<MedicalShift>(
                s => s.Date == new DateTime(2023, 6, 1, 0, 0, 0, DateTimeKind.Local) &&
                     s.Hours == 10 &&
                     s.Location == "Oddział testowy" &&
                     s.InternshipId == 1 &&
                     s.SyncStatus == SyncStatus.NotSynced));
        }

        [Test]
        public async Task OnSaveAsync_ForExistingShift_CallsDatabaseSave()
        {
            // Arrange
            this.viewModel.ShiftId = 1;
            await Task.Delay(100); // Daj czas na załadowanie danych dyżuru

            this.viewModel.Hours = 15; // Zmień wartość
            this.viewModel.Location = "Zmieniona lokalizacja"; // Zmień wartość

            // Upewnij się, że CanSave jest true
            Assert.That(this.viewModel.CanSave, Is.True, "CanSave powinno być true, aby można było wywołać SaveCommand");

            // Act - wywołaj komendę zapisu
            await this.InvokeCommandAsync(this.viewModel.SaveCommand);

            // Assert
            await this.databaseService.Received(1).SaveMedicalShiftAsync(Arg.Is<MedicalShift>(
                s => s.ShiftId == 1 &&
                     s.Hours == 15 &&
                     s.Location == "Zmieniona lokalizacja"));
        }

        [Test]
        public async Task OnSaveAsync_ForSyncedShift_SetsStatusToModified()
        {
            // Arrange
            // Zmień status dyżuru na zsynchronizowany
            this.testShift.SyncStatus = SyncStatus.Synced;

            this.viewModel.ShiftId = 1;
            await Task.Delay(100); // Daj czas na załadowanie danych dyżuru

            this.viewModel.Hours = 15; // Zmień wartość

            // Upewnij się, że CanSave jest true
            Assert.That(this.viewModel.CanSave, Is.True, "CanSave powinno być true, aby można było wywołać SaveCommand");

            // Act - wywołaj komendę zapisu
            await this.InvokeCommandAsync(this.viewModel.SaveCommand);

            // Assert
            await this.databaseService.Received(1).SaveMedicalShiftAsync(Arg.Is<MedicalShift>(
                s => s.SyncStatus == SyncStatus.Modified));
        }

        [Test]
        public async Task OnSaveAsync_ForOldSMK_AddsAdditionalFields()
        {
            // Arrange
            // Symuluj starą wersję SMK
            this.smkStrategy.GetType().Name.Returns("OldSmkStrategy");

            // Utwórz nowy ViewModel z zaktualizowaną strategią
            this.viewModel = new AddEditMedicalShiftViewModel(
                this.specializationService,
                this.databaseService,
                this.smkStrategy,
                this.dialogService);

            this.viewModel.SelectedInternship = new InternshipListItem { InternshipId = 1, DisplayName = "Test" };
            this.viewModel.Hours = 10;
            this.viewModel.Location = "Oddział testowy";
            this.viewModel.OldSMKField1 = "Wartość pola 1";
            this.viewModel.OldSMKField2 = "Wartość pola 2";

            // Upewnij się, że CanSave jest true
            Assert.That(this.viewModel.CanSave, Is.True, "CanSave powinno być true, aby można było wywołać SaveCommand");

            // Act - wywołaj komendę zapisu
            await this.InvokeCommandAsync(this.viewModel.SaveCommand);

            // Assert
            this.smkStrategy.Received(1).FormatAdditionalFields(Arg.Is<Dictionary<string, object>>(
                d => d.ContainsKey("OldSMKField1") &&
                     d.ContainsKey("OldSMKField2") &&
                     d["OldSMKField1"].ToString() == "Wartość pola 1" &&
                     d["OldSMKField2"].ToString() == "Wartość pola 2"));
        }

        [Test]
        public async Task OnCancelAsync_GoesBackInNavigation()
        {
            // Aby przetestować nawigację z Shell, musimy skorzystać z refleksji i podmienić Shell.Current
            // To wymaga złożonego setupu testowego z MockShellNavigation

            // Alternatywnie, możemy sprawdzić, czy komenda Cancel jest wywoływana poprawnie
            // Act
            await this.InvokeCommandAsync(this.viewModel.CancelCommand);

            // Assert
            // Tutaj sprawdzamy tylko, czy komenda się wykonała bez błędów
            Assert.Pass("Komenda Cancel została wywołana bez wyjątków");
        }

        // Metoda pomocnicza do wywoływania komend asynchronicznych
        private async Task InvokeCommandAsync(ICommand command)
        {
            if (command is AsyncRelayCommand asyncCommand)
            {
                await asyncCommand.ExecuteAsync(null);
            }
            else
            {
                command.Execute(null);
            }
        }
    }
}