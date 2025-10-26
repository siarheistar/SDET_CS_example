using Microsoft.Playwright;

namespace SDET.Tests.Interfaces;

/// <summary>
/// Base interface for all Page Objects.
/// Defines the contract for page interaction and navigation.
/// SOLID: Dependency Inversion - Depend on abstractions, not concrete pages
/// </summary>
public interface IPageObject
{
    /// <summary>
    /// Gets the Playwright page instance
    /// </summary>
    IPage Page { get; }

    /// <summary>
    /// Gets the base URL of the application
    /// </summary>
    string BaseUrl { get; }

    /// <summary>
    /// Gets the relative path for this page
    /// </summary>
    string PagePath { get; }

    /// <summary>
    /// Navigates to this page
    /// </summary>
    Task NavigateToAsync();

    /// <summary>
    /// Checks if the page is currently loaded
    /// </summary>
    Task<bool> IsLoadedAsync();

    /// <summary>
    /// Waits for the page to be fully loaded
    /// </summary>
    /// <param name="timeout">Timeout in milliseconds</param>
    Task WaitForPageLoadAsync(int timeout = 30000);

    /// <summary>
    /// Gets the current page title
    /// </summary>
    Task<string> GetPageTitleAsync();

    /// <summary>
    /// Gets the current page URL
    /// </summary>
    string GetCurrentUrl();
}
