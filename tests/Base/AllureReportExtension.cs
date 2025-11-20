using NUnit.Framework;
using NUnit.Framework.Interfaces;
using Allure.Net.Commons;
using System;
using System.IO;

namespace SDET.Tests.Base;

/// <summary>
/// Custom NUnit extension to manually generate Allure results
/// Compatible with Allure.Net.Commons 2.14.1 API
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class AllureReportAttribute : Attribute, ITestAction
{
    private static readonly AllureLifecycle Allure = AllureLifecycle.Instance;
    private string _testResultUuid = string.Empty;

    public ActionTargets Targets => ActionTargets.Test;

    public void BeforeTest(ITest test)
    {
        _testResultUuid = Guid.NewGuid().ToString("N");

        var testResult = new TestResult
        {
            uuid = _testResultUuid,
            name = test.Name,
            fullName = test.FullName,
            labels = new System.Collections.Generic.List<Label>
            {
                Label.Suite(test.ClassName ?? "Unknown"),
                Label.Thread(),
                Label.Host(),
                Label.TestClass(test.ClassName ?? "Unknown"),
                Label.TestMethod(test.MethodName ?? "Unknown"),
                Label.Package(test.ClassName ?? "Unknown")
            },
            status = Status.none
        };

        Allure.StartTestCase(testResult);
    }

    public void AfterTest(ITest test)
    {
        var outcome = TestContext.CurrentContext.Result.Outcome;
        var status = outcome.Status switch
        {
            TestStatus.Passed => Status.passed,
            TestStatus.Failed => Status.failed,
            TestStatus.Skipped => Status.skipped,
            _ => Status.broken
        };

        Allure.UpdateTestCase(tr =>
        {
            tr.status = status;
            tr.statusDetails = new StatusDetails
            {
                message = TestContext.CurrentContext.Result.Message,
                trace = TestContext.CurrentContext.Result.StackTrace
            };
        });

        Allure.StopTestCase();
        Allure.WriteTestCase();
    }
}