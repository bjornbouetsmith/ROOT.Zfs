using System;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Core.Helpers
{
    internal static class SmartInfoParser
    {
        internal static SmartInfo ParseStdOut(string deviceId, string stdOut)
        {
            var info = new SmartInfo();
            info.DeviceId = deviceId;
            info.RawSmartInfo = stdOut;
            info.Status = ParseStatus(stdOut);
            return info;
        }

        private static string ParseStatus(string stdOut)
        {
            var pattern = "SMART overall-health self-assessment test result:";
            var index = stdOut.IndexOf(pattern, StringComparison.OrdinalIgnoreCase);
            if (index == -1)
            {
                return "Unknown";
            }
            var indexOfEOL = stdOut.IndexOf('\n', index);
            if (indexOfEOL == -1)
            {
                return "Unknown";
            }
            var startOfStatus = index + pattern.Length;
            return stdOut[startOfStatus..indexOfEOL].Trim();
        }
    }
}
