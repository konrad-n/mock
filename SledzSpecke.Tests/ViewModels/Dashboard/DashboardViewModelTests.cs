using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using NSubstitute;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Database;
using SledzSpecke.App.Services.Dialog;
using SledzSpecke.App.Services.Specialization;
using SledzSpecke.App.ViewModels.Dashboard;

namespace SledzSpecke.Tests.ViewModels.Dashboard
{
    [TestFixture]
    public class DashboardViewModelTests
    {
        private DashboardViewModel viewModel;
        private ISpecializationService specializationService;
        private IDatabaseService databaseService;

        private Specialization testSpecialization;
        private Module testBasicModule;
        private Module testSpecialisticModule;
        private IDialogService dialogService;

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
            this.viewModel = new DashboardViewModel(
                this.specializationService,
                this.databaseService,
                this.dialogService);
        }

        private void SetupTestData()
        {
            // Przygotowanie danych testowych
            this.testSpecialization = new Specialization
            {
                SpecializationId = 1,
                Name = "Kardiologia",
                HasModules = true,
                StartDate = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Local),
                PlannedEndDate = new DateTime(2028, 1, 1, 0, 0, 0, DateTimeKind.Local),
                CalculatedEndDate = new DateTime(2028, 3, 1, 0, 0, 0, DateTimeKind.Local),
                CurrentModuleId = 1,
                TotalInternships = 10,
                CompletedInternships = 5,
                TotalCourses = 8,
                CompletedCourses = 3,
            };

            this.testBasicModule = new Module
            {
                ModuleId = 1,
                SpecializationId = 1,
                Type = ModuleType.Basic,
                Name = "Moduł podstawowy",
                StartDate = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Local),
                EndDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Local),
                TotalInternships = 5,
                CompletedInternships = 3,
                TotalCourses = 4,
                CompletedCourses = 2,
                TotalProceduresA = 100,
                CompletedProceduresA = 50,
                TotalProceduresB = 50,
                CompletedProceduresB = 20,
            };

            this.testSpecialisticModule = new Module
            {
                ModuleId = 2,
                SpecializationId = 1,
                Type = ModuleType.Specialistic,
                Name = "Moduł specjalistyczny",
                StartDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Local),
                EndDate = new DateTime(2028, 1, 1, 0, 0, 0, DateTimeKind.Local),
                TotalInternships = 5,
                CompletedInternships = 2,
                TotalCourses = 4,
                CompletedCourses = 1,
                TotalProceduresA = 150,
                CompletedProceduresA = 30,
                TotalProceduresB = 75,
                CompletedProceduresB = 10,
            };

            // Przypisanie modułów do specjalizacji
            this.testSpecialization.Modules = new List<Module>
            {
                this.testBasicModule,
                this.testSpecialisticModule
            };
        }

        private void SetupMockBehavior()
        {
            // Konfiguracja zachowania serwisu specjalizacji
            this.specializationService.GetCurrentSpecializationAsync()
                .Returns(this.testSpecialization);

            this.specializationService.GetCurrentModuleAsync()
                .Returns(this.testBasicModule);

            this.specializationService.GetInternshipCountAsync(Arg.Any<int?>())
                .Returns(5);

            this.specializationService.GetProcedureCountAsync(Arg.Any<int?>())
                .Returns(70);

            this.specializationService.GetCourseCountAsync(Arg.Any<int?>())
                .Returns(3);

            this.specializationService.GetShiftCountAsync(Arg.Any<int?>())
                .Returns(120);

            this.specializationService.GetSelfEducationCountAsync(Arg.Any<int?>())
                .Returns(10);

            this.specializationService.GetPublicationCountAsync(Arg.Any<int?>())
                .Returns(2);

            // Konfiguracja zachowania serwisu bazy danych
            this.databaseService.GetModulesAsync(1)
                .Returns(new List<Module> { this.testBasicModule, this.testSpecialisticModule });

            // Ustawienie domyślnego zachowania dla ProgressCalculator (zastąpione w konkretnych testach)
            // Ze względu na statyczną naturę ProgressCalculator, trudno jest go bezpośrednio mockować
        }

        [Test]
        public void Constructor_InitializesPropertiesAndCommands()
        {
            // Assert
            Assert.That(this.viewModel.RefreshCommand, Is.Not.Null);
            Assert.That(this.viewModel.SelectModuleCommand, Is.Not.Null);
            Assert.That(this.viewModel.NavigateToInternshipsCommand, Is.Not.Null);
            Assert.That(this.viewModel.NavigateToProceduresCommand, Is.Not.Null);
            Assert.That(this.viewModel.NavigateToShiftsCommand, Is.Not.Null);
            Assert.That(this.viewModel.NavigateToCoursesCommand, Is.Not.Null);
            Assert.That(this.viewModel.NavigateToSelfEducationCommand, Is.Not.Null);
            Assert.That(this.viewModel.NavigateToPublicationsCommand, Is.Not.Null);
            Assert.That(this.viewModel.NavigateToAbsencesCommand, Is.Not.Null);
            Assert.That(this.viewModel.NavigateToStatisticsCommand, Is.Not.Null);
            Assert.That(this.viewModel.NavigateToExportCommand, Is.Not.Null);
            Assert.That(this.viewModel.NavigateToRecognitionsCommand, Is.Not.Null);
        }

        [Test]
        public async Task LoadDataAsync_DefaultsToCurrentModule_WhenHasModules()
        {
            // Arrange
            this.viewModel.CurrentModuleId = 0;  // Resetuj ID modułu

            // Act
            await this.InvokeCommandAsync(this.viewModel.RefreshCommand);

            // Assert
            Assert.That(this.viewModel.CurrentModuleId, Is.EqualTo(1));  // Wartość z CurrentModuleId specjalizacji
            Assert.That(this.viewModel.BasicModuleSelected, Is.True);
            Assert.That(this.viewModel.SpecialisticModuleSelected, Is.False);
        }

        [Test]
        public async Task CurrentModuleId_TriggersDataReload_WhenChanged()
        {
            // Arrange
            this.specializationService.ClearReceivedCalls();

            // Act
            this.viewModel.CurrentModuleId = 2;

            // Assert
            await this.specializationService.Received(1).SetCurrentModuleAsync(2);
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