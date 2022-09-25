using System;
using System.Collections.Generic;
using System.Linq;
using ROOT.Shared.Utils.OS;
using ROOT.Zfs.Core.Commands;
using ROOT.Zfs.Public;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Core
{
    internal class DataSets : ZfsBase, IDataSets
    {
        public DataSets(RemoteProcessCall remoteConnection) : base(remoteConnection)
        {
        }

        public DataSet GetDataSet(string fullName)
        {
            var pc = BuildCommand(DataSetCommands.ProcessCalls.GetDataSet(fullName));

            var response = pc.LoadResponse();

            var line = response.StdOut.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Skip(1).FirstOrDefault();
            if (line == null)
            {
                return null;
            }

            return DataSet.FromString(line);
        }

        public IEnumerable<DataSet> GetDataSets()
        {
            var pc = BuildCommand(DataSetCommands.ProcessCalls.GetDataSets());

            var response = pc.LoadResponse();
            if (!response.Success)
            {
                throw response.ToException();
            }

            foreach (var line in response.StdOut.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Skip(1))
            {
                yield return DataSet.FromString(line);
            }
        }

        public DataSet CreateDataSet(string dataSetName, PropertyValue[] properties)
        {
            var pc = BuildCommand(DataSetCommands.ProcessCalls.CreateDataSet(dataSetName, properties));

            var response = pc.LoadResponse();

            if (!response.Success)
            {
                throw response.ToException();
            }

            return GetDataSet(dataSetName);
        }

        public void DestroyDataSet(string fullName)
        {
            var pc = BuildCommand(DataSetCommands.ProcessCalls.DestroyDataSet(fullName));

            var response = pc.LoadResponse();

            if (!response.Success)
            {
                throw response.ToException();
            }
        }
    }
}
