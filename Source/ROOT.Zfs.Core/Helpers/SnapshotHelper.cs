﻿using System;
using ROOT.Shared.Utils.Serialization;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Core.Helpers
{
    internal static class SnapshotHelper
    {
        /// <summary>
        /// Expects one line of output from the zfs list -t snapshot command
        /// With the following:
        /// 'zfs list -Hpr -o type,creation,name,used,refer,avail,mountpoint,origin'
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        internal static Snapshot FromString(string line)
        {
            var parts = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 8)
            {
                throw new FormatException($"{line} could not be parsed, expected 8 parts, got: {parts.Length} - requires an output of  type,creation,name,used,refer,avail,mountpoint,origin to be used for snapshot list");
            }

            var snapshot = new Snapshot();
            var snapshotNameParts = parts[2].Split('@', StringSplitOptions.RemoveEmptyEntries);
            snapshot.Dataset = snapshotNameParts[0].Trim();
            snapshot.SnapshotName = snapshotNameParts[1].Trim();
            if (long.TryParse(parts[1], out var secs))
            {
                snapshot.CreationDate = DateUtils.ToDateTime(secs);
            }
            snapshot.Size = new Size(parts[3]);
            
            return snapshot;
        }
    }
}
