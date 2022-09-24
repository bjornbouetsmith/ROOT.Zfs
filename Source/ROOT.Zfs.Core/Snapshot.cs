using System;
using System.Collections.Generic;
using System.Diagnostics;
using ROOT.Shared.Utils.OS;

namespace ROOT.Zfs.Core
{
    public class Snapshot : ZfsBase
    {
        public Snapshot(RemoteProcessCall remoteConnection) : base(remoteConnection)
        {
        }

        public IEnumerable<Info.Snapshot> LoadSnapshots(string dataset, ProcessCall previousCall = null)
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
