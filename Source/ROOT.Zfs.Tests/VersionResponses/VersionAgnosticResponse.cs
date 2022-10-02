using System;
using System.Linq;

namespace ROOT.Zfs.Tests.VersionResponses
{
    internal class VersionAgnosticResponse : IVersionResponse
    {
        private readonly string _version;

        public VersionAgnosticResponse(string version)
        {
            _version = version;
        }
        public virtual (string StdOut, string StdError) LoadResponse(string commandLine)
        {
            switch (commandLine)
            {
                case "/sbin/zfs --version":
                    return (GetVersion(), null);
                case "/usr/bin/lsblk --include 8 -p|grep disk":
                    return (LoadBlockDevices(), null);
                case "/usr/bin/ls -l /dev/disk/by-id/ | awk -F ' ' '{print $9,$11}'":
                    return (LoadDisks(), null);
                case "/sbin/zfs list -Hpr -o type,creation,name,used,refer,avail,mountpoint -t filesystem":
                    return (LoadFileSystems(""), null);
                case "/sbin/zfs list -Hpr -o type,creation,name,used,refer,avail,mountpoint -t filesystem tank":
                    return (LoadFileSystems("tank"), null);
                case "/sbin/zfs list -Hpr -o type,creation,name,used,refer,avail,mountpoint -t filesystem ungabunga":
                    return (null, "cannot open 'ungabunga': dataset does not exist");
                default:
                    throw new NotImplementedException($"Missing FAKE implementation of {commandLine}");
            }
        }

        private string LoadFileSystems(string contains)
        {

            var content = @"filesystem      1663702213      tank    1111025664      33792   15011878912     /tank
filesystem      1664632596      tank/45dfe1b7-5e45-496c-b887-dcc9eded8201       24576   24576   15011878912     /tank/45dfe1b7-5e45-496c-b887-dcc9eded8201
filesystem      1663777408      tank/myds       292864  25600   15011878912     /tank/myds
filesystem      1664209129      tank/myds/ROOT  75776   25600   15011878912     /tank/myds/ROOT
filesystem      1664210834      tank/myds/ROOT/child    50176   25600   15011878912     /tank/myds/ROOT/child
filesystem      1664210840      tank/myds/ROOT/child/granchild  24576   24576   15011878912     /tank/myds/ROOT/child/granchild
filesystem      1663946563      tank/myds34     24576   24576   524263424       /tank/myds34
filesystem      1663956157      tank/myds37     24576   24576   15011878912     /tank/myds37
filesystem      1663946343      tank/myds44     24576   24576   15011878912     /tank/myds44
filesystem      1663781153      tank/mytestds2  24576   24576   1073717248      /tank/mytestds2";
            return string.Join("\r\n", content.Split(new[] { '\r', '\n' }).Where(l => l.Contains(contains)));
        }

        private string GetVersion()
        {
            return $@"zfs-{_version}
zfs-kmod-{_version}";
        }

        private string LoadBlockDevices()
        {
            return @"/dev/sda      8:0    0   16G  0 disk
/dev/sdb      8:16   0   16G  0 disk
/dev/sdc      8:32   0   16G  0 disk
/dev/sdd      8:48   0   16G  0 disk
/dev/sde      8:64   0   16G  0 disk
/dev/sdf      8:80   0   16G  0 disk
/dev/sdg      8:96   0   32G  0 disk
";
        }

        private string LoadDisks()
        {
            return @"
ata-QEMU_HARDDISK_QM00013 ../../sdg
ata-QEMU_HARDDISK_QM00013-part1 ../../sdg1
ata-QEMU_HARDDISK_QM00013-part2 ../../sdg2
ata-QEMU_HARDDISK_QM00013-part3 ../../sdg3
ata-QEMU_HARDDISK_QM00013-part4 ../../sdg4
scsi-0QEMU_QEMU_HARDDISK_drive-scsi1 ../../sdf
scsi-0QEMU_QEMU_HARDDISK_drive-scsi1-part1 ../../sdf1
scsi-0QEMU_QEMU_HARDDISK_drive-scsi1-part9 ../../sdf9
scsi-0QEMU_QEMU_HARDDISK_drive-scsi2 ../../sde
scsi-0QEMU_QEMU_HARDDISK_drive-scsi2-part1 ../../sde1
scsi-0QEMU_QEMU_HARDDISK_drive-scsi2-part9 ../../sde9
scsi-0QEMU_QEMU_HARDDISK_drive-scsi3 ../../sdd
scsi-0QEMU_QEMU_HARDDISK_drive-scsi3-part1 ../../sdd1
scsi-0QEMU_QEMU_HARDDISK_drive-scsi3-part9 ../../sdd9
scsi-0QEMU_QEMU_HARDDISK_drive-scsi4 ../../sdc
scsi-0QEMU_QEMU_HARDDISK_drive-scsi4-part1 ../../sdc1
scsi-0QEMU_QEMU_HARDDISK_drive-scsi4-part9 ../../sdc9
scsi-0QEMU_QEMU_HARDDISK_drive-scsi5 ../../sdb
scsi-0QEMU_QEMU_HARDDISK_drive-scsi5-part1 ../../sdb1
scsi-0QEMU_QEMU_HARDDISK_drive-scsi5-part9 ../../sdb9
scsi-0QEMU_QEMU_HARDDISK_drive-scsi6 ../../sda
scsi-0QEMU_QEMU_HARDDISK_drive-scsi6-part1 ../../sda1
scsi-0QEMU_QEMU_HARDDISK_drive-scsi6-part9 ../../sda9
";
        }
    }
}