using System.Collections.Generic;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using UIKit;

namespace SledzSpecke.App.Platforms.iOS
{
    public class ThemeManager
    {
        // Singleton
        private static ThemeManager _instance;
        public static ThemeManager Instance => _instance ??= new ThemeManager();
        
        // Słownik kolorów semantycznych dla iOS
        private readonly Dictionary<string, UIColor> _iOSSemanticColors = new Dictionary<string, UIColor>
        {
            // Kolory systemowe iOS
            { "Primary", UIColor.SystemBlueColor },
            { "Secondary", UIColor.SystemOrangeColor },
            { "Background", UIColor.SystemBackgroundColor },
            { "Surface", UIColor.SecondarySystemBackgroundColor },
            { "TextPrimary", UIColor.LabelColor },
            { "TextSecondary", UIColor.SecondaryLabelColor },
            { "Accent", UIColor.SystemIndigoColor },
            { "Success", UIColor.SystemGreenColor },
            { "Warning", UIColor.SystemYellowColor },
            { "Error", UIColor.SystemRedColor },
            { "Separator", UIColor.SeparatorColor },
            { "GroupedBackground", UIColor.SystemGroupedBackgroundColor }
        };
        
        // Metoda do zastosowania motywu specyficznego dla iOS
        public void ApplyiOSTheme(ResourceDictionary resources, bool forceDarkMode = false)
        {
            // Sprawdź, czy urządzenie jest w trybie ciemnym
            bool isDarkMode = forceDarkMode || UITraitCollection.CurrentTraitCollection.UserInterfaceStyle == UIUserInterfaceStyle.Dark;
            
            // Zastosuj odpowiednie kolory
            foreach (var colorKey in _iOSSemanticColors.Keys)
            {
                var uiColor = _iOSSemanticColors[colorKey];
                
                // Konwersja z UIColor na Color MAUI
                var resolvedColor = new Color(
                    (float)uiColor.CIColor.Red,
                    (float)uiColor.CIColor.Green,
                    (float)uiColor.CIColor.Blue,
                    (float)uiColor.CIColor.Alpha
                );
                
                // Zaktualizuj kolory w zasobach aplikacji
                if (resources.ContainsKey(colorKey))
                {
                    resources[colorKey] = resolvedColor;
                }
            }
            
            // Aktualizacja stylów systemowych
            UpdateSystemStyles(resources, isDarkMode);
        }
        
        // Aktualizacja stylów do zgodności z iOS Human Interface Guidelines
        private void UpdateSystemStyles(ResourceDictionary resources, bool isDarkMode)
        {
            // Zaktualizuj style przycisków zgodnie z iOS HIG
            if (resources.TryGetValue("PrimaryButton", out var primaryButtonStyle) && 
                primaryButtonStyle is Style buttonStyle)
            {
                // Ustaw odpowiednie marginesy i zaokrąglenia zgodne z iOS
                buttonStyle.Setters.Add(new Setter 
                { 
                    Property = Button.CornerRadiusProperty, 
                    Value = 10 
                });
                
                buttonStyle.Setters.Add(new Setter 
                { 
                    Property = Button.HeightRequestProperty, 
                    Value = 44 // Minimalna wysokość dotykalna w iOS
                });
                
                buttonStyle.Setters.Add(new Setter 
                { 
                    Property = Button.PaddingProperty, 
                    Value = new Thickness(16, 0) 
                });
            }
            
            // Podobnie można zaktualizować inne elementy UI zgodnie z wytycznymi Apple
        }
    }
}
