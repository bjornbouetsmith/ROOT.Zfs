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

        public IEnumerable<Snapshot> GetSnapshots(string datasetOrVolume)
        {
            var pc = BuildCommand(SnapshotCommands.ListSnapshots(datasetOrVolume));

            var response = pc.LoadResponse();
            if (!response.Success)
            {
                throw response.ToException();
            }

            foreach (var line in response.StdOut.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                yield return SnapshotHelper.FromString(line);
            }
        }

        public void DestroySnapshot(string datasetOrVolume, string snapName, bool isExactName)
        {
            if (isExactName)
            {
                var pc = BuildCommand(SnapshotCommands.DestroySnapshot(datasetOrVolume, snapName));

                var response = pc.LoadResponse();
                if (!response.Success)
                {
                    throw response.ToException();
                }
            }
            else
            {
                // Find all snapshots that begins with snapName and delete them one by one
                foreach (var snapshot in GetSnapshots(datasetOrVolume).Where(sn => SnapshotMatches(datasetOrVolume, sn.Name, snapName)))
                {
                    DestroySnapshot(datasetOrVolume, snapshot.Name, true);
                }
            }
        }

        /// <summary>
        /// Snapshot name matching - will only match the pattern with a starts with, i.e. the raw snapshot name needs to begin with the raw pattern
        /// </summary>
        internal static bool SnapshotMatches(string datasetOrVolume, string snapshotName, string pattern)
        {
            datasetOrVolume = DataSetHelper.Decode(datasetOrVolume);
            var skipLen = datasetOrVolume.Length + 1;
            var trimmedName = pattern;
            if (pattern.Contains('@'))
            {
                var skipPatternLen = pattern.IndexOf('@') + 1;
                trimmedName = pattern[skipPatternLen..];
            }

            string realName = snapshotName;
            if (snapshotName.Contains('@'))
            {
                realName = snapshotName[skipLen..];
            }

            return realName.StartsWith(trimmedName, StringComparison.OrdinalIgnoreCase);
        }


        public void CreateSnapshot(string dataset)
        {
            CreateSnapshot(dataset, DateTime.UtcNow.ToLocalTime().ToString("yyyyMMddHHmmss"));
        }

        public void CreateSnapshot(string dataset, string snapName)
        {
            var pc = BuildCommand(SnapshotCommands.CreateSnapshot(dataset, snapName));

            var response = pc.LoadResponse();
            if (!response.Success)
            {
                throw response.ToException();
            }
        }
    }
}
