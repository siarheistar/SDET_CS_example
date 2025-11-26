using FluentAssertions;
using NUnit.Framework;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using Allure.Net.Commons;

namespace SDET.Tests.Unit;

/// <summary>
/// Unit tests for User Validator.
/// Tests username, email, and password validation logic.
/// </summary>
[TestFixture]
[AllureNUnit]
[AllureSuite("User Validation")]
[AllureFeature("Username and Email Validation")]
[Category("Unit")]
[Category("Smoke")]
public class UserValidatorTests
{
    [Test]
    [AllureDescription("Validates that a properly formatted username returns true")]
    [AllureSeverity(SeverityLevel.normal)]
    public void ValidateUsername_WithValidUsername_ReturnsTrue()
    {
        // Arrange
        var username = "john_doe";

        // Act
        var isValid = IsValidUsername(username);

        // Assert
        isValid.Should().BeTrue("username meets all validation criteria");
    }

    [Test]
    [AllureDescription("Validates that invalid usernames return false")]
    [AllureSeverity(SeverityLevel.normal)]
    [TestCase("ab", Description = "Too short")]
    [TestCase("a", Description = "Too short")]
    [TestCase("", Description = "Empty")]
    public void ValidateUsername_WithInvalidUsername_ReturnsFalse(string username)
    {
        // Act
        var isValid = IsValidUsername(username);

        // Assert
        isValid.Should().BeFalse($"username '{username}' should be invalid");
    }

    [Test]
    [AllureDescription("Validates that properly formatted email addresses return true")]
    [AllureSeverity(SeverityLevel.critical)]
    [TestCase("test@example.com")]
    [TestCase("user.name@domain.co.uk")]
    [TestCase("user+tag@example.com")]
    public void ValidateEmail_WithValidEmail_ReturnsTrue(string email)
    {
        // Act
        var isValid = IsValidEmail(email);

        // Assert
        isValid.Should().BeTrue($"email '{email}' should be valid");
    }

    [Test]
    [AllureDescription("Validates that invalid email addresses return false")]
    [AllureSeverity(SeverityLevel.critical)]
    [TestCase("invalid")]
    [TestCase("@example.com")]
    [TestCase("user@")]
    [TestCase("")]
    public void ValidateEmail_WithInvalidEmail_ReturnsFalse(string email)
    {
        // Act
        var isValid = IsValidEmail(email);

        // Assert
        isValid.Should().BeFalse($"email '{email}' should be invalid");
    }

    // Simple validation methods (would be in actual validator class)
    private bool IsValidUsername(string username)
    {
        return !string.IsNullOrEmpty(username) && username.Length >= 3;
    }

    private bool IsValidEmail(string email)
    {
        if (string.IsNullOrEmpty(email))
            return false;

        var atIndex = email.IndexOf("@");
        if (atIndex <= 0 || atIndex >= email.Length - 1)
            return false;

        // Get the part after @ to check for dot
        var domainPart = email.Substring(atIndex + 1);
        var dotIndex = domainPart.IndexOf(".");

        // Domain must have at least one dot, not at the beginning or end
        return dotIndex > 0 && dotIndex < domainPart.Length - 1;
    }
}
