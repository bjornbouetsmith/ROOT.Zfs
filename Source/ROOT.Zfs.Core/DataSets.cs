using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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

    /// <inheritdoc />
    public async Task<IReadOnlyList<Dataset>> ListAsync(DatasetListArgs args, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var pc = BuildCommand(_commands.List(args));
        var response = await pc.LoadResponseAsync(true, cancellationToken: cancellationToken).ConfigureAwait(false);
        return DatasetHelper.ParseStdOut(response.StdOut);
    }

    /// <inheritdoc />
    public async Task<Dataset> CreateAsync(DatasetCreationArgs arguments, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var pc = BuildCommand(_commands.Create(arguments));
        await pc.LoadResponseAsync(true, cancellationToken: cancellationToken).ConfigureAwait(false);

        var listArgs = new DatasetListArgs { Root = arguments.DatasetName, IncludeChildren = false, DatasetTypes = arguments.Type };
        var datasets = await ListAsync(listArgs, cancellationToken).ConfigureAwait(false);
        return datasets.Count > 0 ? datasets[0] : null;
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