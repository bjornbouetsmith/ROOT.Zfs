using System.Collections.Generic;
using ROOT.Zfs.Public.Data;
using ROOT.Zfs.Public.Data.DataSets;

namespace ROOT.Zfs.Public
{
    public interface IDataSets
    {
        DataSet GetDataSet(string fullName);
        IEnumerable<DataSet> GetDataSets();
        DataSet CreateDataSet(string dataSetName, PropertyValue[] properties);
        /// <summary>
        /// Destroys the given dataset.
        /// </summary>
        /// <param name="fullName">Full name of the dataset</param>
        /// <param name="destroyFlags">Controls how the dataset is destroyed <see cref="DataSetDestroyFlags"/> </param>
        /// <returns>A dataset response with the flags used and potential dry run response</returns>
        DataSetDestroyResponse DestroyDataSet(string fullName, DataSetDestroyFlags destroyFlags);
    }
}