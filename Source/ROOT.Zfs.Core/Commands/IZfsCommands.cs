using ROOT.Shared.Utils.OS;

namespace ROOT.Zfs.Core.Commands;

/// <summary>
/// Commands for the zfs binary itself.
/// </summary>
public interface IZfsCommands
{
    /// <summary>
    /// Returns a command that outputs the zfs version.
    /// </summary>
    IProcessCall GetVersion();
}