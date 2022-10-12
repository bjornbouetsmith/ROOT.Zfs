using ROOT.Zfs.Public.Data.Pools;

namespace ROOT.Zfs.Core.Helpers
{
    internal static class ZPoolIOStatParser
    {
        public static IOStats ParseStdOut(string responseStdOut)
        {
            return new IOStats();
        }
    }
}
