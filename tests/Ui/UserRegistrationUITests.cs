using FluentAssertions;
using NUnit.Framework;
using SDET.Tests.Base;
using SDET.Tests.Factories;
using SDET.Tests.Pages;

namespace SDET.Tests.Ui;

/// <summary>
/// UI tests for User Registration functionality.
/// Tests user registration through the web interface using Playwright.
/// </summary>
[TestFixture]
[Category("UI")]
[Category("Smoke")]
[Explicit("Requires Playwright browser setup. Use --filter 'TestCategory=UI' to run.")]
public class UserRegistrationUITests : BaseUITest
{
    private TestDataFactory _testDataFactory = null!;
    private HomePage _homePage = null!;

    protected override void OnFixtureSetUp()
    {
        base.OnFixtureSetUp();
        _testDataFactory = new TestDataFactory();
    }

    protected override async void OnTestSetUp()
    {
        base.OnTestSetUp();

        _homePage = new HomePage(Page, BaseUrl);
        await _homePage.NavigateToAsync();

        LogStep("Verifying home page loaded");
        var isLoaded = await _homePage.IsLoadedAsync();
        isLoaded.Should().BeTrue("home page should be loaded");
    }

    [Test]
    public async Task RegisterUser_WithValidData_ShowsSuccessMessage()
    {
        // Arrange
        LogStep("Creating valid user test data");
        var userData = _testDataFactory.CreateValidUser();
        LogTestData("User Data", userData);

        // Act
        LogStep($"Registering user: {userData.Username}");
        await _homePage.RegisterUserAsync(
            userData.Username,
            userData.Email,
            userData.Password
        );

        // Assert
        LogAssertion("Success message should be visible");
        await _homePage.WaitForResultMessageAsync(timeout: 5000);

        var isVisible = await _homePage.IsResultMessageVisibleAsync();
        isVisible.Should().BeTrue("success message should be displayed");

        var message = await _homePage.GetResultMessageAsync();
        LogAssertion($"Message received: {message}");

        message.Should().Contain("successfully", "success message should contain 'successfully'");
    }

    [Test]
    [TestCase("ab", "valid@example.com", "ValidPass123!", "at least 3 characters", Description = "Username too short")]
    [TestCase("validuser", "invalid-email", "ValidPass123!", "valid email", Description = "Invalid email")]
    public async Task RegisterUser_WithInvalidData_ShowsErrorMessage(
        string username, string email, string password, string expectedError)
    {
        // Act
        LogStep($"Attempting registration with invalid data: {username}, {email}");
        await _homePage.RegisterUserAsync(username, email, password);

        // Assert
        LogAssertion("Error message should be visible");
        await _homePage.WaitForResultMessageAsync(timeout: 5000);

        var message = await _homePage.GetResultMessageAsync();
        LogAssertion($"Error message received: {message}");

        message.Should().Contain(expectedError, $"error message should mention '{expectedError}'");
    }

    [Test]
    public async Task RegistrationForm_AllFieldsVisible()
    {
        // Act & Assert
        LogAssertion("All registration form fields should be visible");
        var isVisible = await _homePage.IsRegistrationFormVisibleAsync();
        isVisible.Should().BeTrue("registration form should be visible with all fields");
    }
}
