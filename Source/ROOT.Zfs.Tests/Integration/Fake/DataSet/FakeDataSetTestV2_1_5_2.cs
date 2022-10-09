using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Shared.Utils.OS;

namespace ROOT.Zfs.Tests.Integration.Fake.DataSet
{
    [TestClass]
    public class FakeDataSetTestV2_1_5_2 : FakeDataSetTest
    {
        protected override IProcessCall CreateProcessCall()
        {
            return new FakeRemoteConnection("2.1.5-2");
        }
    }
}