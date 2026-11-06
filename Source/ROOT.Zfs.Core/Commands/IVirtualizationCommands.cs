using ROOT.Shared.Utils.OS;

namespace ROOT.Zfs.Core.Commands;

/// <summary>
/// Commands used to detect whether the target machine is a virtual machine.
/// </summary>
internal interface IVirtualizationCommands
{
    /// <summary>
    /// Uses <c>systemd-detect-virt --vm</c> to detect whether the system is running inside a virtual machine.
    /// The command exits with code 0 and prints the virtualization type when a virtual machine is detected.
    /// </summary>
    IProcessCall DetectVirtualization();

    /// <summary>
    /// Uses <c>virt-what</c> to detect virtualization on systems without systemd.
    /// The command prints one or more virtualization types when virtualization is detected.
    /// </summary>
    IProcessCall DetectVirtualizationWithVirtWhat();

    /// <summary>
    /// Returns the output of <c>hostnamectl status</c>, which contains a Virtualization entry when running in a virtual machine.
    /// </summary>
    IProcessCall GetHostnamectlStatus();

    /// <summary>
    /// Reads the DMI system vendor and product name, which identify many hypervisors on systems without systemd.
    /// </summary>
    IProcessCall GetDmiInfo();

    /// <summary>
    /// Reads <c>/proc/cpuinfo</c> so the hypervisor CPU flag can be inspected.
    /// </summary>
    IProcessCall GetCpuInfo();
}
