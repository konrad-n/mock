using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using SledzSpecke.App.Common.ViewModels;
using SledzSpecke.Core.Models;
using SledzSpecke.Core.Models.Enums;
using SledzSpecke.Infrastructure.Database;

namespace SledzSpecke.App.Features.Absences.ViewModels
{
    public partial class AbsenceDetailsViewModel : ViewModelBase
    {
        private readonly IDatabaseService databaseService;
        private Action<Absence>? onSaveCallback;
        private Absence? absence;
        private string pageTitle = string.Empty;
        private bool isApproved;
        private int absenceTypeSelectedIndex;
        private bool isExistingAbsence;
        private DateTime startDate = DateTime.Now;
        private DateTime endDate = DateTime.Now;
        private string durationDays = string.Empty;
        private string description = string.Empty;
        private bool affectsSpecializationLength;
        private string documentReference = string.Empty;
        private string year = string.Empty;

        public AbsenceDetailsViewModel(
            IDatabaseService databaseService,
            ILogger<AbsenceDetailsViewModel> logger)
            : base(logger)
        {
            this.databaseService = databaseService;
            this.Title = "Nieobecnosc";
        }

        public string PageTitle
        {
            get => this.pageTitle;
            set => this.SetProperty(ref this.pageTitle, value);
        }

        public bool IsApproved
        {
            get => this.isApproved;
            set => this.SetProperty(ref this.isApproved, value);
        }

        public int AbsenceTypeSelectedIndex
        {
            get => this.absenceTypeSelectedIndex;
            set => this.SetProperty(ref this.absenceTypeSelectedIndex, value);
        }

        public bool IsExistingAbsence
        {
            get => this.isExistingAbsence;
            set => this.SetProperty(ref this.isExistingAbsence, value);
        }

        public DateTime StartDate
        {
            get => this.startDate;
            set => this.SetProperty(ref this.startDate, value);
        }

        public DateTime EndDate
        {
            get => this.endDate;
            set => this.SetProperty(ref this.endDate, value);
        }

        public string DurationDays
        {
            get => this.durationDays;
            set => this.SetProperty(ref this.durationDays, value);
        }

        public string Description
        {
            get => this.description;
            set => this.SetProperty(ref this.description, value);
        }

        public bool AffectsSpecializationLength
        {
            get => this.affectsSpecializationLength;
            set => this.SetProperty(ref this.affectsSpecializationLength, value);
        }

        public string DocumentReference
        {
            get => this.documentReference;
            set => this.SetProperty(ref this.documentReference, value);
        }

        public string Year
        {
            get => this.year;
            set => this.SetProperty(ref this.year, value);
        }

        public void Initialize(Absence? absenceParam, Action<Absence> saveCallback)
        {
            this.onSaveCallback = saveCallback;

            if (absenceParam == null)
            {
                this.absence = new Absence
                {
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now,
                    Type = AbsenceType.SickLeave,
                    Year = DateTime.Now.Year,
                    AffectsSpecializationLength = true,
                };
                this.PageTitle = "Dodaj nieobecnosc";
                this.IsExistingAbsence = false;
                this.AbsenceTypeSelectedIndex = 0;
                this.Year = DateTime.Now.Year.ToString();
                this.CalculateDuration();
            }
            else
            {
                this.absence = absenceParam;
                this.PageTitle = "Edytuj nieobecnosc";
                this.IsExistingAbsence = true;
                this.StartDate = absenceParam.StartDate;
                this.EndDate = absenceParam.EndDate;
                this.DurationDays = absenceParam.DurationDays.ToString();
                this.Description = absenceParam.Description ?? string.Empty;
                this.AffectsSpecializationLength = absenceParam.AffectsSpecializationLength;
                this.DocumentReference = absenceParam.DocumentReference ?? string.Empty;
                this.Year = absenceParam.Year.ToString();
                this.IsApproved = absenceParam.IsApproved;
                this.AbsenceTypeSelectedIndex = (int)absenceParam.Type;
            }
        }

        public void CalculateDuration()
        {
            if (this.EndDate >= this.StartDate)
            {
                int days = (this.EndDate - this.StartDate).Days + 1;
                this.DurationDays = days.ToString();
            }
            else
            {
                this.DurationDays = "0";
            }
        }

        [RelayCommand]
        private static async Task CancelAsync()
        {
            await Shell.Current.Navigation.PopAsync();
        }

        [RelayCommand]
        private void UpdateAbsenceType(int index)
        {
            if (this.absence == null)
            {
                return;
            }

            this.absence.Type = (AbsenceType)index;

            switch (this.absence.Type)
            {
                case AbsenceType.SelfEducationLeave:
                    this.AffectsSpecializationLength = false;
                    break;
                case AbsenceType.SickLeave:
                case AbsenceType.MaternityLeave:
                case AbsenceType.ParentalLeave:
                    this.AffectsSpecializationLength = true;
                    break;
                case AbsenceType.VacationLeave:
                    this.AffectsSpecializationLength = false;
                    break;
            }
        }

        [RelayCommand]
        private async Task DeleteAsync()
        {
            if (!this.IsExistingAbsence || this.absence == null)
            {
                return;
            }

            var window = Application.Current?.Windows[0];
            var page = window?.Page;

            if (page == null)
            {
                return;
            }

            bool confirm = await page.DisplayAlert(
                "Potwierdzenie",
                "Czy na pewno chcesz usunac te nieobecnosc?",
                "Tak",
                "Nie");

            if (confirm)
            {
                try
                {
                    await this.databaseService.DeleteAsync(this.absence);
                    await Shell.Current.Navigation.PopAsync();
                }
                catch (Exception ex)
                {
                    this.logger.LogError(ex, "Error deleting absence");
                    await page.DisplayAlert(
                        "Blad",
                        $"Nie udalo sie usunac nieobecnosci: {ex.Message}",
                        "OK");
                }
            }
        }

        [RelayCommand]
        private async Task SaveAsync()
        {
            var window = Application.Current?.Windows[0];
            var page = window?.Page;

            if (page == null)
            {
                return;
            }

            if (this.StartDate > this.EndDate)
            {
                await page.DisplayAlert(
                    "Blad",
                    "Data zakonczenia musi byc pózniejsza lub równa dacie rozpoczecia.",
                    "OK");
                return;
            }

            if (!int.TryParse(this.Year, out int yearValue))
            {
                await page.DisplayAlert(
                    "Blad",
                    "Wprowadz poprawny rok.",
                    "OK");
                return;
            }

            var newAbsence = new Absence
            {
                StartDate = this.StartDate,
                EndDate = this.EndDate,
                DurationDays = int.Parse(this.DurationDays),
                Description = this.Description,
                AffectsSpecializationLength = this.AffectsSpecializationLength,
                DocumentReference = this.DocumentReference,
                Year = yearValue,
                IsApproved = this.IsApproved,
                Type = (AbsenceType)this.AbsenceTypeSelectedIndex,
            };

            if (this.absence != null && this.IsExistingAbsence)
            {
                newAbsence.Id = this.absence.Id;
                newAbsence.SpecializationId = this.absence.SpecializationId;
            }

            this.onSaveCallback?.Invoke(newAbsence);

            await Shell.Current.Navigation.PopAsync();
        }
    }
}
