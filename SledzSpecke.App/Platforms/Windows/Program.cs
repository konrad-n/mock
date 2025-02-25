using Microsoft.UI.Xaml;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SledzSpecke.App.WinUI;

/// <summary>
/// Provides a Windows-specific entry point for your app.
/// </summary>
public static class Program
{
    /// <summary>
    /// Initialization code. Don't use any Avalonia, third-party APIs or any
    /// SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    /// yet and stuff might break.
    /// </summary>
    [STAThread]
    public static void Main(string[] args)
    {
        Microsoft.UI.Xaml.Application.Start((p) => {
            var app = new App();
        });
    }
}
