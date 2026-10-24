using System;
using System.Collections.Generic;
using System.Linq;
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
    public IList<Snapshot> List(SnapshotListArgs args)
    {
        var pc = BuildCommand(_commands.ListSnapshots(args));

        var response = pc.LoadResponse(true);
        var list = new List<Snapshot>();

        foreach (var line in response.StdOut.Split(Constants.LinebreakCharacters, StringSplitOptions.RemoveEmptyEntries))
        {
            list.Add(SnapshotHelper.FromString(line));
        }
        return list;
    }

    /// <inheritdoc />
    public void Destroy(SnapshotDestroyArgs args)
    {
        if (args.IsExactName)
        {
            var pc = BuildCommand(_commands.DestroySnapshot(args));

            pc.LoadResponse(true);

        }
        else
        {
            var listArg = new SnapshotListArgs { Root = args.Dataset };
            // Find all snapshots that begins with snapName and delete them one by one
            foreach (var snapshot in List(listArg).Where(sn => SnapshotMatches(args.Dataset, sn.SnapshotName, args.Snapshot)))
            {
                var subArgs = new SnapshotDestroyArgs { Dataset = args.Dataset, Snapshot = snapshot.SnapshotName, IsExactName = true };
                Destroy(subArgs);
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
            if (!decodedSnapshot.StartsWith(datasetOrVolume))
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
    public void Create(SnapshotCreateArgs args)
    {
        var pc = BuildCommand(_commands.CreateSnapshot(args));

        pc.LoadResponse(true);
    }

    /// <inheritdoc />
    public void Clone(SnapshotCloneArgs args)
    {
        var pc = BuildCommand(_commands.Clone(args));
        pc.LoadResponse(true);
    }

    /// <inheritdoc />
    public void Hold(SnapshotHoldArgs args)
    {
        var command = BuildCommand(_commands.Hold(args));
        command.LoadResponse(true);
    }

    /// <inheritdoc />
    public IList<SnapshotHold> Holds(SnapshotHoldsArgs args)
    {
        var command = BuildCommand(_commands.Holds(args));
        var response = command.LoadResponse(true);
        return SnapshotHoldParser.ParseStdOut(response.StdOut);
    }

    /// <inheritdoc />
    public void Release(SnapshotReleaseArgs args)
    {
        var command = BuildCommand(_commands.Release(args));
        command.LoadResponse(true);
    }
}