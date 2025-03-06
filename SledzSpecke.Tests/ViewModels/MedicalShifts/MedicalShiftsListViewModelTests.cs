using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using NSubstitute;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Database;
using SledzSpecke.App.Services.Dialog;
using SledzSpecke.App.Services.Specialization;
using SledzSpecke.App.ViewModels.MedicalShifts;

namespace SledzSpecke.Tests.ViewModels.MedicalShifts
{
    [TestFixture]
    public class MedicalShiftsListViewModelTests
    {
        private MedicalShiftsListViewModel viewModel;
        private ISpecializationService specializationService;
        private IDatabaseService databaseService;
        private IDialogService dialogService;

        private Internship testInternship;
        private List<MedicalShift> testShifts;

        [SetUp]
        public void Setup()
        {
            // Inicjalizacja mocków serwisów
            this.specializationService = Substitute.For<ISpecializationService>();
            this.databaseService = Substitute.For<IDatabaseService>();
            this.dialogService = Substitute.For<IDialogService>();

            // Konfiguracja danych testowych
            this.SetupTestData();

            // Konfiguracja zachowania mocków
            this.SetupMockBehavior();

            // Inicjalizacja ViewModel-a
            this.viewModel = new MedicalShiftsListViewModel(
                this.specializationService,
                this.databaseService,
                this.dialogService);
        }

        private void SetupTestData()
        {
            // Przygotowanie danych testowych
            this.testInternship = new Internship
            {
                InternshipId = 1,
                SpecializationId = 1,
                InstitutionName = "Szpital Kliniczny nr 1",
                DepartmentName = "Oddział Kardiologii",
                InternshipName = "Staż podstawowy z kardiologii",
            };

            this.testShifts = new List<MedicalShift>
            {
                new MedicalShift
                {
                    ShiftId = 1,
                    InternshipId = 1,
                    Date = new DateTime(2023, 5, 15, 0, 0, 0, DateTimeKind.Local),
                    Hours = 12,
                    Minutes = 30,
                    Location = "Oddział Kardiologii",
                    Year = 1,
                    SyncStatus = SyncStatus.NotSynced,
                },
                new MedicalShift
                {
                    ShiftId = 2,
                    InternshipId = 1,
                    Date = new DateTime(2023, 5, 20, 0, 0, 0, DateTimeKind.Local),
                    Hours = 10,
                    Minutes = 0,
                    Location = "Oddział Kardiologii",
                    Year = 1,
                    SyncStatus = SyncStatus.Synced,
                },
                new MedicalShift
                {
                    ShiftId = 3,
                    InternshipId = 2,
                    Date = new DateTime(2023, 5, 25, 0, 0, 0, DateTimeKind.Local),
                    Hours = 8,
                    Minutes = 15,
                    Location = "Oddział Intensywnej Terapii",
                    Year = 1,
                    SyncStatus = SyncStatus.NotSynced,
                },
            };
        }

        private void SetupMockBehavior()
        {
            // Konfiguracja zachowania serwisu specjalizacji
            this.specializationService.GetCurrentInternshipAsync()
                .Returns(this.testInternship);

            // Konfiguracja zachowania serwisu bazy danych
            this.databaseService.GetMedicalShiftsAsync(Arg.Any<int?>())
                .Returns(callInfo =>
                {
                    int? internshipId = callInfo.Arg<int?>();
                    if (internshipId.HasValue)
                    {
                        return this.testShifts
                            .Where(s => s.InternshipId == internshipId.Value)
                            .ToList();
                    }
                    return this.testShifts;
                });
        }

        [Test]
        public void Constructor_InitializesPropertiesAndCommands()
        {
            // Assert
            Assert.That(this.viewModel.Title, Is.EqualTo("Dyżury medyczne"));
            Assert.That(this.viewModel.Shifts, Is.Not.Null);
            Assert.That(this.viewModel.RefreshCommand, Is.Not.Null);
            Assert.That(this.viewModel.FilterShiftsCommand, Is.Not.Null);
            Assert.That(this.viewModel.FilterCommand, Is.Not.Null);
            Assert.That(this.viewModel.ShiftSelectedCommand, Is.Not.Null);
            Assert.That(this.viewModel.AddShiftCommand, Is.Not.Null);
        }

        [Test]
        public async Task FilterShiftsCommand_SetsFilterState_ForAllShifts()
        {
            // Act
            await this.InvokeCommandAsync(this.viewModel.FilterShiftsCommand, "All");

            // Assert
            Assert.That(this.viewModel.AllShiftsSelected, Is.True);
            Assert.That(this.viewModel.CurrentInternshipSelected, Is.False);
        }

        [Test]
        public async Task FilterShiftsCommand_SetsFilterState_ForCurrentInternship()
        {
            // Act
            await this.InvokeCommandAsync(this.viewModel.FilterShiftsCommand, "Current");

            // Assert
            Assert.That(this.viewModel.AllShiftsSelected, Is.False);
            Assert.That(this.viewModel.CurrentInternshipSelected, Is.True);
        }

        [Test]
        public void IsBusy_ControlsRefreshVisibility()
        {
            // Arrange & Act
            this.viewModel.IsBusy = true;

            // Assert
            Assert.That(this.viewModel.IsBusy, Is.True);

            // Act again
            this.viewModel.IsBusy = false;

            // Assert again
            Assert.That(this.viewModel.IsBusy, Is.False);
        }

        // Metoda pomocnicza do wywoływania komend asynchronicznych
        private async Task InvokeCommandAsync(ICommand command, object parameter = null)
        {
            if (command is AsyncRelayCommand asyncCommand)
            {
                await asyncCommand.ExecuteAsync(parameter);
            }
            else if (command is AsyncRelayCommand<string> asyncCommandWithParam && parameter is string stringParam)
            {
                await asyncCommandWithParam.ExecuteAsync(stringParam);
            }
            else
            {
                command.Execute(parameter);
            }
        }
    }
}