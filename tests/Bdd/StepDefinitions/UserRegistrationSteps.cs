using FluentAssertions;
using NUnit.Framework;
using SDET.Tests.Base;
using SDET.Tests.Factories;
using SDET.Tests.Interfaces;
using SDET.Tests.Pages;
using TechTalk.SpecFlow;

namespace SDET.Tests.Bdd.StepDefinitions;

/// <summary>
/// Step definitions for User Registration feature.
/// Implements BDD steps for both UI and API testing.
/// SOLID: Single Responsibility - Only handles user registration steps
/// </summary>
[Binding]
public class UserRegistrationSteps
{
    private readonly ScenarioContext _scenarioContext;
    private readonly ITestDataFactory _testDataFactory;
    private HomePage? _homePage;
    private string _resultMessage = string.Empty;

    /// <summary>
    /// Initializes a new instance of the UserRegistrationSteps class
    /// </summary>
    public UserRegistrationSteps(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
        _testDataFactory = new TestDataFactory();
    }

    // ==================== Given Steps ====================

    [Given(@"I am on the home page")]
    public async Task GivenIAmOnTheHomePage()
    {
        // Get page from scenario context (injected by test hook)
        var page = _scenarioContext.Get<Microsoft.Playwright.IPage>("Page");
        _homePage = new HomePage(page);

        await _homePage.NavigateToAsync();
        await _homePage.WaitForPageLoadAsync();

        // Verify we're on the correct page
        var isLoaded = await _homePage.IsLoadedAsync();
        isLoaded.Should().BeTrue("Home page should be loaded");
    }

    [Given(@"the registration form is visible")]
    public async Task GivenTheRegistrationFormIsVisible()
    {
        _homePage.Should().NotBeNull("Home page should be initialized");

        var isVisible = await _homePage!.IsRegistrationFormVisibleAsync();
        isVisible.Should().BeTrue("Registration form should be visible");
    }

    // ==================== When Steps ====================

    [When(@"I register with the following details:")]
    public async Task WhenIRegisterWithTheFollowingDetails(Table table)
    {
        _homePage.Should().NotBeNull("Home page should be initialized");

        var username = table.Rows[0]["Value"];
        var email = table.Rows[1]["Value"];
        var password = table.Rows[2]["Value"];

        await _homePage!.RegisterUserAsync(username, email, password);

        // Store data in scenario context for later verification
        _scenarioContext["Username"] = username;
        _scenarioContext["Email"] = email;
    }

    [When(@"I register user ""(.*)"" with email ""(.*)"" and password ""(.*)""")]
    public async Task WhenIRegisterUserWithEmailAndPassword(string username, string email, string password)
    {
        _homePage.Should().NotBeNull("Home page should be initialized");

        await _homePage!.RegisterUserAsync(username, email, password);

        _scenarioContext["Username"] = username;
        _scenarioContext["Email"] = email;
    }

    [When(@"I register with username ""(.*)"" and email ""(.*)"" and password ""(.*)""")]
    public async Task WhenIRegisterWithUsernameAndEmailAndPassword(string username, string email, string password)
    {
        _homePage.Should().NotBeNull("Home page should be initialized");

        await _homePage!.RegisterUserAsync(username, email, password);
    }

    // ==================== Then Steps ====================

    [Then(@"I should see a success message")]
    public async Task ThenIShouldSeeASuccessMessage()
    {
        _homePage.Should().NotBeNull("Home page should be initialized");

        await _homePage!.WaitForResultMessageAsync(timeout: 5000);

        var isVisible = await _homePage.IsResultMessageVisibleAsync();
        isVisible.Should().BeTrue("Success message should be visible");

        _resultMessage = await _homePage.GetResultMessageAsync();
        _resultMessage.Should().NotBeNullOrEmpty("Success message should have content");
    }

    [Then(@"the success message should contain ""(.*)""")]
    public void ThenTheSuccessMessageShouldContain(string expectedText)
    {
        _resultMessage.Should().NotBeNullOrEmpty("Result message should be captured");
        _resultMessage.Should().Contain(expectedText, $"Success message should contain '{expectedText}'");
    }

    [Then(@"I should see an error message")]
    public async Task ThenIShouldSeeAnErrorMessage()
    {
        _homePage.Should().NotBeNull("Home page should be initialized");

        await _homePage!.WaitForResultMessageAsync(timeout: 5000);

        var isVisible = await _homePage.IsResultMessageVisibleAsync();
        isVisible.Should().BeTrue("Error message should be visible");

        _resultMessage = await _homePage.GetResultMessageAsync();
        _resultMessage.Should().NotBeNullOrEmpty("Error message should have content");
    }

    [Then(@"the error message should contain ""(.*)""")]
    public void ThenTheErrorMessageShouldContain(string expectedText)
    {
        _resultMessage.Should().NotBeNullOrEmpty("Result message should be captured");
        _resultMessage.Should().Contain(expectedText, $"Error message should contain '{expectedText}'");
    }

    // ==================== Accessibility Steps ====================

    [Then(@"the username input should have a label or placeholder")]
    public async Task ThenTheUsernameInputShouldHaveALabelOrPlaceholder()
    {
        _homePage.Should().NotBeNull("Home page should be initialized");

        var placeholder = await _homePage!.Page.GetAttributeAsync("[data-testid='username-input']", "placeholder");
        var ariaLabel = await _homePage.Page.GetAttributeAsync("[data-testid='username-input']", "aria-label");

        (placeholder != null || ariaLabel != null).Should().BeTrue("Username input should have placeholder or aria-label");
    }

    [Then(@"the email input should have a label or placeholder")]
    public async Task ThenTheEmailInputShouldHaveALabelOrPlaceholder()
    {
        _homePage.Should().NotBeNull("Home page should be initialized");

        var placeholder = await _homePage!.Page.GetAttributeAsync("[data-testid='email-input']", "placeholder");
        var ariaLabel = await _homePage.Page.GetAttributeAsync("[data-testid='email-input']", "aria-label");

        (placeholder != null || ariaLabel != null).Should().BeTrue("Email input should have placeholder or aria-label");
    }

    [Then(@"the password input should have a label or placeholder")]
    public async Task ThenThePasswordInputShouldHaveALabelOrPlaceholder()
    {
        _homePage.Should().NotBeNull("Home page should be initialized");

        var placeholder = await _homePage!.Page.GetAttributeAsync("[data-testid='password-input']", "placeholder");
        var ariaLabel = await _homePage.Page.GetAttributeAsync("[data-testid='password-input']", "aria-label");

        (placeholder != null || ariaLabel != null).Should().BeTrue("Password input should have placeholder or aria-label");
    }

    [Then(@"the register button should be keyboard accessible")]
    public async Task ThenTheRegisterButtonShouldBeKeyboardAccessible()
    {
        _homePage.Should().NotBeNull("Home page should be initialized");

        var tabIndex = await _homePage!.Page.GetAttributeAsync("[data-testid='register-button']", "tabindex");
        var isEnabled = await _homePage.Page.IsEnabledAsync("[data-testid='register-button']");

        isEnabled.Should().BeTrue("Register button should be enabled");
        // Button should either not have tabindex (naturally focusable) or have tabindex >= 0
        if (tabIndex != null)
        {
            int.TryParse(tabIndex, out var index).Should().BeTrue();
            index.Should().BeGreaterThanOrEqualTo(0, "Tab index should be non-negative");
        }
    }

    // ==================== Responsive Design Steps ====================

    [When(@"I resize the browser to mobile size")]
    public async Task WhenIResizeTheBrowserToMobileSize()
    {
        _homePage.Should().NotBeNull("Home page should be initialized");
        await _homePage!.Page.SetViewportSizeAsync(375, 667); // iPhone SE size
    }

    [When(@"I resize the browser to tablet size")]
    public async Task WhenIResizeTheBrowserToTabletSize()
    {
        _homePage.Should().NotBeNull("Home page should be initialized");
        await _homePage!.Page.SetViewportSizeAsync(768, 1024); // iPad size
    }

    [When(@"I resize the browser to desktop size")]
    public async Task WhenIResizeTheBrowserToDesktopSize()
    {
        _homePage.Should().NotBeNull("Home page should be initialized");
        await _homePage!.Page.SetViewportSizeAsync(1920, 1080); // Full HD
    }

    [Then(@"the registration form should still be visible")]
    public async Task ThenTheRegistrationFormShouldStillBeVisible()
    {
        _homePage.Should().NotBeNull("Home page should be initialized");

        await Task.Delay(500); // Allow time for responsive layout to adjust

        var isVisible = await _homePage!.IsRegistrationFormVisibleAsync();
        isVisible.Should().BeTrue("Registration form should remain visible after resize");
    }
}
