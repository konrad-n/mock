namespace SledzSpecke.App.Services.Interfaces
{
    public interface INavigationService
    {
        Task NavigateToAsync(Type pageType);

        Task NavigateToAsync<T>()
            where T : Page;

        Task PopAsync();

        Task DisplayAlertAsync(string title, string message, string cancel);

        Task<bool> DisplayAlertAsync(string title, string message, string accept, string cancel);
    }
}