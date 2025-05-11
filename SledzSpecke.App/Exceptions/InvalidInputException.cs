namespace SledzSpecke.App.Exceptions
{
    public class InvalidInputException : AppBaseException
    {
        public InvalidInputException(
            string message,
            string userFriendlyMessage = "Wprowadzono nieprawidłowe dane.",
            Exception innerException = null,
            Dictionary<string, object> errorDetails = null)
            : base(message, userFriendlyMessage, innerException, errorDetails)
        {
        }
    }
}