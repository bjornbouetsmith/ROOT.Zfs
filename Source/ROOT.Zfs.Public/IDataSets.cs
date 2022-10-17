using System.Collections.Generic;
using ROOT.Zfs.Public.Data;
using ROOT.Zfs.Public.Data.Datasets;

namespace ROOT.Zfs.Public
{
    public interface IDatasets
    {
        Dataset GetDataset(string fullName);
        IEnumerable<Dataset> GetDatasets();
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