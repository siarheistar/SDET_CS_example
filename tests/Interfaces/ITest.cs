namespace SDET.Tests.Interfaces;

/// <summary>
/// Base interface for all test types.
/// Defines the fundamental contract that all tests must implement.
/// SOLID: Interface Segregation Principle - minimal, focused interface
/// </summary>
public interface ITest
{
    /// <summary>
    /// Setup method called before each test execution
    /// </summary>
    void SetUp();

    /// <summary>
    /// Teardown method called after each test execution
    /// </summary>
    void TearDown();

    /// <summary>
    /// Cleanup method for test resources
    /// </summary>
    void Cleanup();

    /// <summary>
    /// Gets the test context information
    /// </summary>
    string GetTestContext();
}
