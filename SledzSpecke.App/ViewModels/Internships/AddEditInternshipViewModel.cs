using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Dialog;
using SledzSpecke.App.Services.Specialization;
using SledzSpecke.App.ViewModels.Base;

namespace SledzSpecke.App.ViewModels.Internships
{
    [QueryProperty(nameof(InternshipId), nameof(InternshipId))]
    [QueryProperty(nameof(InternshipRequirementId), nameof(InternshipRequirementId))]
    [QueryProperty(nameof(ModuleId), nameof(ModuleId))]
    public class AddEditInternshipViewModel : BaseViewModel
    {
        private readonly ISpecializationService specializationService;
        private readonly IDialogService dialogService;

        private bool isEdit;
        private Internship internship;
        private int internshipRequirementId;
        private int moduleId;

        public AddEditInternshipViewModel(
            ISpecializationService specializationService,
            IDialogService dialogService)
        {
            this.specializationService = specializationService;
            this.dialogService = dialogService;

            this.internship = new Internship
            {
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(30),
                DaysCount = 30,
                Year = 1,
                SyncStatus = SyncStatus.NotSynced
            };

            this.SaveCommand = new AsyncRelayCommand(this.SaveAsync);
            this.CancelCommand = new AsyncRelayCommand(this.CancelAsync);
        }

        public string InternshipId
        {
            set
            {
                if (!string.IsNullOrEmpty(value) && int.TryParse(value, out int id) && id > 0)
                {
                    this.LoadInternshipAsync(id).ConfigureAwait(false);
                }
                else
                {
                    this.IsEdit = false;
                    this.Title = "Dodaj staż";
                }
            }
        }

        public string InternshipRequirementId
        {
            set
            {
                if (!string.IsNullOrEmpty(value) && int.TryParse(value, out int id) && id > 0)
                {
                    this.internshipRequirementId = id;
                    this.LoadInternshipRequirementAsync(this.internshipRequirementId).ConfigureAwait(false);
                }
            }
        }

        public string ModuleId
        {
            set
            {
                if (!string.IsNullOrEmpty(value) && int.TryParse(value, out int id) && id > 0)
                {
                    this.moduleId = id;
                    this.internship.ModuleId = id;
                }
            }
        }

        public bool IsEdit
        {
            get => this.isEdit;
            set => this.SetProperty(ref this.isEdit, value);
        }

        public Internship Internship
        {
            get => this.internship;
            set => this.SetProperty(ref this.internship, value);
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public async Task InitializeAsync()
        {
            if (this.IsBusy)
            {
                return;
            }

            this.IsBusy = true;

            // Dodatkowy kod inicjalizujący, jeśli potrzebny

            this.IsBusy = false;
        }

        private async Task LoadInternshipAsync(int internshipId)
        {
            if (this.IsBusy)
            {
                return;
            }

            this.IsBusy = true;

            var loadedInternship = await this.specializationService.GetInternshipAsync(internshipId);
            if (loadedInternship != null)
            {
                this.IsEdit = true;
                this.Title = "Edytuj staż";
                this.Internship = loadedInternship;
            }
            else
            {
                this.IsEdit = false;
                this.Title = "Dodaj staż";
                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    "Nie znaleziono stażu o podanym identyfikatorze.",
                    "OK");
            }

            this.IsBusy = false;
        }

        private async Task LoadInternshipRequirementAsync(int requirementId)
        {
            if (this.IsBusy)
            {
                return;
            }

            this.IsBusy = true;

            var currentModule = await this.specializationService.GetCurrentModuleAsync();
            if (currentModule != null)
            {
                var internships = await this.specializationService.GetInternshipsAsync(currentModule.ModuleId);
                var requirement = internships.FirstOrDefault(i => i.InternshipId == requirementId);

                if (requirement != null)
                {
                    this.Internship.InternshipName = requirement.InternshipName;
                    this.Internship.DaysCount = requirement.DaysCount;
                    this.Internship.EndDate = this.Internship.StartDate.AddDays(requirement.DaysCount - 1);
                }
            }

            this.IsBusy = false;
        }

        private async Task SaveAsync()
        {
            if (this.IsBusy)
            {
                return;
            }

            this.IsBusy = true;

            if (string.IsNullOrWhiteSpace(this.Internship.InstitutionName))
            {
                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    "Nazwa placówki realizującej szkolenie jest wymagana.",
                    "OK");
                this.IsBusy = false;
                return;
            }

            if (string.IsNullOrWhiteSpace(this.Internship.DepartmentName))
            {
                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    "Nazwa komórki organizacyjnej jest wymagana.",
                    "OK");
                this.IsBusy = false;
                return;
            }

            if (this.Internship.DaysCount <= 0)
            {
                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    "Liczba dni musi być większa od zera.",
                    "OK");
                this.IsBusy = false;
                return;
            }

            if (this.Internship.EndDate < this.Internship.StartDate)
            {
                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    "Data zakończenia nie może być wcześniejsza niż data rozpoczęcia.",
                    "OK");
                this.IsBusy = false;
                return;
            }

            // Obliczanie liczby dni między datami
            TimeSpan duration = this.Internship.EndDate - this.Internship.StartDate;
            this.Internship.DaysCount = duration.Days + 1;

            bool success;
            if (this.IsEdit)
            {
                success = await this.specializationService.UpdateInternshipAsync(this.Internship);
            }
            else
            {
                this.Internship.ModuleId = this.moduleId;
                success = await this.specializationService.AddInternshipAsync(this.Internship);
            }

            if (success)
            {
                await this.dialogService.DisplayAlertAsync(
                    "Sukces",
                    this.IsEdit ? "Staż został zaktualizowany." : "Staż został dodany.",
                    "OK");
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    "Nie udało się zapisać stażu. Sprawdź poprawność danych.",
                    "OK");
            }

            this.IsBusy = false;
        }

        private async Task CancelAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}