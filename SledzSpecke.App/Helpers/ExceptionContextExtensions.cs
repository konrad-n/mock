// Helpers/ExceptionContextExtensions.cs
namespace SledzSpecke.App.Helpers
{
    public static class ExceptionContextExtensions
    {
        public static Dictionary<string, object> ToContext(this int value, string key = "Id")
        {
            return new Dictionary<string, object> { { key, value } };
        }

        public static Dictionary<string, object> ToContext(this string value, string key = "Name")
        {
            return new Dictionary<string, object> { { key, value } };
        }

        public static Dictionary<string, object> AddContext(this Dictionary<string, object> context, string key, object value)
        {
            if (context == null)
            {
                context = new Dictionary<string, object>();
            }

            if (!context.ContainsKey(key) && value != null)
            {
                context[key] = value;
            }

            return context;
        }
    }
}
