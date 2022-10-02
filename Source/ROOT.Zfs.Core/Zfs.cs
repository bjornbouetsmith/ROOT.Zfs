using System;
using System.Collections.Generic;
using ROOT.Shared.Utils.OS;
using ROOT.Zfs.Core.Helpers;
using ROOT.Zfs.Public;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Core
{
    public class Zfs : ZfsBase, IZfs
    {
        /// <summary>
        /// Creates an instance of the Zfs class.
        /// </summary>
        /// <param name="remoteConnection">Any remote connection that should be used. This parameter is optional - if null is passed then the local machine will be used.</param>
        public Zfs(SSHProcessCall remoteConnection = null)
            : base(remoteConnection)
        {
            Snapshots = new Snapshots(remoteConnection);
            DataSets = new DataSets(remoteConnection);
            Properties = new Properties(remoteConnection);
            Pool = new ZPool(remoteConnection);
        }

        public ISnapshots Snapshots { get; }
        public IDataSets DataSets { get; }
        public IProperties Properties { get; }
        public IZPool Pool { get; }

        public VersionInfo GetVersionInfo()
        {
            var pc = BuildCommand(Commands.ZfsCommands.GetVersion());
            var response = pc.LoadResponse();
            if (!response.Success)
            {
                throw response.ToException();
            }
            return new VersionInfo { Lines = response.StdOut.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries) };
        }

        public IEnumerable<DiskInfo> ListDisks()
        {
            var disksCommand = BuildCommand(Commands.BaseCommands.ListBlockDevices());
            var disksReponse = disksCommand.LoadResponse();
            if (!disksReponse.Success)
            {
                throw disksReponse.ToException();
            }

            var blockDevices = DiskHelper.BlockDevicesFromStdOutput(disksReponse.StdOut);

            var pc = BuildCommand(Commands.BaseCommands.ListDisks());
            var response = pc.LoadResponse();
            if (!response.Success)
            {
                throw response.ToException();
            }

            foreach (var line in response.StdOut.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (line.Trim().Length == 0)
                {
                    continue;
                }

                yield return DiskHelper.FromString(line, blockDevices);
            }
        }
    }
}
