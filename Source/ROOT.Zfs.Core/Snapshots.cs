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
        public Snapshots(RemoteProcessCall remoteConnection) : base(remoteConnection)
        {
        }

        public IEnumerable<Snapshot> GetSnapshots(string dataset)
        {
            var pc = BuildCommand(SnapshotCommands.ProcessCalls.ListSnapshots(dataset));

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

        public void DestroySnapshot(string dataset, string snapName, bool isExactName)
        {
            if (isExactName)
            {
                var pc = BuildCommand(SnapshotCommands.ProcessCalls.DestroySnapshot(dataset, snapName));

                var response = pc.LoadResponse();
                if (!response.Success)
                {
                    throw response.ToException();
                }
            }
            else
            {
                // Find all snapshots that begins with snapName and delete them one by one
                foreach (var snapshot in GetSnapshots(dataset).Where(sn => SnapshotMatches(dataset, sn.Name, snapName)))
                {
                    DestroySnapshot(dataset, snapshot.Name, true);
                }
            }
        }

        /// <summary>
        /// Snapshot name matching - will only match the pattern with a starts with, i.e. the raw snapshot name needs to begin with the raw pattern
        /// </summary>
        internal static bool SnapshotMatches(string dataset, string snapshotName, string pattern)
        {
            var skipLen = dataset.Length + 1;
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
            var pc = BuildCommand(SnapshotCommands.ProcessCalls.CreateSnapshot(dataset, snapName));

            var response = pc.LoadResponse();
            if (!response.Success)
            {
                throw response.ToException();
            }
        }
    }
}
