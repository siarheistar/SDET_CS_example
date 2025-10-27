using Allure.Net.Commons;
using NUnit.Framework;

namespace SDET.Tests;

/// <summary>
/// Global Allure configuration for all tests
/// </summary>
[SetUpFixture]
public class AllureConfig
{
    [OneTimeSetUp]
    public void GlobalSetup()
    {
        // Set environment variable for Allure results directory if not already set
        var resultsDirectory = Environment.GetEnvironmentVariable("ALLURE_RESULTS_DIRECTORY")
            ?? Environment.GetEnvironmentVariable("ALLURE_RESULTS_PATH")
            ?? "allure-results";

        // Set the environment variable that Allure.NUnit reads
        Environment.SetEnvironmentVariable("ALLURE_RESULTS_DIRECTORY", resultsDirectory);

        // Ensure directory exists
        Directory.CreateDirectory(resultsDirectory);

        Console.WriteLine($"✅ Allure configured to write results to: {resultsDirectory}");
        Console.WriteLine($"✅ ALLURE_RESULTS_DIRECTORY environment variable set");
    }
}
