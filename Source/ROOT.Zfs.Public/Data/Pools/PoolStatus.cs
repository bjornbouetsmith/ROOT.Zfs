using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROOT.Zfs.Public.Data.Pools
{
    /*
   pool: tank
 state: ONLINE
  scan: resilvered 126K in 00:00:00 with 0 errors on Tue Sep 27 17:57:17 2022
config:

        NAME                           STATE     READ WRITE CKSUM
        tank                           ONLINE       0     0     0
          mirror-0                     ONLINE       0     0     0
            ata-QEMU_HARDDISK_QM00015  ONLINE       0     0     0
            ata-QEMU_HARDDISK_QM00017  ONLINE       0     0     0

  pool: tank
 state: DEGRADED
status: One or more devices could not be used because the label is missing or
        invalid.  Sufficient replicas exist for the pool to continue
        functioning in a degraded state.
action: Replace the device using 'zpool replace'.
   see: https://openzfs.github.io/openzfs-docs/msg/ZFS-8000-4J
config:

        NAME                           STATE     READ WRITE CKSUM
        tank                           DEGRADED     0     0     0
          mirror-0                     DEGRADED     0     0     0
            ata-QEMU_HARDDISK_QM00015  ONLINE       0     0     0
            2252240780663618111        UNAVAIL      0     0     0  was /dev/disk/by-id/ata-QEMU_HARDDISK_QM00017-part1


     */
    public class PoolStatus
    {
        public State State { get; set; }
        public Pool Pool { get; set; }
        public string ScanStatus { get; set; }
        public string StatusText { get; set; }
        public string Action { get; set; }
        public string See { get; set; }
    }
}
