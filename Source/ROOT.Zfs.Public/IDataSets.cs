using System;
using System.Collections.Generic;
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
        /// Gets datasets with the given name, possibly also returning child datasets
        /// </summary>
        /// <param name="fullName">The full name of the dataset, i.e. tank/xxx</param>
        /// <param name="datasetType">The type of dataset to retrieve, this can be a combination of dataset types, or just a single type.</param>
        /// <param name="includeChildren">Whether or not to return child datasets to the dataset requested</param>
        IEnumerable<Dataset> GetDatasets(string fullName, DatasetTypes datasetType, bool includeChildren);

        /// <summary>
        /// Creates a dataset based on the input arguments.
        /// </summary>
        /// <param name="arguments">The arguments to use for creating the dataset</param>
        /// <exception cref="ArgumentException">If any of the passed values in the arguments are not valid</exception>
        /// <returns>The dataset just created</returns>
        Dataset CreateDataset(DatasetCreationArgs arguments);
        /// <summary>
        /// Destroys the given dataset.
        /// </summary>
        /// <param name="fullName">Full name of the dataset</param>
        /// <param name="destroyFlags">Controls how the dataset is destroyed <see cref="DatasetDestroyFlags"/> </param>
        /// <returns>A dataset response with the flags used and potential dry run response</returns>
        DatasetDestroyResponse DestroyDataset(string fullName, DatasetDestroyFlags destroyFlags);
        
        /// <summary>
        /// Promotes the dataset or volume from a clone to a real dataset or volume.
        /// see https://openzfs.github.io/openzfs-docs/man/8/zfs-promote.8.html
        /// </summary>
        void Promote(string dataset);
    }
}