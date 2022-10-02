namespace ROOT.Zfs.Tests.VersionResponses
{
    internal class VersionAgnosticResponse : IVersionResponse
    {
        private readonly string _version;

        public VersionAgnosticResponse(string version)
        {
            _version = version;
        }
        public virtual string LoadResponse(string commandLine)
        {
            switch (commandLine)
            {
                case "/sbin/zfs --version":
                    return GetVersion();
                case "/usr/bin/lsblk --include 8 -p|grep disk":
                    return LoadBlockDevices();
                case "/usr/bin/ls -l /dev/disk/by-id/ | awk -F ' ' '{print $9,$11}'":
                    return LoadDisks();
                default:
                    return null;
            }
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