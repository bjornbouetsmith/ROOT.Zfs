using System;
using ROOT.Shared.Utils.OS;
using ROOT.Zfs.Public.Arguments.Snapshots;

namespace ROOT.Zfs.Core.Commands;

/// <summary>
/// Contains all snapshot related commands
/// </summary>
internal sealed class SnapshotCommands : CommandBuilderBase, ISnapshotCommands
{
    private readonly IDatasetCommands _datasetCommands;

    public SnapshotCommands(IZfsBinaries binaries, IDatasetCommands datasetCommands) : base(binaries)
    {
        _datasetCommands = datasetCommands ?? throw new ArgumentNullException(nameof(datasetCommands));
    }

    /// <summary>
    /// Creates a standard snapshot name based on the time passed into the method
    /// </summary>
    public string CreateSnapshotName(DateTime dateTime) => dateTime.ToString("yyyyMMddHHmmss");

    /// <summary>
    /// Returns a command to list snapshots in the given dataset or volume
    /// </summary>
    /// <exception cref="ArgumentException">If arguments are not valid</exception>
    public IProcessCall ListSnapshots(SnapshotListArgs args)
    {
        if (!args.Validate(out var errors))
        {
            throw ToArgumentException(errors, args);
        }

        return _datasetCommands.List(args);
    }

    /// <summary>
    /// Returns a command to destroy the snapshot in the given dataset
    /// Snapshot has to be a child of dataset
    /// </summary>
    /// <exception cref="ArgumentException">If arguments are not valid</exception>
    public IProcessCall DestroySnapshot(SnapshotDestroyArgs args)
    {
        if (!args.Validate(out var errors))
        {
            throw ToArgumentException(errors, args);
        }

        return new ProcessCall(Binaries.Zfs, args.ToString());
    }

    /// <summary>
    /// Returns a command to create a snapshot of the dataset with the given name
    /// </summary>
    /// <exception cref="ArgumentException">If arguments are not valid</exception>
    public IProcessCall CreateSnapshot(SnapshotCreateArgs args)
    {
        if (!args.Validate(out var errors))
        {
            throw ToArgumentException(errors, args);
        }

        return new ProcessCall(Binaries.Zfs, args.ToString());
    }

    /// <summary>
    /// Returns a command to clone a snapshot into the given target dataset
    /// </summary>
    /// <exception cref="ArgumentException">If arguments are not valid</exception>
    public IProcessCall Clone(SnapshotCloneArgs args)
    {
        if (!args.Validate(out var errors))
        {
            throw ToArgumentException(errors, args);
        }

        return new ProcessCall(Binaries.Zfs, args.ToString());
    }

    /// <summary>
    /// Returns a command to add a tag to a snapshot, i.e.
    /// zfs hold
    /// </summary>
    public IProcessCall Hold(SnapshotHoldArgs args)
    {
        if (!args.Validate(out var errors))
        {
            throw ToArgumentException(errors, args);
        }

        return new ProcessCall(Binaries.Zfs, args.ToString());
    }

    /// <summary>
    /// Returns a command to list holds on the given snapshot and possibly also descendents
    /// </summary>
    public IProcessCall Holds(SnapshotHoldsArgs args)
    {
        if (!args.Validate(out var errors))
        {
            throw ToArgumentException(errors, args);
        }

        return new ProcessCall(Binaries.Zfs, args.ToString());
    }

    /// <summary>
    /// Returns a command that releases a hold on a snapshot with the given tag and possibly also decendents
    /// </summary>
    public IProcessCall Release(SnapshotReleaseArgs args)
    {
        if (!args.Validate(out var errors))
        {
            throw ToArgumentException(errors, args);
        }

        return new ProcessCall(Binaries.Zfs, args.ToString());
    }
}