using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SledzSpecke.Core.Models.Domain;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace SledzSpecke.App.Platforms.Windows.Services
{
    public class WindowsTileService : ITileService
    {
        private readonly ILogger<WindowsTileService> _logger;
        
        public WindowsTileService(ILogger<WindowsTileService> logger)
        {
            _logger = logger;
        }
        
        public void UpdateLiveTile(double progressPercentage, string statusText)
        {
            try
            {
                // Utwórz XML dla kafelka
                var tileXml = GenerateProgressTileXml(progressPercentage, statusText);
                
                // Utwórz powiadomienie kafelka
                var tileNotification = new TileNotification(tileXml);
                
                // Zaktualizuj kafelek
                TileUpdateManager.CreateTileUpdaterForApplication().Update(tileNotification);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating live tile");
            }
        }
        
        public void UpdateTileWithUpcomingDuties(List<Duty> duties)
        {
            try
            {
                if (duties == null || duties.Count == 0)
                {
                    // Wyczyść kafelek, jeśli nie ma nadchodzących dyżurów
                    TileUpdateManager.CreateTileUpdaterForApplication().Clear();
                    return;
                }
                
                // Utwórz XML dla kafelka z nadchodzącymi dyżurami
                var tileXml = GenerateUpcomingDutiesTileXml(duties);
                
                // Utwórz powiadomienie kafelka
                var tileNotification = new TileNotification(tileXml);
                
                // Zaktualizuj kafelek
                TileUpdateManager.CreateTileUpdaterForApplication().Update(tileNotification);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating tile with upcoming duties");
            }
        }
        
        public void ClearTile()
        {
            try
            {
                // Wyczyść kafelek
                TileUpdateManager.CreateTileUpdaterForApplication().Clear();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing tile");
            }
        }
        
        public void EnableNotifications(bool enabled)
        {
            try
            {
                var updater = TileUpdateManager.CreateTileUpdaterForApplication();
                
                if (enabled)
                {
                    // Włącz powiadomienia kafelka
                    updater.EnableNotificationQueue(true);
                    updater.EnableNotificationQueueForSquare150x150(true);
                    updater.EnableNotificationQueueForWide310x150(true);
                    updater.EnableNotificationQueueForSquare310x310(true);
                }
                else
                {
                    // Wyłącz powiadomienia kafelka
                    updater.EnableNotificationQueue(false);
                    updater.EnableNotificationQueueForSquare150x150(false);
                    updater.EnableNotificationQueueForWide310x150(false);
                    updater.EnableNotificationQueueForSquare310x310(false);
                    
                    // Wyczyść powiadomienia
                    ClearTile();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enabling/disabling tile notifications");
            }
        }
        
        private XmlDocument GenerateProgressTileXml(double progressPercentage, string statusText)
        {
            try
            {
                // Przygotuj zmienne
                string progressText = $"{progressPercentage:P0}";
                
                // Utwórz szablon XML dla kafelka
                string tileContent = $@"
                <tile>
                    <visual>
                        <!-- Small Tile -->
                        <binding template='TileSmall'>
                            <text>{progressText}</text>
                        </binding>
                
                        <!-- Medium Tile -->
                        <binding template='TileMedium'>
                            <text>Postęp specjalizacji</text>
                            <text>{progressText}</text>
                            <text>{statusText}</text>
                        </binding>
                
                        <!-- Wide Tile -->
                        <binding template='TileWide'>
                            <text>Postęp specjalizacji</text>
                            <text>{progressText}</text>
                            <text>{statusText}</text>
                            <progress value='{progressPercentage}' title='Postęp' />
                        </binding>
                
                        <!-- Large Tile -->
                        <binding template='TileLarge'>
                            <text>Postęp specjalizacji</text>
                            <text>{progressText}</text>
                            <text>{statusText}</text>
                            <progress value='{progressPercentage}' title='Postęp' />
                        </binding>
                    </visual>
                </tile>";
                
                // Parsuj XML
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(tileContent);
                
                return xmlDoc;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating progress tile XML");
                throw;
            }
        }
        
        private XmlDocument GenerateUpcomingDutiesTileXml(List<Duty> duties)
        {
            try
            {
                // Ogranicz liczbę dyżurów do 3 (dla czytelności kafelka)
                var displayDuties = duties.Count > 3 ? duties.GetRange(0, 3) : duties;
                
                // Przygotuj teksty dla kafelka
                var dutiesText = new List<string>();
                foreach (var duty in displayDuties)
                {
                    var dutyDate = duty.StartTime.ToString("d");
                    var dutyTime = duty.StartTime.ToString("t") + " - " + duty.EndTime.ToString("t");
                    dutiesText.Add($"{duty.Location}: {dutyDate} {dutyTime}");
                }
                
                // Utwórz szablon XML dla kafelka
                string tileContent = $@"
                <tile>
                    <visual>
                        <!-- Small Tile -->
                        <binding template='TileSmall'>
                            <text>Dyżury</text>
                        </binding>
                
                        <!-- Medium Tile -->
                        <binding template='TileMedium'>
                            <text>Nadchodzące dyżury</text>
                            <text>{dutiesText.Count > 0 ? dutiesText[0] : "Brak nadchodzących dyżurów"}</text>
                            {dutiesText.Count > 1 ? $"<text>{dutiesText[1]}</text>" : ""}
                        </binding>
                
                        <!-- Wide Tile -->
                        <binding template='TileWide'>
                            <text>Nadchodzące dyżury</text>
                            <text>{dutiesText.Count > 0 ? dutiesText[0] : "Brak nadchodzących dyżurów"}</text>
                            {dutiesText.Count > 1 ? $"<text>{dutiesText[1]}</text>" : ""}
                            {dutiesText.Count > 2 ? $"<text>{dutiesText[2]}</text>" : ""}
                        </binding>
                
                        <!-- Large Tile -->
                        <binding template='TileLarge'>
                            <text>Nadchodzące dyżury</text>
                            <text>{dutiesText.Count > 0 ? dutiesText[0] : "Brak nadchodzących dyżurów"}</text>
                            {dutiesText.Count > 1 ? $"<text>{dutiesText[1]}</text>" : ""}
                            {dutiesText.Count > 2 ? $"<text>{dutiesText[2]}</text>" : ""}
                            <text>Otwórz aplikację, aby zobaczyć więcej.</text>
                        </binding>
                    </visual>
                </tile>";
                
                // Parsuj XML
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(tileContent);
                
                return xmlDoc;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating upcoming duties tile XML");
                throw;
            }
        }
    }
    
    public interface ITileService
    {
        void UpdateLiveTile(double progressPercentage, string statusText);
        void UpdateTileWithUpcomingDuties(List<Duty> duties);
        void ClearTile();
        void EnableNotifications(bool enabled);
    }
}
