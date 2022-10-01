using System;
using ROOT.Zfs.Public.Data.Pools;

namespace ROOT.Zfs.Public.Data
{
    public static class EnumExt
    {
        /// <summary>
        /// Returns string representation of VDevCreationType as required by zpool create
        /// </summary>
        /// <exception cref="NotImplementedException">If you pass some value not yet supported</exception>
        public static string AsString(this VDevCreationType type)
        {
            switch (type)
            {
                case VDevCreationType.Cache:
                    return "cache";
                case VDevCreationType.Dedup:
                    return "dedup";
                case VDevCreationType.DRaidz:
                    return "draidz";
                case VDevCreationType.Log:
                    return "log";
                case VDevCreationType.Mirror:
                    return "mirror";
                case VDevCreationType.Raidz:
                    return "raidz";
                case VDevCreationType.Spare:
                    return "spare";
                case VDevCreationType.Special:
                    return "special";
                default:
                    throw new NotImplementedException($"Please implement AsString for {type} and ensure nothing is broken");
            }
        }
    }
}
