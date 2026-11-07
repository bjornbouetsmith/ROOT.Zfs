using Xunit;
using ROOT.Zfs.Core.Helpers;

namespace ROOT.Zfs.Tests.Helpers;

public class ExceptionHelperTest
{
    [Fact]
    public void FormatExceptionTest()
    {
        var ex = ExceptionHelper.FormatException(5, "some-bad-content", 10);
        Assert.NotNull(ex);
        Assert.Contains("index:5", ex.Message);
        Assert.Contains("some-bad-content", ex.Message);
        Assert.Contains("Split into 10 parts", ex.Message);
    }
}