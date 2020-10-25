using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Shared.Utils.OS;
using ROOT.Shared.Utils.Serialization;
using ROOT.Zfs.Core.Commands;
using ROOT.Zfs.Core.Info;

namespace ROOT.Zfs.Tests
{
    [TestClass]
    public class SnapshotTest
    {
        private const string SnapshotList = @"1590930547      nvme/nfs@auto-20200531.1509-2w  5165371392
1591016947      nvme/nfs@auto-20200601.1509-2w  2669617152
1591103347      nvme/nfs@auto-20200602.1509-2w  2752487424

1591189747      nvme/nfs@auto-20200603.1509-2w  2914050048
1591276147      nvme/nfs@auto-20200604.1509-2w  3080941568
";


        [TestMethod]
        public void SnapshotParserTest()
        {
            var list = SnapshotParser.Parse(SnapshotList).ToList();

            Assert.AreEqual(5, list.Count);

            foreach (var snap in list)
            {
                Console.WriteLine(snap.CreationDate.AsString());
                Console.WriteLine(snap.Name);
                Console.WriteLine(snap.Size.AsString());
            }
        }

        [TestMethod]
        public void RemoteSnapshotTest()
        {
            RemoteProcessCall pc = new RemoteProcessCall("bbs", "zfsdev.root.dom", true);

            var remote = pc | Snapshots.ProcessCalls.ListSnapshots("tank/myds");

            var response = remote.LoadResponse();
            if (response.Success)
            {
                Console.WriteLine($"Command: {remote.FullCommandLine} success");
                Console.WriteLine(response.StdOut);
                var snapshots = SnapshotParser.Parse(response.StdOut);
                foreach (var snap in snapshots)
                {
                    Console.WriteLine(snap.CreationDate.AsString());
                    Console.WriteLine(snap.Name);
                    Console.WriteLine(snap.Size.AsString());
                }
            }
            else
            {
                var ex = response.ToException();
                Console.WriteLine(ex);
            }

        }

        [TestMethod]
        public void RemoteCreateSnapshot()
        {
            RemoteProcessCall pc = new RemoteProcessCall("bbs", "zfsdev.root.dom", true);
            var snapName = "RemoteCreateSnapshot" + DateTime.UtcNow.ToString("yyyyMMddhhmmss");
            var remote = pc | Snapshots.ProcessCalls.CreateSnapshot("tank/myds", snapName);

            var response = remote.LoadResponse();
            if (response.Success)
            {
                Console.WriteLine($"Command: {remote.FullCommandLine} success");

                Console.WriteLine(response.StdOut);
            }
            else
            {
                var ex = response.ToException();
                Console.WriteLine(ex);
            }
        }


        [TestMethod]
        public void RemoteDestroySnapshot()
        {
            RemoteProcessCall pc = new RemoteProcessCall("bbs", "zfsdev.root.dom", true);
            var snapName = "RemoteCreateSnapshot" + DateTime.UtcNow.ToString("yyyyMMddhhmmss");
            var remote = pc | Snapshots.ProcessCalls.CreateSnapshot("tank/myds", snapName);

            var response = remote.LoadResponse();
            if (response.Success)
            {
                Console.WriteLine($"Command: {remote.FullCommandLine} success");
                var remoteDestroy = pc | Snapshots.ProcessCalls.DestroySnapshot("tank/myds", snapName);
                response = remoteDestroy.LoadResponse();
                if (response.Success)
                {
                    Console.WriteLine($"Command: {remoteDestroy.FullCommandLine} success");
                }
                else
                {
                    var ex = response.ToException();
                    Console.WriteLine(ex);
                }
            }
            else
            {
                var ex = response.ToException();
                Console.WriteLine(ex);
            }
        }

       
    }
}
