namespace SledzSpecke.App.Exceptions
{
    public abstract class AppBaseException : Exception
    {
        public string UserFriendlyMessage { get; }
        public Dictionary<string, object> ErrorDetails { get; }

        protected AppBaseException(
            string message, 
            string userFriendlyMessage = "Wystąpił błąd w aplikacji.", 
            Exception innerException = null, 
            Dictionary<string, object> errorDetails = null) 
            : base(message, innerException)
        {
            UserFriendlyMessage = userFriendlyMessage;
            ErrorDetails = errorDetails ?? new Dictionary<string, object>();
        }
    }
}
