using System;
using System.Collections.Generic;

namespace ROOT.Zfs.Tests.VersionResponses
{
    /// <summary>
    /// Entry point to get a version response test implementation for a given version
    /// </summary>
    internal static class ZfsResponses
    {
        private static readonly Dictionary<string, IVersionResponse> _zfsVersions = new Dictionary<string, IVersionResponse>
        {
            {"2.1.5-2",new VersionResponse2_1_5_2()}
        };

        internal static IVersionResponse GetVersionResponse(string version)
        {
            if (!_zfsVersions.TryGetValue(version, out var response))
            {
                throw new InvalidOperationException($"Missing version response mock for version {version}");
            }
            return response;
        }
    }
}