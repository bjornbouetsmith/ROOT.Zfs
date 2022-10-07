using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Zfs.Core.Helpers;

namespace ROOT.Zfs.Tests.Helpers
{
    [TestClass]
    public class SmartInfoParserTest
    {
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
";
            var info = SmartInfoParser.ParseStdOut("/dev/disk/by-id/ata-TOSHIBA_THNSN8960PCSE_26MS1049TB1V", txt);

            Assert.AreEqual("PASSED", info.Status);
            Assert.AreEqual("/dev/disk/by-id/ata-TOSHIBA_THNSN8960PCSE_26MS1049TB1V", info.DeviceId);
            Assert.AreEqual(txt, info.RawSmartInfo);

        }

    }
}
