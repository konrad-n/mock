using System.Threading.Tasks;

namespace SledzSpecke.App.Services.Dialog
{
    /// <summary>
    /// Implementacja IDialogService, która uwzględnia przypadki, gdy Shell.Current może być niedostępny.
    /// </summary>
    public class DialogService : IDialogService
    {
        public async Task<bool> DisplayAlertAsync(string title, string message, string accept, string cancel)
        {
            // Sprawdzamy, czy Shell.Current jest dostępny, jeśli nie, używamy Application.Current.MainPage
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
                // Jeśli nie ma dostępnej strony do wyświetlenia alertu, zwróć domyślną wartość
                System.Diagnostics.Debug.WriteLine($"Nie można wyświetlić alertu: {title} - {message}. Brak dostępnej strony.");
                return false;
            }
        }

        public async Task DisplayAlertAsync(string title, string message, string accept)
        {
            try
            {
                // Sprawdzamy, czy Shell.Current jest dostępny, jeśli nie, używamy Application.Current.MainPage
                if (Shell.Current != null)
                {
                    await Shell.Current.DisplayAlert(title, message, accept);
                }
                else if (Application.Current?.MainPage != null)
                {
                    await Application.Current.MainPage.DisplayAlert(title, message, accept);
                }
                else
                {
                    // Jeśli nie ma dostępnej strony do wyświetlenia alertu, zaloguj to
                    System.Diagnostics.Debug.WriteLine($"Nie można wyświetlić alertu: {title} - {message}. Brak dostępnej strony.");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas wyświetlania alertu: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
            }
        }

        public async Task<string> DisplayActionSheetAsync(string title, string cancel, string destruction, params string[] buttons)
        {
            // Sprawdzamy, czy Shell.Current jest dostępny, jeśli nie, używamy Application.Current.MainPage
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
                // Jeśli nie ma dostępnej strony do wyświetlenia action sheet, zaloguj to i zwróć null
                System.Diagnostics.Debug.WriteLine($"Nie można wyświetlić action sheet: {title}. Brak dostępnej strony.");
                return null;
            }
        }

        public async Task<string> DisplayPromptAsync(string title, string message, string accept = "OK", string cancel = "Cancel", string placeholder = null, int maxLength = -1, Keyboard keyboard = null, string initialValue = "")
        {
            // Sprawdzamy, czy Shell.Current jest dostępny, jeśli nie, używamy Application.Current.MainPage
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
                // Jeśli nie ma dostępnej strony do wyświetlenia promptu, zaloguj to i zwróć null
                System.Diagnostics.Debug.WriteLine($"Nie można wyświetlić promptu: {title} - {message}. Brak dostępnej strony.");
                return null;
            }
        }
    }
}