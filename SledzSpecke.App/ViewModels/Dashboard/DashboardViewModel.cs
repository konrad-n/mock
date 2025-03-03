namespace SledzSpecke.App.ViewModels.Dashboard
{
    public class DashboardViewModel
    {

        // W DashboardViewModel
        // Przełączanie między modułami specjalizacji
        private int currentModuleId;

        public int CurrentModuleId
        {
            get => this.currentModuleId;
            set
            {
                this.SetProperty(ref this.currentModuleId, value);

                // Reload all data with new module filter
                this.LoadDataAsync();
            }
        }

        // Widoczność przełącznika modułów
        public bool ShowModuleSelector => this.CurrentSpecialization?.HasModules ?? false;

        public List<ModuleInfo> AvailableModules => this.CurrentSpecialization?.Modules?.Select(m => new ModuleInfo { Id = m.Id, Name = m.Name }).ToList();
    }
}
