namespace SledzSpecke.App.Common.Helpers
{
    public static class ServiceHelper
    {
        public static TService GetService<TService>(ContentPage page)
            where TService : class
        {
            var handler = page.Handler;
            if (handler == null)
                throw new InvalidOperationException("Page handler is not initialized");

            var services = handler.MauiContext?.Services
                ?? throw new InvalidOperationException("Unable to find service provider");

            var service = services.GetService<TService>();
            if (service == null)
                throw new InvalidOperationException($"Service {typeof(TService).Name} not found");

            return service;
        }
    }
}
