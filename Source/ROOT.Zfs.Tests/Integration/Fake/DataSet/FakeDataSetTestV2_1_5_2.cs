using Xunit;

namespace ROOT.Zfs.Tests.Integration.Fake.DataSet;

public class FakeDataSetTestV2_1_5_2 : FakeDataSetTest
{
    public FakeDataSetTestV2_1_5_2(ITestOutputHelper output) : base(output)
    {
    }

    internal override FakeRemoteConnection CreateProcessCall()
    {
        return new FakeRemoteConnection("2.1.5-2");
    }
}