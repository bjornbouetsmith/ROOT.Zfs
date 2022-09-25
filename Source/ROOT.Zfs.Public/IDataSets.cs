using System.Collections.Generic;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Public
{
    public interface IDataSets
    {
        DataSet GetDataSet(string fullName);
        IEnumerable<DataSet> GetDataSets();
        DataSet CreateDataSet(string dataSetName, PropertyValue[] properties);
        void DestroyDataSet(string fullName);
    }
}