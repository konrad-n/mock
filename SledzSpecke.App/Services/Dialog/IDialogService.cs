namespace SledzSpecke.App.Services.Dialog
{
    public interface IDialogService
    {
        Task<bool> DisplayAlertAsync(string title, string message, string accept, string cancel);

        Task DisplayAlertAsync(string title, string message, string accept);

        Task<string> DisplayActionSheetAsync(string title, string cancel, string destruction, params string[] buttons);

        Task<string> DisplayPromptAsync(string title, string message, string accept = "OK", string cancel = "Cancel", string placeholder = null, int maxLength = -1, Keyboard keyboard = null, string initialValue = "");

        Task<bool> DisplayConfirmationAsync(string title, string message, string accept, string cancel);
    }
}