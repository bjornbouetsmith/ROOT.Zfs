using ROOT.Shared.Utils.OS;
using ROOT.Zfs.Public.Arguments.Dataset;

namespace ROOT.Zfs.Core.Commands;

/// <summary>
/// Contains all commands related to datasets.
/// </summary>
public interface IDatasetCommands
{
    /// <summary>
    /// Returns a command that lists datasets.
    /// </summary>
    IProcessCall List(DatasetListArgs args);

    /// <summary>
    /// Returns a command to create a dataset.
    /// </summary>
    IProcessCall Create(DatasetCreationArgs arguments);

    /// <summary>
    /// Returns a command to destroy a dataset.
    /// </summary>
    IProcessCall Destroy(DatasetDestroyArgs args);

    /// <summary>
    /// Returns a command to promote a clone into a real dataset.
    /// </summary>
    IProcessCall Promote(PromoteArgs args);

    /// <summary>
    /// Returns a command to mount a dataset.
    /// </summary>
    IProcessCall Mount(MountArgs mountArgs);

    /// <summary>
    /// Returns a command to unmount a dataset.
    /// </summary>
    IProcessCall Unmount(UnmountArgs unmountArgs);
}