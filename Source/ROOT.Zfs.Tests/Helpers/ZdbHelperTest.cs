using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Shared.Utils.Serialization;
using ROOT.Zfs.Core.Helpers;

namespace ROOT.Zfs.Tests.Helpers
{
    [TestClass]
    public class ZdbHelperTest
    {
        private const string OutputTwoPools = @"backup:
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
tank3:
    version: 5000
    name: 'tank3'
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

        [TestMethod]
        public void ShouldParseVersionsCorrectlyForMultiplePools()
        {
            var versions = ZdbHelper.ParsePoolVersions(OutputTwoPools);
            Console.WriteLine(versions.Dump(new JsonFormatter()));
            Assert.AreEqual(2, versions.Count);
            var info = versions[0];
            Assert.AreEqual("backup", info.Name);
            Assert.AreEqual(5000, info.Version);
            info = versions[1];

            Assert.AreEqual("tank3", info.Name);
            Assert.AreEqual(5000, info.Version);
        }

        [TestMethod]
        public void TruncatedOutputShouldthrow()
        {
            Assert.ThrowsException<FormatException>(() => ZdbHelper.ParsePoolVersions("   version:5000"));
        }

        [TestMethod]
        public void UnknownVersionOutputShouldNotThrow()
        {
            var versions= ZdbHelper.ParsePoolVersions(@"backup:
    version: v5000
    name: 'backup'");

            Assert.AreEqual(1, versions.Count);
            var info = versions[0];
            Assert.AreEqual("backup", info.Name);
            Assert.AreEqual(0, info.Version);
        }

    }
}
