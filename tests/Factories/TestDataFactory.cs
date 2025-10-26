using Bogus;
using Newtonsoft.Json;
using SDET.Tests.Interfaces;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace SDET.Tests.Factories;

/// <summary>
/// Factory for creating test data using Bogus (Faker) library.
/// Implements ITestDataFactory interface.
/// SOLID: Single Responsibility - Only responsible for test data generation
/// SOLID: Dependency Inversion - Clients depend on ITestDataFactory abstraction
/// </summary>
public class TestDataFactory : ITestDataFactory
{
    private readonly Faker _faker;
    private readonly Random _random;

    /// <summary>
    /// Initializes a new instance of the TestDataFactory class
    /// </summary>
    public TestDataFactory()
    {
        _faker = new Faker();
        _random = new Random();
    }

    /// <summary>
    /// Creates a valid user test data object
    /// </summary>
    public UserTestData CreateValidUser()
    {
        return new UserTestData
        {
            Username = _faker.Internet.UserName(),
            Email = _faker.Internet.Email(),
            Password = GenerateSecurePassword(),
            FirstName = _faker.Name.FirstName(),
            LastName = _faker.Name.LastName()
        };
    }

    /// <summary>
    /// Creates an invalid user test data object
    /// </summary>
    /// <param name="invalidField">Field to make invalid (username, email, password)</param>
    public UserTestData CreateInvalidUser(string invalidField)
    {
        var user = CreateValidUser();

        return invalidField.ToLower() switch
        {
            "username" => user with { Username = "ab" }, // Too short
            "email" => user with { Email = "invalid-email" }, // Invalid format
            "password" => user with { Password = "123" }, // Too short
            "empty_username" => user with { Username = string.Empty },
            "empty_email" => user with { Email = string.Empty },
            "empty_password" => user with { Password = string.Empty },
            _ => throw new ArgumentException($"Invalid field name: {invalidField}")
        };
    }

    /// <summary>
    /// Creates multiple valid users
    /// </summary>
    /// <param name="count">Number of users to create</param>
    public List<UserTestData> CreateValidUsers(int count)
    {
        var users = new List<UserTestData>();

        for (int i = 0; i < count; i++)
        {
            users.Add(CreateValidUser());
        }

        return users;
    }

    /// <summary>
    /// Creates a valid product test data object
    /// </summary>
    public ProductTestData CreateValidProduct()
    {
        return new ProductTestData
        {
            Id = _random.Next(1, 1000),
            Name = _faker.Commerce.ProductName(),
            Price = decimal.Parse(_faker.Commerce.Price()),
            Description = _faker.Commerce.ProductDescription(),
            Stock = _random.Next(1, 100)
        };
    }

    /// <summary>
    /// Creates a valid order test data object
    /// </summary>
    public OrderTestData CreateValidOrder()
    {
        return new OrderTestData
        {
            ProductId = _random.Next(1, 100),
            Quantity = _random.Next(1, 10),
            CustomerName = _faker.Name.FullName()
        };
    }

    /// <summary>
    /// Loads test data from a JSON file
    /// </summary>
    public T LoadTestData<T>(string filePath) where T : class
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Test data file not found: {filePath}");
        }

        var extension = Path.GetExtension(filePath).ToLower();

        return extension switch
        {
            ".json" => LoadFromJson<T>(filePath),
            ".yaml" or ".yml" => LoadFromYaml<T>(filePath),
            _ => throw new NotSupportedException($"File format not supported: {extension}")
        };
    }

    /// <summary>
    /// Loads test data from a JSON file
    /// </summary>
    private T LoadFromJson<T>(string filePath) where T : class
    {
        var json = File.ReadAllText(filePath);
        return JsonConvert.DeserializeObject<T>(json)
            ?? throw new InvalidOperationException($"Failed to deserialize JSON from: {filePath}");
    }

    /// <summary>
    /// Loads test data from a YAML file
    /// </summary>
    private T LoadFromYaml<T>(string filePath) where T : class
    {
        var yaml = File.ReadAllText(filePath);
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        return deserializer.Deserialize<T>(yaml)
            ?? throw new InvalidOperationException($"Failed to deserialize YAML from: {filePath}");
    }

    /// <summary>
    /// Generates a secure password
    /// </summary>
    private string GenerateSecurePassword()
    {
        return _faker.Internet.Password(
            length: 12,
            memorable: false,
            regexPattern: @"[a-zA-Z0-9!@#$%^&*]"
        );
    }

    /// <summary>
    /// Creates invalid user data for negative testing
    /// </summary>
    public List<UserTestData> CreateInvalidUsersForNegativeTesting()
    {
        return new List<UserTestData>
        {
            CreateInvalidUser("username"),
            CreateInvalidUser("email"),
            CreateInvalidUser("password"),
            CreateInvalidUser("empty_username"),
            CreateInvalidUser("empty_email"),
            CreateInvalidUser("empty_password")
        };
    }

    /// <summary>
    /// Creates boundary test data for usernames
    /// </summary>
    public List<string> CreateUsernameBoundaryData()
    {
        return new List<string>
        {
            "ab",                                    // Too short (2 chars)
            "abc",                                   // Minimum valid (3 chars)
            new string('a', 50),                     // Maximum valid (50 chars)
            new string('a', 51),                     // Too long (51 chars)
            "user@name",                             // Invalid characters
            "user name",                             // Contains space
            string.Empty,                            // Empty
            "ValidUser123"                           // Valid with numbers
        };
    }

    /// <summary>
    /// Creates boundary test data for emails
    /// </summary>
    public List<string> CreateEmailBoundaryData()
    {
        return new List<string>
        {
            "a@b.c",                                 // Minimum valid
            "user@example.com",                      // Standard valid
            "user.name+tag@example.co.uk",           // Complex valid
            "invalid",                               // Missing @
            "invalid@",                              // Missing domain
            "@invalid.com",                          // Missing local part
            "invalid@.com",                          // Invalid domain
            string.Empty                             // Empty
        };
    }

    /// <summary>
    /// Creates calculator test data
    /// </summary>
    public List<CalculatorTestData> CreateCalculatorTestData()
    {
        return new List<CalculatorTestData>
        {
            new() { Num1 = 10, Num2 = 5, Operation = "add", Expected = 15 },
            new() { Num1 = 10, Num2 = 5, Operation = "subtract", Expected = 5 },
            new() { Num1 = 10, Num2 = 5, Operation = "multiply", Expected = 50 },
            new() { Num1 = 10, Num2 = 5, Operation = "divide", Expected = 2 },
            new() { Num1 = -10, Num2 = 5, Operation = "add", Expected = -5 },
            new() { Num1 = 10, Num2 = 0, Operation = "divide", Expected = double.PositiveInfinity }
        };
    }
}

/// <summary>
/// Calculator test data model
/// </summary>
public record CalculatorTestData
{
    public double Num1 { get; init; }
    public double Num2 { get; init; }
    public string Operation { get; init; } = string.Empty;
    public double Expected { get; init; }
}
