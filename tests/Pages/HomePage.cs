using Microsoft.Playwright;

namespace SDET.Tests.Pages;

/// <summary>
/// Page Object for the Home/Landing page.
/// Encapsulates all interactions with the home page.
/// SOLID: Single Responsibility - Only manages Home page interactions
/// </summary>
public class HomePage : BasePage
{
    // ==================== Page Path ====================

    public override string PagePath => "/";

    // ==================== Locators - User Registration Section ====================

    private const string UsernameInput = "[data-testid='username-input']";
    private const string EmailInput = "[data-testid='email-input']";
    private const string PasswordInput = "[data-testid='password-input']";
    private const string RegisterButton = "[data-testid='register-button']";

    // ==================== Locators - User Login Section ====================

    private const string LoginUsernameInput = "[data-testid='login-username-input']";
    private const string LoginPasswordInput = "[data-testid='login-password-input']";
    private const string LoginButton = "[data-testid='login-button']";

    // ==================== Locators - Products Section ====================

    private const string LoadProductsButton = "[data-testid='load-products-button']";
    private const string ProductsList = "#products-list";

    // ==================== Locators - Create Order Section ====================

    private const string ProductIdInput = "[data-testid='product-id-input']";
    private const string QuantityInput = "[data-testid='quantity-input']";
    private const string CreateOrderButton = "[data-testid='create-order-button']";

    // ==================== Locators - Calculator Section ====================

    private const string Num1Input = "[data-testid='num1-input']";
    private const string Num2Input = "[data-testid='num2-input']";
    private const string OperationSelect = "[data-testid='operation-select']";
    private const string CalculateButton = "[data-testid='calculate-button']";

    // ==================== Locators - Result Message ====================

    private const string ResultMessage = "[data-testid='result-message']";

    // ==================== Constructor ====================

    /// <summary>
    /// Initializes a new instance of the HomePage class
    /// </summary>
    /// <param name="page">Playwright page instance</param>
    /// <param name="baseUrl">Base URL of the application</param>
    public HomePage(IPage page, string baseUrl = "http://localhost:5001")
        : base(page, baseUrl)
    {
    }

    // ==================== User Registration Actions ====================

    /// <summary>
    /// Registers a new user
    /// </summary>
    /// <param name="username">Username</param>
    /// <param name="email">Email address</param>
    /// <param name="password">Password</param>
    public async Task RegisterUserAsync(string username, string email, string password)
    {
        Logger.Information($"Registering user: {username}, {email}");

        await FillAsync(UsernameInput, username);
        await FillAsync(EmailInput, email);
        await FillAsync(PasswordInput, password);
        await ClickAsync(RegisterButton);

        Logger.Information($"User registration submitted for: {username}");
    }

    /// <summary>
    /// Fills the username field
    /// </summary>
    public async Task FillUsernameAsync(string username)
    {
        await FillAsync(UsernameInput, username);
    }

    /// <summary>
    /// Fills the email field
    /// </summary>
    public async Task FillEmailAsync(string email)
    {
        await FillAsync(EmailInput, email);
    }

    /// <summary>
    /// Fills the password field
    /// </summary>
    public async Task FillPasswordAsync(string password)
    {
        await FillAsync(PasswordInput, password);
    }

    /// <summary>
    /// Clicks the register button
    /// </summary>
    public async Task ClickRegisterButtonAsync()
    {
        await ClickAsync(RegisterButton);
    }

    /// <summary>
    /// Checks if registration form is visible
    /// </summary>
    public async Task<bool> IsRegistrationFormVisibleAsync()
    {
        return await IsVisibleAsync(UsernameInput) &&
               await IsVisibleAsync(EmailInput) &&
               await IsVisibleAsync(PasswordInput) &&
               await IsVisibleAsync(RegisterButton);
    }

    // ==================== User Login Actions ====================

    /// <summary>
    /// Logs in a user
    /// </summary>
    /// <param name="username">Username</param>
    /// <param name="password">Password</param>
    public async Task LoginUserAsync(string username, string password)
    {
        Logger.Information($"Logging in user: {username}");

        await FillAsync(LoginUsernameInput, username);
        await FillAsync(LoginPasswordInput, password);
        await ClickAsync(LoginButton);

        Logger.Information($"User login submitted for: {username}");
    }

    /// <summary>
    /// Fills the login username field
    /// </summary>
    public async Task FillLoginUsernameAsync(string username)
    {
        await FillAsync(LoginUsernameInput, username);
    }

    /// <summary>
    /// Fills the login password field
    /// </summary>
    public async Task FillLoginPasswordAsync(string password)
    {
        await FillAsync(LoginPasswordInput, password);
    }

    /// <summary>
    /// Clicks the login button
    /// </summary>
    public async Task ClickLoginButtonAsync()
    {
        await ClickAsync(LoginButton);
    }

    /// <summary>
    /// Checks if login form is visible
    /// </summary>
    public async Task<bool> IsLoginFormVisibleAsync()
    {
        return await IsVisibleAsync(LoginUsernameInput) &&
               await IsVisibleAsync(LoginPasswordInput) &&
               await IsVisibleAsync(LoginButton);
    }

    // ==================== Products Actions ====================

    /// <summary>
    /// Loads the products list
    /// </summary>
    public async Task LoadProductsAsync()
    {
        Logger.Information("Loading products");

        await ClickAsync(LoadProductsButton);
        await WaitForElementAsync(ProductsList);

        Logger.Information("Products loaded");
    }

    /// <summary>
    /// Gets the products list
    /// </summary>
    public async Task<string> GetProductsListAsync()
    {
        await WaitForElementAsync(ProductsList);
        return await GetTextAsync(ProductsList);
    }

    /// <summary>
    /// Checks if products are loaded
    /// </summary>
    public async Task<bool> AreProductsLoadedAsync()
    {
        return await IsVisibleAsync(ProductsList);
    }

    // ==================== Create Order Actions ====================

    /// <summary>
    /// Creates a new order
    /// </summary>
    /// <param name="productId">Product ID</param>
    /// <param name="quantity">Quantity</param>
    public async Task CreateOrderAsync(int productId, int quantity)
    {
        Logger.Information($"Creating order: Product ID = {productId}, Quantity = {quantity}");

        await FillAsync(ProductIdInput, productId.ToString());
        await FillAsync(QuantityInput, quantity.ToString());
        await ClickAsync(CreateOrderButton);

        Logger.Information("Order creation submitted");
    }

    /// <summary>
    /// Fills the product ID field
    /// </summary>
    public async Task FillProductIdAsync(int productId)
    {
        await FillAsync(ProductIdInput, productId.ToString());
    }

    /// <summary>
    /// Fills the quantity field
    /// </summary>
    public async Task FillQuantityAsync(int quantity)
    {
        await FillAsync(QuantityInput, quantity.ToString());
    }

    /// <summary>
    /// Clicks the create order button
    /// </summary>
    public async Task ClickCreateOrderButtonAsync()
    {
        await ClickAsync(CreateOrderButton);
    }

    /// <summary>
    /// Checks if create order form is visible
    /// </summary>
    public async Task<bool> IsCreateOrderFormVisibleAsync()
    {
        return await IsVisibleAsync(ProductIdInput) &&
               await IsVisibleAsync(QuantityInput) &&
               await IsVisibleAsync(CreateOrderButton);
    }

    // ==================== Calculator Actions ====================

    /// <summary>
    /// Performs a calculation
    /// </summary>
    /// <param name="num1">First number</param>
    /// <param name="num2">Second number</param>
    /// <param name="operation">Operation (add, subtract, multiply, divide)</param>
    public async Task PerformCalculationAsync(double num1, double num2, string operation)
    {
        Logger.Information($"Performing calculation: {num1} {operation} {num2}");

        await FillAsync(Num1Input, num1.ToString());
        await FillAsync(Num2Input, num2.ToString());
        await SelectOptionAsync(OperationSelect, operation);
        await ClickAsync(CalculateButton);

        Logger.Information("Calculation submitted");
    }

    /// <summary>
    /// Checks if calculator is visible
    /// </summary>
    public async Task<bool> IsCalculatorVisibleAsync()
    {
        return await IsVisibleAsync(Num1Input) &&
               await IsVisibleAsync(Num2Input) &&
               await IsVisibleAsync(OperationSelect) &&
               await IsVisibleAsync(CalculateButton);
    }

    // ==================== Result Message Actions ====================

    /// <summary>
    /// Gets the result message text
    /// </summary>
    public async Task<string> GetResultMessageAsync()
    {
        await WaitForElementAsync(ResultMessage, timeout: 3000);
        return await GetTextAsync(ResultMessage);
    }

    /// <summary>
    /// Checks if result message is visible
    /// </summary>
    public async Task<bool> IsResultMessageVisibleAsync()
    {
        return await IsVisibleAsync(ResultMessage);
    }

    /// <summary>
    /// Waits for result message to appear
    /// </summary>
    /// <param name="timeout">Timeout in milliseconds</param>
    public async Task WaitForResultMessageAsync(int timeout = 5000)
    {
        await WaitForElementAsync(ResultMessage, timeout);
    }

    // ==================== Page Verification ====================

    /// <summary>
    /// Verifies all main sections are visible
    /// </summary>
    public async Task<bool> VerifyAllSectionsVisibleAsync()
    {
        var registrationVisible = await IsRegistrationFormVisibleAsync();
        var loginVisible = await IsLoginFormVisibleAsync();
        var orderVisible = await IsCreateOrderFormVisibleAsync();
        var calculatorVisible = await IsCalculatorVisibleAsync();

        Logger.Information($"Sections visibility - Registration: {registrationVisible}, " +
                           $"Login: {loginVisible}, Order: {orderVisible}, Calculator: {calculatorVisible}");

        return registrationVisible && loginVisible && orderVisible && calculatorVisible;
    }
}
