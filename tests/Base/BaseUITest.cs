using Microsoft.Playwright;
using NUnit.Framework;
using SDET.Tests.Interfaces;

namespace SDET.Tests.Base;

/// <summary>
/// Base class for UI tests using Playwright.
/// Implements IUITest interface with browser automation capabilities.
/// SOLID: Single Responsibility - Manages browser lifecycle only
/// SOLID: Open/Closed - Open for extension through virtual methods
/// </summary>
[TestFixture]
[Category("UI")]
public abstract class BaseUITest : BaseTest, IUITest
{
    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private IBrowserContext? _context;
    private IPage? _page;

    /// <summary>
    /// Gets the Playwright browser instance
    /// </summary>
    public IBrowser Browser => _browser ?? throw new InvalidOperationException("Browser not initialized. Call InitializeBrowserAsync first.");

    /// <summary>
    /// Gets the current browser context
    /// </summary>
    public IBrowserContext Context => _context ?? throw new InvalidOperationException("Context not initialized. Call InitializeBrowserAsync first.");

    /// <summary>
    /// Gets the current page
    /// </summary>
    public IPage Page => _page ?? throw new InvalidOperationException("Page not initialized. Call InitializeBrowserAsync first.");

    /// <summary>
    /// Gets the base URL from configuration
    /// </summary>
    protected string BaseUrl => TestConfiguration.BaseUrl;

    /// <summary>
    /// Gets the browser type from configuration
    /// </summary>
    protected string BrowserType => TestConfiguration.BrowserType;

    /// <summary>
    /// Gets whether to run in headless mode
    /// </summary>
    protected bool Headless => TestConfiguration.Headless;

    /// <summary>
    /// Extension point for fixture-level setup
    /// </summary>
    protected override void OnFixtureSetUp()
    {
        base.OnFixtureSetUp();
        Logger.Information("Initializing Playwright for UI tests");
    }

    /// <summary>
    /// Extension point for test-level setup
    /// </summary>
    protected override async void OnTestSetUp()
    {
        base.OnTestSetUp();
        await InitializeBrowserAsync(BrowserType, Headless);
        LogStep($"Browser initialized: {BrowserType}, Headless: {Headless}");
    }

    /// <summary>
    /// Extension point for test-level teardown
    /// </summary>
    protected override async void OnTestTearDown()
    {
        base.OnTestTearDown();
        await CloseBrowserAsync();
    }

    /// <summary>
    /// Extension point for test failure handling - captures screenshot
    /// </summary>
    protected override async void OnTestFailure()
    {
        base.OnTestFailure();

        if (_page != null)
        {
            try
            {
                var screenshotPath = await TakeScreenshotAsync($"failure_{TestName}_{DateTime.Now:yyyyMMddHHmmss}");
                Logger.Error($"Screenshot captured: {screenshotPath}");
                TestContext.AddTestAttachment(screenshotPath, "Failure Screenshot");
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to capture screenshot: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Initializes the browser for testing
    /// </summary>
    public async Task InitializeBrowserAsync(string browserType = "chromium", bool headless = true)
    {
        try
        {
            // Create Playwright instance
            _playwright = await Playwright.CreateAsync();

            // Launch browser based on type
            _browser = browserType.ToLower() switch
            {
                "firefox" => await _playwright.Firefox.LaunchAsync(new BrowserTypeLaunchOptions
                {
                    Headless = headless,
                    SlowMo = TestConfiguration.SlowMo
                }),
                "webkit" => await _playwright.Webkit.LaunchAsync(new BrowserTypeLaunchOptions
                {
                    Headless = headless,
                    SlowMo = TestConfiguration.SlowMo
                }),
                _ => await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
                {
                    Headless = headless,
                    SlowMo = TestConfiguration.SlowMo
                })
            };

            // Create browser context
            _context = await _browser.NewContextAsync(new BrowserNewContextOptions
            {
                ViewportSize = new ViewportSize { Width = 1920, Height = 1080 },
                AcceptDownloads = true,
                RecordVideoDir = TestConfiguration.RecordVideo ? "videos/" : null
            });

            // Create page
            _page = await _context.NewPageAsync();

            Logger.Information($"Browser initialized successfully: {browserType}");
        }
        catch (Exception ex)
        {
            Logger.Error($"Failed to initialize browser: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Takes a screenshot of the current page
    /// </summary>
    public async Task<string> TakeScreenshotAsync(string filename)
    {
        var screenshotsDir = "screenshots";
        Directory.CreateDirectory(screenshotsDir);

        var filepath = Path.Combine(screenshotsDir, $"{filename}.png");
        await Page.ScreenshotAsync(new PageScreenshotOptions
        {
            Path = filepath,
            FullPage = true
        });

        Logger.Information($"Screenshot saved: {filepath}");
        return filepath;
    }

    /// <summary>
    /// Closes the browser and cleans up resources
    /// </summary>
    public async Task CloseBrowserAsync()
    {
        try
        {
            if (_page != null)
            {
                await _page.CloseAsync();
                _page = null;
            }

            if (_context != null)
            {
                await _context.CloseAsync();
                _context = null;
            }

            if (_browser != null)
            {
                await _browser.CloseAsync();
                _browser = null;
            }

            _playwright?.Dispose();
            _playwright = null;

            Logger.Information("Browser closed successfully");
        }
        catch (Exception ex)
        {
            Logger.Error($"Error closing browser: {ex.Message}");
        }
    }

    /// <summary>
    /// Helper method to navigate to a URL
    /// </summary>
    protected async Task NavigateToAsync(string url)
    {
        LogStep($"Navigating to: {url}");
        await Page.GotoAsync(url, new PageGotoOptions
        {
            WaitUntil = WaitUntilState.NetworkIdle,
            Timeout = TestConfiguration.Timeout
        });
        LogStep($"Successfully navigated to: {url}");
    }

    /// <summary>
    /// Helper method to wait for an element
    /// </summary>
    protected async Task WaitForElementAsync(string selector, int timeout = 5000)
    {
        await Page.WaitForSelectorAsync(selector, new PageWaitForSelectorOptions
        {
            Timeout = timeout
        });
    }

    /// <summary>
    /// Helper method to click an element
    /// </summary>
    protected async Task ClickAsync(string selector)
    {
        LogStep($"Clicking element: {selector}");
        await Page.ClickAsync(selector);
    }

    /// <summary>
    /// Helper method to fill a text input
    /// </summary>
    protected async Task FillAsync(string selector, string value)
    {
        LogStep($"Filling '{selector}' with value: {value}");
        await Page.FillAsync(selector, value);
    }

    /// <summary>
    /// Helper method to get text from an element
    /// </summary>
    protected async Task<string> GetTextAsync(string selector)
    {
        var text = await Page.TextContentAsync(selector);
        return text ?? string.Empty;
    }

    /// <summary>
    /// Helper method to check if element is visible
    /// </summary>
    protected async Task<bool> IsVisibleAsync(string selector)
    {
        return await Page.IsVisibleAsync(selector);
    }
}

/// <summary>
/// Test configuration helper class
/// </summary>
internal static partial class TestConfiguration
{
    public static string BaseUrl => "http://localhost:5001";
    public static string BrowserType => "chromium";
    public static bool Headless => true;
    public static int SlowMo => 0;
    public static int Timeout => 30000;
    public static bool RecordVideo => false;
}
