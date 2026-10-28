using System.Collections.Generic;
using System.Linq;
using ROOT.Shared.Utils.OS;
using ROOT.Zfs.Core.Commands;
using ROOT.Zfs.Core.Helpers;
using ROOT.Zfs.Public;
using ROOT.Zfs.Public.Arguments.Dataset;
using ROOT.Zfs.Public.Data.Datasets;

namespace ROOT.Zfs.Core;

/// <inheritdoc cref="IZfs" />
internal sealed class Datasets : ZfsBase, IDatasets
{
    private readonly IDatasetCommands _commands;

    public Datasets(IProcessCall remoteConnection, IDatasetCommands commands) : base(remoteConnection)
    {
        _commands = commands;
    }

    public IList<Dataset> List(DatasetListArgs args)
    {
        var pc = BuildCommand(_commands.List(args));
        var response = pc.LoadResponse(true);
        return DatasetHelper.ParseStdOut(response.StdOut);
    }

    /// <inheritdoc />
    public Dataset Create(DatasetCreationArgs arguments)
    {
        var pc = BuildCommand(_commands.Create(arguments));

        pc.LoadResponse(true);
        var listArgs = new DatasetListArgs { Root = arguments.DatasetName, IncludeChildren = false, DatasetTypes = arguments.Type };
        return List(listArgs).FirstOrDefault();
    }

    /// <inheritdoc />
    public DatasetDestroyResponse Destroy(DatasetDestroyArgs args)
    {
        var pc = BuildCommand(_commands.Destroy(args));

        var response = pc.LoadResponse(true);

        if (!args.DestroyFlags.HasFlag(DatasetDestroyFlags.DryRun))
        {
            return new DatasetDestroyResponse { Flags = args.DestroyFlags };
        }

        return new DatasetDestroyResponse { Flags = args.DestroyFlags, DryRun = response.StdOut };
    }

    /// <inheritdoc />
    public void Promote(PromoteArgs args)
    {
        var pc = BuildCommand(_commands.Promote(args));
        pc.LoadResponse(true);
    }

    /// <inheritdoc />
    public void Mount(MountArgs mountArgs)
    {
        var pc = BuildCommand(_commands.Mount(mountArgs));
        pc.LoadResponse(true);
    }

    /// <inheritdoc />
    public void Unmount(UnmountArgs unmountArgs)
    {
        var pc = BuildCommand(_commands.Unmount(unmountArgs));
        pc.LoadResponse(true);
    }
}