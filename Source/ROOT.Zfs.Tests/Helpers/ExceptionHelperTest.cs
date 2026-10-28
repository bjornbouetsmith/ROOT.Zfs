using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Zfs.Core.Helpers;

namespace ROOT.Zfs.Tests.Helpers;

[TestClass]
public class ExceptionHelperTest
{
    [TestMethod]
    public void FormatExceptionTest()
    {
        var ex = ExceptionHelper.FormatException(5, "some-bad-content", 10);
        Assert.IsNotNull(ex);
        Assert.Contains("index:5", ex.Message);
        Assert.Contains("some-bad-content", ex.Message);
        Assert.Contains("Split into 10 parts", ex.Message);
    }
}