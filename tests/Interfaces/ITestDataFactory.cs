namespace SDET.Tests.Interfaces;

/// <summary>
/// Interface for test data factory.
/// Provides methods to create test data objects.
/// SOLID: Single Responsibility - Only concerned with test data creation
/// SOLID: Dependency Inversion - Clients depend on this abstraction
/// </summary>
public interface ITestDataFactory
{
    /// <summary>
    /// Creates a valid user test data object
    /// </summary>
    UserTestData CreateValidUser();

    /// <summary>
    /// Creates an invalid user test data object
    /// </summary>
    /// <param name="invalidField">Field to make invalid</param>
    UserTestData CreateInvalidUser(string invalidField);

    /// <summary>
    /// Creates multiple valid users
    /// </summary>
    /// <param name="count">Number of users to create</param>
    List<UserTestData> CreateValidUsers(int count);

    /// <summary>
    /// Creates a valid product test data object
    /// </summary>
    ProductTestData CreateValidProduct();

    /// <summary>
    /// Creates a valid order test data object
    /// </summary>
    OrderTestData CreateValidOrder();

    /// <summary>
    /// Loads test data from a file
    /// </summary>
    /// <param name="filePath">Path to the test data file</param>
    /// <typeparam name="T">Type of test data</typeparam>
    T LoadTestData<T>(string filePath) where T : class;
}

/// <summary>
/// User test data model
/// </summary>
public record UserTestData
{
    public string Username { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
}

/// <summary>
/// Product test data model
/// </summary>
public record ProductTestData
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public string Description { get; init; } = string.Empty;
    public int Stock { get; init; }
}

/// <summary>
/// Order test data model
/// </summary>
public record OrderTestData
{
    public int ProductId { get; init; }
    public int Quantity { get; init; }
    public string CustomerName { get; init; } = string.Empty;
}
