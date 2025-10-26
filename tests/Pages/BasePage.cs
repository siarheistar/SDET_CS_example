using Microsoft.Playwright;
using Serilog;
using SDET.Tests.Interfaces;

namespace SDET.Tests.Pages;

/// <summary>
/// Base class for all Page Objects.
/// Implements IPageObject interface with common page interaction methods.
/// SOLID: Single Responsibility - Manages page interactions only
/// SOLID: Open/Closed - Open for extension via virtual methods
/// SOLID: Dependency Inversion - Depends on IPage abstraction
/// </summary>
public abstract class BasePage : IPageObject
{
    protected readonly ILogger Logger;

    /// <summary>
    /// Initializes a new instance of the BasePage class
    /// </summary>
    /// <param name="page">Playwright page instance</param>
    /// <param name="baseUrl">Base URL of the application</param>
    protected BasePage(IPage page, string baseUrl = "http://localhost:5001")
    {
        Page = page ?? throw new ArgumentNullException(nameof(page));
        BaseUrl = baseUrl;
        Logger = Log.ForContext(GetType());
    }

    /// <summary>
    /// Gets the Playwright page instance
    /// </summary>
    public IPage Page { get; }

    /// <summary>
    /// Gets the base URL of the application
    /// </summary>
    public string BaseUrl { get; }

    /// <summary>
    /// Gets the relative path for this page (must be implemented by derived classes)
    /// </summary>
    public abstract string PagePath { get; }

    /// <summary>
    /// Gets the full URL for this page
    /// </summary>
    protected string FullUrl => $"{BaseUrl}{PagePath}";

    /// <summary>
    /// Navigates to this page
    /// </summary>
    public virtual async Task NavigateToAsync()
    {
        Logger.Information($"Navigating to page: {FullUrl}");

        await Page.GotoAsync(FullUrl, new PageGotoOptions
        {
            WaitUntil = WaitUntilState.NetworkIdle,
            Timeout = 30000
        });

        await WaitForPageLoadAsync();

        Logger.Information($"Successfully navigated to: {FullUrl}");
    }

    /// <summary>
    /// Checks if the page is currently loaded
    /// </summary>
    public virtual async Task<bool> IsLoadedAsync()
    {
        try
        {
            var currentUrl = Page.Url;
            var isCorrectPage = currentUrl.Contains(PagePath);

            Logger.Debug($"Page loaded check: Current URL = {currentUrl}, Expected Path = {PagePath}, Match = {isCorrectPage}");

            return isCorrectPage;
        }
        catch (Exception ex)
        {
            Logger.Error($"Error checking if page is loaded: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Waits for the page to be fully loaded
    /// </summary>
    public virtual async Task WaitForPageLoadAsync(int timeout = 30000)
    {
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle, new PageWaitForLoadStateOptions
        {
            Timeout = timeout
        });

        Logger.Debug("Page load state: NetworkIdle");
    }

    /// <summary>
    /// Gets the current page title
    /// </summary>
    public virtual async Task<string> GetPageTitleAsync()
    {
        return await Page.TitleAsync();
    }

    /// <summary>
    /// Gets the current page URL
    /// </summary>
    public virtual string GetCurrentUrl()
    {
        return Page.Url;
    }

    // ==================== Element Interaction Methods ====================

    /// <summary>
    /// Waits for an element to be visible
    /// </summary>
    protected async Task WaitForElementAsync(string selector, int timeout = 5000)
    {
        Logger.Debug($"Waiting for element: {selector}");

        await Page.WaitForSelectorAsync(selector, new PageWaitForSelectorOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = timeout
        });
    }

    /// <summary>
    /// Gets an element locator by selector
    /// </summary>
    protected ILocator GetElement(string selector)
    {
        return Page.Locator(selector);
    }

    /// <summary>
    /// Gets an element locator by test ID
    /// </summary>
    protected ILocator GetElementByTestId(string testId)
    {
        return Page.GetByTestId(testId);
    }

    /// <summary>
    /// Clicks an element
    /// </summary>
    protected async Task ClickAsync(string selector)
    {
        Logger.Debug($"Clicking element: {selector}");

        await WaitForElementAsync(selector);
        await Page.ClickAsync(selector);

        Logger.Debug($"Clicked element: {selector}");
    }

    /// <summary>
    /// Clicks an element by test ID
    /// </summary>
    protected async Task ClickByTestIdAsync(string testId)
    {
        Logger.Debug($"Clicking element with test-id: {testId}");

        var element = GetElementByTestId(testId);
        await element.ClickAsync();

        Logger.Debug($"Clicked element with test-id: {testId}");
    }

    /// <summary>
    /// Fills a text input
    /// </summary>
    protected async Task FillAsync(string selector, string value)
    {
        Logger.Debug($"Filling element '{selector}' with value: {value}");

        await WaitForElementAsync(selector);
        await Page.FillAsync(selector, value);

        Logger.Debug($"Filled element: {selector}");
    }

    /// <summary>
    /// Fills a text input by test ID
    /// </summary>
    protected async Task FillByTestIdAsync(string testId, string value)
    {
        Logger.Debug($"Filling element with test-id '{testId}' with value: {value}");

        var element = GetElementByTestId(testId);
        await element.FillAsync(value);

        Logger.Debug($"Filled element with test-id: {testId}");
    }

    /// <summary>
    /// Gets text content from an element
    /// </summary>
    protected async Task<string> GetTextAsync(string selector)
    {
        await WaitForElementAsync(selector);
        var text = await Page.TextContentAsync(selector);
        return text ?? string.Empty;
    }

    /// <summary>
    /// Gets text content from an element by test ID
    /// </summary>
    protected async Task<string> GetTextByTestIdAsync(string testId)
    {
        var element = GetElementByTestId(testId);
        var text = await element.TextContentAsync();
        return text ?? string.Empty;
    }

    /// <summary>
    /// Checks if an element is visible
    /// </summary>
    protected async Task<bool> IsVisibleAsync(string selector)
    {
        return await Page.IsVisibleAsync(selector);
    }

    /// <summary>
    /// Checks if an element by test ID is visible
    /// </summary>
    protected async Task<bool> IsVisibleByTestIdAsync(string testId)
    {
        var element = GetElementByTestId(testId);
        return await element.IsVisibleAsync();
    }

    /// <summary>
    /// Checks if an element is enabled
    /// </summary>
    protected async Task<bool> IsEnabledAsync(string selector)
    {
        return await Page.IsEnabledAsync(selector);
    }

    /// <summary>
    /// Selects an option from a dropdown by value
    /// </summary>
    protected async Task SelectOptionAsync(string selector, string value)
    {
        Logger.Debug($"Selecting option '{value}' from: {selector}");

        await WaitForElementAsync(selector);
        await Page.SelectOptionAsync(selector, value);

        Logger.Debug($"Selected option: {value}");
    }

    /// <summary>
    /// Gets attribute value from an element
    /// </summary>
    protected async Task<string?> GetAttributeAsync(string selector, string attributeName)
    {
        await WaitForElementAsync(selector);
        return await Page.GetAttributeAsync(selector, attributeName);
    }

    /// <summary>
    /// Checks a checkbox
    /// </summary>
    protected async Task CheckAsync(string selector)
    {
        Logger.Debug($"Checking checkbox: {selector}");

        await WaitForElementAsync(selector);
        await Page.CheckAsync(selector);

        Logger.Debug($"Checked checkbox: {selector}");
    }

    /// <summary>
    /// Unchecks a checkbox
    /// </summary>
    protected async Task UncheckAsync(string selector)
    {
        Logger.Debug($"Unchecking checkbox: {selector}");

        await WaitForElementAsync(selector);
        await Page.UncheckAsync(selector);

        Logger.Debug($"Unchecked checkbox: {selector}");
    }

    /// <summary>
    /// Takes a screenshot of the page
    /// </summary>
    protected async Task<string> TakeScreenshotAsync(string filename)
    {
        var screenshotsDir = "screenshots";
        Directory.CreateDirectory(screenshotsDir);

        var filepath = Path.Combine(screenshotsDir, $"{filename}_{DateTime.Now:yyyyMMddHHmmss}.png");

        await Page.ScreenshotAsync(new PageScreenshotOptions
        {
            Path = filepath,
            FullPage = true
        });

        Logger.Information($"Screenshot saved: {filepath}");
        return filepath;
    }

    /// <summary>
    /// Waits for navigation to complete
    /// </summary>
    protected async Task WaitForNavigationAsync(Func<Task> action)
    {
        await Page.RunAndWaitForNavigationAsync(action, new PageRunAndWaitForNavigationOptions
        {
            WaitUntil = WaitUntilState.NetworkIdle
        });
    }

    /// <summary>
    /// Executes JavaScript on the page
    /// </summary>
    protected async Task<T> EvaluateAsync<T>(string script)
    {
        return await Page.EvaluateAsync<T>(script);
    }

    /// <summary>
    /// Scrolls to an element
    /// </summary>
    protected async Task ScrollToElementAsync(string selector)
    {
        await WaitForElementAsync(selector);
        await Page.Locator(selector).ScrollIntoViewIfNeededAsync();
        Logger.Debug($"Scrolled to element: {selector}");
    }
}
