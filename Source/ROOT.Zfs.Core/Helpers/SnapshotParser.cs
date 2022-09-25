using System;
using System.Collections.Generic;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Core.Helpers
{
    internal static class SnapshotParser
    {
        internal static IEnumerable<Snapshot> Parse(string snapshotResponse)
        {
            foreach (var line in snapshotResponse.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                yield return SnapshotHelper.FromString(line);
            }
        }
    }
}