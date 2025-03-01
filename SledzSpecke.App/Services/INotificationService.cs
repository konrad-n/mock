namespace SledzSpecke.App.Services
{
    public interface INotificationService
    {
        Task CheckAndScheduleNotificationsAsync();
    }
}