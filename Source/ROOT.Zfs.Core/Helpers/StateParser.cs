using ROOT.Zfs.Public.Data.Pools;

namespace ROOT.Zfs.Core.Helpers
{
    internal static class StateParser
    {
        public static State Parse(string stateText)
        {
            switch (stateText ?? "UNKNOWN")
            {
                case "ONLINE":
                    return State.Online;
                case "DEGRADED":
                    return State.Degraded;
                case "OFFLINE":
                    return State.Offline;
                case "FAULTED":
                    return State.Faulted;
                case "REMOVED":
                    return State.Removed;
                case "UNAVAIL":
                    return State.Unavailable;
                case "AVAIL":
                    return State.Available;
                default:
                    return State.Unknown;
            }
        }
    }
}
