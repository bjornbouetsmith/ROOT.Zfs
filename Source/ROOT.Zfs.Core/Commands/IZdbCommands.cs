using ROOT.Shared.Utils.OS;

namespace ROOT.Zfs.Core.Commands;

/// <summary>
/// Commands for the zdb binary.
/// </summary>
public interface IZdbCommands
{
    /// <summary>
    /// Returns a command that outputs raw zdb data.
    /// </summary>
    IProcessCall GetRawZdbOutput();
}