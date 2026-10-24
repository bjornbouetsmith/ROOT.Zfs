using System;
using ROOT.Shared.Utils.OS;
using ROOT.Zfs.Public.Arguments.Dataset;

namespace ROOT.Zfs.Core.Commands;

/// <summary>
/// Contains all command related to datasets.
/// </summary>
internal sealed class DatasetCommands : CommandBuilderBase, IDatasetCommands
{
    public DatasetCommands(IZfsBinaries binaries) : base(binaries)
    {
    }

    /// <summary>
    /// Returns a command that lists datasets
    /// </summary>
    /// <exception cref="ArgumentException">If arguments are not valid or missing</exception>
    public IProcessCall List(DatasetListArgs args)
    {
        if (!args.Validate(out var errors))
        {
            throw ToArgumentException(errors, args);
        }

        return new ProcessCall(Binaries.Zfs, args.ToString());
    }

    /// <summary>
    /// returns a command to create a dataset
    /// </summary>
    /// <exception cref="ArgumentException">If arguments are not valid or missing</exception>
    public IProcessCall Create(DatasetCreationArgs arguments)
    {
        if (!arguments.Validate(out var errors))
        {
            throw ToArgumentException(errors, arguments);
        }

        return new ProcessCall(Binaries.Zfs, arguments.ToString());
    }

    /// <summary>
    /// Returns a command to destroy a dataset
    /// </summary>
    /// <exception cref="ArgumentException">If arguments are not valid or missing</exception>
    public IProcessCall Destroy(DatasetDestroyArgs args)
    {
        if (!args.Validate(out var errors))
        {
            throw ToArgumentException(errors, args);
        }
        return new ProcessCall(Binaries.Zfs, args.ToString());
    }

    /// <summary>
    /// Promotes the dataset or volume from a clone to a real dataset or volume.
    /// see https://openzfs.github.io/openzfs-docs/man/8/zfs-promote.8.html
    /// </summary>
    public IProcessCall Promote(PromoteArgs args)
    {
        if (!args.Validate(out var errors))
        {
            throw ToArgumentException(errors, args);
        }
        return new ProcessCall(Binaries.Zfs, args.ToString());
    }

    /// <summary>
    /// Returns a command to mount a dataset
    /// </summary>
    /// <param name="mountArgs">the arguments which contains the options for mounting</param>
    /// <exception cref="ArgumentException">If arguments are not valid</exception>
    public IProcessCall Mount(MountArgs mountArgs)
    {
        if (!mountArgs.Validate(out var errors))
        {
            throw ToArgumentException(errors, mountArgs);
        }

        return new ProcessCall(Binaries.Zfs, mountArgs.ToString());
    }

    /// <summary>
    /// Returns a command to unmount a filesystem or mount point
    /// </summary>
    /// <exception cref="ArgumentException">If arguments are not valid</exception>
    public IProcessCall Unmount(UnmountArgs unmountArgs)
    {
        if (!unmountArgs.Validate(out var errors))
        {
            throw ToArgumentException(errors, unmountArgs);
        }

        return new ProcessCall(Binaries.Zfs, unmountArgs.ToString());
    }
}