using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ROOT.Shared.Utils.OS;
using ROOT.Zfs.Core.Commands;
using ROOT.Zfs.Core.Helpers;
using ROOT.Zfs.Public;
using ROOT.Zfs.Public.Arguments.Snapshots;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Core;

internal sealed class Snapshots : ZfsBase, ISnapshots
{
    private readonly ISnapshotCommands _commands;

    public Snapshots(IProcessCall remoteConnection, ISnapshotCommands commands) : base(remoteConnection)
    {
        _commands = commands;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Snapshot>> ListAsync(SnapshotListArgs args, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var pc = BuildCommand(_commands.ListSnapshots(args));

        var response = await pc.LoadResponseAsync(true, cancellationToken: cancellationToken).ConfigureAwait(false);
        var list = new List<Snapshot>();

        foreach (var line in response.StdOut.Split(Constants.LinebreakCharacters, StringSplitOptions.RemoveEmptyEntries))
        {
            list.Add(SnapshotHelper.FromString(line));
        }

        return list;
    }

    /// <inheritdoc />
    public async Task DestroyAsync(SnapshotDestroyArgs args, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (args.IsExactName)
        {
            var pc = BuildCommand(_commands.DestroySnapshot(args));

            await pc.LoadResponseAsync(true, cancellationToken: cancellationToken).ConfigureAwait(false);
        }
        else
        {
            var listArg = new SnapshotListArgs { Root = args.Dataset };
            var snapshots = await ListAsync(listArg, cancellationToken).ConfigureAwait(false);

            // Find all snapshots that begins with snapName and delete them one by one
            foreach (var snapshot in snapshots.Where(sn => SnapshotMatches(args.Dataset, sn.SnapshotName, args.Snapshot)))
            {
                var subArgs = new SnapshotDestroyArgs { Dataset = args.Dataset, Snapshot = snapshot.SnapshotName, IsExactName = true };
                await DestroyAsync(subArgs, cancellationToken).ConfigureAwait(false);
            }
        }
    }

    /// <summary>
    /// Snapshot name matching - will only match the pattern with a starts with, i.e. the raw snapshot name needs to begin with the raw pattern
    /// </summary>
    internal static bool SnapshotMatches(string datasetOrVolume, string snapshotName, string pattern)
    {
        datasetOrVolume = DatasetHelper.Decode(datasetOrVolume);
        pattern = DatasetHelper.Decode(pattern);
        var trimmedName = pattern;
        var realName = snapshotName;
        var decodedSnapshot = DatasetHelper.Decode(snapshotName);
        if (decodedSnapshot.Contains('@'))
        {
            realName = decodedSnapshot[(decodedSnapshot.IndexOf('@') + 1)..];
            if (!decodedSnapshot.StartsWith(datasetOrVolume, StringComparison.OrdinalIgnoreCase))
            {
                // Wrong dataset
                return false;
            }
        }

        if (pattern.Contains('@'))
        {
            var skipPatternLen = pattern.IndexOf('@') + 1;
            trimmedName = pattern[skipPatternLen..];
        }

        return realName.StartsWith(trimmedName, StringComparison.OrdinalIgnoreCase);
    }

    /// <inheritdoc />
    public async Task CreateAsync(SnapshotCreateArgs args, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var pc = BuildCommand(_commands.CreateSnapshot(args));

        await pc.LoadResponseAsync(true, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task CloneAsync(SnapshotCloneArgs args, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var pc = BuildCommand(_commands.Clone(args));
        await pc.LoadResponseAsync(true, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task HoldAsync(SnapshotHoldArgs args, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var command = BuildCommand(_commands.Hold(args));
        await command.LoadResponseAsync(true, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<SnapshotHold>> HoldsAsync(SnapshotHoldsArgs args, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var command = BuildCommand(_commands.Holds(args));
        var response = await command.LoadResponseAsync(true, cancellationToken: cancellationToken).ConfigureAwait(false);
        return SnapshotHoldParser.ParseStdOut(response.StdOut);
    }

    /// <inheritdoc />
    public async Task ReleaseAsync(SnapshotReleaseArgs args, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var command = BuildCommand(_commands.Release(args));
        await command.LoadResponseAsync(true, cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}
