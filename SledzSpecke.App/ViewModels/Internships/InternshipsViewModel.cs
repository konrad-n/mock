using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.ViewModels.Base;
using SledzSpecke.Core.Interfaces.Services;
using SledzSpecke.Core.Models.Domain;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace SledzSpecke.App.ViewModels.Internships
{
    public partial class InternshipsViewModel : BaseViewModel
    {
        private readonly IInternshipService _internshipService;
        
        public InternshipsViewModel(IInternshipService internshipService)
        {
            _internshipService = internshipService;
            Title = "Staże";
            Internships = new ObservableCollection<Internship>();
            AvailableYears = new ObservableCollection<int>();
        }

        [ObservableProperty]
        private ObservableCollection<Internship> internships;
        
        [ObservableProperty]
        private ObservableCollection<int> availableYears;
        
        [ObservableProperty]
        private int selectedYear;
        
        [ObservableProperty]
        private double completionPercentage;
        
        [ObservableProperty]
        private Dictionary<string, (int Required, int Completed)> yearProgress;
        
        [ObservableProperty]
        private ObservableCollection<InternshipDefinition> recommendedInternships;

        public override async Task LoadDataAsync()
        {
            if (IsBusy) return;
            
            try
            {
                IsBusy = true;
                
                // Załaduj staże
                var userInternships = await _internshipService.GetUserInternshipsAsync();
                Internships.Clear();
                foreach (var internship in userInternships)
                {
                    Internships.Add(internship);
                }
                
                // Załaduj dostępne lata
                var allInternships = await _internshipService.GetRequiredInternshipsAsync();
                var years = allInternships.Select(i => i.RecommendedYear).Distinct().OrderBy(y => y).ToList();
                AvailableYears.Clear();
                AvailableYears.Add(0); // 0 oznacza "Wszystkie"
                foreach (var year in years)
                {
                    AvailableYears.Add(year);
                }
                
                // Załaduj postępy
                CompletionPercentage = await _internshipService.GetInternshipProgressAsync();
                YearProgress = await _internshipService.GetInternshipProgressByYearAsync();
                
                // Załaduj rekomendowane staże
                var recommended = await _internshipService.GetRecommendedInternshipsForCurrentYearAsync();
                RecommendedInternships = new ObservableCollection<InternshipDefinition>(recommended);
                
                // Ustaw domyślne filtrowanie
                SelectedYear = 0; // Wszystkie
            }
            catch (System.Exception ex)
            {
                await Shell.Current.DisplayAlert("Błąd", $"Nie udało się załadować danych: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        partial void OnSelectedYearChanged(int value)
        {
            _ = FilterInternshipsAsync();
        }

        private async Task FilterInternshipsAsync()
        {
            if (IsBusy) return;
            
            try
            {
                IsBusy = true;
                var allInternships = await _internshipService.GetUserInternshipsAsync();
                
                // Filtruj według roku
                if (SelectedYear != 0) // 0 oznacza "Wszystkie"
                {
                    // Potrzebujemy definicji staży, aby znać ich przypisany rok
                    var requiredInternships = await _internshipService.GetRequiredInternshipsAsync();
                    var internshipsByYear = requiredInternships
                        .Where(i => i.RecommendedYear == SelectedYear)
                        .Select(i => i.Id)
                        .ToHashSet();
                    
                    allInternships = allInternships
                        .Where(i => internshipsByYear.Contains(i.InternshipDefinitionId))
                        .ToList();
                }
                
                Internships.Clear();
                foreach (var internship in allInternships)
                {
                    Internships.Add(internship);
                }
            }
            catch (System.Exception ex)
            {
                await Shell.Current.DisplayAlert("Błąd", $"Nie udało się zafiltrować danych: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task AddInternshipAsync()
        {
            await Shell.Current.GoToAsync("internship/add");
        }

        [RelayCommand]
        private async Task ViewInternshipAsync(int id)
        {
            await Shell.Current.GoToAsync($"internship/edit?id={id}");
        }

        [RelayCommand]
        private async Task ViewInternshipDetailsAsync(int id)
        {
            await Shell.Current.GoToAsync($"internship/details?id={id}");
        }
    }
}
