using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Zfs.Core.Helpers;
using ROOT.Zfs.Public.Data.Pools;

namespace ROOT.Zfs.Tests.Helpers
{
    [TestClass]
    public class StateParserTest
    {
        [TestMethod]
        [DataRow("ONLINE",State.Online)]
        [DataRow("OFFLINE", State.Offline)]
        [DataRow("DEGRADED", State.Degraded)]
        [DataRow("FAULTED", State.Faulted)]
        [DataRow("REMOVED", State.Removed)]
        [DataRow("AVAIL", State.Available)]
        [DataRow("UNAVAIL", State.Unavailable)]
        [DataRow("HOTSTUFF", State.Unknown)]
        public void ParseStateTest(string stateText, State expected)
        {
            var state = StateParser.Parse(stateText);
            Assert.AreEqual(expected, state);
        }

    }
}
