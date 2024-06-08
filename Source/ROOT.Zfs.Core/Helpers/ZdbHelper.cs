using System;
using System.Collections.Generic;
using System.Diagnostics;
using ROOT.Zfs.Public.Data.Pools;

namespace ROOT.Zfs.Core.Helpers
{
    /// <summary>
    /// Helps parse data from zdb
    /// </summary>
    internal static class ZdbHelper
    {
        /// <summary>
        /// Expects an output of '        version: 5000'
        /// </summary>
        private static int ParseVersionResponse(string stdOut)
        {
            const string versionTag = "version:";
            var index = stdOut.IndexOf(versionTag, StringComparison.InvariantCulture);

            var versionString = stdOut[(index + versionTag.Length)..].Trim();

            if (int.TryParse(versionString, out var version))
            {
                return version;
            }

            Trace.TraceError($"Could not parse '{stdOut}' into a version");
            return 0;
        }
        /// <summary>
        /// Parses output from command 'zdb' into a list of pool version info objects
        /// </summary>
        /// <exception cref="FormatException">If format is not the supported format</exception>
        public static IList<PoolVersionInfo> ParsePoolVersions(string fullStdOut)
        {
            var result = new List<PoolVersionInfo>();
            PoolVersionInfo info = null;

            foreach (var line in fullStdOut.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (line.StartsWith(' '))
                {
                    if (line.Trim().StartsWith("version:"))
                    {
                        if (info == null)
                        {
                            throw new FormatException($"Unknown format from zdb encountered, cannot parse:{Environment.NewLine}{fullStdOut}");
                        }

                        info.Version = ParseVersionResponse(line);
                    }
                }
                else
                {
                    // pool
                    info = new PoolVersionInfo();
                    var indexOfColon = line.IndexOf(':');
                    info.PoolName = line[..indexOfColon].Trim();
                    result.Add(info);
                }
            }

            return result;
        }
    }
}
