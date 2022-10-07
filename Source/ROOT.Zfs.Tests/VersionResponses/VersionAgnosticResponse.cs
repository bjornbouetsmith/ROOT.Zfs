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
                case "/usr/bin/lsblk --include 8 --include 259 -p|grep disk":
                    return (LoadBlockDevices(), null);
                case "/usr/bin/ls -l /dev/disk/by-id/ | awk -F ' ' '{print $9,$11}'":
                    return (LoadDisks(), null);
                case "/sbin/zfs list -Hpr -o type,creation,name,used,refer,avail,mountpoint -t filesystem":
                    return (LoadFileSystems(""), null);
                case "/sbin/zfs list -Hpr -o type,creation,name,used,refer,avail,mountpoint -t filesystem tank":
                    return (LoadFileSystems("tank"), null);
                case "/sbin/zfs list -Hpr -o type,creation,name,used,refer,avail,mountpoint -t filesystem tank/myds":
                    return (LoadFileSystems("tank/myds"), null);
                case "/sbin/zfs list -Hpr -o type,creation,name,used,refer,avail,mountpoint -t filesystem ungabunga":
                    return (null, "cannot open 'ungabunga': dataset does not exist");
                case "/sbin/zfs get -H":
                    return (null, GetAvailableProperties());
                case "/sbin/zfs get all tank/myds -H":
                    return (GetAllProperties(), null);
                case "/sbin/zfs set atime=off tank/myds":
                case "/sbin/zfs set atime=on tank/myds":
                    return (null, null);
                case "/sbin/zfs get atime tank/myds -H":
                    return ("tank/myd	satime	off	local", null);
                case "/sbin/zfs inherit -rS atime tank/myds":
                    return (null, null);
                case "/sbin/zfs list -Hpr -o type,creation,name,used,refer,avail,mountpoint -t snapshot tank/myds":
                    return (GetSnapshots(), null);
                case "/sbin/zfs snap tank/myds@RemoteCreateSnapshot20220922091347":
                case "/sbin/zfs snap tank/myds@20220922211347000-1":
                case "/sbin/zfs snap tank/myds@20220922211347000-2":
                case "/sbin/zfs snap tank/myds@20220922211347000-3":
                    return (null, null);
                case "/sbin/zfs destroy tank/myds@RemoteCreateSnapshot20220922091347":
                case "/sbin/zfs destroy tank/myds@20220922211347000-1":
                case "/sbin/zfs destroy tank/myds@20220922211347000-2":
                case "/sbin/zfs destroy tank/myds@20220922211347000-3":
                case "/sbin/zfs destroy -r tank/myds":
                    return (null, null);
                case "/sbin/zpool status -vP tank":
                    return (GetTankPoolStatus(), null);
                case "/sbin/zpool status -vP mytest":
                    return (GetMyTestPoolStatus(), null);
                case "/sbin/zpool create mytest mirror /dev/sda /dev/sdb":
                return (null, null);
                case "/sbin/zpool destroy -f mytest":
                    return (null, null);
                case "/sbin/zpool history -l tank":
                    return (GetTankHistory(), null);
                case "/sbin/zpool list -PH":
                    return (GetAllPoolInfos(""), null);
                case "/sbin/zpool list -PH tank2":
                    return (GetAllPoolInfos("tank2"), null);
                case "/usr/sbin/smartctl -a /dev/disk/by-id/scsi-0QEMU_QEMU_HARDDISK_drive-scsi5":
                    return (GetSmartInfoSCSI5(), null);
                case "/usr/sbin/smartctl -a /dev/disk/by-id/scsi-0QEMU_QEMU_HARDDISK_drive-scsi6":
                    return (GetSmartInfoSCSI6(), null);
                case "/sbin/zpool get -H":
                    return (null, GetAvailablePoolProperties());
                case "/sbin/zfs create -o atime=off tank/myds":
                    return (null, null);

                default:
                    throw new NotImplementedException($"Missing FAKE implementation of {commandLine}");
            }
        }

        private string GetAvailablePoolProperties()
        {
            return @"usage:
        get [-Hp] [-o ""all"" | field[,...]] <""all"" | property[,...]> <pool> ...

the following properties are supported:

        PROPERTY             EDIT   VALUES

        allocated              NO   <size>
        capacity               NO   <size>
        checkpoint             NO   <size>
        dedupratio             NO   <1.00x or higher if deduped>
        expandsize             NO   <size>
        fragmentation          NO   <percent>
        free                   NO   <size>
        freeing                NO   <size>
        guid                   NO   <guid>
        health                 NO   <state>
        leaked                 NO   <size>
        load_guid              NO   <load_guid>
        size                   NO   <size>
        altroot               YES   <path>
        ashift                YES   <ashift, 9-16, or 0=default>
        autoexpand            YES   on | off
        autoreplace           YES   on | off
        autotrim              YES   on | off
        bootfs                YES   <filesystem>
        cachefile             YES   <file> | none
        comment               YES   <comment-string>
        compatibility         YES   <file[,file...]> | off | legacy
        delegation            YES   on | off
        failmode              YES   wait | continue | panic
        listsnapshots         YES   on | off
        multihost             YES   on | off
        readonly              YES   on | off
        version               YES   <version>
        feature@...           YES   disabled | enabled | active

The feature@ properties must be appended with a feature name.
See zpool-features(7).
";
        }

        private string GetSmartInfoSCSI6()
        {
            return @"smartctl 7.2 2020-12-30 r5155 [x86_64-linux-5.15.35-1-pve] (local build)
Copyright (C) 2002-20, Bruce Allen, Christian Franke, www.smartmontools.org

=== START OF INFORMATION SECTION ===
Model Family:     Toshiba HK4R Series SSD
Device Model:     TOSHIBA THNSN8960PCSE
Serial Number:    26MS1049TB1V
LU WWN Device Id: 5 00080d 910596e6c
Firmware Version: 8EET6101
User Capacity:    960,197,124,096 bytes [960 GB]
Sector Size:      512 bytes logical/physical
Rotation Rate:    Solid State Device
Form Factor:      2.5 inches
TRIM Command:     Available, deterministic, zeroed
Device is:        In smartctl database [for details use: -P show]
ATA Version is:   ACS-3 (minor revision not indicated)
SATA Version is:  SATA 3.2, 6.0 Gb/s (current: 6.0 Gb/s)
Local Time is:    Fri Oct  7 16:57:57 2022 CEST
SMART support is: Available - device has SMART capability.
SMART support is: Enabled

=== START OF READ SMART DATA SECTION ===
SMART overall-health self-assessment test result: PASSED

General SMART Values:
Offline data collection status:  (0x00) Offline data collection activity
                                        was never started.
                                        Auto Offline Data Collection: Disabled.
Self-test execution status:      (   0) The previous self-test routine completed
                                        without error or no self-test has ever
                                        been run.
";
        }

        private string GetSmartInfoSCSI5()
        {
            return @"smartctl 7.2 2020-12-30 r5155 [x86_64-linux-5.15.35-1-pve] (local build)
Copyright (C) 2002-20, Bruce Allen, Christian Franke, www.smartmontools.org

=== START OF INFORMATION SECTION ===
Model Family:     Toshiba HK4R Series SSD
Device Model:     TOSHIBA THNSN8960PCSE
Serial Number:    26MS1049TB1V
LU WWN Device Id: 5 00080d 910596e6c
Firmware Version: 8EET6101
User Capacity:    960,197,124,096 bytes [960 GB]
Sector Size:      512 bytes logical/physical
Rotation Rate:    Solid State Device
Form Factor:      2.5 inches
TRIM Command:     Available, deterministic, zeroed
Device is:        In smartctl database [for details use: -P show]
ATA Version is:   ACS-3 (minor revision not indicated)
SATA Version is:  SATA 3.2, 6.0 Gb/s (current: 6.0 Gb/s)
Local Time is:    Fri Oct  7 16:57:57 2022 CEST
SMART support is: Available - device has SMART capability.
SMART support is: Enabled

=== START OF READ SMART DATA SECTION ===
SMART overall-health self-assessment test result: FAILED

General SMART Values:
Offline data collection status:  (0x00) Offline data collection activity
                                        was never started.
                                        Auto Offline Data Collection: Disabled.
Self-test execution status:      (   0) The previous self-test routine completed
                                        without error or no self-test has ever
                                        been run.
";
        }

        private static string GetAllPoolInfos(string filter)
        {
            var lines =  @"tank    15.5G   2.66M   15.5G   -       -       1.2%      0%      1.00x   ONLINE  -
tank2    15.5T   2.66T   15.5T   -       -       7.3%      6.5%      1.43x   ONLINE  -";

            if (!string.IsNullOrWhiteSpace(filter))
            {
                var filtered = lines.Split(new[]{'\r','\n'},StringSplitOptions.RemoveEmptyEntries).Where(l=>l.StartsWith(filter));
                return string.Join("\r\n", filtered);
            }

            return lines;
        }

        private static string GetTankHistory()
        {
            return @"2022-09-20.21:30:14 zpool create tank mirror /dev/disk/by-id/ata-QEMU_HARDDISK_QM00015 /dev/disk/by-id/ata-QEMU_HARDDISK_QM00017 [user 0 (root) on zfsdev.root.dom:linux]
2022-09-20.21:30:45 zpool import -c /etc/zfs/zpool.cache -aN [user 0 (root) on zfsdev.root.dom:linux]
2022-10-03.17:43:33 zfs destroy tank/myds@20221003154325139-1 [user 0 (root) on zfsdev.root.dom:linux]
2022-10-03.17:43:33 zfs destroy tank/myds@20221003154325139-2 [user 0 (root) on zfsdev.root.dom:linux]
2022-10-03.17:43:33 zfs destroy tank/myds@20221003154325139-3 [user 0 (root) on zfsdev.root.dom:linux]
2022-10-03.18:04:00 zfs set atime=off tank/myds [user 0 (root) on zfsdev.root.dom:linux]";
        }

        private static string GetTankPoolStatus()
        {
            return @"  pool: tank
 state: ONLINE
  scan: resilvered 126K in 00:00:00 with 0 errors on Tue Sep 27 17:57:17 2022
config:

        NAME                                                            STATE     READ WRITE CKSUM
        tank                                                            ONLINE       0     0     0
          mirror-0                                                      ONLINE       0     0     0
            /dev/disk/by-id/scsi-0QEMU_QEMU_HARDDISK_drive-scsi6-part1  ONLINE       0     0     0
            /dev/disk/by-id/scsi-0QEMU_QEMU_HARDDISK_drive-scsi5-part1  ONLINE       0     0     0

errors: No known data errors";
        }

        private static string GetMyTestPoolStatus()
        {
            return @"  pool: mytest
 state: ONLINE
  scan: resilvered 126K in 00:00:00 with 0 errors on Tue Sep 27 17:57:17 2022
config:

        NAME                                                            STATE     READ WRITE CKSUM
        mytest                                                          ONLINE       0     0     0
          mirror-0                                                      ONLINE       0     0     0
            /dev/sda                                                    ONLINE       0     0     0
            /dev/sdb                                                    ONLINE       0     0     0

errors: No known data errors";
        }

        private static string GetSnapshots()
        {
            return @"snapshot        1664097229      tank/myds@20220925111346        0       24576   -       -
snapshot        1664115215      tank/myds@20220925161329        15360   25600   -       -
snapshot        1664121611      tank/myds@20220925180007        14336   25600   -       -
snapshot        1664718665      tank/myds@20221002155059        13312   25600   -       -
snapshot        1664718665      tank/myds@20220922211347000-1        13312   25600   -       -
snapshot        1664718665      tank/myds@20220922211347000-2        13312   25600   -       -
snapshot        1664718665      tank/myds@20220922211347000-3        13312   25600   -       -";
        }

        private static string GetAllProperties()
        {
            return @"tank/myds	type	filesystem	-
tank/myds	creation	Wed Sep 21 18:23 2022	-
tank/myds	used	314K	-
tank/myds	available	14.0G	-
tank/myds	referenced	25K	-
tank/myds	compressratio	1.01x	-
tank/myds	mounted	yes	-
tank/myds	quota	none	default
tank/myds	reservation	none	default
tank/myds	recordsize	128K	default
tank/myds	mountpoint	/tank/myds	default
tank/myds	sharenfs	off	default
tank/myds	checksum	on	default
tank/myds	compression	gzip	inherited from tank
tank/myds	atime	off	local
tank/myds	devices	on	default
tank/myds	exec	on	default
tank/myds	setuid	on	default
tank/myds	readonly	off	default
tank/myds	zoned	off	default
tank/myds	snapdir	hidden	default
tank/myds	aclmode	discard	default
tank/myds	aclinherit	restricted	default
tank/myds	createtxg	14698	-
tank/myds	canmount	on	default
tank/myds	xattr	on	default
tank/myds	copies	1	default
tank/myds	version	5	-
tank/myds	utf8only	off	-
tank/myds	normalization	none	-
tank/myds	casesensitivity	sensitive	-
tank/myds	vscan	off	default
tank/myds	nbmand	off	default
tank/myds	sharesmb	off	default
tank/myds	refquota	none	default
tank/myds	refreservation	none	default
tank/myds	guid	5551566229605187679	-
tank/myds	primarycache	all	default
tank/myds	secondarycache	all	default
tank/myds	usedbysnapshots	215K	-
tank/myds	usedbydataset	25K	-
tank/myds	usedbychildren	74K	-
tank/myds	usedbyrefreservation	0B	-
tank/myds	logbias	latency	default
tank/myds	objsetid	82	-
tank/myds	dedup	off	default
tank/myds	mlslabel	none	default
tank/myds	sync	standard	default
tank/myds	dnodesize	legacy	default
tank/myds	refcompressratio	1.04x	-
tank/myds	written	0	-
tank/myds	logicalused	160K	-
tank/myds	logicalreferenced	13K	-
tank/myds	volmode	default	default
tank/myds	filesystem_limit	none	default
tank/myds	snapshot_limit	none	default
tank/myds	filesystem_count	none	default
tank/myds	snapshot_count	none	default
tank/myds	snapdev	hidden	default
tank/myds	acltype	off	default
tank/myds	context	none	default
tank/myds	fscontext	none	default
tank/myds	defcontext	none	default
tank/myds	rootcontext	none	default
tank/myds	relatime	off	default
tank/myds	redundant_metadata	all	default
tank/myds	overlay	on	default
tank/myds	encryption	off	default
tank/myds	keylocation	none	default
tank/myds	keyformat	none	default
tank/myds	pbkdf2iters	0	default
tank/myds	special_small_blocks	0	default
";
        }

        private static string GetAvailableProperties()
        {
            return @"missing property argument
usage:
        get [-rHp] [-d max] [-o ""all"" | field[,...]]
            [-t type[,...]] [-s source[,...]]
            <""all"" | property[,...]> [filesystem|volume|snapshot|bookmark] ...

The following properties are supported:

        PROPERTY       EDIT  INHERIT   VALUES

        available        NO       NO   <size>
        clones           NO       NO   <dataset>[,...]
        compressratio    NO       NO   <1.00x or higher if compressed>
        createtxg        NO       NO   <uint64>
        creation         NO       NO   <date>
        defer_destroy    NO       NO   yes | no
        encryptionroot   NO       NO   <filesystem | volume>
        filesystem_count  NO       NO   <count>
        guid             NO       NO   <uint64>
        keystatus        NO       NO   none | unavailable | available
        logicalreferenced  NO       NO   <size>
        logicalused      NO       NO   <size>
        mounted          NO       NO   yes | no
        objsetid         NO       NO   <uint64>
        origin           NO       NO   <snapshot>
        receive_resume_token  NO       NO   <string token>
        redact_snaps     NO       NO   <snapshot>[,...]
        refcompressratio  NO       NO   <1.00x or higher if compressed>
        referenced       NO       NO   <size>
        snapshot_count   NO       NO   <count>
        type             NO       NO   filesystem | volume | snapshot | bookmark
        used             NO       NO   <size>
        usedbychildren   NO       NO   <size>
        usedbydataset    NO       NO   <size>
        usedbyrefreservation  NO       NO   <size>
        usedbysnapshots  NO       NO   <size>
        userrefs         NO       NO   <count>
        written          NO       NO   <size>
        aclinherit      YES      YES   discard | noallow | restricted | passthrough | passthrough-x
        aclmode         YES      YES   discard | groupmask | passthrough | restricted
        acltype         YES      YES   off | nfsv4 | posix
        atime           YES      YES   on | off
        canmount        YES       NO   on | off | noauto
        casesensitivity  NO      YES   sensitive | insensitive | mixed
        checksum        YES      YES   on | off | fletcher2 | fletcher4 | sha256 | sha512 | skein | edonr
        compression     YES      YES   on | off | lzjb | gzip | gzip-[1-9] | zle | lz4 | zstd | zstd-[1-19] | zstd-fast | zstd-fast-[1-10,20,30,40,50,60,70,80,90,100,500,1000]
        context         YES       NO   <selinux context>
        copies          YES      YES   1 | 2 | 3
        dedup           YES      YES   on | off | verify | sha256[,verify] | sha512[,verify] | skein[,verify] | edonr,verify
        defcontext      YES       NO   <selinux defcontext>
        devices         YES      YES   on | off
        dnodesize       YES      YES   legacy | auto | 1k | 2k | 4k | 8k | 16k
        encryption       NO      YES   on | off | aes-128-ccm | aes-192-ccm | aes-256-ccm | aes-128-gcm | aes-192-gcm | aes-256-gcm
        exec            YES      YES   on | off
        filesystem_limit YES       NO   <count> | none
        fscontext       YES       NO   <selinux fscontext>
        keyformat        NO       NO   none | raw | hex | passphrase
        keylocation     YES       NO   prompt | <file URI> | <https URL> | <http URL>
        logbias         YES      YES   latency | throughput
        mlslabel        YES      YES   <sensitivity label>
        mountpoint      YES      YES   <path> | legacy | none
        nbmand          YES      YES   on | off
        normalization    NO      YES   none | formC | formD | formKC | formKD
        overlay         YES      YES   on | off
        pbkdf2iters      NO       NO   <iters>
        primarycache    YES      YES   all | none | metadata
        quota           YES       NO   <size> | none
        readonly        YES      YES   on | off
        recordsize      YES      YES   512 to 1M, power of 2
        redundant_metadata YES      YES   all | most
        refquota        YES       NO   <size> | none
        refreservation  YES       NO   <size> | none
        relatime        YES      YES   on | off
        reservation     YES       NO   <size> | none
        rootcontext     YES       NO   <selinux rootcontext>
        secondarycache  YES      YES   all | none | metadata
        setuid          YES      YES   on | off
        sharenfs        YES      YES   on | off | NFS share options
        sharesmb        YES      YES   on | off | SMB share options
        snapdev         YES      YES   hidden | visible
        snapdir         YES      YES   hidden | visible
        snapshot_limit  YES       NO   <count> | none
        special_small_blocks YES      YES   zero or 512 to 1M, power of 2
        sync            YES      YES   standard | always | disabled
        utf8only         NO      YES   on | off
        version         YES       NO   1 | 2 | 3 | 4 | 5 | current
        volblocksize     NO      YES   512 to 128k, power of 2
        volmode         YES      YES   default | full | geom | dev | none
        volsize         YES       NO   <size>
        vscan           YES      YES   on | off
        xattr           YES      YES   on | off | dir | sa
        zoned           YES      YES   on | off
        userused@...     NO       NO   <size>
        groupused@...    NO       NO   <size>
        projectused@...  NO       NO   <size>
        userobjused@...  NO       NO   <size>
        groupobjused@...  NO       NO   <size>
        projectobjused@...  NO       NO   <size>
        userquota@...   YES       NO   <size> | none
        groupquota@...  YES       NO   <size> | none
        projectquota@... YES       NO   <size> | none
        userobjquota@... YES       NO   <size> | none
        groupobjquota@... YES       NO   <size> | none
        projectobjquota@... YES       NO   <size> | none
        written@<snap>   NO       NO   <size>
        written#<bookmark>  NO       NO   <size>

Sizes are specified in bytes with standard units such as K, M, G, etc.

User-defined properties can be specified by using a name containing a colon (:).

The {user|group|project}[obj]{used|quota}@ properties must be appended with
a user|group|project specifier of one of these forms:
    POSIX name      (eg: ""matt"")
    POSIX id        (eg: ""126829"")
    SMB name@domain (eg: ""matt@sun"")
    SMB SID         (eg: ""S-1-234-567-89"")";
        }

        private static string LoadFileSystems(string contains)
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

        private static string LoadBlockDevices()
        {
            return @"/dev/sda      8:0    0   16G  0 disk
/dev/sdb      8:16   0   16G  0 disk
";
        }

        private static string LoadDisks()
        {
            return @"scsi-0QEMU_QEMU_HARDDISK_drive-scsi5 ../../sdb
scsi-0QEMU_QEMU_HARDDISK_drive-scsi5-part1 ../../sdb1
scsi-0QEMU_QEMU_HARDDISK_drive-scsi5-part9 ../../sdb9
scsi-0QEMU_QEMU_HARDDISK_drive-scsi6 ../../sda
scsi-0QEMU_QEMU_HARDDISK_drive-scsi6-part1 ../../sda1
scsi-0QEMU_QEMU_HARDDISK_drive-scsi6-part9 ../../sda9
";
        }
    }
}