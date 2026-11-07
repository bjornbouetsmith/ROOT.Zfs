using Xunit;
using ROOT.Zfs.Core.Helpers;
using ROOT.Zfs.Public.Data.Pools;

namespace ROOT.Zfs.Tests.Helpers;

public class StateParserTest
{
    [Theory]
    [InlineData("ONLINE",State.Online)]
    [InlineData("OFFLINE", State.Offline)]
    [InlineData("DEGRADED", State.Degraded)]
    [InlineData("FAULTED", State.Faulted)]
    [InlineData("REMOVED", State.Removed)]
    [InlineData("AVAIL", State.Available)]
    [InlineData("UNAVAIL", State.Unavailable)]
    [InlineData("HOTSTUFF", State.Unknown)]
    [InlineData(null, State.Unknown)]
    [InlineData("", State.Unknown)]
    public void ParseStateTest(string stateText, State expected)
    {
        var state = StateParser.Parse(stateText);
        Assert.Equal(expected, state);
    }

}