using Microsoft.Extensions.Logging;
using SledzSpecke.App.Services.Interfaces;

namespace SledzSpecke.App.Services.Implementations
{
    public class NavigationService : INavigationService
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger<NavigationService> logger;

        public NavigationService(IServiceProvider serviceProvider, ILogger<NavigationService> logger)
        {
            this.serviceProvider = serviceProvider;
            this.logger = logger;
        }

        public async Task NavigateToAsync(Type pageType)
        {
            try
            {
                var page = this.serviceProvider.GetService(pageType) as Page;
                if (page == null)
                {
                    this.logger.LogWarning("Could not resolve page of type {PageType}", pageType.Name);
                    return;
                }

                if (Shell.Current != null)
                {
                    await Shell.Current.Navigation.PushAsync(page);
                }
                else if (Application.Current.MainPage is NavigationPage navPage)
                {
                    await navPage.PushAsync(page);
                }
                else
                {
                    Application.Current.MainPage = new NavigationPage(page);
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Navigation failed");
            }
        }

        public async Task NavigateToAsync<T>()
            where T : Page
        {
            await this.NavigateToAsync(typeof(T));
        }

        public async Task PopAsync()
        {
            try
            {
                if (Shell.Current != null)
                {
                    await Shell.Current.Navigation.PopAsync();
                }
                else if (Application.Current.MainPage is NavigationPage navPage)
                {
                    await navPage.PopAsync();
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Navigation pop failed");
            }
        }

        public async Task DisplayAlertAsync(string title, string message, string cancel)
        {
            if (Application.Current.MainPage != null)
            {
                await Application.Current.MainPage.DisplayAlert(title, message, cancel);
            }
        }

        public async Task<bool> DisplayAlertAsync(string title, string message, string accept, string cancel)
        {
            if (Application.Current.MainPage != null)
            {
                return await Application.Current.MainPage.DisplayAlert(title, message, accept, cancel);
            }
            return false;
        }
    }
}
