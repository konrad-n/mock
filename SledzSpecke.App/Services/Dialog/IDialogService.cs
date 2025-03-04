namespace SledzSpecke.App.Services.Dialog
{
    /// <summary>
    /// Interface for dialog operations, allowing for better testability.
    /// </summary>
    public interface IDialogService
    {
        /// <summary>
        /// Displays an alert dialog with an accept and cancel button.
        /// </summary>
        /// <param name="title">The title of the alert.</param>
        /// <param name="message">The message to display.</param>
        /// <param name="accept">The text for the accept button.</param>
        /// <param name="cancel">The text for the cancel button.</param>
        /// <returns>True if the user clicked accept, false otherwise.</returns>
        Task<bool> DisplayAlertAsync(string title, string message, string accept, string cancel);

        /// <summary>
        /// Displays an alert dialog with only an accept button.
        /// </summary>
        /// <param name="title">The title of the alert.</param>
        /// <param name="message">The message to display.</param>
        /// <param name="accept">The text for the accept button.</param>
        Task DisplayAlertAsync(string title, string message, string accept);

        /// <summary>
        /// Displays an action sheet with multiple buttons.
        /// </summary>
        /// <param name="title">The title of the action sheet.</param>
        /// <param name="cancel">The text for the cancel button.</param>
        /// <param name="destruction">The text for the destruction button.</param>
        /// <param name="buttons">The texts for the other buttons.</param>
        /// <returns>The text of the button that was clicked.</returns>
        Task<string> DisplayActionSheetAsync(string title, string cancel, string destruction, params string[] buttons);

        /// <summary>
        /// Displays a prompt dialog that accepts user input.
        /// </summary>
        /// <param name="title">The title of the prompt.</param>
        /// <param name="message">The message to display.</param>
        /// <param name="accept">The text for the accept button.</param>
        /// <param name="cancel">The text for the cancel button.</param>
        /// <param name="placeholder">The placeholder text for the input field.</param>
        /// <param name="maxLength">The maximum length of the input.</param>
        /// <param name="keyboard">The keyboard type to use.</param>
        /// <param name="initialValue">The initial value for the input field.</param>
        /// <returns>The text that was input by the user.</returns>
        Task<string> DisplayPromptAsync(string title, string message, string accept = "OK", string cancel = "Cancel", string placeholder = null, int maxLength = -1, Keyboard keyboard = null, string initialValue = "");
    }
}