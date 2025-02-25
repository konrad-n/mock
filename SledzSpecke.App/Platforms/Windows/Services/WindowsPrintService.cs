using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SledzSpecke.Core.Models.Domain;
using Windows.Graphics.Printing;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Printing;
using Windows.UI.Xaml.Controls;

namespace SledzSpecke.App.Platforms.Windows.Services
{
    public class WindowsPrintService : IPrintService
    {
        private readonly ILogger<WindowsPrintService> _logger;
        private PrintManager _printManager;
        private PrintDocument _printDocument;
        private IPrintDocumentSource _printDocumentSource;
        private PrintTask _printTask;
        
        // Elementy zawartości do drukowania
        private Canvas _printCanvas;
        private UIElement _printContent;
        
        public WindowsPrintService(ILogger<WindowsPrintService> logger)
        {
            _logger = logger;
            
            try
            {
                // Inicjalizacja menedżera drukowania
                _printManager = PrintManager.GetForCurrentView();
                _printManager.PrintTaskRequested += PrintManager_PrintTaskRequested;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing print manager");
            }
        }
        
        public async Task<bool> PrintDocumentAsync(string documentPath, string documentTitle)
        {
            try
            {
                // Sprawdź, czy dokument istnieje
                if (!File.Exists(documentPath))
                {
                    _logger.LogWarning("Document does not exist: {DocumentPath}", documentPath);
                    return false;
                }
                
                // Przygotuj dokument PDF lub obrazu do drukowania
                if (Path.GetExtension(documentPath).ToLower() == ".pdf")
                {
                    return await PrintPdfAsync(documentPath, documentTitle);
                }
                else if (Path.GetExtension(documentPath).ToLower() == ".png" ||
                         Path.GetExtension(documentPath).ToLower() == ".jpg" ||
                         Path.GetExtension(documentPath).ToLower() == ".jpeg")
                {
                    return await PrintImageAsync(documentPath, documentTitle);
                }
                else
                {
                    _logger.LogWarning("Unsupported document format: {DocumentFormat}", Path.GetExtension(documentPath));
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error printing document");
                return false;
            }
        }
        
        public async Task<bool> PrintProcedureReportAsync(List<ProcedureExecution> procedures, string title)
        {
            try
            {
                // Utwórz zawartość raportu procedur
                var content = new StackPanel
                {
                    Width = 670, // Szerokość A4 w pikselach przy 96 DPI
                    Background = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.White)
                };
                
                // Dodaj tytuł raportu
                content.Children.Add(new TextBlock
                {
                    Text = title,
                    FontSize = 24,
                    Margin = new Thickness(0, 0, 0, 20),
                    HorizontalAlignment = HorizontalAlignment.Center
                });
                
                // Dodaj datę raportu
                content.Children.Add(new TextBlock
                {
                    Text = $"Data wygenerowania: {DateTime.Now:d}",
                    FontSize = 12,
                    Margin = new Thickness(0, 0, 0, 30),
                    HorizontalAlignment = HorizontalAlignment.Right
                });
                
                // Dodaj tabelę procedur
                var gridPanel = new Grid();
                gridPanel.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(200) });
                gridPanel.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(100) });
                gridPanel.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(100) });
                gridPanel.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(270) });
                
                // Dodaj nagłówki tabeli
                gridPanel.RowDefinitions.Add(new RowDefinition { Height = new GridLength(40) });
                
                var headerName = new TextBlock
                {
                    Text = "Nazwa procedury",
                    FontWeight = Windows.UI.Text.FontWeights.Bold,
                    VerticalAlignment = VerticalAlignment.Center
                };
                Grid.SetColumn(headerName, 0);
                Grid.SetRow(headerName, 0);
                gridPanel.Children.Add(headerName);
                
                var headerDate = new TextBlock
                {
                    Text = "Data",
                    FontWeight = Windows.UI.Text.FontWeights.Bold,
                    VerticalAlignment = VerticalAlignment.Center
                };
                Grid.SetColumn(headerDate, 1);
                Grid.SetRow(headerDate, 0);
                gridPanel.Children.Add(headerDate);
                
                var headerType = new TextBlock
                {
                    Text = "Typ",
                    FontWeight = Windows.UI.Text.FontWeights.Bold,
                    VerticalAlignment = VerticalAlignment.Center
                };
                Grid.SetColumn(headerType, 2);
                Grid.SetRow(headerType, 0);
                gridPanel.Children.Add(headerType);
                
                var headerLocation = new TextBlock
                {
                    Text = "Miejsce",
                    FontWeight = Windows.UI.Text.FontWeights.Bold,
                    VerticalAlignment = VerticalAlignment.Center
                };
                Grid.SetColumn(headerLocation, 3);
                Grid.SetRow(headerLocation, 0);
                gridPanel.Children.Add(headerLocation);
                
                // Dodaj linie poziome
                var headerLine = new Border
                {
                    Height = 1,
                    Background = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.Black),
                    Margin = new Thickness(0, 5, 0, 5)
                };
                Grid.SetColumnSpan(headerLine, 4);
                Grid.SetRow(headerLine, 0);
                gridPanel.Children.Add(headerLine);
                
                // Dodaj dane procedur
                for (int i = 0; i < procedures.Count; i++)
                {
                    var procedure = procedures[i];
                    gridPanel.RowDefinitions.Add(new RowDefinition { Height = new GridLength(30) });
                    
                    var procName = new TextBlock
                    {
                        Text = procedure.Name,
                        VerticalAlignment = VerticalAlignment.Center
                    };
                    Grid.SetColumn(procName, 0);
                    Grid.SetRow(procName, i + 1);
                    gridPanel.Children.Add(procName);
                    
                    var procDate = new TextBlock
                    {
                        Text = procedure.ExecutionDate.ToString("d"),
                        VerticalAlignment = VerticalAlignment.Center
                    };
                    Grid.SetColumn(procDate, 1);
                    Grid.SetRow(procDate, i + 1);
                    gridPanel.Children.Add(procDate);
                    
                    var procType = new TextBlock
                    {
                        Text = procedure.Type.ToString(),
                        VerticalAlignment = VerticalAlignment.Center
                    };
                    Grid.SetColumn(procType, 2);
                    Grid.SetRow(procType, i + 1);
                    gridPanel.Children.Add(procType);
                    
                    var procLocation = new TextBlock
                    {
                        Text = procedure.Location,
                        VerticalAlignment = VerticalAlignment.Center
                    };
                    Grid.SetColumn(procLocation, 3);
                    Grid.SetRow(procLocation, i + 1);
                    gridPanel.Children.Add(procLocation);
                    
                    // Dodaj linie poziome
                    var rowLine = new Border
                    {
                        Height = 1,
                        Background = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.LightGray),
                        Margin = new Thickness(0, 0, 0, 0)
                    };
                    Grid.SetColumnSpan(rowLine, 4);
                    Grid.SetRow(rowLine, i + 1);
                    gridPanel.Children.Add(rowLine);
                }
                
                content.Children.Add(gridPanel);
                
                // Dodaj stopkę
                content.Children.Add(new TextBlock
                {
                    Text = "Wygenerowano przez: Śledź Speckę",
                    FontSize = 10,
                    Margin = new Thickness(0, 30, 0, 0),
                    HorizontalAlignment = HorizontalAlignment.Center
                });
                
                // Ustaw zawartość do drukowania
                _printContent = content;
                
                // Pokaż interfejs drukowania systemu
                return await ShowPrintUIAsync(title);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error printing procedure report");
                return false;
            }
        }
        
        public async Task<bool> PrintCourseReportAsync(List<Course> courses, string title)
        {
            try
            {
                // Implementacja podobna do PrintProcedureReportAsync, ale dla kursów
                
                // Tutaj dodalibyśmy kod do generowania raportu kursów
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error printing course report");
                return false;
            }
        }
        
        private async Task<bool> PrintPdfAsync(string pdfPath, string documentTitle)
        {
            try
            {
                // Wyświetl systemowy interfejs drukowania i wydrukuj plik PDF
                var printManager = PrintManager.GetForCurrentView();
                
                // Przygotuj źródło dokumentu do drukowania
                printManager.PrintTaskRequested += async (sender, args) =>
                {
                    var deferral = args.Request.GetDeferral();
                    
                    try
                    {
                        var pdfFile = await Windows.Storage.StorageFile.GetFileFromPathAsync(pdfPath);
                        var printTask = args.Request.CreatePrintTask(documentTitle, async (taskSourceRequestedArgs) =>
                        {
                            var pdfSource = Windows.Graphics.Printing.PrintTaskSourceRequestedArgs.GetDeferral();
                            
                            try
                            {
                                // Utwórz źródło dokumentu do drukowania
                                var printDocumentSource = await Windows.Graphics.Printing.PrintTaskSourceRequestedArgs.
                                    SetSource(pdfFile);
                                
                                pdfSource.Complete();
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Error creating print source for PDF");
                                pdfSource.Complete();
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error handling print task requested");
                    }
                    finally
                    {
                        deferral.Complete();
                    }
                };
                
                // Pokaż interfejs drukowania systemu
                bool printResult = await Windows.Graphics.Printing.PrintManager.ShowPrintUIAsync();
                
                // Usuń obsługę zdarzenia
                printManager.PrintTaskRequested -= PrintManager_PrintTaskRequested;
                
                return printResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error printing PDF");
                return false;
            }
        }
        
        private async Task<bool> PrintImageAsync(string imagePath, string documentTitle)
        {
            try
            {
                // Implementacja drukowania obrazu
                // Podobna do drukowania PDF, ale z dodatkową logiką 
                // do przygotowania obrazu do drukowania
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error printing image");
                return false;
            }
        }
        
        private async Task<bool> ShowPrintUIAsync(string documentTitle)
        {
            try
            {
                // Inicjalizacja dokumentu drukowania
                _printDocument = new PrintDocument();
                _printDocumentSource = _printDocument.DocumentSource;
                
                // Obsługa zdarzeń dokumentu drukowania
                _printDocument.Paginate += PrintDocument_Paginate;
                _printDocument.GetPreviewPage += PrintDocument_GetPreviewPage;
                _printDocument.AddPages += PrintDocument_AddPages;
                
                // Rejestracja obsługi żądania drukowania
                _printManager.PrintTaskRequested += async (sender, args) =>
                {
                    var deferral = args.Request.GetDeferral();
                    
                    try
                    {
                        _printTask = args.Request.CreatePrintTask(documentTitle, (taskSourceRequestedArgs) =>
                        {
                            taskSourceRequestedArgs.SetSource(_printDocumentSource);
                        });
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error creating print task");
                    }
                    finally
                    {
                        deferral.Complete();
                    }
                };
                
                // Pokaż interfejs drukowania systemu
                bool printResult = await Windows.Graphics.Printing.PrintManager.ShowPrintUIAsync();
                
                // Usuń obsługę zdarzeń
                _printDocument.Paginate -= PrintDocument_Paginate;
                _printDocument.GetPreviewPage -= PrintDocument_GetPreviewPage;
                _printDocument.AddPages -= PrintDocument_AddPages;
                _printManager.PrintTaskRequested -= PrintManager_PrintTaskRequested;
                
                return printResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error showing print UI");
                return false;
            }
        }
        
        private void PrintManager_PrintTaskRequested(PrintManager sender, PrintTaskRequestedEventArgs args)
        {
            var deferral = args.Request.GetDeferral();
            
            try
            {
                // Utwórz zadanie drukowania
                _printTask = args.Request.CreatePrintTask("SledzSpecke Document", (taskSourceRequestedArgs) =>
                {
                    taskSourceRequestedArgs.SetSource(_printDocumentSource);
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling print task requested");
            }
            finally
            {
                deferral.Complete();
            }
        }
        
        private void PrintDocument_Paginate(object sender, PaginateEventArgs e)
        {
            // Oblicz liczbę stron na podstawie zawartości
            _printDocument.SetPreviewPageCount(1, PreviewPageCountType.Final);
        }
        
        private void PrintDocument_GetPreviewPage(object sender, GetPreviewPageEventArgs e)
        {
            // Przygotuj podgląd strony
            _printDocument.SetPreviewPage(e.PageNumber, _printContent);
        }
        
        private void PrintDocument_AddPages(object sender, AddPagesEventArgs e)
        {
            // Dodaj strony do dokumentu drukowania
            _printDocument.AddPage(_printContent);
            _printDocument.AddPagesComplete();
        }
    }
    
    public interface IPrintService
    {
        Task<bool> PrintDocumentAsync(string documentPath, string documentTitle);
        Task<bool> PrintProcedureReportAsync(List<ProcedureExecution> procedures, string title);
        Task<bool> PrintCourseReportAsync(List<Course> courses, string title);
    }
}
