namespace SledzSpecke.E2E.Tests.Core;

/// <summary>
/// Base interface for all page objects following Interface Segregation Principle
/// </summary>
public interface IPageObject
{
    /// <summary>
    /// Navigates to the page
    /// </summary>
    Task NavigateAsync();
    
    /// <summary>
    /// Waits for the page to be fully loaded
    /// </summary>
    Task WaitForLoadAsync();
    
    /// <summary>
    /// Validates that we're on the correct page
    /// </summary>
    Task<bool> IsCurrentPageAsync();
    
    /// <summary>
    /// Gets the page title
    /// </summary>
    Task<string> GetTitleAsync();
}