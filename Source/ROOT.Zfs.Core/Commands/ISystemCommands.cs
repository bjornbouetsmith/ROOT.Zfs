using ROOT.Shared.Utils.OS;

namespace ROOT.Zfs.Core.Commands;

/// <summary>
/// Commands that are not specific to a single ZFS entity, such as resolving binaries and reading disk information.
/// </summary>
public interface ISystemCommands
{
    /// <summary>
    /// Calls `which` to find the full path to a binary.
    /// </summary>
    IProcessCall Which(string command);

    /// <summary>
    /// Returns disks and partitions from the system by id with their /dev/xx name.
    /// </summary>
    IProcessCall ListDisks();

    /// <summary>
    /// Returns the block devices of type disk.
    /// </summary>
    IProcessCall ListBlockDevices();

    /// <summary>
    /// Returns smart info for the given device.
    /// </summary>
    IProcessCall GetSmartInfo(string deviceId);
}