using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ROOT.Shared.Utils.OS;
using ROOT.Zfs.Core.Helpers;
using ROOT.Zfs.Public;
using ROOT.Zfs.Public.Data;
using ROOT.Zfs.Public.Data.Smart;

namespace ROOT.Zfs.Core
{
    /// <inheritdoc cref="IZfs" />
    internal class Zfs : ZfsBase, IZfs
    {
        /// <summary>
        /// Creates an instance of the Zfs class.
        /// </summary>
        /// <param name="remoteConnection">Any remote connection that should be used. This parameter is optional - if null is passed then the local machine will be used.</param>
        public Zfs(IProcessCall remoteConnection = null)
            : base(remoteConnection)
        {
            Snapshots = new Snapshots(remoteConnection);
            Datasets = new Datasets(remoteConnection);
            Properties = new Properties(remoteConnection);
            Pool = new ZPool(remoteConnection);
        }

        /// <inheritdoc />
        public ISnapshots Snapshots { get; }

        /// <inheritdoc />
        public IDatasets Datasets { get; }

        /// <inheritdoc />
        public IProperties Properties { get; }

        /// <inheritdoc />
        public IZPool Pool { get; }

        /// <inheritdoc />
        public VersionInfo GetVersionInfo()
        {
            var pc = BuildCommand(Commands.ZfsCommands.GetVersion());
            var response = pc.LoadResponse(true);

            return new VersionInfo { Lines = response.StdOut.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries) };
        }

        /// <inheritdoc />
        public IEnumerable<DiskInfo> ListDisks()
        {
            var disksCommand = BuildCommand(Commands.Commands.ListBlockDevices());
            var disksReponse = disksCommand.LoadResponse(true);

            var blockDevices = DiskHelper.BlockDevicesFromStdOutput(disksReponse.StdOut);

            var pc = BuildCommand(Commands.Commands.ListDisks());
            var response = pc.LoadResponse(true);

            foreach (var line in response.StdOut.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (line.Trim().Length == 0)
                {
                    continue;
                }

                yield return DiskHelper.FromString(line, blockDevices);
            }
        }

        /// <inheritdoc />
        public IEnumerable<SmartInfo> GetSmartInfos()
        {
            List<SmartInfo> smartInfos = new List<SmartInfo>();
            foreach (var id in ListDisks().Where(d => d.Type == DeviceType.Disk).Select(disk => disk.Id))
            {
                var command = BuildCommand(Commands.Commands.GetSmartInfo(id));
                var response = command.LoadResponse(true);
                var info = SmartInfoParser.ParseStdOut(id, response.StdOut);
                smartInfos.Add(info);
            }

            return smartInfos;
        }

        /// <inheritdoc />
        public SmartInfo GetSmartInfo(string name)
        {
            var command = BuildCommand(Commands.Commands.GetSmartInfo(name));
            var response = command.LoadResponse(true);
            return SmartInfoParser.ParseStdOut(name, response.StdOut);
        }

        /// <inheritdoc />
        public void Initialize()
        {
            var zfs = GetBinaryLocation("zfs", Commands.Commands.WhichZfs);
            var zpool = GetBinaryLocation("zpool", Commands.Commands.WhichZpool);
            var zdb = GetBinaryLocation("zdb", Commands.Commands.WhichZdb);
            var ls = GetBinaryLocation("ls", Commands.Commands.WhichLs);
            var lsblk = GetBinaryLocation("lsblk", Commands.Commands.WhichLsblk);
            var smartctl = GetBinaryLocation("smartctl", Commands.Commands.WhichSmartctl);
            Commands.Commands.WhichZpool = zpool;
            Commands.Commands.WhichZfs = zfs;
            Commands.Commands.WhichZdb = zdb;
            Commands.Commands.WhichLs = ls;
            Commands.Commands.WhichLsblk = lsblk;
            Commands.Commands.WhichSmartctl = smartctl;
            Initialized = true;
        }

        /// <inheritdoc />
        public bool Initialized { get; private set; }

        private string GetBinaryLocation(string binary, string defaultBinary)
        {
            var command = BuildCommand(Commands.Commands.Which(binary));
            var response = command.LoadResponse(false);

            if (response.Success)
            {
                Trace.WriteLine($"{binary}={response.StdOut.Trim()}");
                return response.StdOut.Trim();
            }

            Trace.TraceError($"Could not find binary:{binary} will default to:{defaultBinary} - response from os was Out{response.StdOut}, Err:{response.StdError}");
            return defaultBinary;
        }
    }
}
