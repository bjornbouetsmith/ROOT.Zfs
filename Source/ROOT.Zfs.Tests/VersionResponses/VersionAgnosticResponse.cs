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
                case "/bin/lsblk --include 8 --include 259 -p|grep disk":
                    return (LoadBlockDevices(), null);
                case "/bin/ls -l /dev/disk/by-id/ | awk -F ' ' '{print $9,$11}'":
                    return (LoadDisks(), null);
                case "/sbin/zfs list -Hp -o type,creation,name,used,refer,avail,mountpoint -t filesystem":
                case "/sbin/zfs list -Hp -o type,creation,name,used,refer,avail,mountpoint -t filesystem,volume":
                    return (LoadFileSystems(""), null);
                case "/sbin/zfs list -Hp -o type,creation,name,used,refer,avail,mountpoint -t filesystem tank":
                case "/sbin/zfs list -Hp -o type,creation,name,used,refer,avail,mountpoint -t filesystem,volume tank":
                case "/sbin/zfs list -Hpr -o type,creation,name,used,refer,avail,mountpoint -d 99 -t filesystem tank":
                    return (LoadFileSystems("tank"), null);
                case "/sbin/zfs list -Hp -o type,creation,name,used,refer,avail,mountpoint -t filesystem tank/myds":
                case "/sbin/zfs list -Hp -o type,creation,name,used,refer,avail,mountpoint -t filesystem,volume tank/myds":
                case "/sbin/zfs list -Hpr -o type,creation,name,used,refer,avail,mountpoint -d 99 -t filesystem tank/myds":
                case "/sbin/zfs list -Hp -o type,creation,name,used,refer,avail,mountpoint tank/myds":
                    return (LoadFileSystems("tank/myds"), null);
                case "/sbin/zfs list -Hp -o type,creation,name,used,refer,avail,mountpoint -t filesystem ungabunga":
                case "/sbin/zfs list -Hp -o type,creation,name,used,refer,avail,mountpoint -t filesystem,volume ungabunga":
                case "/sbin/zfs list -Hpr -o type,creation,name,used,refer,avail,mountpoint -d 99 -t filesystem ungabunga":
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
                case "/sbin/zfs list -Hpr -o type,creation,name,used,refer,avail,mountpoint -d 99 -t snapshot tank/myds":
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
                case "/sbin/zpool create mytest draid1:2d:4c:1s /dev/sda /dev/sdb /dev/sdc /dev/sdd":
                    return (null, null);
                case "/sbin/zpool destroy -f mytest":
                    return (null, null);
                case "/sbin/zpool history -l tank":
                    return (GetTankHistory(), null);
                case "/sbin/zpool list -PHp":
                    return (GetAllPoolInfos(""), null);
                case "/sbin/zpool list -PHp tank2":
                    return (GetAllPoolInfos("tank2"), null);
                case "/sbin/smartctl -x /dev/disk/by-id/scsi-0QEMU_QEMU_HARDDISK_drive-scsi5":
                    return (GetSmartInfoSCSI5(), null);
                case "/sbin/smartctl -x /dev/disk/by-id/scsi-0QEMU_QEMU_HARDDISK_drive-scsi6":
                    return (GetSmartInfoSCSI6(), null);
                case "/sbin/zpool get -H":
                    return (null, GetAvailablePoolProperties());
                case "/sbin/zfs create -o atime=off tank/myds":
                    return (null, null);
                case "/bin/which zfs":
                    return (Core.Commands.Commands.WhichZfs, null);
                case "/bin/which zpool":
                    return (Core.Commands.Commands.WhichZpool, null);
                case "/bin/which zdb":
                    return (Core.Commands.Commands.WhichZdb, null);
                case "/bin/which ls":
                    return (Core.Commands.Commands.WhichLs, null);
                case "/bin/which lsblk":
                    return (Core.Commands.Commands.WhichLsblk, null);
                case "/bin/which smartctl":
                    return (null, "which: no smartctl in (/sbin:/bin:/usr/sbin:/usr/bin)");
                case "/sbin/zpool iostat -LlpPvH tank /dev/sda":
                    return (GetIOStats(), null);
                case "/sbin/zpool offline tank /dev/sda":
                    return (null, null);
                case "/sbin/zpool online tank /dev/sda":
                    return (null, null);
                case "/sbin/zpool clear tank /dev/sda":
                    return (null, null);
                case "/sbin/zpool resilver tank":
                    return (null, null);
                case "/sbin/zfs promote tank/myds":
                    return (null, null);
                case "/sbin/zfs mount tank/myds":
                    return (null, null);
                case "/sbin/zfs unmount tank/myds":
                    return (null, null);
                case "/sbin/zfs hold mytag tank/myds@12345":
                    return (null, null);
                case "/sbin/zfs holds -H tank/myds@12345":
                    return ("tank/myds@12345  mytag  Wed Oct 26 09:20 2022", null);
                case "/sbin/zfs release mytag tank/myds@12345":
                    return (null, null);
                case "/sbin/zpool scrub tank":
                    return (null, null);
                case "/sbin/zpool trim tank":
                    return (null, null);
                case "/sbin/zpool upgrade tank":
                    return (null, null);
                case "/sbin/zpool upgrade":
                    return (GetUpgradeablePools(), null);
                case "/sbin/zdb":
                    return (GetZdbOutput(), null);
                case "/sbin/zpool detach tank /dev/sdb":
                    return (null, null);
                case "/sbin/zpool attach tank /dev/sdb /dev/sdc":
                    return (null, null);
                case "/sbin/zpool replace tank /dev/sdb /dev/sdc":
                    return (null, null);
                case "/sbin/zpool add -f tank mirror /dev/sda /dev/sdb":
                    return (null, null);
                case "/sbin/zpool remove tank mirror-1":
                    return (null, null);
                default:
                    throw new NotImplementedException($"Missing FAKE implementation of {commandLine}");
            }
        }

        private static string GetZdbOutput()
        {
           return @"tank:
    version: 5000
    name: 'backup'
    state: 0
    txg: 810574
    pool_guid: 11716779571321215786
    errata: 0
    hostid: 3627665124
    hostname: 'pve3'
    com.delphix:has_per_vdev_zaps
    hole_array[0]: 1
    vdev_children: 2
    vdev_tree:
        type: 'root'
        id: 0
        guid: 11716779571321215786
        create_txg: 4
        children[0]:
            type: 'mirror'
            id: 0
            guid: 10134074011336589054
            metaslab_array: 256
            metaslab_shift: 33
            ashift: 12
            asize: 958044831744
            is_log: 0
            create_txg: 4
            com.delphix:vdev_zap_top: 129
            children[0]:
                type: 'disk'
                id: 0
                guid: 2372260810221097781
                path: '/dev/disk/by-id/wwn-0x500a0751286aa686-part2'
                phys_path: 'id1,enc@n3061686369656d30/type@0/slot@4/elmdesc@Slot_03/p2'
                DTL: 63547
                create_txg: 4
                com.delphix:vdev_zap_leaf: 130
            children[1]:
                type: 'disk'
                id: 1
                guid: 6037064352240182977
                path: '/dev/disk/by-id/wwn-0x500a07512869791d-part2'
                phys_path: 'id1,enc@n3061686369656d30/type@0/slot@6/elmdesc@Slot_05/p2'
                DTL: 63546
                create_txg: 4
                com.delphix:vdev_zap_leaf: 131
        children[1]:
            type: 'hole'
            id: 1
            guid: 0
            whole_disk: 0
            metaslab_array: 0
            metaslab_shift: 0
            ashift: 0
            asize: 0
            is_log: 0
            is_hole: 1
    features_for_read:
        com.delphix:hole_birth
        com.delphix:embedded_data
tank2:
    version: 5000
    name: 'tank2'
    state: 0
    txg: 14800556
    pool_guid: 6172714080073011794
    errata: 0
    hostid: 3627665124
    hostname: 'pve3'
    com.delphix:has_per_vdev_zaps
    hole_array[0]: 1
    vdev_children: 2
    vdev_tree:
        type: 'root'
        id: 0
        guid: 6172714080073011794
        create_txg: 4
        children[0]:
            type: 'mirror'
            id: 0
            guid: 14919969538254957009
            metaslab_array: 39
            metaslab_shift: 36
            ashift: 12
            asize: 11997986160640
            is_log: 0
            create_txg: 4
            com.delphix:vdev_zap_top: 36
            children[0]:
                type: 'disk'
                id: 0
                guid: 2614503943830512158
                path: '/dev/disk/by-id/wwn-0x5000cca26ff026b9-part2'
                phys_path: 'id1,enc@n3061686369656d30/type@0/slot@5/elmdesc@Slot_04/p2'
                whole_disk: 1
                DTL: 402
                create_txg: 4
                com.delphix:vdev_zap_leaf: 37
            children[1]:
                type: 'disk'
                id: 1
                guid: 17927660636314546953
                path: '/dev/disk/by-id/ata-WDC_WD120EDAZ-11F3RA0_8CKE63TE-part2'
                phys_path: 'id1,enc@n3061686369656d30/type@0/slot@3/elmdesc@Slot_02/p2'
                whole_disk: 1
                DTL: 401
                create_txg: 4
                com.delphix:vdev_zap_leaf: 38
        children[1]:
            type: 'hole'
            id: 1
            guid: 0
            whole_disk: 0
            metaslab_array: 0
            metaslab_shift: 0
            ashift: 0
            asize: 0
            is_log: 0
            is_hole: 1
    features_for_read:
        com.delphix:hole_birth
        com.delphix:embedded_data
";
        }

        private static string GetUpgradeablePools()
        {
                return @"This system supports ZFS pool feature flags.

All pools are formatted using feature flags.


Some supported features are not enabled on the following pools. Once a
feature is enabled the pool may become incompatible with software
that does not support the feature. See zpool-features(7) for details.

Note that the pool 'compatibility' feature can be used to inhibit
feature upgrades.

POOL  FEATURE
---------------
backup
      edonr
      draid
";
        }

        private static string GetIOStats()
        {
            return "/dev/sda       0       0       1       17      47969   195641  2202421 669642  727243  176755  723335  95931   693500  514367  10005303        4934782";
        }

        private static string GetAvailablePoolProperties()
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

        private static string GetSmartInfoSCSI6()
        {
            return @"smartctl 7.2 2020-12-30 r5155 [x86_64-linux-5.15.35-1-pve] (local build)
Copyright (C) 2002-20, Bruce Allen, Christian Franke, www.smartmontools.org

=== START OF INFORMATION SECTION ===
Model Family:     Toshiba HK4R Series SSD
Device Model:     TOSHIBA THNSN8960PCSE
Serial Number:    26MS109WTB1V
LU WWN Device Id: 5 00080d 910596db9
Firmware Version: 8EET6101
User Capacity:    960,197,124,096 bytes [960 GB]
Sector Size:      512 bytes logical/physical
Rotation Rate:    Solid State Device
Form Factor:      2.5 inches
TRIM Command:     Available, deterministic, zeroed
Device is:        In smartctl database [for details use: -P show]
ATA Version is:   ACS-3 (minor revision not indicated)
SATA Version is:  SATA 3.2, 6.0 Gb/s (current: 6.0 Gb/s)
Local Time is:    Sun Oct 16 13:11:01 2022 CEST
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
Total time to complete Offline
data collection:                (  120) seconds.
Offline data collection
capabilities:                    (0x5b) SMART execute Offline immediate.
                                        Auto Offline data collection on/off support.
                                        Suspend Offline collection upon new
                                        command.
                                        Offline surface scan supported.
                                        Self-test supported.
                                        No Conveyance Self-test supported.
                                        Selective Self-test supported.
SMART capabilities:            (0x0003) Saves SMART data before entering
                                        power-saving mode.
                                        Supports SMART auto save timer.
Error logging capability:        (0x01) Error logging supported.
                                        General Purpose Logging supported.
Short self-test routine
recommended polling time:        (   2) minutes.
Extended self-test routine
recommended polling time:        (  34) minutes.
SCT capabilities:              (0x003d) SCT Status supported.
                                        SCT Error Recovery Control supported.
                                        SCT Feature Control supported.
                                        SCT Data Table supported.

SMART Attributes Data Structure revision number: 16
Vendor Specific SMART Attributes with Thresholds:
ID# ATTRIBUTE_NAME          FLAG     VALUE WORST THRESH TYPE      UPDATED  WHEN_FAILED RAW_VALUE
  1 Raw_Read_Error_Rate     0x000a   100   100   000    Old_age   Always       -       0
  2 Throughput_Performance  0x0005   100   100   050    Pre-fail  Offline      -       0
  3 Spin_Up_Time            0x0007   100   100   050    Pre-fail  Always       -       0
  5 Reallocated_Sector_Ct   0x0013   100   100   050    Pre-fail  Always       -       0
  7 Unknown_SSD_Attribute   0x000b   100   100   050    Pre-fail  Always       -       0
  8 Unknown_SSD_Attribute   0x0005   100   100   050    Pre-fail  Offline      -       0
  9 Power_On_Hours          0x0012   100   100   000    Old_age   Always       -       50118
 10 Unknown_SSD_Attribute   0x0013   100   100   050    Pre-fail  Always       -       0
 12 Power_Cycle_Count       0x0012   100   100   000    Old_age   Always       -       58
167 SSD_Protect_Mode        0x0022   100   100   000    Old_age   Always       -       0
168 SATA_PHY_Error_Count    0x0012   100   100   000    Old_age   Always       -       20
169 Bad_Block_Count         0x0013   100   100   010    Pre-fail  Always       -       100
170 Unknown_Attribute       0x0013   100   100   010    Pre-fail  Always       -       0
173 Erase_Count             0x0012   200   200   000    Old_age   Always       -       2019349
174 Unknown_Attribute       0x0012   200   200   000    Old_age   Always       -       1300879
175 Program_Fail_Count_Chip 0x0013   100   100   010    Pre-fail  Always       -       0
187 Reported_Uncorrect      0x0032   100   100   000    Old_age   Always       -       0
192 Power-Off_Retract_Count 0x0012   100   100   000    Old_age   Always       -       51
194 Temperature_Celsius     0x0022   063   057   000    Old_age   Always       -       37 (Min/Max 16/43)
197 Current_Pending_Sector  0x0012   100   100   000    Old_age   Always       -       0
232 Available_Reservd_Space 0x0022   100   100   000    Old_age   Always       -       0
240 Unknown_SSD_Attribute   0x0013   100   100   050    Pre-fail  Always       -       0
241 Total_LBAs_Written      0x0012   100   100   000    Old_age   Always       -       1508762
242 Total_LBAs_Read         0x0012   100   100   000    Old_age   Always       -       1861766
243 Unknown_Attribute       0x0012   100   100   000    Old_age   Always       -       17
249 Unknown_Attribute       0x0022   100   100   000    Old_age   Always       -       96871

SMART Error Log Version: 1
ATA Error Count: 19 (device log contains only the most recent five errors)
        CR = Command Register [HEX]
        FR = Features Register [HEX]
        SC = Sector Count Register [HEX]
        SN = Sector Number Register [HEX]
        CL = Cylinder Low Register [HEX]
        CH = Cylinder High Register [HEX]
        DH = Device/Head Register [HEX]
        DC = Device Command Register [HEX]
        ER = Error register [HEX]
        ST = Status register [HEX]
Powered_Up_Time is measured from power on, and printed as
DDd+hh:mm:SS.sss where DD=days, hh=hours, mm=minutes,
SS=sec, and sss=millisec. It ""wraps"" after 49.710 days.

Error 19 occurred at disk power-on lifetime: 45784 hours(1907 days + 16 hours)
  When the command that caused the error occurred, the device was active or idle.

  After command completion occurred, registers were:
            ER ST SC SN CL CH DH
            --------------
  84 51 00 00 00 00 00

  Commands leading to the command that caused the error were:
  CR FR SC SN CL CH DH DC   Powered_Up_Time Command/ Feature_Name
 ----------------------------------------------------
  ff ff ff ff ff ff ff ff      00:19:57.106[VENDOR SPECIFIC]
  ff ff ff ff ff ff ff ff      00:19:57.106[VENDOR SPECIFIC]
  61 90 08 f8 62 00 40 00      00:19:57.105  WRITE FPDMA QUEUED
  61 00 00 f8 61 00 40 00      00:19:57.104  WRITE FPDMA QUEUED
  61 08 08 f0 61 00 40 00      00:19:57.103  WRITE FPDMA QUEUED

Error 18 occurred at disk power-on lifetime: 45784 hours(1907 days + 16 hours)
  When the command that caused the error occurred, the device was active or idle.

  After command completion occurred, registers were:
            ER ST SC SN CL CH DH
            --------------
  84 51 00 00 00 00 00

  Commands leading to the command that caused the error were:
  CR FR SC SN CL CH DH DC   Powered_Up_Time Command/ Feature_Name
 ----------------------------------------------------
  ff ff ff ff ff ff ff ff      00:19:20.000[VENDOR SPECIFIC]
  ff ff ff ff ff ff ff ff      00:19:20.000[VENDOR SPECIFIC]
  61 08 00 20 60 00 40 00      00:19:19.999  WRITE FPDMA QUEUED
  61 08 00 38 39 00 40 00      00:19:19.999  WRITE FPDMA QUEUED
  61 10 00 78 31 00 40 00      00:19:19.999  WRITE FPDMA QUEUED

Error 17 occurred at disk power-on lifetime: 45784 hours(1907 days + 16 hours)
  When the command that caused the error occurred, the device was active or idle.

  After command completion occurred, registers were:
            ER ST SC SN CL CH DH
            --------------
  84 51 00 00 00 00 00

  Commands leading to the command that caused the error were:
  CR FR SC SN CL CH DH DC   Powered_Up_Time Command/ Feature_Name
 ----------------------------------------------------
  ff ff ff ff ff ff ff ff      00:17:52.961[VENDOR SPECIFIC]
  ff ff ff ff ff ff ff ff      00:17:52.961[VENDOR SPECIFIC]
  61 08 00 b0 5a 00 40 00      00:17:52.959  WRITE FPDMA QUEUED
  61 18 00 98 5a 00 40 00      00:17:52.959  WRITE FPDMA QUEUED
  61 08 00 90 5a 00 40 00      00:17:52.959  WRITE FPDMA QUEUED

Error 16 occurred at disk power-on lifetime: 45784 hours(1907 days + 16 hours)
  When the command that caused the error occurred, the device was active or idle.

  After command completion occurred, registers were:
            ER ST SC SN CL CH DH
            --------------
  84 51 00 00 00 00 00

  Commands leading to the command that caused the error were:
  CR FR SC SN CL CH DH DC   Powered_Up_Time Command/ Feature_Name
 ----------------------------------------------------
  ff ff ff ff ff ff ff ff      00:14:48.633[VENDOR SPECIFIC]
  ff ff ff ff ff ff ff ff      00:14:48.633[VENDOR SPECIFIC]
  61 28 00 78 5a 00 40 00      00:14:48.632  WRITE FPDMA QUEUED
  61 20 00 98 35 00 40 00      00:14:48.632  WRITE FPDMA QUEUED
  61 30 00 f0 52 00 40 00      00:14:48.632  WRITE FPDMA QUEUED

Error 15 occurred at disk power-on lifetime: 45784 hours(1907 days + 16 hours)
  When the command that caused the error occurred, the device was active or idle.

  After command completion occurred, registers were:
            ER ST SC SN CL CH DH
            --------------
  84 51 00 00 00 00 00

  Commands leading to the command that caused the error were:
  CR FR SC SN CL CH DH DC   Powered_Up_Time Command/ Feature_Name
 ----------------------------------------------------
  ff ff ff ff ff ff ff ff      00:12:45.079[VENDOR SPECIFIC]
  ff ff ff ff ff ff ff ff      00:12:45.079[VENDOR SPECIFIC]
  61 40 00 d8 32 00 40 00      00:12:45.078  WRITE FPDMA QUEUED
  ea 00 00 00 00 00 00 00      00:12:43.824  FLUSH CACHE EXT
  61 40 00 d8 31 00 40 00      00:12:43.824  WRITE FPDMA QUEUED

SMART Self - test log structure revision number 1
Num Test_Description    Status Remaining  LifeTime(hours)  LBA_of_first_error
# 1  Extended offline    Completed without error       00%     45785         -
# 2  Short offline       Completed without error       00%     45784         -
# 3  Short offline       Completed without error       00%        19         -
# 4  Short offline       Aborted by host               00%        19         -
# 5  Short offline       Completed without error       00%        14         -
# 6  Short offline       Completed without error       00%        14         -

SMART Selective self-test log data structure revision number 1
 SPAN MIN_LBA  MAX_LBA CURRENT_TEST_STATUS
    1        0        0  Not_testing
    2        0        0  Not_testing
    3        0        0  Not_testing
    4        0        0  Not_testing
    5        0        0  Not_testing
Selective self - test flags(0x0):
  After scanning selected spans, do NOT read-scan remainder of disk.
If Selective self - test is pending on power - up, resume after 0 minute delay.";
        }

        private static string GetSmartInfoSCSI5()
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
Local Time is:    Sun Oct 16 13:10:28 2022 CEST
SMART support is: Available - device has SMART capability.
SMART support is: Enabled

=== START OF READ SMART DATA SECTION ===
SMART overall-health self-assessment test result: FAIL

General SMART Values:
Offline data collection status:  (0x00) Offline data collection activity
                                        was never started.
                                        Auto Offline Data Collection: Disabled.
Self-test execution status:      (   0) The previous self-test routine completed
                                        without error or no self-test has ever
                                        been run.
Total time to complete Offline
data collection:                (  120) seconds.
Offline data collection
capabilities:                    (0x5b) SMART execute Offline immediate.
                                        Auto Offline data collection on/off support.
                                        Suspend Offline collection upon new
                                        command.
                                        Offline surface scan supported.
                                        Self-test supported.
                                        No Conveyance Self-test supported.
                                        Selective Self-test supported.
SMART capabilities:            (0x0003) Saves SMART data before entering
                                        power-saving mode.
                                        Supports SMART auto save timer.
Error logging capability:        (0x01) Error logging supported.
                                        General Purpose Logging supported.
Short self-test routine
recommended polling time:        (   2) minutes.
Extended self-test routine
recommended polling time:        (  34) minutes.
SCT capabilities:              (0x003d) SCT Status supported.
                                        SCT Error Recovery Control supported.
                                        SCT Feature Control supported.
                                        SCT Data Table supported.

SMART Attributes Data Structure revision number: 16
Vendor Specific SMART Attributes with Thresholds:
ID# ATTRIBUTE_NAME          FLAG     VALUE WORST THRESH TYPE      UPDATED  WHEN_FAILED RAW_VALUE
  1 Raw_Read_Error_Rate     0x000a   100   014   000    Old_age   Always       -       0
  2 Throughput_Performance  0x0005   100   100   050    Pre-fail  Offline      -       0
  3 Spin_Up_Time            0x0007   100   100   050    Pre-fail  Always       -       0
  5 Reallocated_Sector_Ct   0x0013   100   100   050    Pre-fail  Always       -       0
  7 Unknown_SSD_Attribute   0x000b   100   100   050    Pre-fail  Always       -       0
  8 Unknown_SSD_Attribute   0x0005   100   100   050    Pre-fail  Offline      -       0
  9 Power_On_Hours          0x0012   100   100   000    Old_age   Always       -       50118
 10 Unknown_SSD_Attribute   0x0013   100   100   050    Pre-fail  Always       -       0
 12 Power_Cycle_Count       0x0012   100   100   000    Old_age   Always       -       50
167 SSD_Protect_Mode        0x0022   100   100   000    Old_age   Always       -       0
168 SATA_PHY_Error_Count    0x0012   100   100   000    Old_age   Always       -       0
169 Bad_Block_Count         0x0013   100   100   010    Pre-fail  Always       -       100
170 Unknown_Attribute       0x0013   100   100   010    Pre-fail  Always       -       0
173 Erase_Count             0x0012   200   200   000    Old_age   Always       -       1622538
174 Unknown_Attribute       0x0012   200   200   000    Old_age   Always       -       1156337
175 Program_Fail_Count_Chip 0x0013   100   100   010    Pre-fail  Always       -       0
187 Reported_Uncorrect      0x0032   100   100   000    Old_age   Always       -       0
192 Power-Off_Retract_Count 0x0012   100   100   000    Old_age   Always       -       44
194 Temperature_Celsius     0x0022   063   057   000    Old_age   Always       -       37 (Min/Max 16/43)
197 Current_Pending_Sector  0x0012   100   100   000    Old_age   Always       -       0
232 Available_Reservd_Space 0x0022   100   100   000    Old_age   Always       -       0
240 Unknown_SSD_Attribute   0x0013   100   100   050    Pre-fail  Always       -       0
241 Total_LBAs_Written      0x0012   100   100   000    Old_age   Always       -       1262090
242 Total_LBAs_Read         0x0012   100   100   000    Old_age   Always       -       1401957
243 Unknown_Attribute       0x0012   100   100   000    Old_age   Always       -       3379
249 Unknown_Attribute       0x0022   100   100   000    Old_age   Always       -       76171

SMART Error Log Version: 1
No Errors Logged

SMART Self-test log structure revision number 1
Num  Test_Description    Status                  Remaining  LifeTime(hours)  LBA_of_first_error
# 1  Extended offline    Completed without error       00%     45785         -
# 2  Short offline       Completed without error       00%     45784         -
# 3  Short offline       Completed without error       00%        17         -
# 4  Short offline       Aborted by host               00%        17         -
# 5  Short offline       Completed without error       00%        13         -
# 6  Short offline       Completed without error       00%        13         -

SMART Selective self-test log data structure revision number 1
 SPAN  MIN_LBA  MAX_LBA  CURRENT_TEST_STATUS
    1        0        0  Not_testing
    2        0        0  Not_testing
    3        0        0  Not_testing
    4        0        0  Not_testing
    5        0        0  Not_testing
Selective self-test flags (0x0):
  After scanning selected spans, do NOT read-scan remainder of disk.
If Selective self-test is pending on power-up, resume after 0 minute delay.";
        }

        private static string GetAllPoolInfos(string filter)
        {
            var lines = @"tank    16642998272     3232256 16639766016     -       -       1.3       2.5       1.13    ONLINE  -
tank2   16642998272     107520  16642890752     -       -       7.30       6.50       1.43    ONLINE  -";

            if (!string.IsNullOrWhiteSpace(filter))
            {
                var filtered = lines.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Where(l => l.StartsWith(filter));
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
            return string.Join("\r\n", content.Split('\r', '\n').Where(l => l.Contains(contains)));
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