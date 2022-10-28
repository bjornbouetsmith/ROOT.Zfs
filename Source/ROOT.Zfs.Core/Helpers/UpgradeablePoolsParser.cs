using System;
using System.Collections.Generic;
using ROOT.Zfs.Public.Data.Pools;

namespace ROOT.Zfs.Core.Helpers
{
    /// <summary>
    /// Parser of output similar to:
    /// This system supports ZFS pool feature flags.
    /// 
    /// All pools are formatted using feature flags.
    /// 
    /// 
    /// Some supported features are not enabled on the following pools. Once a
    /// feature is enabled the pool may become incompatible with software
    /// that does not support the feature. See zpool-features(7) for details.
    /// 
    /// Note that the pool 'compatibility' feature can be used to inhibit
    /// feature upgrades.
    /// 
    /// POOL  FEATURE
    /// ---------------
    /// backup
    ///       edonr
    ///       draid
    /// tank3
    ///       large_dnode
    ///       edonr
    ///       userobj_accounting
    ///       encryption
    ///       project_quota
    ///       device_removal
    ///       obsolete_counts
    ///       zpool_checkpoint
    ///       spacemap_v2
    ///       allocation_classes
    ///       resilver_defer
    ///       bookmark_v2
    ///       redaction_bookmarks
    ///       redacted_datasets
    ///       bookmark_written
    ///       log_spacemap
    ///       livelist
    ///       device_rebuild
    ///       zstd_compress
    ///       draid
    ///
    /// Or
    /// 
    /// This system supports ZFS pool feature flags.
    /// 
    /// All pools are formatted using feature flags.
    /// 
    /// Every feature flags pool has all supported and requested features enabled.
    /// </summary>
    internal static class UpgradeablePoolsParser
    {
        public static IList<UpgradeablePool> ParseStdOut(string stdOut)
        {
            var pools = new List<UpgradeablePool>();
            const string poolFeature = "POOL  FEATURE";
            var poolIndex = stdOut.IndexOf(poolFeature, StringComparison.InvariantCulture);
            if (poolIndex == -1)
            {
                // Not found, which means no pools can be upgraded
                return pools;
            }

            var pastPoolFeature = poolIndex + poolFeature.Length;

            const string separator = "---------------";
            var startIndex = stdOut.IndexOf(separator, pastPoolFeature, StringComparison.InvariantCulture);
            startIndex += separator.Length;
            var upgradeable = stdOut[startIndex..];
            UpgradeablePool pool = null;
            foreach (var line in upgradeable.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (line[0] == ' ')
                {
                    if (pool == null)
                    {
                        throw new FormatException($"Unexpected format - no pool name has been seen yet in data:{Environment.NewLine}{stdOut}");
                    }

                    // feature line
                    pool.MissingFeatures.Add(line.Trim());
                }
                else
                {
                    pool = new UpgradeablePool { MissingFeatures = new List<string>(), PoolName = line.Trim() };
                    pools.Add(pool);
                }
            }
            return pools;
        }
    }
}
