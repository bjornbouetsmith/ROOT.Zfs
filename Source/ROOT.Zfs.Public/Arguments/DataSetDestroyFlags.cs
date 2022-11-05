using System;

namespace ROOT.Zfs.Public.Arguments
{
    /// <summary>
    /// See: https://openzfs.github.io/openzfs-docs/man/8/zfs-destroy.8.html
    ///  
    /// </summary>
    [Flags]
    public enum DatasetDestroyFlags
    {
        /// <summary>
        /// No flags set
        /// </summary>
        None = 0,
        /// <summary>
        /// Recursively destroy dataset and all children (-r)
        /// </summary>
        Recursive = 1,
        /// <summary>
        /// Recursively destroy dataset and all dependents, including cloned file systems outside the target hierarchy (-R)
        /// </summary>
        RecursiveClones = 2,
        /// <summary>
        /// Forcibly unmount file systems. This option has no effect on non-file systems or unmounted file systems (-f)
        /// </summary>
        ForceUmount = 4,
        /// <summary>
        /// Do a dry-run (“No-op”) deletion. No data will be deleted. This is useful in conjunction with the -v or -p flags to determine what data would be deleted. (-n)
        /// </summary>
        DryRun = 8,
    }
}