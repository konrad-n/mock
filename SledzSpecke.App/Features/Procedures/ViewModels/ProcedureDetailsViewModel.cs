using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using SledzSpecke.App.Common.ViewModels;
using SledzSpecke.Core.Models;
using SledzSpecke.Core.Models.Enums;

namespace SledzSpecke.App.Features.Procedures.ViewModels
{
    public partial class ProcedureDetailsViewModel : ViewModelBase
    {
        private MedicalProcedure procedure;
        private Func<MedicalProcedure, Task> onSaveCallback;
        private List<Internship> internships;

        [ObservableProperty]
        private string pageTitle;

        [ObservableProperty]
        private string requiredCount;

        [ObservableProperty]
        private string completedCount;

        [ObservableProperty]
        private string notes;

        [ObservableProperty]
        private int procedureTypePickerSelectedIndex;

        [ObservableProperty]
        private int modulePickerSelectedIndex;

        [ObservableProperty]
        private int internshipPickerSelectedIndex;

        [ObservableProperty]
        private ObservableCollection<string> internshipItems = new();

        [ObservableProperty]
        private List<Internship> filteredInternships = new();

        public MedicalProcedure Procedure => this.procedure;

        public ProcedureDetailsViewModel(ILogger<ProcedureDetailsViewModel> logger) : base(logger)
        {
            this.Title = "Szczególy procedury";
        }

        public void Initialize(
            MedicalProcedure procedure,
            ModuleType currentModule,
            ProcedureType currentProcedureType,
            Func<MedicalProcedure, Task> onSaveCallback,
            List<Internship> internships)
        {
            this.procedure = procedure ?? new MedicalProcedure();
            this.onSaveCallback = onSaveCallback;
            this.internships = internships;

            if (procedure == null)
            {
                this.procedure = new MedicalProcedure
                {
                    Id = new Random().Next(1000, 9999),
                    Module = currentModule,
                    ProcedureType = currentProcedureType,
                    CompletedCount = 0,
                };
                this.PageTitle = "Dodaj procedure";
                this.RequiredCount = "1";
                this.CompletedCount = "0";
            }
            else
            {
                this.PageTitle = "Szczególy procedury";
                this.RequiredCount = procedure.RequiredCount.ToString();
                this.CompletedCount = procedure.CompletedCount.ToString();
                this.Notes = procedure.Description;
            }

            this.ProcedureTypePickerSelectedIndex = this.procedure.ProcedureType == ProcedureType.TypeA ? 0 : 1;
            this.ModulePickerSelectedIndex = this.procedure.Module == ModuleType.Basic ? 0 : 1;
            this.LoadInternships(this.procedure.Module);
        }

        [RelayCommand]
        public void UpdateProcedureType(int selectedIndex)
        {
            if (this.procedure != null)
            {
                this.procedure.ProcedureType = selectedIndex == 0 ? ProcedureType.TypeA : ProcedureType.TypeB;
            }
        }

        [RelayCommand]
        public void UpdateModule(int selectedIndex)
        {
            if (this.procedure != null)
            {
                this.procedure.Module = selectedIndex == 0 ? ModuleType.Basic : ModuleType.Specialistic;
                int? currentInternshipId = this.procedure.InternshipId;
                this.LoadInternships(this.procedure.Module);

                if (currentInternshipId.HasValue)
                {
                    int index = this.FilteredInternships.FindIndex(i => i.Id == currentInternshipId.Value);
                    if (index >= 0)
                    {
                        this.InternshipPickerSelectedIndex = index;
                        this.procedure.InternshipId = currentInternshipId;
                    }
                }
            }
        }

        [RelayCommand]
        public void UpdateInternship(int selectedIndex)
        {
            if (this.procedure != null && selectedIndex >= 0 && selectedIndex < this.FilteredInternships.Count)
            {
                this.procedure.InternshipId = this.FilteredInternships[selectedIndex].Id;
            }
            else if (this.procedure != null)
            {
                this.procedure.InternshipId = null;
            }
        }

        [RelayCommand]
        private async Task CancelAsync()
        {
            await Shell.Current.Navigation.PopAsync();
        }

        [RelayCommand]
        private async Task SaveAsync()
        {
            if (string.IsNullOrWhiteSpace(this.procedure.Name))
            {
                await Application.Current.MainPage.DisplayAlert("Blad", "Nazwa procedury jest wymagana.", "OK");
                return;
            }

            if (!int.TryParse(this.RequiredCount, out int requiredCount) || requiredCount <= 0)
            {
                await Application.Current.MainPage.DisplayAlert("Blad", "Wprowadz poprawna wymagana liczbe procedur.", "OK");
                return;
            }

            if (this.InternshipPickerSelectedIndex < 0 ||
                this.InternshipPickerSelectedIndex >= this.FilteredInternships.Count)
            {
                await Application.Current.MainPage.DisplayAlert("Blad", "Wybierz staz, w ramach którego wykonywana jest procedura.", "OK");
                return;
            }

            this.procedure.RequiredCount = requiredCount;
            this.procedure.Description = this.Notes;
            this.procedure.InternshipId = this.FilteredInternships[this.InternshipPickerSelectedIndex].Id;

            if (this.onSaveCallback != null)
            {
                await this.onSaveCallback(this.procedure);
            }

            await Shell.Current.Navigation.PopAsync();
        }

        private void LoadInternships(ModuleType moduleType)
        {
            this.InternshipItems.Clear();
            this.FilteredInternships = this.internships.Where(i => i.Module == moduleType).ToList();

            foreach (var internship in this.FilteredInternships)
            {
                this.InternshipItems.Add(internship.Name);
            }

            if (this.procedure.InternshipId.HasValue)
            {
                int index = this.FilteredInternships.FindIndex(i => i.Id == this.procedure.InternshipId);
                if (index >= 0)
                {
                    this.InternshipPickerSelectedIndex = index;
                }
                else
                {
                    this.InternshipPickerSelectedIndex = -1;
                    this.procedure.InternshipId = null;
                }
            }
            else
            {
                this.InternshipPickerSelectedIndex = -1;
            }
        }
    }
}
