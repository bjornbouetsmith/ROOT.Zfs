using ROOT.Shared.Utils.OS;

namespace ROOT.Zfs.Core.Commands;

/// <summary>
/// Command builder for the commands used to detect virtual machines.
/// </summary>
internal sealed class VirtualizationCommands : CommandBuilderBase, IVirtualizationCommands
{
    public VirtualizationCommands(IZfsBinaries binaries) : base(binaries)
    {
    }

    /// <inheritdoc />
    public IProcessCall DetectVirtualization()
    {
        return new ProcessCall(Binaries.SystemdDetectVirt, "--vm");
    }

    /// <inheritdoc />
    public IProcessCall DetectVirtualizationWithVirtWhat()
    {
        return new ProcessCall(Binaries.VirtWhat);
    }

    /// <inheritdoc />
    public IProcessCall GetHostnamectlStatus()
    {
        return new ProcessCall(Binaries.Hostnamectl, "status");
    }

    /// <inheritdoc />
    public IProcessCall GetDmiInfo()
    {
        return new ProcessCall(Binaries.Cat, "/sys/class/dmi/id/sys_vendor /sys/class/dmi/id/product_name");
    }

    /// <inheritdoc />
    public IProcessCall GetCpuInfo()
    {
        return new ProcessCall(Binaries.Cat, "/proc/cpuinfo");
    }
}
