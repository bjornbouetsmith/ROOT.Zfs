using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ROOT.Zfs.Tests.Integration.Fake.DataSet
{
    [TestClass]
    public class FakeDataSetTestV2_1_5_2 : FakeDataSetTest
    {
        internal override FakeRemoteConnection CreateProcessCall()
        {
            return new FakeRemoteConnection("2.1.5-2");
        }
    }
}