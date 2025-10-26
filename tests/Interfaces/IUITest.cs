using Microsoft.Playwright;

namespace SDET.Tests.Interfaces;

/// <summary>
/// Interface for UI-based tests using Playwright.
/// Extends ITest with UI-specific capabilities.
/// SOLID: Interface Segregation - UI tests get only UI-related methods
/// </summary>
public interface IUITest : ITest
{
    /// <summary>
    /// Gets the Playwright browser instance
    /// </summary>
    IBrowser Browser { get; }

    /// <summary>
    /// Gets the current browser context
    /// </summary>
    IBrowserContext Context { get; }

    /// <summary>
    /// Gets the current page
    /// </summary>
    IPage Page { get; }

    /// <summary>
    /// Initializes the browser for testing
    /// </summary>
    /// <param name="browserType">Type of browser (chromium, firefox, webkit)</param>
    /// <param name="headless">Whether to run in headless mode</param>
    Task InitializeBrowserAsync(string browserType = "chromium", bool headless = true);

    /// <summary>
    /// Takes a screenshot of the current page
    /// </summary>
    /// <param name="filename">Name of the screenshot file</param>
    Task<string> TakeScreenshotAsync(string filename);

    /// <summary>
    /// Closes the browser and cleans up resources
    /// </summary>
    Task CloseBrowserAsync();
}
