using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Zfs.Core.Helpers;

namespace ROOT.Zfs.Tests.Helpers;

[TestClass]
public class ExceptionHelperTest
{
    [TestMethod]
    public void FormatExceptionTest()
    {
        var ex = ExceptionHelper.FormatException(5, "some-bad-content");
        Assert.IsNotNull(ex);
        StringAssert.Contains(ex.Message, "index:5");
        StringAssert.Contains(ex.Message, "some-bad-content");
    }
}