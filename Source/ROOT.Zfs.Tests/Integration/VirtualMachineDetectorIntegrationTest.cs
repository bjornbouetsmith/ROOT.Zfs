using System.Threading.Tasks;
using Xunit;
using ROOT.Shared.Utils.OS;
using ROOT.Zfs.Core.Commands;
using ROOT.Zfs.Core.Helpers;

namespace ROOT.Zfs.Tests.Integration;

public class VirtualMachineDetectorIntegrationTest
{
    private readonly IProcessCall _remoteProcessCall = TestHelpers.RequiresRemoteConnection ? new SSHProcessCall("bbs", "zfsdev.root.dom", true) : null;

    [Fact]
    [Trait("Category", "Integration")]
    public async Task IsVirtualMachineAsyncTest()
    {
        var binaries = new ZfsBinaries(_remoteProcessCall);
        var detector = new VirtualMachineDetector(_remoteProcessCall, new VirtualizationCommands(binaries))
        {
            RequiresSudo = TestHelpers.RequiresSudo
        };

        var isVirtualMachine = await BadAsserts.DoesNotThrow(async () => await detector.IsVirtualMachineAsync(TestContext.Current.CancellationToken));

        // The result depends on the machine running the test, so only verify that detection completed.
        TestContext.Current.TestOutputHelper.WriteLine($"IsVirtualMachine: {isVirtualMachine}");
    }
}
