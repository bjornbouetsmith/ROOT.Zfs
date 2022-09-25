using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ROOT.Shared.Utils.OS;

namespace ROOT.Zfs.Core
{
    public class Snapshot : ZfsBase
    {
        public Snapshot(RemoteProcessCall remoteConnection) : base(remoteConnection)
        {
        }

        public IEnumerable<Info.Snapshot> GetSnapshots(string dataset, ProcessCall previousCall = null)
        {
            var pc = BuildCommand(Commands.Snapshots.ProcessCalls.ListSnapshots(dataset), previousCall);

            var response = pc.LoadResponse();
            if (response.Success)
            {
                Debug.WriteLine($"Command: {pc.FullCommandLine} success");

                foreach (var line in response.StdOut.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    yield return Info.Snapshot.FromString(line);
                }
            }
            else
            {
                throw response.ToException();
            }
        }

        public void DestroySnapshot(string dataset, string snapName, ProcessCall previousCall = null)
        {
            DestroySnapshot(dataset, snapName, true, previousCall);

        }

        public void DestroySnapshot(string dataset, string snapName, bool isExactName, ProcessCall previousCall = null)
        {
            if (isExactName)
            {
                var pc = BuildCommand(Commands.Snapshots.ProcessCalls.DestroySnapshot(dataset, snapName), previousCall);

                var response = pc.LoadResponse();
                if (response.Success)
                {
                    Debug.WriteLine($"Command: {pc.FullCommandLine} success");
                }
                else
                {
                    throw response.ToException();
                }
            }
            else
            {
                // Find all snapshots that begins with snapName and delete them one by one
                foreach (var snapshot in GetSnapshots(dataset, previousCall).Where(sn => SnapshotMatches(dataset, sn.Name, snapName)))
                {
                    DestroySnapshot(dataset, snapshot.Name, true, previousCall);
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


        public void CreateSnapshot(string dataset, ProcessCall previousCall = null)
        {
            CreateSnapshot(dataset, DateTime.UtcNow.ToLocalTime().ToString("yyyyMMddHHmmss"));
        }

        public void CreateSnapshot(string dataset, string snapName, ProcessCall previousCall = null)
        {
            var pc = BuildCommand(Commands.Snapshots.ProcessCalls.CreateSnapshot(dataset, snapName), previousCall);

            var response = pc.LoadResponse();
            if (response.Success)
            {
                Debug.WriteLine($"Command: {pc.FullCommandLine} success");
            }
            else
            {
                throw response.ToException();
            }
        }
    }
}
