using System;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace SledzSpecke.App.Services.Localization
{
    public class LocalizationService : ILocalizationService
    {
        private readonly ILogger<LocalizationService> _logger;
        private readonly ResourceManager _resourceManager;
        
        public LocalizationService(ILogger<LocalizationService> logger)
        {
            _logger = logger;
            // Inicjalizacja ResourceManager dla plików zasobów
            _resourceManager = new ResourceManager("SledzSpecke.App.Resources.AppResources", typeof(LocalizationService).Assembly);
        }
        
        public string GetString(string key)
        {
            try
            {
                return _resourceManager.GetString(key, CultureInfo.CurrentCulture) ?? key;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting localized string for key: {Key}", key);
                return key;
            }
        }
        
        public string GetString(string key, params object[] args)
        {
            try
            {
                var format = _resourceManager.GetString(key, CultureInfo.CurrentCulture) ?? key;
                return string.Format(format, args);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error formatting localized string for key: {Key}", key);
                return key;
            }
        }
        
        public void SetCulture(string cultureName)
        {
            try
            {
                var culture = new CultureInfo(cultureName);
                CultureInfo.CurrentCulture = culture;
                CultureInfo.CurrentUICulture = culture;
                
                // Informuj system o zmianie kultury
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting culture to: {Culture}", cultureName);
            }
        }
        
        public List<CultureInfo> GetAvailableCultures()
        {
            return new List<CultureInfo>
            {
                new CultureInfo("pl-PL"), // Polski
                new CultureInfo("en-US"), // Angielski (USA)
                new CultureInfo("de-DE"), // Niemiecki
                new CultureInfo("fr-FR"), // Francuski
                new CultureInfo("es-ES")  // Hiszpański
            };
        }
        
        public CultureInfo GetCurrentCulture()
        {
            return CultureInfo.CurrentCulture;
        }
    }
    
    public interface ILocalizationService
    {
        string GetString(string key);
        string GetString(string key, params object[] args);
        void SetCulture(string cultureName);
        List<CultureInfo> GetAvailableCultures();
        CultureInfo GetCurrentCulture();
    }
}
