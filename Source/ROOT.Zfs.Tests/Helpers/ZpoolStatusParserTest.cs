using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Shared.Utils.Serialization;
using ROOT.Zfs.Core.Helpers;
using ROOT.Zfs.Public.Data.Pools;

namespace ROOT.Zfs.Tests.Helpers
{
    [TestClass]
    public class ZpoolStatusParserTest
    {
        private const string OkPool = @"  pool: tank
 state: ONLINE
  scan: resilvered 126K in 00:00:00 with 0 errors on Tue Sep 27 17:57:17 2022
config:

	NAME                                                            STATE     READ WRITE CKSUM
	tank                                                            ONLINE       0     0     0
	  mirror-0                                                      ONLINE       0     0     0
	    /dev/disk/by-id/scsi-0QEMU_QEMU_HARDDISK_drive-scsi6-part1  ONLINE       0     0     0
	    /dev/disk/by-id/scsi-0QEMU_QEMU_HARDDISK_drive-scsi5-part1  ONLINE       0     0     0
	  mirror-1                                                      ONLINE       0     0     0
	    /dev/disk/by-id/scsi-0QEMU_QEMU_HARDDISK_drive-scsi7-part1  ONLINE       0     0     0
	    /dev/disk/by-id/scsi-0QEMU_QEMU_HARDDISK_drive-scsi8-part1  ONLINE       0     0     0
errors: No known data errors";
        private const string OkPool2 = @"";

        private const string TruncatedResponse = @"  pool: tank
 state: ONLINE
  scan: resilvered 126K in 00:00:00 with 0 errors on Tue Sep 27 17:57:17 2022
config:

	NAME                                                            STATE     READ WRITE CKSUM
	tank                                                            ONLINE       0     0     0
	  mirror-0                                                      ONLINE       0     0     0
	    /dev/disk/by-id/scsi-0QEMU_QEMU_HARDDISK_drive-scsi6-part1  ONLINE       0     0     0
	    /dev/disk/by-id/scsi-0QEMU_QEMU_HARDDISK_drive-scsi5-part1  ONLINE       0     0     0

err";
        private const string BadPool1 = @"  pool: tank
 state: DEGRADED
status: One or more devices could not be used because the label is missing or
        invalid.  Sufficient replicas exist for the pool to continue
        functioning in a degraded state.
action: Replace the device using 'zpool replace'.
   see: https://openzfs.github.io/openzfs-docs/msg/ZFS-8000-4J
config:

        NAME                                           STATE     READ WRITE CKSUM
        tank                                           DEGRADED     0     0     0
          mirror-0                                     DEGRADED     0     0     0
            /dev/disk/by-id/ata-QEMU_HARDDISK_QM00015  ONLINE       0     0     0
            /dev/disk/by-id/2252240780663618111        UNAVAIL      0     0     0  was /dev/disk/by-id/ata-QEMU_HARDDISK_QM00017-part1

errors: No known data errors";
        private const string BadPool2 = @"";

        private const string PoolWithSpare = @"  pool: TestP768e8f58-4091-4c3a-91c6-35c65c7336a1
 state: ONLINE
config:

	NAME                                           STATE     READ WRITE CKSUM
	TestP768e8f58-4091-4c3a-91c6-35c65c7336a1      ONLINE       0     0     0
	  raidz1-0                                     ONLINE       0     0     0
	    /tmp/f067c903-c3a8-46ee-bf6b-12d8e84ccbcf  ONLINE       0     0     0
	    /tmp/af5e1f8a-2f5c-4e8a-a809-09ce20683e28  ONLINE       0     0     0
	spares
	  /tmp/7750f053-1937-4d0a-a104-29ead1847366    AVAIL";

        [TestMethod]
        public void ParseOKStatus()
        {

            var status = ZPoolStatusParser.Parse(OkPool);
            Assert.IsNotNull(status);
            Assert.AreEqual("tank", status.Pool.PoolName);
            Assert.AreEqual(State.Online, status.State);
            Console.WriteLine(status.Dump(new JsonFormatter()));

        }

        [TestMethod]
        public void PoolNotFoundShouldReturnInNull()
        {
            var status = ZPoolStatusParser.Parse("cannot open 'unbabunga': no such pool");
            Assert.IsNull(status);
        }

        [TestMethod]
        public void ParseTruncatedStatus()
        {
            Assert.ThrowsException<FormatException>(() => ZPoolStatusParser.Parse(TruncatedResponse));
        }

        [TestMethod]
        public void ParseBadStatus1Status()
        {

            var status = ZPoolStatusParser.Parse(BadPool1);
            Assert.IsNotNull(status);
            Assert.AreEqual("tank", status.Pool.PoolName);
            Assert.AreEqual(State.Degraded, status.State);
            Console.WriteLine(status.Dump(new JsonFormatter()));

        }

        [TestMethod]
        public void ParsePoolStatusWithSpares()
        {
            var status = ZPoolStatusParser.Parse(PoolWithSpare);
            Assert.IsNotNull(status);
            var spares = status.Pool.VDevs.First(d => d.VDevName == "spares");
            Assert.AreEqual(State.Available, spares.State);
            var spare = spares.Devices.First();
            Assert.AreEqual(State.Available, spare.State);
            Console.WriteLine(status.Dump(new JsonFormatter()));
        }
    }
}
