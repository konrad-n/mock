namespace SledzSpecke.App.Common.Views
{
    public abstract class BaseContentPage : ContentPage
    {
        protected IServiceProvider ServiceProvider { get; private set; }

        public new string Title
        {
            get => base.Title;
            set => base.Title = value;
        }

        protected BaseContentPage()
        {
            Loaded += OnPageLoaded;
        }

        protected override void OnHandlerChanged()
        {
            base.OnHandlerChanged();
            if (Handler != null)
            {
                ServiceProvider = Handler.MauiContext?.Services
                    ?? throw new InvalidOperationException("No service provider available.");
            }
        }

        private void OnPageLoaded(object sender, EventArgs e)
        {
            InitializePageAsync();
        }

        protected virtual Task InitializePageAsync()
        {
            return Task.CompletedTask;
        }

        protected T GetRequiredService<T>() where T : class
        {
            if (ServiceProvider == null)
                throw new InvalidOperationException("Service Provider not initialized");

            return ServiceProvider.GetRequiredService<T>();
        }
    }
}
