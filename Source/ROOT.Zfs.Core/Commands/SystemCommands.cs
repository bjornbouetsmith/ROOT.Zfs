using ROOT.Shared.Utils.OS;

namespace ROOT.Zfs.Core.Commands;

/// <summary>
/// Command builder for system/diagnostic commands.
/// </summary>
internal sealed class SystemCommands : CommandBuilderBase, ISystemCommands
{
    public SystemCommands(IZfsBinaries binaries) : base(binaries)
    {
    }

    /// <inheritdoc />
    public IProcessCall Which(string command)
    {
        return new ProcessCall(Binaries.Which, command);
    }

    /// <inheritdoc />
    public IProcessCall ListDisks()
    {
        return new ProcessCall(Binaries.Ls, "-l /dev/disk/by-id/ | awk -F ' ' '{print $9,$11}'");
    }

    /// <inheritdoc />
    public IProcessCall ListBlockDevices()
    {
        return new ProcessCall(Binaries.Lsblk, "--include 8 --include 259 -p|grep disk");
    }

    /// <inheritdoc />
    public IProcessCall GetSmartInfo(string deviceId)
    {
        return new ProcessCall(Binaries.Smartctl, $"-x {deviceId}");
    }
}