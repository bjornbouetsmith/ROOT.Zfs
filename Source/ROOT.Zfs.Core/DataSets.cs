﻿using System;
using System.Collections.Generic;
using System.Linq;
using ROOT.Shared.Utils.OS;
using ROOT.Zfs.Core.Commands;
using ROOT.Zfs.Core.Helpers;
using ROOT.Zfs.Public;
using ROOT.Zfs.Public.Data;
using ROOT.Zfs.Public.Data.DataSets;

namespace ROOT.Zfs.Core
{
    internal class DataSets : ZfsBase, IDataSets
    {
        public DataSets(IProcessCall remoteConnection) : base(remoteConnection)
        {
        }

        public DataSet GetDataSet(string fullName)
        {
            var pc = BuildCommand(DataSetCommands.GetDataSet(fullName));

            var response = pc.LoadResponse();

            var line = response.StdOut.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
            if (line == null)
            {
                return null;
            }

            return DataSetHelper.FromString(line);
        }

        public IEnumerable<DataSet> GetDataSets()
        {
            var pc = BuildCommand(DataSetCommands.ZfsList(ListTypes.FileSystem, null));

            var response = pc.LoadResponse();
            if (!response.Success)
            {
                throw response.ToException();
            }

            foreach (var line in response.StdOut.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                yield return DataSetHelper.FromString(line);
            }
        }

        public DataSet CreateDataSet(string dataSetName, PropertyValue[] properties)
        {
            var pc = BuildCommand(DataSetCommands.CreateDataSet(dataSetName, properties));

            var response = pc.LoadResponse();

            if (!response.Success)
            {
                throw response.ToException();
            }

            return GetDataSet(dataSetName);
        }

        public DataSetDestroyResponse DestroyDataSet(string fullName, DataSetDestroyFlags destroyFlags)
        {
            var pc = BuildCommand(DataSetCommands.DestroyDataSet(fullName, destroyFlags));

            var response = pc.LoadResponse();

            if (!response.Success)
            {
                throw response.ToException();
            }

            if (!destroyFlags.HasFlag(DataSetDestroyFlags.DryRun))
            {
                return new DataSetDestroyResponse { Flags = destroyFlags };
            }

            return new DataSetDestroyResponse { Flags = destroyFlags, DryRun = response.StdOut };
        }
    }
}
