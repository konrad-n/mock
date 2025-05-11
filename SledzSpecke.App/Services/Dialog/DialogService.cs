using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.ApplicationModel;

namespace SledzSpecke.App.Services.Dialog
{
    public class DialogService : IDialogService
    {
        public async Task<bool> DisplayAlertAsync(string title, string message, string accept, string cancel)
        {
            var tcs = new TaskCompletionSource<bool>();

            await MainThread.InvokeOnMainThreadAsync(async () => {
                var page = Shell.Current ?? Application.Current?.MainPage;
                if (page != null)
                {
                    var result = await page.DisplayAlert(title, message, accept, cancel);
                    tcs.SetResult(result);
                }
                else
                {
                    tcs.SetResult(false);
                }
            });

            return await tcs.Task;
        }

        public async Task DisplayAlertAsync(string title, string message, string accept)
        {
            await MainThread.InvokeOnMainThreadAsync(async () => {
                var page = Shell.Current ?? Application.Current?.MainPage;
                if (page != null)
                {
                    await page.DisplayAlert(title, message, accept);
                }
            });
        }

        public async Task<string> DisplayActionSheetAsync(string title, string cancel, string destruction, params string[] buttons)
        {
            var tcs = new TaskCompletionSource<string>();

            await MainThread.InvokeOnMainThreadAsync(async () => {
                var page = Shell.Current ?? Application.Current?.MainPage;
                if (page != null)
                {
                    var result = await page.DisplayActionSheet(title, cancel, destruction, buttons);
                    tcs.SetResult(result);
                }
                else
                {
                    tcs.SetResult(null);
                }
            });

            return await tcs.Task;
        }

        public async Task<string> DisplayPromptAsync(string title, string message, string accept = "OK", string cancel = "Cancel", string placeholder = null, int maxLength = -1, Keyboard keyboard = null, string initialValue = "")
        {
            var tcs = new TaskCompletionSource<string>();

            await MainThread.InvokeOnMainThreadAsync(async () => {
                var page = Shell.Current ?? Application.Current?.MainPage;
                if (page != null)
                {
                    var result = await page.DisplayPromptAsync(title, message, accept, cancel, placeholder, maxLength, keyboard, initialValue);
                    tcs.SetResult(result);
                }
                else
                {
                    tcs.SetResult(null);
                }
            });

            return await tcs.Task;
        }
    }
}
