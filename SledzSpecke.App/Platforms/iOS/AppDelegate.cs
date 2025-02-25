using Foundation;
using Microsoft.Maui;
using Microsoft.Maui.Hosting;
using SledzSpecke.App.Platforms.iOS;
using UIKit;

namespace SledzSpecke.App
{
    [Register("AppDelegate")]
    public class AppDelegate : MauiUIApplicationDelegate
    {
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            var result = base.FinishedLaunching(application, launchOptions);

            // Konfiguracja wyglądu nawigacji zgodnie z iOS
            ConfigureIOSAppearance();
            
            // Rejestracja do powiadomień push
            RegisterForPushNotifications();
            
            // Konfiguracja iCloud
            SetupICloudIntegration();
            
            // Konfiguracja zaawansowanych funkcji iOS
            SetupIOSFeatures();
            
            return result;
        }
        
        private void ConfigureIOSAppearance()
        {
            // Konfiguracja wyglądu nawigacji
            UINavigationBar.Appearance.TintColor = UIColor.SystemBlueColor;
            UINavigationBar.Appearance.PrefersLargeTitles = true;
            
            // Konfiguracja paska zakładek
            UITabBar.Appearance.TintColor = UIColor.SystemBlueColor;
            
            // Konfiguracja pasków narzędzi
            UIToolbar.Appearance.TintColor = UIColor.SystemBlueColor;
            
            // Ustawienia kolorów systemowych
            if (UIDevice.CurrentDevice.CheckSystemVersion(13, 0))
            {
                var appearance = new UINavigationBarAppearance();
                appearance.ConfigureWithDefaultBackground();
                appearance.LargeTitleTextAttributes = new UIStringAttributes
                {
                    ForegroundColor = UIColor.Label
                };
                appearance.TitleTextAttributes = new UIStringAttributes
                {
                    ForegroundColor = UIColor.Label
                };
                
                UINavigationBar.Appearance.StandardAppearance = appearance;
                UINavigationBar.Appearance.ScrollEdgeAppearance = appearance;
                
                // Konfiguracja wyglądu kart dla iOS 13+
                var tabBarAppearance = new UITabBarAppearance();
                tabBarAppearance.ConfigureWithDefaultBackground();
                
                UITabBar.Appearance.StandardAppearance = tabBarAppearance;
                if (UIDevice.CurrentDevice.CheckSystemVersion(15, 0))
                {
                    UITabBar.Appearance.ScrollEdgeAppearance = tabBarAppearance;
                }
            }
        }
        
        private void RegisterForPushNotifications()
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                UNUserNotificationCenter.Current.RequestAuthorization(
                    UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound,
                    (granted, error) =>
                    {
                        if (granted)
                        {
                            InvokeOnMainThread(UIApplication.SharedApplication.RegisterForRemoteNotifications);
                        }
                    });
            }
            else
            {
                var settings = UIUserNotificationSettings.GetSettingsForTypes(
                    UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound,
                    new NSSet());
                
                UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);
                UIApplication.SharedApplication.RegisterForRemoteNotifications();
            }
        }
        
        private void SetupICloudIntegration()
        {
            // Sprawdź dostępność iCloud
            var ubiquityUrl = NSFileManager.DefaultManager.GetUrlForUbiquityContainer(null);
            if (ubiquityUrl != null)
            {
                // iCloud jest dostępny
                Console.WriteLine("iCloud is available");
                
                // Możemy zainicjować synchronizację
                NSUbiquitousKeyValueStore.DefaultStore.Synchronize();
            }
            else
            {
                Console.WriteLine("iCloud is not available");
            }
        }
        
        private void SetupIOSFeatures()
        {
            // Konfiguracja HealthKit (jeśli aplikacja go używa)
            // Tu można dodać kod inicjalizacyjny dla HealthKit
            
            // Konfiguracja Apple Pencil
            if (UIDevice.CurrentDevice.CheckSystemVersion(12, 1))
            {
                // Obsługa dla Apple Pencil 2
            }
            
            // Konfiguracja innych funkcji specyficznych dla iOS
        }
        
        public override void ReceivedRemoteNotification(UIApplication application, NSDictionary userInfo)
        {
            // Obsługa powiadomień push
        }
        
        public override void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
        {
            Console.WriteLine($"Failed to register for remote notifications: {error}");
        }
        
        public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
            var tokenString = BitConverter.ToString(deviceToken.ToArray()).Replace("-", "").ToLowerInvariant();
            Console.WriteLine($"Device token: {tokenString}");
            
            // Wyślij token do serwera
        }
    }
}
