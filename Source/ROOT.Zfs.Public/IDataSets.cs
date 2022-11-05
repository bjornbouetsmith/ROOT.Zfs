using System;
using System.Collections.Generic;
using ROOT.Zfs.Public.Arguments.Dataset;
using ROOT.Zfs.Public.Data;
using ROOT.Zfs.Public.Data.Datasets;

namespace ROOT.Zfs.Public
{
    /// <summary>
    /// Contains zfs dataset related commands, i.e.
    /// Create/Destroy filesystems &amp; volumes
    /// </summary>
    public interface IDatasets
    {
        /// <summary>
        /// List datasets with the given name, possibly also returning child datasets
        /// see https://openzfs.github.io/openzfs-docs/man/8/zfs-list.8.html
        /// </summary>
        /// <param name="datasetType">The type of dataset to retrieve, this can be a combination of dataset types, or just a single type.</param>
        /// <param name="fullName">The full name of the dataset, i.e. tank/xxx. This is optional and null can be passed in to just retrieve all datasets of the given type</param>
        /// <param name="includeChildren">Whether or not to return child datasets to the dataset requested</param>
        IEnumerable<Dataset> List(DatasetTypes datasetType, string fullName, bool includeChildren);

        /// <summary>
        /// Creates a dataset based on the input arguments.
        /// </summary>
        /// <param name="arguments">The arguments to use for creating the dataset</param>
        /// <exception cref="ArgumentException">If any of the passed values in the arguments are not valid</exception>
        /// <returns>The dataset just created</returns>
        Dataset Create(DatasetCreationArgs arguments);
        /// <summary>
        /// Destroys the given dataset.
        /// </summary>
        /// <param name="fullName">Full name of the dataset</param>
        /// <param name="destroyFlags">Controls how the dataset is destroyed <see cref="DatasetDestroyFlags"/> </param>
        /// <returns>A dataset response with the flags used and potential dry run response</returns>
        DatasetDestroyResponse Destroy(string fullName, DatasetDestroyFlags destroyFlags);
        
        /// <summary>
        /// Promotes the dataset or volume from a clone to a real dataset or volume.
        /// see https://openzfs.github.io/openzfs-docs/man/8/zfs-promote.8.html
        /// </summary>
        void Promote(string dataset);

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