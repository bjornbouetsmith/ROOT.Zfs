﻿using System;
using System.Collections.Generic;
using ROOT.Zfs.Public.Data.Pools;

namespace ROOT.Zfs.Public.Data
{
    /// <summary>
    /// Enum extension methods
    /// </summary>
    public static class EnumExt
    {
        private static readonly Dictionary<VDevCreationType, string> _VdevMappings = new Dictionary<VDevCreationType, string> 
            {
            {VDevCreationType.Cache,"cache"},
            {VDevCreationType.Dedup,"dedup"},
            {VDevCreationType.DRaid1,"draid1"},
            {VDevCreationType.DRaid2,"draid2"},
            {VDevCreationType.DRaid3,"draid3"},
            {VDevCreationType.Log,"log"},
            {VDevCreationType.Mirror,"mirror"},
            {VDevCreationType.Raidz1,"raidz1"},
            {VDevCreationType.Raidz2,"raidz2"},
            {VDevCreationType.Raidz3,"raidz3"},
            {VDevCreationType.Spare,"spare"},
            {VDevCreationType.Special,"special"},
            };

        /// <summary>
        /// Returns string representation of VDevCreationType as required by zpool create
        /// </summary>
        /// <exception cref="NotSupportedException">If you pass some value not yet supported</exception>
        public static string AsString(this VDevCreationType type)
        {
            if (!_VdevMappings.TryGetValue(type, out var stringVer))
            {
                throw new NotSupportedException($"Please implement AsString for {type} and ensure nothing is broken");
            }

            return stringVer;
        }

        /// <summary>
        /// Returns a string representation of DataSetTypes
        /// </summary>
        public static string AsString(this DatasetTypes listtypes)
        {
            List<string> types = new();
            if (listtypes == DatasetTypes.None)
            {
                // This is what zfs does, if you do not specify type, you get filesystems and volumnes
                listtypes = DatasetTypes.Filesystem | DatasetTypes.Volume;
            }

            if ((listtypes & DatasetTypes.Bookmark) != 0)
            {
                types.Add("bookmark");
            }
            if ((listtypes & DatasetTypes.Filesystem) != 0)
            {
                types.Add("filesystem");
            }
            if ((listtypes & DatasetTypes.Snapshot) != 0)
            {
                types.Add("snapshot");
            }
            if ((listtypes & DatasetTypes.Volume) != 0)
            {
                types.Add("volume");
            }
            
            return string.Join(",", types);
        }
    }
}
