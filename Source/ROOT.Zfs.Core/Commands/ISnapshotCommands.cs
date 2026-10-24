using System;
using ROOT.Shared.Utils.OS;
using ROOT.Zfs.Public.Arguments.Snapshots;

namespace ROOT.Zfs.Core.Commands;

/// <summary>
/// Contains all snapshot related commands.
/// </summary>
public interface ISnapshotCommands
{
    /// <summary>
    /// Creates a standard snapshot name based on the time passed into the method.
    /// </summary>
    string CreateSnapshotName(DateTime dateTime);

    /// <summary>
    /// Returns a command to list snapshots in the given dataset or volume.
    /// </summary>
    IProcessCall ListSnapshots(SnapshotListArgs args);

    /// <summary>
    /// Returns a command to destroy a snapshot.
    /// </summary>
    IProcessCall DestroySnapshot(SnapshotDestroyArgs args);

    /// <summary>
    /// Returns a command to create a snapshot.
    /// </summary>
    IProcessCall CreateSnapshot(SnapshotCreateArgs args);

    /// <summary>
    /// Returns a command to clone a snapshot.
    /// </summary>
    IProcessCall Clone(SnapshotCloneArgs args);

    /// <summary>
    /// Returns a command to add a hold to a snapshot.
    /// </summary>
    IProcessCall Hold(SnapshotHoldArgs args);

    /// <summary>
    /// Returns a command to list holds on a snapshot.
    /// </summary>
    IProcessCall Holds(SnapshotHoldsArgs args);

    /// <summary>
    /// Returns a command to release a hold on a snapshot.
    /// </summary>
    IProcessCall Release(SnapshotReleaseArgs args);
}