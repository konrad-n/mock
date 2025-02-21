using SledzSpecke.App.ViewModels.Base;

namespace SledzSpecke.App.ViewModels.Dashboard
{
    public partial class DashboardViewModel : BaseViewModel
    {
        public DashboardViewModel()
        {
            Title = "Dashboard";
        }

        public override async Task LoadDataAsync()
        {
            // TODO: Implementacja ładowania danych
        }
    }
}
