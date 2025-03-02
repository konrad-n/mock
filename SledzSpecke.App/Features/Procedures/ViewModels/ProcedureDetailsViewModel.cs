using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using SledzSpecke.App.Common.ViewModels;
using SledzSpecke.Core.Models;
using SledzSpecke.Core.Models.Enums;
using System.Collections.ObjectModel;

namespace SledzSpecke.App.Features.Procedures.ViewModels
{
    public partial class ProcedureDetailsViewModel : ViewModelBase
    {
        private MedicalProcedure _procedure;
        private ModuleType _currentModule;
        private ProcedureType _currentProcedureType;
        private Func<MedicalProcedure, Task> _onSaveCallback;
        private List<Internship> _internships;

        [ObservableProperty]
        private string _pageTitle;

        [ObservableProperty]
        private string _requiredCount;

        [ObservableProperty]
        private string _completedCount;

        [ObservableProperty]
        private string _notes;

        [ObservableProperty]
        private int _procedureTypePickerSelectedIndex;

        [ObservableProperty]
        private int _modulePickerSelectedIndex;

        [ObservableProperty]
        private int _internshipPickerSelectedIndex;

        [ObservableProperty]
        private ObservableCollection<string> _internshipItems = new();

        [ObservableProperty]
        private List<Internship> _filteredInternships = new();

        public MedicalProcedure Procedure => this._procedure;

        public ProcedureDetailsViewModel(ILogger<ProcedureDetailsViewModel> logger) : base(logger)
        {
            this.Title = "Szczegóły procedury";
        }

        public void Initialize(MedicalProcedure procedure, ModuleType currentModule,
            ProcedureType currentProcedureType, Func<MedicalProcedure, Task> onSaveCallback,
            List<Internship> internships)
        {
            this._procedure = procedure ?? new MedicalProcedure();
            this._currentModule = currentModule;
            this._currentProcedureType = currentProcedureType;
            this._onSaveCallback = onSaveCallback;
            this._internships = internships;

            if (procedure == null)
            {
                // Nowa procedura
                this._procedure = new MedicalProcedure
                {
                    Id = new Random().Next(1000, 9999), // Tymczasowe ID
                    Module = currentModule,
                    ProcedureType = currentProcedureType,
                    CompletedCount = 0
                };
                this.PageTitle = "Dodaj procedurę";
                this.RequiredCount = "1";
                this.CompletedCount = "0";
            }
            else
            {
                // Edycja istniejącej procedury
                this.PageTitle = "Szczegóły procedury";
                this.RequiredCount = procedure.RequiredCount.ToString();
                this.CompletedCount = procedure.CompletedCount.ToString();
                this.Notes = procedure.Description;
            }

            // Ustawienie pickerów
            this.ProcedureTypePickerSelectedIndex = this._procedure.ProcedureType == ProcedureType.TypeA ? 0 : 1;
            this.ModulePickerSelectedIndex = this._procedure.Module == ModuleType.Basic ? 0 : 1;

            // Wypełnienie pickera stażów
            this.LoadInternships(this._procedure.Module);
        }

        private void LoadInternships(ModuleType moduleType)
        {
            this.InternshipItems.Clear();
            this.FilteredInternships = this._internships.Where(i => i.Module == moduleType).ToList();

            foreach (var internship in this.FilteredInternships)
            {
                this.InternshipItems.Add(internship.Name);
            }

            // Jeśli edytujemy procedurę, ustawiamy wybrany staż
            if (this._procedure.InternshipId.HasValue)
            {
                int index = this.FilteredInternships.FindIndex(i => i.Id == this._procedure.InternshipId);
                if (index >= 0)
                {
                    this.InternshipPickerSelectedIndex = index;
                }
                else
                {
                    // Staż nie należy do wybranego modułu, więc ustawiamy pusty
                    this.InternshipPickerSelectedIndex = -1;
                    // Czyścimy przypisanie stażu
                    this._procedure.InternshipId = null;
                }
            }
            else
            {
                this.InternshipPickerSelectedIndex = -1;
            }
        }

        [RelayCommand]
        public void UpdateProcedureType(int selectedIndex)
        {
            if (this._procedure != null)
            {
                this._procedure.ProcedureType = selectedIndex == 0 ? ProcedureType.TypeA : ProcedureType.TypeB;
            }
        }

        [RelayCommand]
        public void UpdateModule(int selectedIndex)
        {
            if (this._procedure != null)
            {
                this._procedure.Module = selectedIndex == 0 ? ModuleType.Basic : ModuleType.Specialistic;

                // Przy zmianie modułu zapamiętujemy aktualny identyfikator stażu
                int? currentInternshipId = this._procedure.InternshipId;

                // Ładujemy nową listę stażów
                this.LoadInternships(this._procedure.Module);

                // Jeśli staż należy do nowego modułu, ustawiamy go ponownie
                if (currentInternshipId.HasValue)
                {
                    int index = this.FilteredInternships.FindIndex(i => i.Id == currentInternshipId.Value);
                    if (index >= 0)
                    {
                        this.InternshipPickerSelectedIndex = index;
                        this._procedure.InternshipId = currentInternshipId;
                    }
                }
            }
        }

        [RelayCommand]
        public void UpdateInternship(int selectedIndex)
        {
            if (this._procedure != null && selectedIndex >= 0 && selectedIndex < this.FilteredInternships.Count)
            {
                this._procedure.InternshipId = this.FilteredInternships[selectedIndex].Id;
            }
            else if (this._procedure != null)
            {
                // Brak wyboru stażu
                this._procedure.InternshipId = null;
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
            // Walidacja
            if (string.IsNullOrWhiteSpace(this._procedure.Name))
            {
                await Application.Current.MainPage.DisplayAlert("Błąd", "Nazwa procedury jest wymagana.", "OK");
                return;
            }

            if (!int.TryParse(this.RequiredCount, out int requiredCount) || requiredCount <= 0)
            {
                await Application.Current.MainPage.DisplayAlert("Błąd", "Wprowadź poprawną wymaganą liczbę procedur.", "OK");
                return;
            }

            if (this.InternshipPickerSelectedIndex < 0 ||
                this.InternshipPickerSelectedIndex >= this.FilteredInternships.Count)
            {
                await Application.Current.MainPage.DisplayAlert("Błąd", "Wybierz staż, w ramach którego wykonywana jest procedura.", "OK");
                return;
            }

            this._procedure.RequiredCount = requiredCount;
            this._procedure.Description = this.Notes;

            // Upewniamy się, że przypisany jest poprawny staż
            this._procedure.InternshipId = this.FilteredInternships[this.InternshipPickerSelectedIndex].Id;

            if (this._onSaveCallback != null)
            {
                await this._onSaveCallback(this._procedure);
            }

            await Shell.Current.Navigation.PopAsync();
        }
    }
}