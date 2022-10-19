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
        IEnumerable<Dataset> GetDatasets(string fullName, DatasetType datasetType, bool includeChildren);
        
        /// <summary>
        /// Gets the datasets of the given type
        /// </summary>
        /// <param name="datasetType"></param>
        /// <returns></returns>
        IEnumerable<Dataset> GetDatasets(DatasetType datasetType);
        Dataset CreateDataset(string dataSetName, PropertyValue[] properties);
        /// <summary>
        /// Destroys the given dataset.
        /// </summary>
        /// <param name="fullName">Full name of the dataset</param>
        /// <param name="destroyFlags">Controls how the dataset is destroyed <see cref="DatasetDestroyFlags"/> </param>
        /// <returns>A dataset response with the flags used and potential dry run response</returns>
        DatasetDestroyResponse DestroyDataset(string fullName, DatasetDestroyFlags destroyFlags);
    }
}