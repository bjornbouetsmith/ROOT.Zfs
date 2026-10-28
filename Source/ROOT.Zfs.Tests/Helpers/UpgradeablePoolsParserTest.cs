using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Zfs.Core.Helpers;

namespace ROOT.Zfs.Tests.Helpers;

[TestClass]
public class UpgradeablePoolsParserTest
{
    [TestMethod]
    public void AllPoolsUpgradedAlready()
    {
        const string output = @"This system supports ZFS pool feature flags.

All pools are formatted using feature flags.

Every feature flags pool has all supported and requested features enabled.
";
        var pools = UpgradeablePoolsParser.ParseStdOut(output);
        Assert.IsEmpty(pools);
    }

    [TestMethod]
    public void SinglePoolUpgradeable()
    {
        const string output = @"This system supports ZFS pool feature flags.

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
        var pools = UpgradeablePoolsParser.ParseStdOut(output);
        Assert.HasCount(1, pools);
        var pool = pools[0];
        Assert.AreEqual("backup", pool.PoolName);

        Assert.HasCount(2, pool.MissingFeatures);
        Assert.AreEqual("edonr", pool.MissingFeatures[0]);
        Assert.AreEqual("draid", pool.MissingFeatures[1]);

    }

    [TestMethod]
    public void MultiplePoolsUpgradeable()
    {
        const string output = @"This system supports ZFS pool feature flags.

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
tank3
      encryption
      project_quota
      device_removal
";
        var pools = UpgradeablePoolsParser.ParseStdOut(output);
        Assert.HasCount(2, pools);
        var pool = pools[0];
        Assert.AreEqual("backup", pool.PoolName);

        Assert.HasCount(2, pool.MissingFeatures);
        Assert.AreEqual("edonr", pool.MissingFeatures[0]);
        Assert.AreEqual("draid", pool.MissingFeatures[1]);

        pool = pools[1];
        Assert.AreEqual("tank3", pool.PoolName);

        Assert.HasCount(3, pool.MissingFeatures);
        Assert.AreEqual("encryption", pool.MissingFeatures[0]);
        Assert.AreEqual("project_quota", pool.MissingFeatures[1]);
        Assert.AreEqual("device_removal", pool.MissingFeatures[2]);

    }

    [TestMethod]
    public void GarbledOutputShouldThrow()
    {
        const string output = @"This system supports ZFS pool feature flags.

All pools are formatted using feature flags.


Some supported features are not enabled on the following pools. Once a
feature is enabled the pool may become incompatible with software
that does not support the feature. See zpool-features(7) for details.

Note that the pool 'compatibility' feature can be used to inhibit
feature upgrades.

POOL  FEATURE
---------------
      edonr
      draid
";
        Assert.ThrowsExactly<FormatException>(() => UpgradeablePoolsParser.ParseStdOut(output));
    }
}