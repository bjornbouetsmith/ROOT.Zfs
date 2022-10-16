using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Shared.Utils.Serialization;
using ROOT.Zfs.Core.Helpers;

namespace ROOT.Zfs.Tests.Helpers
{
    [TestClass]
    public class SmartInfoParserTest
    {
#region Smart output

private const string SmartInfo512 = @"smartctl 7.2 2020-12-30 r5155 [x86_64-linux-5.15.35-1-pve] (local build)
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

private const string SmartInfo512_4096= @"smartctl 7.2 2020-12-30 r5155 [x86_64-linux-5.15.35-1-pve] (local build)
Copyright (C) 2002-20, Bruce Allen, Christian Franke, www.smartmontools.org

=== START OF INFORMATION SECTION ===
Model Family:     Micron 5100 Pro / 52x0 / 5300 SSDs
Device Model:     Micron_5200_MTFDDAK960TDD
Serial Number:    2020286AA686
LU WWN Device Id: 5 00a075 1286aa686
Firmware Version: D1RL004
User Capacity:    960,197,124,096 bytes [960 GB]
Sector Sizes:     512 bytes logical, 4096 bytes physical
Rotation Rate:    Solid State Device
Form Factor:      2.5 inches
TRIM Command:     Available, deterministic, zeroed
Device is:        In smartctl database [for details use: -P show]
ATA Version is:   ACS-3 T13/2161-D revision 5
SATA Version is:  SATA 3.2, 6.0 Gb/s (current: 6.0 Gb/s)
Local Time is:    Sun Oct 16 11:17:07 2022 CEST
SMART support is: Available - device has SMART capability.
SMART support is: Enabled

=== START OF READ SMART DATA SECTION ===
SMART overall-health self-assessment test result: PASSED

General SMART Values:
Offline data collection status:  (0x03) Offline data collection activity
                                        is in progress.
                                        Auto Offline Data Collection: Disabled.
Self-test execution status:      (   0) The previous self-test routine completed
                                        without error or no self-test has ever
                                        been run.
Total time to complete Offline
data collection:                (  935) seconds.
Offline data collection
capabilities:                    (0x7b) SMART execute Offline immediate.
                                        Auto Offline data collection on/off support.
                                        Suspend Offline collection upon new
                                        command.
                                        Offline surface scan supported.
                                        Self-test supported.
                                        Conveyance Self-test supported.
                                        Selective Self-test supported.
SMART capabilities:            (0x0003) Saves SMART data before entering
                                        power-saving mode.
                                        Supports SMART auto save timer.
Error logging capability:        (0x01) Error logging supported.
                                        General Purpose Logging supported.
Short self-test routine
recommended polling time:        (   2) minutes.
Extended self-test routine
recommended polling time:        (   8) minutes.
Conveyance self-test routine
recommended polling time:        (   3) minutes.
SCT capabilities:              (0x0035) SCT Status supported.
                                        SCT Feature Control supported.
                                        SCT Data Table supported.

SMART Attributes Data Structure revision number: 16
Vendor Specific SMART Attributes with Thresholds:
ID# ATTRIBUTE_NAME          FLAG     VALUE WORST THRESH TYPE      UPDATED  WHEN_FAILED RAW_VALUE
  1 Raw_Read_Error_Rate     0x002f   100   100   050    Pre-fail  Always       -       0
  5 Reallocated_Sector_Ct   0x0032   100   100   001    Old_age   Always       -       0
  9 Power_On_Hours          0x0032   100   100   000    Old_age   Always       -       13332
 12 Power_Cycle_Count       0x0032   100   100   001    Old_age   Always       -       20
170 Reserved_Block_Pct      0x0033   100   100   010    Pre-fail  Always       -       0
171 Program_Fail_Count      0x0032   100   100   000    Old_age   Always       -       0
172 Erase_Fail_Count        0x0032   100   100   001    Old_age   Always       -       0
173 Avg_Block-Erase_Count   0x0032   100   100   000    Old_age   Always       -       9
174 Unexpect_Power_Loss_Ct  0x0032   100   100   000    Old_age   Always       -       9
183 SATA_Int_Downshift_Ct   0x0032   100   100   000    Old_age   Always       -       0
184 End-to-End_Error        0x0032   100   100   000    Old_age   Always       -       0
187 Reported_Uncorrect      0x0032   100   100   000    Old_age   Always       -       0
188 Command_Timeout         0x0032   100   100   000    Old_age   Always       -       14
194 Temperature_Celsius     0x0022   071   041   000    Old_age   Always       -       29 (Min/Max 22/59)
195 Hardware_ECC_Recovered  0x0032   100   100   000    Old_age   Always       -       0
196 Reallocated_Event_Count 0x0032   100   100   000    Old_age   Always       -       0
197 Current_Pending_Sector  0x0032   100   100   000    Old_age   Always       -       0
198 Offline_Uncorrectable   0x0030   100   100   000    Old_age   Offline      -       0
199 UDMA_CRC_Error_Count    0x0032   100   100   000    Old_age   Always       -       0
202 Percent_Lifetime_Remain 0x0030   100   100   001    Old_age   Offline      -       0
206 Write_Error_Rate        0x000e   100   100   000    Old_age   Always       -       0
246 Total_LBAs_Written      0x0032   100   100   000    Old_age   Always       -       21640090130
247 Host_Program_Page_Count 0x0032   100   100   000    Old_age   Always       -       676121749
248 Bckgnd_Program_Page_Cnt 0x0032   100   100   000    Old_age   Always       -       77775447
180 Unused_Rsvd_Blk_Cnt_Tot 0x0033   000   000   000    Pre-fail  Always       -       5352
210 RAIN_Success_Recovered  0x0032   100   100   000    Old_age   Always       -       0

SMART Error Log Version: 1
No Errors Logged

SMART Self-test log structure revision number 1
Num  Test_Description    Status                  Remaining  LifeTime(hours)  LBA_of_first_error
# 1  Vendor (0xff)       Completed without error       00%     13332         -
# 2  Vendor (0xff)       Completed without error       00%     13212         -
# 3  Vendor (0xff)       Completed without error       00%     13044         -
# 4  Vendor (0xff)       Completed without error       00%     12853         -
# 5  Vendor (0xff)       Completed without error       00%     12683         -
# 6  Vendor (0xff)       Completed without error       00%     12515         -
# 7  Vendor (0xff)       Completed without error       00%     12346         -
# 8  Vendor (0xff)       Completed without error       00%     12157         -
# 9  Vendor (0xff)       Completed without error       00%     11987         -
#10  Vendor (0xff)       Completed without error       00%     11820         -
#11  Vendor (0xff)       Completed without error       00%     11650         -
#12  Vendor (0xff)       Completed without error       00%     11484         -
#13  Vendor (0xff)       Completed without error       00%     11314         -
#14  Vendor (0xff)       Completed without error       00%     11146         -
#15  Vendor (0xff)       Completed without error       00%     10979         -
#16  Vendor (0xff)       Completed without error       00%     10811         -
#17  Vendor (0xff)       Completed without error       00%     10678         -
#18  Vendor (0xff)       Completed without error       00%     10570         -
#19  Vendor (0xff)       Completed without error       00%     10402         -
#20  Vendor (0xff)       Completed without error       00%     10219         -
#21  Vendor (0xff)       Completed without error       00%     10035         -

SMART Selective self-test log data structure revision number 1
 SPAN  MIN_LBA  MAX_LBA  CURRENT_TEST_STATUS
    1        0        0  Completed [00% left] (261241856-261307391)
    2        0        0  Not_testing
    3        0        0  Not_testing
    4        0        0  Not_testing
    5        0        0  Not_testing
Selective self-test flags (0x0):
  After scanning selected spans, do NOT read-scan remainder of disk.
If Selective self-test is pending on power-up, resume after 0 minute delay.
";

private const string SamsungNVME = @"smartctl 7.2 2020-12-30 r5155 [x86_64-linux-5.15.35-1-pve] (local build)
Copyright (C) 2002-20, Bruce Allen, Christian Franke, www.smartmontools.org

=== START OF INFORMATION SECTION ===
Model Number:                       Samsung SSD 960 EVO 250GB
Serial Number:                      S3ESNX0JC22874D
Firmware Version:                   3B7QCXE7
PCI Vendor/Subsystem ID:            0x144d
IEEE OUI Identifier:                0x002538
Total NVM Capacity:                 250,059,350,016 [250 GB]
Unallocated NVM Capacity:           0
Controller ID:                      2
NVMe Version:                       1.2
Number of Namespaces:               1
Namespace 1 Size/Capacity:          250,059,350,016 [250 GB]
Namespace 1 Utilization:            113,762,824,192 [113 GB]
Namespace 1 Formatted LBA Size:     512
Namespace 1 IEEE EUI-64:            002538 5c71b0595a
Local Time is:                      Sun Oct 16 14:06:27 2022 CEST
Firmware Updates (0x16):            3 Slots, no Reset required
Optional Admin Commands (0x0007):   Security Format Frmw_DL
Optional NVM Commands (0x001f):     Comp Wr_Unc DS_Mngmt Wr_Zero Sav/Sel_Feat
Log Page Attributes (0x03):         S/H_per_NS Cmd_Eff_Lg
Maximum Data Transfer Size:         512 Pages
Warning  Comp. Temp. Threshold:     77 Celsius
Critical Comp. Temp. Threshold:     79 Celsius

Supported Power States
St Op     Max   Active     Idle   RL RT WL WT  Ent_Lat  Ex_Lat
 0 +     6.04W       -        -    0  0  0  0        0       0
 1 +     5.09W       -        -    1  1  1  1        0       0
 2 +     4.08W       -        -    2  2  2  2        0       0
 3 -   0.0400W       -        -    3  3  3  3      210    1500
 4 -   0.0050W       -        -    4  4  4  4     2200    6000

Supported LBA Sizes (NSID 0x1)
Id Fmt  Data  Metadt  Rel_Perf
 0 +     512       0         0

=== START OF SMART DATA SECTION ===
SMART overall-health self-assessment test result: PASSED

SMART/Health Information (NVMe Log 0x02)
Critical Warning:                   0x00
Temperature:                        49 Celsius
Available Spare:                    100%
Available Spare Threshold:          10%
Percentage Used:                    4%
Data Units Read:                    23,412,324 [11.9 TB]
Data Units Written:                 29,908,334 [15.3 TB]
Host Read Commands:                 388,560,918
Host Write Commands:                420,326,836
Controller Busy Time:               1,360
Power Cycles:                       995
Power On Hours:                     4,666
Unsafe Shutdowns:                   79
Media and Data Integrity Errors:    0
Error Information Log Entries:      1,197
Warning  Comp. Temperature Time:    0
Critical Comp. Temperature Time:    0
Temperature Sensor 1:               49 Celsius
Temperature Sensor 2:               52 Celsius

Error Information (NVMe Log 0x01, 16 of 64 entries)
Num   ErrCount  SQId   CmdId  Status  PELoc          LBA  NSID    VS
  0       1197     0  0x1004  0x4004      -            0     0     -
  1       1196     0  0x1009  0x4004      -            0     0     -
  2       1195     0  0x1012  0x4004      -            0     0     -
  3       1194     0  0x100e  0x4004      -            0     0     -
  4       1193     0  0x1006  0x4004      -            0     0     -
  5       1192     0  0x100e  0x4004      -            0     0     -
  6       1191     0  0x0007  0x4004      -            0     0     -
  7       1190     0  0x000a  0x4212  0x028            0     0     -
  8       1189     0  0x0007  0x4004      -            0     0     -
  9       1188     0  0x0045  0x4212  0x028            0     0     -
 10       1187     0  0x0007  0x4004      -            0     0     -
 11       1186     0  0x002b  0x4212  0x028            0     0     -
 12       1185     0  0x0007  0x4004      -            0     0     -
 13       1184     0  0x0007  0x4004      -            0     0     -
 14       1183     0  0x0007  0x4004      -            0     0     -
 15       1182     0  0x0007  0x4004      -            0     0     -
... (48 entries not read)";

#endregion


        [TestMethod]
        public void ParseStatusCorrectly()
        {
            var txt = @"smartctl 7.2 2020-12-30 r5155 [x86_64-linux-5.15.35-1-pve] (local build)
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
SMART Attributes Data Structure revision number: 16
";
            var info = SmartInfoParser.ParseStdOut("/dev/disk/by-id/ata-TOSHIBA_THNSN8960PCSE_26MS1049TB1V", txt);

            Assert.AreEqual("PASSED", info.DataSection.Status);
            Assert.AreEqual("/dev/disk/by-id/ata-TOSHIBA_THNSN8960PCSE_26MS1049TB1V", info.DeviceId);
            Assert.AreEqual(txt, info.RawSmartInfo);

        }

        [TestMethod]
        [DataRow("246 Total_LBAs_Written      0x0032   100   100   000    Old_age   Always       -       21640090130", 246, "Total_LBAs_Written", "21640090130")]
        [DataRow("194 Temperature_Celsius     0x0022   071   041   000    Old_age   Always       -       29 (Min/Max 22/59)", 194, "Temperature_Celsius", "29 (Min/Max 22/59)")]
        public void ParseSingleAttribute(string rawLine, int expectedId, string expectedName, string expectedRawValue)
        {
            var attribute = SmartInfoParser.ParseAttributeLine(rawLine);
            Console.WriteLine(attribute.Dump(new JsonFormatter()));
            Assert.AreEqual(expectedId, attribute.Id);
            Assert.AreEqual(expectedName, attribute.Name);
            Assert.AreEqual(expectedRawValue, attribute.RawValue);
        }

        [TestMethod]
        public void ParseSmartInfoSection()
        {
            const string stdOut= @"smartctl 7.2 2020-12-30 r5155 [x86_64-linux-5.15.35-1-pve] (local build)
Copyright (C) 2002-20, Bruce Allen, Christian Franke, www.smartmontools.org

=== START OF INFORMATION SECTION ===
Model Family:     Micron 5100 Pro / 52x0 / 5300 SSDs
Device Model:     Micron_5200_MTFDDAK960TDD
Serial Number:    2020286AA686
LU WWN Device Id: 5 00a075 1286aa686
Firmware Version: D1RL004
User Capacity:    960,197,124,096 bytes [960 GB]
Sector Sizes:     512 bytes logical, 4096 bytes physical
Rotation Rate:    Solid State Device
Form Factor:      2.5 inches
TRIM Command:     Available, deterministic, zeroed
Device is:        In smartctl database [for details use: -P show]
ATA Version is:   ACS-3 T13/2161-D revision 5
SATA Version is:  SATA 3.2, 6.0 Gb/s (current: 6.0 Gb/s)
Local Time is:    Sun Oct 16 11:17:07 2022 CEST
SMART support is: Available - device has SMART capability.
SMART support is: Enabled

=== START OF READ SMART DATA SECTION ===
SMART overall-health self-assessment test result: PASSED

General SMART Values:
Offline data collection status:  (0x03) Offline data collection activity
                                        is in progress.";
            var section = SmartInfoParser.ParseSmartInfoSection(stdOut);
            Console.WriteLine(section.Dump(new JsonFormatter()));
            var serial = section.Fields.FirstOrDefault(f => f.Name.Equals("Serial Number"));
            Assert.IsNotNull(serial);
            Assert.AreEqual("2020286AA686", serial.Value);
        }

        [TestMethod]
        public void BytesWrittenTest512ByteLogicalAndPhysical()
        {
            // LBA's writtn: 1262090
            var info = SmartInfoParser.ParseStdOut("/dev/sda",SmartInfo512);
            Console.WriteLine(info.BytesWritten);
            Assert.AreEqual(1262090 * 512, info.BytesWritten.Bytes);
            Console.WriteLine(info.BytesRead);
            Assert.AreEqual(1401957 * 512, info.BytesRead.Bytes);

        }
        [TestMethod]
        public void BytesWrittenTest512ByteLogicalAnd4KPhysical()
        {
            // LBA's writtn: 21640090130
            var info = SmartInfoParser.ParseStdOut("/dev/sda", SmartInfo512_4096);
            Console.WriteLine(info.BytesWritten);
            Assert.AreEqual(21640090130*512, info.BytesWritten.Bytes);
        }

        [TestMethod]
        public void SamsungNVMEUsageTest()
        {
            // LBA's writtn: 21640090130
            var info = SmartInfoParser.ParseStdOut("/dev/nvme0", SamsungNVME);
            Console.WriteLine(info.BytesWritten);
            Assert.AreEqual(29908334L * 512, info.BytesWritten.Bytes);
        }
    }
}
