using System;
using System.Collections.Generic;
using System.Linq;
using ROOT.Shared.Utils.OS;
using ROOT.Zfs.Core.Commands;
using ROOT.Zfs.Core.Helpers;
using ROOT.Zfs.Public;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Core
{
    internal class Snapshots : ZfsBase, ISnapshots
    {
        public Snapshots(IProcessCall remoteConnection) : base(remoteConnection)
        {
        }

        /// <inheritdoc />
        public IEnumerable<Snapshot> List(string datasetOrVolume)
        {
            var pc = BuildCommand(SnapshotCommands.ListSnapshots(datasetOrVolume));

            var response = pc.LoadResponse(true);

            foreach (var line in response.StdOut.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                yield return SnapshotHelper.FromString(line);
            }
        }

        /// <inheritdoc />
        public void Destroy(string datasetOrVolume, string snapName, bool isExactName)
        {
            if (isExactName)
            {
                var pc = BuildCommand(SnapshotCommands.DestroySnapshot(datasetOrVolume, snapName));

                pc.LoadResponse(true);

            }
            else
            {
                // Find all snapshots that begins with snapName and delete them one by one
                foreach (var snapshot in List(datasetOrVolume).Where(sn => SnapshotMatches(datasetOrVolume, sn.Name, snapName)))
                {
                    Destroy(datasetOrVolume, snapshot.Name, true);
                }
            }
        }

        /// <summary>
        /// Snapshot name matching - will only match the pattern with a starts with, i.e. the raw snapshot name needs to begin with the raw pattern
        /// </summary>
        private static bool SnapshotMatches(string datasetOrVolume, string snapshotName, string pattern)
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
