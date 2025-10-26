using System.Net;
using FluentAssertions;
using NUnit.Framework;
using SDET.Tests.Base;
using SDET.Tests.Factories;

namespace SDET.Tests.Api;

/// <summary>
/// API tests for User endpoints.
/// Tests user registration and login via REST API.
/// </summary>
[TestFixture]
[Category("API")]
[Category("Smoke")]
[Explicit("Requires API server running. Use --filter 'TestCategory=API' to run.")]
public class UserApiTests : BaseAPITest
{
    private TestDataFactory _testDataFactory = null!;

    protected override void OnFixtureSetUp()
    {
        base.OnFixtureSetUp();
        _testDataFactory = new TestDataFactory();
    }

    [Test]
    public async Task RegisterUser_WithValidData_Returns201Created()
    {
        // Arrange
        LogStep("Creating valid user test data");
        var userData = _testDataFactory.CreateValidUser();
        LogTestData("User Data", userData);

        // Act
        LogStep("Sending POST request to /api/users/register");
        var response = await PostAsync("/users/register", new
        {
            username = userData.Username,
            email = userData.Email,
            password = userData.Password
        });

        // Assert
        LogAssertion("Response status should be 201 Created");
        ValidateStatusCode(response, HttpStatusCode.Created);

        LogAssertion("Response should contain user ID");
        ValidateResponseContainsFields(response, "id", "username", "email");

        LogAssertion("Response should NOT contain password");
        response.Content.Should().NotContain("password", "password should not be exposed");

        ValidateResponseTime(response, 2000);
    }

    [Test]
    [TestCase("ab", "valid@email.com", "Password123!", Description = "Username too short")]
    [TestCase("validuser", "invalid-email", "Password123!", Description = "Invalid email")]
    [TestCase("validuser", "valid@email.com", "12", Description = "Password too short")]
    public async Task RegisterUser_WithInvalidData_Returns400BadRequest(
        string username, string email, string password)
    {
        // Act
        var response = await PostAsync("/users/register", new
        {
            username,
            email,
            password
        });

        // Assert
        ValidateStatusCode(response, HttpStatusCode.BadRequest);
        response.Content.Should().NotBeNullOrEmpty("error message should be provided");
    }
}
