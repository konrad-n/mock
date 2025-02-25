using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Uwp.Notifications;
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;

namespace SledzSpecke.App.Platforms.Windows.Notifications
{
    public class WindowsNotificationService : IWindowsNotificationService
    {
        private readonly ILogger<WindowsNotificationService> _logger;
        
        public WindowsNotificationService(ILogger<WindowsNotificationService> logger)
        {
            _logger = logger;
        }
        
        public void ShowToast(string title, string message, ToastNotificationPriority priority = ToastNotificationPriority.Default)
        {
            try
            {
                // Utwórz prosty toast
                var toastContent = new ToastContentBuilder()
                    .AddText(title)
                    .AddText(message)
                    .GetToastContent();
                
                // Utwórz toast z treści
                var toast = new ToastNotification(toastContent.GetXml())
                {
                    Priority = priority
                };
                
                // Pokaż toast
                ToastNotificationManager.CreateToastNotifier().Show(toast);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error showing toast notification");
            }
        }
        
        public void ShowToastWithActions(
            string title, 
            string message, 
            string actionButtonText, 
            string actionId,
            string arguments, 
            ToastNotificationPriority priority = ToastNotificationPriority.Default)
        {
            try
            {
                // Utwórz toast z przyciskiem akcji
                var toastContent = new ToastContentBuilder()
                    .AddText(title)
                    .AddText(message)
                    .AddButton(new ToastButton(actionButtonText, arguments)
                    {
                        ActivationType = ToastActivationType.Foreground,
                        ImageUri = "ms-appx:///Assets/Icons/check.png",
                        Id = actionId
                    })
                    .GetToastContent();
                
                // Utwórz toast z treści
                var toast = new ToastNotification(toastContent.GetXml())
                {
                    Priority = priority
                };
                
                // Dodaj obsługę zdarzeń
                toast.Activated += Toast_Activated;
                toast.Dismissed += Toast_Dismissed;
                toast.Failed += Toast_Failed;
                
                // Pokaż toast
                ToastNotificationManager.CreateToastNotifier().Show(toast);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error showing toast notification with actions");
            }
        }
        
        public void ShowReminderToast(string title, string message, DateTime dueTime)
        {
            try
            {
                // Utwórz przypomnienie
                var toastContent = new ToastContentBuilder()
                    .AddText(title)
                    .AddText(message)
                    .AddText($"Przypomnienie na: {dueTime:g}")
                    .AddButton(new ToastButton("Zaznacz jako wykonane", "action=markComplete"))
                    .AddButton(new ToastButton("Odłóż", "action=snooze"))
                    .GetToastContent();
                
                // Utwórz toast z treści
                var toast = new ToastNotification(toastContent.GetXml());
                
                // Pokaż toast
                ToastNotificationManager.CreateToastNotifier().Show(toast);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error showing reminder toast notification");
            }
        }
        
        public void ShowProgressToast(string title, string message, double progressValue, string progressStatus)
        {
            try
            {
                // Utwórz toast z paskiem postępu
                var toastContent = new ToastContentBuilder()
                    .AddText(title)
                    .AddText(message)
                    .AddProgressBar(progressValue, progressStatus)
                    .GetToastContent();
                
                // Utwórz toast z treści
                var toast = new ToastNotification(toastContent.GetXml());
                
                // Pokaż toast
                ToastNotificationManager.CreateToastNotifier().Show(toast);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error showing progress toast notification");
            }
        }
        
        public void ScheduleToast(string title, string message, DateTime triggerTime)
        {
            try
            {
                // Utwórz treść toastu
                var toastContent = new ToastContentBuilder()
                    .AddText(title)
                    .AddText(message)
                    .GetToastContent();
                
                // Utwórz zaplanowane powiadomienie
                var scheduled = new ScheduledToastNotification(
                    toastContent.GetXml(),
                    new DateTimeOffset(triggerTime));
                
                // Zaplanuj powiadomienie
                ToastNotificationManager.CreateToastNotifier().AddToSchedule(scheduled);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error scheduling toast notification");
            }
        }
        
        public void ClearAllNotifications()
        {
            try
            {
                ToastNotificationManager.History.Clear();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing toast notifications");
            }
        }
        
        private void Toast_Activated(ToastNotification sender, object args)
        {
            // Obsługa kliknięcia w toast
            // Tu należy dodać logikę obsługi akcji
        }
        
        private void Toast_Dismissed(ToastNotification sender, ToastDismissedEventArgs args)
        {
            // Obsługa odrzucenia toastu
        }
        
        private void Toast_Failed(ToastNotification sender, ToastFailedEventArgs args)
        {
            _logger.LogError("Toast notification failed: {Error}", args.ErrorCode);
        }
    }
    
    public interface IWindowsNotificationService
    {
        void ShowToast(string title, string message, ToastNotificationPriority priority = ToastNotificationPriority.Default);
        void ShowToastWithActions(string title, string message, string actionButtonText, string actionId, string arguments, ToastNotificationPriority priority = ToastNotificationPriority.Default);
        void ShowReminderToast(string title, string message, DateTime dueTime);
        void ShowProgressToast(string title, string message, double progressValue, string progressStatus);
        void ScheduleToast(string title, string message, DateTime triggerTime);
        void ClearAllNotifications();
    }
}
