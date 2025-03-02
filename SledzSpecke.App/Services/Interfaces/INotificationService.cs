namespace SledzSpecke.App.Services.Interfaces
{
    public interface INotificationService
    {
        Task CheckAndScheduleNotificationsAsync();
    }
}