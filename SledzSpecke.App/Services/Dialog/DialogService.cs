namespace SledzSpecke.App.Services.Dialog
{
    /// <summary>
    /// Implementation of IDialogService that uses Shell.Current for dialogs.
    /// </summary>
    public class DialogService : IDialogService
    {
        public async Task<bool> DisplayAlertAsync(string title, string message, string accept, string cancel)
        {
            return await Shell.Current.DisplayAlert(title, message, accept, cancel);
        }

        public async Task DisplayAlertAsync(string title, string message, string accept)
        {
            await Shell.Current.DisplayAlert(title, message, accept);
        }

        public async Task<string> DisplayActionSheetAsync(string title, string cancel, string destruction, params string[] buttons)
        {
            return await Shell.Current.DisplayActionSheet(title, cancel, destruction, buttons);
        }

        public async Task<string> DisplayPromptAsync(string title, string message, string accept = "OK", string cancel = "Cancel", string placeholder = null, int maxLength = -1, Keyboard keyboard = null, string initialValue = "")
        {
            return await Shell.Current.DisplayPromptAsync(title, message, accept, cancel, placeholder, maxLength, keyboard, initialValue);
        }
    }
}