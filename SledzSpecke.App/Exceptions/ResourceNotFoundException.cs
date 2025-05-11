namespace SledzSpecke.App.Exceptions
{
    public class ResourceNotFoundException : AppBaseException
    {
        public ResourceNotFoundException(
            string message,
            string userFriendlyMessage = "Nie znaleziono żądanego zasobu.",
            Exception innerException = null,
            Dictionary<string, object> errorDetails = null)
            : base(message, userFriendlyMessage, innerException, errorDetails)
        {
        }
    }
}