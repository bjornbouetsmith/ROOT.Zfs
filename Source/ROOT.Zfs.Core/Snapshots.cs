using System;
using System.Collections.Generic;
using System.Linq;
using ROOT.Shared.Utils.OS;
using ROOT.Zfs.Core.Commands;
using ROOT.Zfs.Core.Helpers;
using ROOT.Zfs.Public;
using ROOT.Zfs.Public.Arguments.Snapshots;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Core
{
    internal class Snapshots : ZfsBase, ISnapshots
    {
        public Snapshots(IProcessCall remoteConnection) : base(remoteConnection)
        {
        }

        /// <inheritdoc />
        public IList<Snapshot> List(SnapshotListArgs args)
        {
            var pc = BuildCommand(SnapshotCommands.ListSnapshots(args));

            var response = pc.LoadResponse(true);
            var list = new List<Snapshot>();

            foreach (var line in response.StdOut.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
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
                var pc = BuildCommand(SnapshotCommands.DestroySnapshot(args));

                pc.LoadResponse(true);

            }
            else
            {
                var listArg = new SnapshotListArgs { Root = args.Dataset };
                // Find all snapshots that begins with snapName and delete them one by one
                foreach (var snapshot in List(listArg).Where(sn => SnapshotMatches(args.Dataset, sn.Name, args.Snapshot)))
                {
                    var subArgs = new SnapshotDestroyArgs { Dataset = args.Dataset, Snapshot = snapshot.Name, IsExactName = true };
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
            var skipLen = datasetOrVolume.Length + 1;
            var trimmedName = pattern;
            if (pattern.Contains('@'))
            {
                var skipPatternLen = pattern.IndexOf('@') + 1;
                trimmedName = pattern[skipPatternLen..];
            }

            var realName = snapshotName[skipLen..];

            return realName.StartsWith(trimmedName, StringComparison.OrdinalIgnoreCase);
        }

        /// <inheritdoc />
        public void Create(string dataset, string snapName)
        {
            var pc = BuildCommand(SnapshotCommands.CreateSnapshot(dataset, snapName));

            pc.LoadResponse(true);
        }

        /// <inheritdoc />
        public void Hold(string snapshot, string tag, bool recursive)
        {
            var command = BuildCommand(SnapshotCommands.Hold(snapshot, tag, recursive));
            command.LoadResponse(true);
        }

        /// <inheritdoc />
        public IList<SnapshotHold> Holds(string snapshot, bool recursive)
        {
            var command = BuildCommand(SnapshotCommands.Holds(snapshot, recursive));
            var response = command.LoadResponse(true);
            return SnapshotHoldParser.ParseStdOut(response.StdOut);
        }

        /// <inheritdoc />
        public void Release(string snapshot, string tag, bool recursive)
        {
            var command = BuildCommand(SnapshotCommands.Release(snapshot, tag, recursive));
            command.LoadResponse(true);
        }
    }
}
