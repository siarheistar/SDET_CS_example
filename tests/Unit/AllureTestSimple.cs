using NUnit.Framework;
using SDET.Tests.Base;

namespace SDET.Tests.Unit;

[TestFixture]
[AllureReport]
public class AllureTestSimple
{
    [Test]
    public void SimpleTest()
    {
        Assert.Pass("This test should generate Allure results");
    }
}
