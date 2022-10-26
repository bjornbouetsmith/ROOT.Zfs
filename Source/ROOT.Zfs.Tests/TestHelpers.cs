using System;

namespace ROOT.Zfs.Tests
{
    internal static class TestHelpers
    {
        // Should be "optimized" - so it only requires sudo when run on github actions, i.e.
        // For certain usernames and machin names if we can detect it
        internal static bool RequiresSudo => Environment.MachineName != "BBS-DESKTOP";

        internal static bool RequiresRemoteConnection => Environment.MachineName == "BBS-DESKTOP";
    }
}
