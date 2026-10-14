using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Zfs.Core.Helpers;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Tests.Helpers
{
    [TestClass]
    public class DataSetPropertiesTest
    {
        [TestMethod]
        public void FromStdOutputTest()
        {
            var stdOut = "tank\taclmode\tdiscard\tdefault\ntank\taclinherit\tdiscard\tdefault";
            var props = DatasetProperties.FromStdOutput(stdOut);

            Assert.AreEqual(2, props.Count);
            
            Assert.AreEqual("aclmode", props[0].Property);
            Assert.AreEqual("discard", props[0].Value);
            Assert.AreEqual("default", props[0].Source);
                        
            Assert.AreEqual("aclinherit", props[1].Property);
            Assert.AreEqual("discard", props[1].Value);
            Assert.AreEqual("default", props[1].Source);
        }

        [TestMethod]
        public void FromStdOutputEmptyTest()
        {
            var props = DatasetProperties.FromStdOutput(string.Empty);
            Assert.AreEqual(0, props.Count);
        }
    }
}
