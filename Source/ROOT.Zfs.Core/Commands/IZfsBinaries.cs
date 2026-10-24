using ROOT.Zfs.Public;

namespace ROOT.Zfs.Core.Commands;

/// <summary>
/// Defines the binary locations used when building zfs/zpool commands.
/// One instance is shared by all command builders belonging to a single Zfs instance.
/// </summary>
public interface IZfsBinaries : IBasicZfs
{
    /// <summary>
    /// Path to the zfs binary. Defaults to <c>/sbin/zfs</c>.
    /// </summary>
    string Zfs { get; set; }

    /// <summary>
    /// Path to the zpool binary. Defaults to <c>/sbin/zpool</c>.
    /// </summary>
    string Zpool { get; set; }

    /// <summary>
    /// Path to the zdb binary. Defaults to <c>/sbin/zdb</c>.
    /// </summary>
    string Zdb { get; set; }

    /// <summary>
    /// Path to the ls binary. Defaults to <c>/bin/ls</c>.
    /// </summary>
    string Ls { get; set; }

    /// <summary>
    /// Path to the lsblk binary. Defaults to <c>/bin/lsblk</c>.
    /// </summary>
    string Lsblk { get; set; }

    /// <summary>
    /// Path to the which binary. Defaults to <c>/bin/which</c>.
    /// </summary>
    string Which { get; set; }

    /// <summary>
    /// Path to the smartctl binary. Defaults to <c>/sbin/smartctl</c>.
    /// </summary>
    string Smartctl { get; set; }

    /// <summary>
    /// Initializes the binary locations to enable auto detection of where the required binaries are located.
    /// Utilizes the command `which` - if that is not installed this will fail.
    /// Initializing is optional, but if not done the default location of the binaries needed for this library to work is not autodetected
    /// and usage of the library might work if the default locations are not as expected.
    /// </summary>
    void Initialize();

    /// <summary>
    /// Gets a value indicating whether or not these binaries have been initialized.
    /// </summary>
    bool Initialized { get; }
}