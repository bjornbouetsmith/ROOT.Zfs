using System;
using ROOT.Shared.Utils.Serialization;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Core.Helpers
{
    internal static class SnapshotHelper
    {
        /// <summary>
        /// Expects one line of output from the zfs list -t snapshot command with output of only creation,name,used
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        internal static Snapshot FromString(string line)
        {
            var parts = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 3)
            {
                throw new ArgumentException($"{line} could not be parsed, expected 3 parts, got: {parts.Length} - requires an output of creation,name,used to be used for snapshot list", nameof(line));
            }

            var snapshot = new Snapshot();

            snapshot.Name = parts[1];
            if (long.TryParse(parts[0], out var secs))
            {
                snapshot.CreationDate = DateUtils.ToDateTime(secs);
            }

            if (long.TryParse(parts[2], out var size))
            {
                snapshot.Size = size;
            }

            return snapshot;
        }
    }
}
