using System;
using Xunit;
using ROOT.Zfs.Core.Helpers;

namespace ROOT.Zfs.Tests.Helpers;

public class UpgradeablePoolsParserTest
{
    [Fact]
    public void AllPoolsUpgradedAlready()
    {
        const string output = @"This system supports ZFS pool feature flags.

All pools are formatted using feature flags.

Every feature flags pool has all supported and requested features enabled.
";
        var pools = UpgradeablePoolsParser.ParseStdOut(output);
        Assert.Empty(pools);
    }

    [Fact]
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
        var pool = Assert.Single(pools);
        Assert.Equal("backup", pool.PoolName);

        Assert.Equal(2, pool.MissingFeatures.Count);
        Assert.Equal("edonr", pool.MissingFeatures[0]);
        Assert.Equal("draid", pool.MissingFeatures[1]);

    }

    [Fact]
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
        Assert.Equal(2, pools.Count);
        var pool = pools[0];
        Assert.Equal("backup", pool.PoolName);

        Assert.Equal(2, pool.MissingFeatures.Count);
        Assert.Equal("edonr", pool.MissingFeatures[0]);
        Assert.Equal("draid", pool.MissingFeatures[1]);

        pool = pools[1];
        Assert.Equal("tank3", pool.PoolName);

        Assert.Equal(3, pool.MissingFeatures.Count);
        Assert.Equal("encryption", pool.MissingFeatures[0]);
        Assert.Equal("project_quota", pool.MissingFeatures[1]);
        Assert.Equal("device_removal", pool.MissingFeatures[2]);

    }

    [Fact]
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
        Assert.Throws<FormatException>(() => UpgradeablePoolsParser.ParseStdOut(output));
    }
}