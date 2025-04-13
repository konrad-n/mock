using System.Threading.Tasks;

namespace SledzSpecke.App.Services.Dialog
{
    public class DialogService : IDialogService
    {
        public async Task<bool> DisplayAlertAsync(string title, string message, string accept, string cancel)
        {
            if (Shell.Current != null)
            {
                return await Shell.Current.DisplayAlert(title, message, accept, cancel);
            }
            else if (Application.Current?.MainPage != null)
            {
                return await Application.Current.MainPage.DisplayAlert(title, message, accept, cancel);
            }
            else
            {
                return false;
            }
        }

        public async Task DisplayAlertAsync(string title, string message, string accept)
        {
            if (Shell.Current != null)
            {
                await Shell.Current.DisplayAlert(title, message, accept);
            }
            else if (Application.Current?.MainPage != null)
            {
                await Application.Current.MainPage.DisplayAlert(title, message, accept);
            }
        }

        public async Task<string> DisplayActionSheetAsync(string title, string cancel, string destruction, params string[] buttons)
        {
            if (Shell.Current != null)
            {
                return await Shell.Current.DisplayActionSheet(title, cancel, destruction, buttons);
            }
            else if (Application.Current?.MainPage != null)
            {
                return await Application.Current.MainPage.DisplayActionSheet(title, cancel, destruction, buttons);
            }
            else
            {
                return null;
            }
        }

        public async Task<string> DisplayPromptAsync(string title, string message, string accept = "OK", string cancel = "Cancel", string placeholder = null, int maxLength = -1, Keyboard keyboard = null, string initialValue = "")
        {
            if (Shell.Current != null)
            {
                return await Shell.Current.DisplayPromptAsync(title, message, accept, cancel, placeholder, maxLength, keyboard, initialValue);
            }
            else if (Application.Current?.MainPage != null)
            {
                return await Application.Current.MainPage.DisplayPromptAsync(title, message, accept, cancel, placeholder, maxLength, keyboard, initialValue);
            }
            else
            {
                return null;
            }
        }

        public async Task<bool> DisplayConfirmationAsync(string title, string message, string accept, string cancel)
        {
            return await this.DisplayAlertAsync(title, message, accept, cancel);
        }
    }
}