using System;
using ROOT.Shared.Utils.Serialization;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Core.Helpers
{
    internal static class SnapshotHelper
    {
        /// <summary>
        /// Expects one line of output from the zfs list -t snapshot command
        /// With the following:
        /// 'zfs list -Hpr -o type,creation,name,used,refer,avail,mountpoint'
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        internal static Snapshot FromString(string line)
        {
            var parts = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 7)
            {
                throw new FormatException($"{line} could not be parsed, expected 7 parts, got: {parts.Length} - requires an output of  type,creation,name,used,refer,avail,mountpoint to be used for snapshot list");
            }

            var snapshot = new Snapshot();

            snapshot.Name = parts[2];
            if (long.TryParse(parts[1], out var secs))
            {
                snapshot.CreationDate = DateUtils.ToDateTime(secs);
            }
            snapshot.Size = new Size(parts[3]);
            
            return snapshot;
        }
    }
}
