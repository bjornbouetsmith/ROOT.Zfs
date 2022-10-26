using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Core.Helpers
{
    /// <summary>
    /// Parses lines like this:
    /// tank/myds@20220925111346  mytag  Wed Oct 26 09:20 2022
    /// into SnapshotHold objects
    /// </summary>
    internal static class SnapshotHoldParser
    {

        internal static IList<SnapshotHold> ParseStdOut(string stdOut)
        {
            var list = new List<SnapshotHold>();
            foreach (var line in stdOut.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var parts = line.Split(new[] { '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                var snapshot = parts[0];
                var tag = parts[1];
                var hold = new SnapshotHold { Snapshot = snapshot, Tag = tag };

                var dateStr = string.Join(' ', parts.Skip(2));
                
                // This will fail if the output from zfs suddenly changes format
                // an issue has been made against openzfs to output date times in a parseable format, i.e. unix timestamps
                // https://github.com/openzfs/zfs/issues/13690
                if (DateTime.TryParseExact(dateStr, "ddd MMM dd HH:mm yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var date))
                {
                    hold.HoldTime = date;
                }

                list.Add(hold);
            }

            return list;
        }
    }
}
