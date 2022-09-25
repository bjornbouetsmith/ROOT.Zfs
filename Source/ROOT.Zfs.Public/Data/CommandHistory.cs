using System;

namespace ROOT.Zfs.Public.Data
{
    public class CommandHistory
    {
        public DateTime Time { get; set; }
        public string Command { get; set; }
        public string Caller { get; set; }
    }
}
