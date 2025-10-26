using NUnit.Framework;
using Serilog;
using SDET.Tests.Interfaces;

namespace SDET.Tests.Base;

/// <summary>
/// Base class for all tests implementing ITest interface.
/// Provides common test lifecycle management and logging.
/// SOLID: Single Responsibility - Manages test lifecycle only
/// SOLID: Open/Closed - Open for extension via virtual methods
/// SOLID: Liskov Substitution - All derived tests can substitute this base
/// </summary>
[TestFixture]
public abstract class BaseTest : ITest
{
    protected ILogger Logger { get; private set; } = null!;
    protected string TestName { get; private set; } = string.Empty;
    protected DateTime TestStartTime { get; private set; }

    /// <summary>
    /// One-time setup for the test fixture
    /// </summary>
    [OneTimeSetUp]
    public virtual void OneTimeSetUp()
    {
        ConfigureLogging();
        Logger.Information("==== Test Fixture Setup Started ====");
        OnFixtureSetUp();
        Logger.Information("==== Test Fixture Setup Completed ====");
    }

    /// <summary>
    /// One-time teardown for the test fixture
    /// </summary>
    [OneTimeTearDown]
    public virtual void OneTimeTearDown()
    {
        Logger.Information("==== Test Fixture Teardown Started ====");
        OnFixtureTearDown();
        Logger.Information("==== Test Fixture Teardown Completed ====");
        Log.CloseAndFlush();
    }

    /// <summary>
    /// Setup before each test
    /// </summary>
    [SetUp]
    public virtual void SetUp()
    {
        TestName = TestContext.CurrentContext.Test.Name;
        TestStartTime = DateTime.Now;

        Logger.Information("==================================================");
        Logger.Information($"TEST STARTED: {TestName}");
        Logger.Information($"Start Time: {TestStartTime:yyyy-MM-dd HH:mm:ss}");
        Logger.Information("==================================================");

        OnTestSetUp();
    }

    /// <summary>
    /// Teardown after each test
    /// </summary>
    [TearDown]
    public virtual void TearDown()
    {
        var testEndTime = DateTime.Now;
        var duration = testEndTime - TestStartTime;
        var testOutcome = TestContext.CurrentContext.Result.Outcome.Status;

        Logger.Information("==================================================");
        Logger.Information($"TEST COMPLETED: {TestName}");
        Logger.Information($"Status: {testOutcome}");
        Logger.Information($"Duration: {duration.TotalSeconds:F2} seconds");
        Logger.Information($"End Time: {testEndTime:yyyy-MM-dd HH:mm:ss}");

        if (testOutcome == NUnit.Framework.Interfaces.TestStatus.Failed)
        {
            Logger.Error($"Failure Message: {TestContext.CurrentContext.Result.Message}");
            Logger.Error($"Stack Trace: {TestContext.CurrentContext.Result.StackTrace}");
            OnTestFailure();
        }

        Logger.Information("==================================================");

        OnTestTearDown();
    }

    /// <summary>
    /// Cleanup test resources
    /// </summary>
    public virtual void Cleanup()
    {
        Logger.Information($"Cleaning up resources for test: {TestName}");
        OnCleanup();
    }

    /// <summary>
    /// Gets the test context information
    /// </summary>
    public string GetTestContext()
    {
        return $"Test: {TestName}, Status: {TestContext.CurrentContext.Result.Outcome.Status}";
    }

    /// <summary>
    /// Extension point for fixture-level setup
    /// </summary>
    protected virtual void OnFixtureSetUp()
    {
        // Override in derived classes for fixture-specific setup
    }

    /// <summary>
    /// Extension point for fixture-level teardown
    /// </summary>
    protected virtual void OnFixtureTearDown()
    {
        // Override in derived classes for fixture-specific teardown
    }

    /// <summary>
    /// Extension point for test-level setup
    /// </summary>
    protected virtual void OnTestSetUp()
    {
        // Override in derived classes for test-specific setup
    }

    /// <summary>
    /// Extension point for test-level teardown
    /// </summary>
    protected virtual void OnTestTearDown()
    {
        // Override in derived classes for test-specific teardown
    }

    /// <summary>
    /// Extension point for test failure handling
    /// </summary>
    protected virtual void OnTestFailure()
    {
        // Override in derived classes for failure-specific actions (e.g., screenshots)
    }

    /// <summary>
    /// Extension point for cleanup logic
    /// </summary>
    protected virtual void OnCleanup()
    {
        // Override in derived classes for cleanup logic
    }

    /// <summary>
    /// Configures Serilog logging
    /// </summary>
    private void ConfigureLogging()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.File(
                path: "logs/test-execution-.log",
                rollingInterval: RollingInterval.Day,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();

        Logger = Log.Logger;
    }

    /// <summary>
    /// Helper method to log test step
    /// </summary>
    protected void LogStep(string step)
    {
        Logger.Information($"  → {step}");
    }

    /// <summary>
    /// Helper method to log test assertion
    /// </summary>
    protected void LogAssertion(string assertion)
    {
        Logger.Information($"  ✓ ASSERT: {assertion}");
    }

    /// <summary>
    /// Helper method to log test data
    /// </summary>
    protected void LogTestData(string dataName, object data)
    {
        Logger.Information($"  📊 TEST DATA [{dataName}]: {data}");
    }
}
