using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Zfs.Core.Helpers;

namespace ROOT.Zfs.Tests.Helpers;

[TestClass]
public class VirtualizationHelperTest
{
    [DataRow("kvm", true)]
    [DataRow("KVM\r\n", true)]
    [DataRow("none", false)]
    [DataRow("NONE", false)]
    [DataRow("", false)]
    [DataRow(null, false)]
    [TestMethod]
    public void IsVirtualMachineTest(string stdOut, bool expected)
    {
        Assert.AreEqual(expected, VirtualizationHelper.IsVirtualMachine(stdOut));
    }

    [DataRow("kvm", true)]
    [DataRow("vmware\n", true)]
    [DataRow("", false)]
    [DataRow(null, false)]
    [TestMethod]
    public void IsVirtualizedByVirtWhatTest(string stdOut, bool expected)
    {
        Assert.AreEqual(expected, VirtualizationHelper.IsVirtualizedByVirtWhat(stdOut));
    }

    [DataRow(" Static hostname: test\n Virtualization: kvm\n", true)]
    [DataRow("Virtualization: none", false)]
    [DataRow(" Static hostname: test\n         Chassis: desktop\n", false)]
    [DataRow("", false)]
    [DataRow(null, false)]
    [TestMethod]
    public void HasVirtualizationEntryTest(string stdOut, bool expected)
    {
        Assert.AreEqual(expected, VirtualizationHelper.HasVirtualizationEntry(stdOut));
    }

    [DataRow("QEMU\nStandard PC (Q35 + ICH9, 2009)", true)]
    [DataRow("VMware, Inc.\nVMware Virtual Platform", true)]
    [DataRow("Microsoft Corporation\nVirtual Machine", true)]
    [DataRow("Xen\nHVM domU", true)]
    [DataRow("Dell Inc.\nPowerEdge R740", false)]
    [DataRow("Microsoft Corporation\nSurface Laptop 4", false)]
    [DataRow("", false)]
    [DataRow(null, false)]
    [TestMethod]
    public void IsVirtualVendorTest(string stdOut, bool expected)
    {
        Assert.AreEqual(expected, VirtualizationHelper.IsVirtualVendor(stdOut));
    }

    [DataRow("processor\t: 0\nflags\t\t: fpu vme de pse tsc msr hypervisor\n", true)]
    [DataRow("processor\t: 0\nflags\t\t: fpu vme de pse tsc msr\n", false)]
    [DataRow("processor\t: 0\nflags\t\t: fpu vme de pse tsc msr\nflags\t\t: hypervisor\n", true)]
    [DataRow("processor\t: 0\nflags\n", false)]
    [DataRow("processor\t: 0\n", false)]
    [DataRow("", false)]
    [DataRow(null, false)]
    [TestMethod]
    public void HasHypervisorFlagTest(string stdOut, bool expected)
    {
        Assert.AreEqual(expected, VirtualizationHelper.HasHypervisorFlag(stdOut));
    }
}
