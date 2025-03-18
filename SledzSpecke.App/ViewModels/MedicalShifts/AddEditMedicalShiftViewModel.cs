using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Database;
using SledzSpecke.App.Services.Dialog;
using SledzSpecke.App.Services.Specialization;
using SledzSpecke.App.ViewModels.Base;

namespace SledzSpecke.App.ViewModels.MedicalShifts
{
    public class AddEditMedicalShiftViewModel : BaseViewModel
    {
        private readonly ISpecializationService specializationService;
        private readonly IDatabaseService databaseService;
        private readonly IDialogService dialogService;

        private bool isEdit;
        private MedicalShift shift;
        private Internship selectedInternship;
        private ObservableCollection<Internship> availableInternships;
        private ObservableCollection<KeyValuePair<string, string>> yearOptions;
        private string selectedYear;

        public AddEditMedicalShiftViewModel(
            ISpecializationService specializationService,
            IDatabaseService databaseService,
            IDialogService dialogService)
        {
            this.specializationService = specializationService;
            this.databaseService = databaseService;
            this.dialogService = dialogService;

            this.AvailableInternships = new ObservableCollection<Internship>();
            this.YearOptions = new ObservableCollection<KeyValuePair<string, string>>();
            this.SaveCommand = new AsyncRelayCommand(this.OnSaveAsync, this.CanSave);
            this.CancelCommand = new AsyncRelayCommand(this.OnCancelAsync);

            this.shift = new MedicalShift
            {
                Date = DateTime.Now,
                Hours = 10,
                Minutes = 5,
                Year = 1,
                SyncStatus = SyncStatus.NotSynced
            };
        }

        public bool IsEdit
        {
            get => this.isEdit;
            set => this.SetProperty(ref this.isEdit, value);
        }

        public MedicalShift Shift
        {
            get => this.shift;
            set => this.SetProperty(ref this.shift, value);
        }

        public ObservableCollection<Internship> AvailableInternships
        {
            get => this.availableInternships;
            set => this.SetProperty(ref this.availableInternships, value);
        }

        public Internship SelectedInternship
        {
            get => this.selectedInternship;
            set
            {
                if (this.SetProperty(ref this.selectedInternship, value) && value != null)
                {
                    this.Shift.InternshipId = value.InternshipId;
                    ((AsyncRelayCommand)this.SaveCommand).NotifyCanExecuteChanged();
                }
            }
        }

        public ObservableCollection<KeyValuePair<string, string>> YearOptions
        {
            get => this.yearOptions;
            set => this.SetProperty(ref this.yearOptions, value);
        }

        public string SelectedYear
        {
            get => this.selectedYear;
            set
            {
                if (this.SetProperty(ref this.selectedYear, value) && !string.IsNullOrEmpty(value))
                {
                    this.Shift.Year = int.Parse(value);
                    ((AsyncRelayCommand)this.SaveCommand).NotifyCanExecuteChanged();
                }
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public async Task InitializeAsync(int? shiftId = null)
        {
            if (this.IsBusy)
            {
                return;
            }

            this.IsBusy = true;

            this.IsEdit = shiftId.HasValue && shiftId.Value > 0;
            this.Title = this.IsEdit ? "Edytuj dyżur" : "Dodaj nowy dyżur";

            await this.LoadYearOptionsAsync();
            await this.LoadInternshipsAsync();

            if (this.IsEdit && shiftId.HasValue)
            {
                var existingShift = await this.databaseService.GetMedicalShiftAsync(shiftId.Value);
                if (existingShift != null)
                {
                    this.Shift = existingShift;
                    this.SelectedInternship = this.AvailableInternships.FirstOrDefault(i => i.InternshipId == existingShift.InternshipId);
                    this.SelectedYear = existingShift.Year.ToString();
                }
            }
            else
            {
                if (this.AvailableInternships.Count > 0)
                {
                    this.SelectedInternship = this.AvailableInternships[0];
                }
                this.SelectedYear = "1";
            }
            this.IsBusy = false;
        }

        private async Task LoadYearOptionsAsync()
        {
            var specialization = await this.specializationService.GetCurrentSpecializationAsync();

            for (int i = 1; i <= specialization.DurationYears; i++)
            {
                this.YearOptions.Add(new KeyValuePair<string, string>(i.ToString(), $"Rok {i}"));
            }
        }

        private async Task LoadInternshipsAsync()
        {
            var currentModule = await this.specializationService.GetCurrentModuleAsync();
            var internships = await this.specializationService.GetInternshipsAsync(moduleId: currentModule?.ModuleId);
            var userInternships = internships.Where(i => i.InternshipId > 0).ToList();
            this.AvailableInternships.Clear();
            foreach (var internship in userInternships)
            {
                this.AvailableInternships.Add(internship);
            }
        }

        private bool CanSave()
        {
            return this.Shift != null
                && this.Shift.Hours > 0
                && this.SelectedInternship != null;
        }

        private async Task OnSaveAsync()
        {
            if (this.IsBusy)
            {
                return;
            }

            this.IsBusy = true;

            this.Shift.InternshipId = this.SelectedInternship.InternshipId;

            bool success;
            if (this.IsEdit)
            {
                success = await this.specializationService.UpdateMedicalShiftAsync(this.Shift);
            }
            else
            {
                success = await this.specializationService.AddMedicalShiftAsync(this.Shift);
            }

            if (success)
            {
                await this.dialogService.DisplayAlertAsync(
                    "Sukces",
                    this.IsEdit ? "Dyżur został zaktualizowany." : "Dyżur został dodany.",
                    "OK");

                await Shell.Current.GoToAsync("..");
            }
            else
            {
                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    "Nie udało się zapisać dyżuru. Sprawdź poprawność danych.",
                    "OK");
            }

            this.IsBusy = false;
        }

        private async Task OnCancelAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}