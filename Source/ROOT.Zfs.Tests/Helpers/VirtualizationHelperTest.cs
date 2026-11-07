using Xunit;
using ROOT.Zfs.Core.Helpers;

namespace ROOT.Zfs.Tests.Helpers;

public class VirtualizationHelperTest
{
    [InlineData("kvm", true)]
    [InlineData("KVM\r\n", true)]
    [InlineData("none", false)]
    [InlineData("NONE", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    [Theory]
    public void IsVirtualMachineTest(string stdOut, bool expected)
    {
        Assert.Equal(expected, VirtualizationHelper.IsVirtualMachine(stdOut));
    }

    [InlineData("kvm", true)]
    [InlineData("vmware\n", true)]
    [InlineData("", false)]
    [InlineData(null, false)]
    [Theory]
    public void IsVirtualizedByVirtWhatTest(string stdOut, bool expected)
    {
        Assert.Equal(expected, VirtualizationHelper.IsVirtualizedByVirtWhat(stdOut));
    }

    [InlineData(" Static hostname: test\n Virtualization: kvm\n", true)]
    [InlineData("Virtualization: none", false)]
    [InlineData(" Static hostname: test\n         Chassis: desktop\n", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    [Theory]
    public void HasVirtualizationEntryTest(string stdOut, bool expected)
    {
        Assert.Equal(expected, VirtualizationHelper.HasVirtualizationEntry(stdOut));
    }

    [InlineData("QEMU\nStandard PC (Q35 + ICH9, 2009)", true)]
    [InlineData("VMware, Inc.\nVMware Virtual Platform", true)]
    [InlineData("Microsoft Corporation\nVirtual Machine", true)]
    [InlineData("Xen\nHVM domU", true)]
    [InlineData("Dell Inc.\nPowerEdge R740", false)]
    [InlineData("Microsoft Corporation\nSurface Laptop 4", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    [Theory]
    public void IsVirtualVendorTest(string stdOut, bool expected)
    {
        Assert.Equal(expected, VirtualizationHelper.IsVirtualVendor(stdOut));
    }

    [InlineData("processor\t: 0\nflags\t\t: fpu vme de pse tsc msr hypervisor\n", true)]
    [InlineData("processor\t: 0\nflags\t\t: fpu vme de pse tsc msr\n", false)]
    [InlineData("processor\t: 0\nflags\t\t: fpu vme de pse tsc msr\nflags\t\t: hypervisor\n", true)]
    [InlineData("processor\t: 0\nflags\n", false)]
    [InlineData("processor\t: 0\n", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    [Theory]
    public void HasHypervisorFlagTest(string stdOut, bool expected)
    {
        Assert.Equal(expected, VirtualizationHelper.HasHypervisorFlag(stdOut));
    }
}
