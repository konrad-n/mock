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

        public MedicalProcedure Procedure => _procedure;

        public ProcedureDetailsViewModel(ILogger<ProcedureDetailsViewModel> logger) : base(logger)
        {
            Title = "Szczegóły procedury";
        }

        public void Initialize(MedicalProcedure procedure, ModuleType currentModule,
            ProcedureType currentProcedureType, Func<MedicalProcedure, Task> onSaveCallback,
            List<Internship> internships)
        {
            _currentModule = currentModule;
            _currentProcedureType = currentProcedureType;
            _onSaveCallback = onSaveCallback;
            _internships = internships;

            if (procedure == null)
            {
                // Nowa procedura
                _procedure = new MedicalProcedure
                {
                    Id = new Random().Next(1000, 9999), // Tymczasowe ID
                    Module = currentModule,
                    ProcedureType = currentProcedureType,
                    CompletedCount = 0
                };
                PageTitle = "Dodaj procedurę";
                RequiredCount = "1";
                CompletedCount = "0";
            }
            else
            {
                // Edycja istniejącej procedury
                _procedure = procedure;
                PageTitle = "Szczegóły procedury";
                RequiredCount = procedure.RequiredCount.ToString();
                CompletedCount = procedure.CompletedCount.ToString();
                Notes = procedure.Description;
            }

            // Ustawienie pickerów
            ProcedureTypePickerSelectedIndex = _procedure.ProcedureType == ProcedureType.TypeA ? 0 : 1;
            ModulePickerSelectedIndex = _procedure.Module == ModuleType.Basic ? 0 : 1;

            // Wypełnienie pickera stażów
            LoadInternships(_procedure.Module);
        }

        private void LoadInternships(ModuleType moduleType)
        {
            InternshipItems.Clear();
            FilteredInternships = _internships.Where(i => i.Module == moduleType).ToList();

            foreach (var internship in FilteredInternships)
            {
                InternshipItems.Add(internship.Name);
            }

            // Jeśli edytujemy procedurę, ustawiamy wybrany staż
            if (_procedure.InternshipId.HasValue)
            {
                int index = FilteredInternships.FindIndex(i => i.Id == _procedure.InternshipId);
                if (index >= 0)
                {
                    InternshipPickerSelectedIndex = index;
                }
                else
                {
                    // Staż nie należy do wybranego modułu, więc ustawiamy pusty
                    InternshipPickerSelectedIndex = -1;
                    // Czyścimy przypisanie stażu
                    _procedure.InternshipId = null;
                }
            }
            else
            {
                InternshipPickerSelectedIndex = -1;
            }
        }

        [RelayCommand]
        public void UpdateProcedureType(int selectedIndex)
        {
            if (_procedure != null)
            {
                _procedure.ProcedureType = selectedIndex == 0 ? ProcedureType.TypeA : ProcedureType.TypeB;
            }
        }

        [RelayCommand]
        public void UpdateModule(int selectedIndex)
        {
            if (_procedure != null)
            {
                _procedure.Module = selectedIndex == 0 ? ModuleType.Basic : ModuleType.Specialistic;

                // Przy zmianie modułu zapamiętujemy aktualny identyfikator stażu
                int? currentInternshipId = _procedure.InternshipId;

                // Ładujemy nową listę stażów
                LoadInternships(_procedure.Module);

                // Jeśli staż należy do nowego modułu, ustawiamy go ponownie
                if (currentInternshipId.HasValue)
                {
                    int index = FilteredInternships.FindIndex(i => i.Id == currentInternshipId.Value);
                    if (index >= 0)
                    {
                        InternshipPickerSelectedIndex = index;
                        _procedure.InternshipId = currentInternshipId;
                    }
                }
            }
        }

        [RelayCommand]
        public void UpdateInternship(int selectedIndex)
        {
            if (_procedure != null && selectedIndex >= 0 && selectedIndex < FilteredInternships.Count)
            {
                _procedure.InternshipId = FilteredInternships[selectedIndex].Id;
            }
            else if (_procedure != null)
            {
                // Brak wyboru stażu
                _procedure.InternshipId = null;
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
            if (string.IsNullOrWhiteSpace(_procedure.Name))
            {
                await Application.Current.MainPage.DisplayAlert("Błąd", "Nazwa procedury jest wymagana.", "OK");
                return;
            }

            if (!int.TryParse(RequiredCount, out int requiredCount) || requiredCount <= 0)
            {
                await Application.Current.MainPage.DisplayAlert("Błąd", "Wprowadź poprawną wymaganą liczbę procedur.", "OK");
                return;
            }

            if (InternshipPickerSelectedIndex < 0 ||
                InternshipPickerSelectedIndex >= FilteredInternships.Count)
            {
                await Application.Current.MainPage.DisplayAlert("Błąd", "Wybierz staż, w ramach którego wykonywana jest procedura.", "OK");
                return;
            }

            _procedure.RequiredCount = requiredCount;
            _procedure.Description = Notes;

            // Upewniamy się, że przypisany jest poprawny staż
            _procedure.InternshipId = FilteredInternships[InternshipPickerSelectedIndex].Id;

            if (_onSaveCallback != null)
            {
                await _onSaveCallback(_procedure);
            }

            await Shell.Current.Navigation.PopAsync();
        }
    }
}