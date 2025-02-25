using System;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls;
using SledzSpecke.Core.Interfaces.Services;
using Windows.UI.ViewManagement;

namespace SledzSpecke.App.Platforms.Windows.UI
{
    public class WindowsLayoutProvider : ILayoutProvider
    {
        private readonly ILogger<WindowsLayoutProvider> _logger;
        private UIViewSettings _viewSettings;
        private DisplayInformation _displayInfo;
        
        // Współczynniki dla różnych trybów wyświetlania
        private const double TabletModeFactor = 1.25; // Większe elementy w trybie tabletu
        private const double TouchModeFactor = 1.2;   // Większe elementy dla ekranów dotykowych
        
        public WindowsLayoutProvider(ILogger<WindowsLayoutProvider> logger)
        {
            _logger = logger;
            
            try
            {
                // Pobierz ustawienia widoku
                _viewSettings = UIViewSettings.GetForCurrentView();
                _displayInfo = DisplayInformation.GetForCurrentView();
                
                // Zarejestruj zdarzenia zmiany trybu wyświetlania
                _viewSettings.UserInteractionModeChanged += ViewSettings_UserInteractionModeChanged;
                _displayInfo.OrientationChanged += DisplayInfo_OrientationChanged;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing Windows layout provider");
            }
        }
        
        public LayoutMode GetCurrentLayoutMode()
        {
            try
            {
                // Określ tryb układu na podstawie trybu interakcji i orientacji
                var userMode = _viewSettings.UserInteractionMode;
                var orientation = _displayInfo.CurrentOrientation;
                
                if (userMode == UserInteractionMode.Touch)
                {
                    // Tryb tabletu
                    if (orientation == DisplayOrientations.Portrait || 
                        orientation == DisplayOrientations.PortraitFlipped)
                    {
                        return LayoutMode.TabletPortrait;
                    }
                    else
                    {
                        return LayoutMode.TabletLandscape;
                    }
                }
                else
                {
                    // Tryb komputera
                    return LayoutMode.Desktop;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current layout mode");
                return LayoutMode.Desktop; // Domyślnie tryb komputera
            }
        }
        
        public double GetScalingFactor()
        {
            try
            {
                var layoutMode = GetCurrentLayoutMode();
                
                switch (layoutMode)
                {
                    case LayoutMode.TabletPortrait:
                        return TabletModeFactor;
                    case LayoutMode.TabletLandscape:
                        return TabletModeFactor * 0.9; // Nieco mniejszy współczynnik dla orientacji poziomej
                    case LayoutMode.Desktop:
                        return 1.0; // Bez skalowania w trybie komputera
                    default:
                        return 1.0;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting scaling factor");
                return 1.0;
            }
        }
        
        public double GetFontScale()
        {
            try
            {
                var layoutMode = GetCurrentLayoutMode();
                
                switch (layoutMode)
                {
                    case LayoutMode.TabletPortrait:
                        return 1.2; // Większa czcionka w trybie tabletu
                    case LayoutMode.TabletLandscape:
                        return 1.1; // Nieco mniejsza czcionka w trybie poziomym tabletu
                    case LayoutMode.Desktop:
                        return 1.0; // Normalna czcionka w trybie komputera
                    default:
                        return 1.0;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting font scale");
                return 1.0;
            }
        }
        
        public bool IsWideLayout()
        {
            try
            {
                var bounds = ApplicationView.GetForCurrentView().VisibleBounds;
                return bounds.Width > 1200; // Arbitralne szerokości dla różnych układów
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking wide layout");
                return false;
            }
        }
        
        public bool IsTouchMode()
        {
            try
            {
                return _viewSettings.UserInteractionMode == UserInteractionMode.Touch;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking touch mode");
                return false;
            }
        }
        
        public void ApplyLayoutToPage(ContentPage page)
        {
            try
            {
                var layoutMode = GetCurrentLayoutMode();
                var scaling = GetScalingFactor();
                var fontScale = GetFontScale();
                
                // Zastosuj odpowiednie style w zależności od trybu układu
                switch (layoutMode)
                {
                    case LayoutMode.TabletPortrait:
                        // Zastosuj style dla trybu tabletu w orientacji pionowej
                        page.Resources["FontSizeNormal"] = 16 * fontScale;
                        page.Resources["FontSizeLarge"] = 22 * fontScale;
                        page.Resources["PagePadding"] = new Thickness(20);
                        
                        // Zmień układ strony dla trybu tabletu
                        if (page.Content is Grid grid)
                        {
                            // Dostosuj układ siatki dla trybu tabletu
                            // Na przykład, zmień układ kolumn i wierszy
                        }
                        break;
                        
                    case LayoutMode.TabletLandscape:
                        // Zastosuj style dla trybu tabletu w orientacji poziomej
                        page.Resources["FontSizeNormal"] = 15 * fontScale;
                        page.Resources["FontSizeLarge"] = 20 * fontScale;
                        page.Resources["PagePadding"] = new Thickness(24);
                        break;
                        
                    case LayoutMode.Desktop:
                        // Zastosuj style dla trybu komputera
                        page.Resources["FontSizeNormal"] = 14;
                        page.Resources["FontSizeLarge"] = 18;
                        page.Resources["PagePadding"] = new Thickness(30);
                        
                        // Zmień układ strony dla trybu komputera
                        if (page.Content is Grid desktopGrid)
                        {
                            // Dostosuj układ siatki dla trybu komputera
                            // Na przykład, dodaj więcej kolumn
                        }
                        break;
                }
                
                // Dostosuj rozmiar elementów interaktywnych w trybie dotykowym
                if (IsTouchMode())
                {
                    page.Resources["ButtonHeight"] = 48;
                    page.Resources["EntryHeight"] = 48;
                }
                else
                {
                    page.Resources["ButtonHeight"] = 36;
                    page.Resources["EntryHeight"] = 36;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error applying layout to page");
            }
        }
        
        private void ViewSettings_UserInteractionModeChanged(UIViewSettings sender, object args)
        {
            try
            {
                // Powiadom aplikację o zmianie trybu interakcji
                LayoutChanged?.Invoke(this, new LayoutChangedEventArgs(GetCurrentLayoutMode()));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling user interaction mode changed");
            }
        }
        
        private void DisplayInfo_OrientationChanged(DisplayInformation sender, object args)
        {
            try
            {
                // Powiadom aplikację o zmianie orientacji
                LayoutChanged?.Invoke(this, new LayoutChangedEventArgs(GetCurrentLayoutMode()));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling orientation changed");
            }
        }
        
        public event EventHandler<LayoutChangedEventArgs> LayoutChanged;
    }
    
    public interface ILayoutProvider
    {
        LayoutMode GetCurrentLayoutMode();
        double GetScalingFactor();
        double GetFontScale();
        bool IsWideLayout();
        bool IsTouchMode();
        void ApplyLayoutToPage(ContentPage page);
        event EventHandler<LayoutChangedEventArgs> LayoutChanged;
    }
    
    public enum LayoutMode
    {
        Desktop,
        TabletPortrait,
        TabletLandscape,
        Mobile
    }
    
    public class LayoutChangedEventArgs : EventArgs
    {
        public LayoutMode NewLayoutMode { get; }
        
        public LayoutChangedEventArgs(LayoutMode newLayoutMode)
        {
            NewLayoutMode = newLayoutMode;
        }
    }
}
