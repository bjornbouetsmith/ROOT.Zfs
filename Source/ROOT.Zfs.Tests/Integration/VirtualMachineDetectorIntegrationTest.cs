using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Shared.Utils.OS;
using ROOT.Zfs.Core.Commands;
using ROOT.Zfs.Core.Helpers;

namespace ROOT.Zfs.Tests.Integration;

[TestClass]
public class VirtualMachineDetectorIntegrationTest
{
    public TestContext TestContext { get; set; }
    private readonly IProcessCall _remoteProcessCall = TestHelpers.RequiresRemoteConnection ? new SSHProcessCall("bbs", "zfsdev.root.dom", true) : null;

    [TestMethod, TestCategory("Integration")]
    public async Task IsVirtualMachineAsyncTest()
    {
        var binaries = new ZfsBinaries(_remoteProcessCall);
        var detector = new VirtualMachineDetector(_remoteProcessCall, new VirtualizationCommands(binaries))
        {
            RequiresSudo = TestHelpers.RequiresSudo
        };

        var isVirtualMachine = await BadAsserts.DoesNotThrow(async () => await detector.IsVirtualMachineAsync(TestContext.CancellationToken));

        // The result depends on the machine running the test, so only verify that detection completed.
        TestContext.WriteLine($"IsVirtualMachine: {isVirtualMachine}");
    }
}
