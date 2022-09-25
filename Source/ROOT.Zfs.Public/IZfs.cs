using System;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Public
{
    public interface IZfs
    {
        ISnapshots Snapshots { get; }
        IDataSets DataSets { get; }
        IProperties Properties { get; }
        IZPool Pool { get; }

        /// <summary>
        /// Gets or set the maximum wait time to wait for a command to process fully
        /// Current default timeout is set to 30 seconds.
        /// When the timeout is reached, the command is aborted - if possible and an exception is raised.
        /// There are no guarantees that the command will be aborted - it is aborted on best effort.
        /// </summary>
        TimeSpan CommandTimeout { get; set; }

        VersionInfo GetVersionInfo();
    }
}
