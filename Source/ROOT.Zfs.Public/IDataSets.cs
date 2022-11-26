using System;
using System.Collections.Generic;
using ROOT.Zfs.Public.Arguments.Dataset;
using ROOT.Zfs.Public.Data.Datasets;

namespace ROOT.Zfs.Public
{
    /// <summary>
    /// Contains zfs dataset related commands, i.e.
    /// Create/Destroy filesystems &amp; volumes
    /// </summary>
    public interface IDatasets : IBasicZfs
    {
        /// <summary>
        /// List datasets with the given root, possibly also returning child datasets
        /// see https://openzfs.github.io/openzfs-docs/man/8/zfs-list.8.html
        /// </summary>
        IList<Dataset> List(DatasetListArgs args);

        /// <summary>
        /// Creates a dataset based on the input arguments.
        /// </summary>
        /// <param name="arguments">The arguments to use for creating the dataset</param>
        /// <exception cref="ArgumentException">If any of the passed values in the arguments are not valid</exception>
        /// <returns>The dataset just created</returns>
        Dataset Create(DatasetCreationArgs arguments);

        /// <summary>
        /// Destroys the given dataset.
        /// see https://openzfs.github.io/openzfs-docs/man/8/zfs-destroy.8.html
        /// </summary>
        DatasetDestroyResponse Destroy(DatasetDestroyArgs args);
        
        /// <summary>
        /// Promotes the dataset or volume from a clone to a real dataset or volume.
        /// see https://openzfs.github.io/openzfs-docs/man/8/zfs-promote.8.html
        /// </summary>
        void Promote(PromoteArgs args);

        /// <summary>
        /// Mount ZFS filesystem on a path described by its mountpoint property, if the path exists and is empty.
        /// If mountpoint is set to legacy, the filesystem should be instead mounted using mount(8).
        /// see https://openzfs.github.io/openzfs-docs/man/8/zfs-mount.8.html
        /// </summary>
        /// <param name="mountArgs">The arguments to control what and how to mount</param>
        void Mount(MountArgs mountArgs);

        /// <summary>
        /// Unmounts currently mounted ZFS file systems.
        /// see https://openzfs.github.io/openzfs-docs/man/8/zfs-unmount.8.html
        /// </summary>
        /// <param name="unmountArgs">The arguments to control what and how to unmount</param>
        void Unmount(UnmountArgs unmountArgs);
    }
}