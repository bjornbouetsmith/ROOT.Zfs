using System;
using System.Collections.Generic;
using System.Linq;
using ROOT.Shared.Utils.OS;
using ROOT.Zfs.Core.Commands;
using ROOT.Zfs.Core.Helpers;
using ROOT.Zfs.Public;
using ROOT.Zfs.Public.Data;
using ROOT.Zfs.Public.Data.Datasets;

namespace ROOT.Zfs.Core
{
    internal class Datasets : ZfsBase, IDatasets
    {
        public Datasets(IProcessCall remoteConnection) : base(remoteConnection)
        {
        }

        public Dataset GetDataset(string fullName)
        {
            var pc = BuildCommand(DatasetCommands.GetDataset(fullName));

            var response = pc.LoadResponse(false);

            if (!response.Success 
                && response.StdError != null 
                && response.StdError.StartsWith("cannot open") 
                && response.StdError.EndsWith("dataset does not exist"))
            {
                return null;
            }

            var line = response.StdOut.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
            if (line == null)
            {
                return null;
            }

            return DatasetHelper.ParseStdOut(line);
        }

        public IEnumerable<Dataset> GetDatasets()
        {
            var pc = BuildCommand(DatasetCommands.ZfsList(ListTypes.FileSystem, null));

            var response = pc.LoadResponse(true);
            
            foreach (var line in response.StdOut.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                yield return DatasetHelper.ParseStdOut(line);
            }
        }

        public Dataset CreateDataset(string dataSetName, PropertyValue[] properties)
        {
            var pc = BuildCommand(DatasetCommands.CreateDataset(dataSetName, properties));

            pc.LoadResponse(true);
            
            return GetDataset(dataSetName);
        }

        public DatasetDestroyResponse DestroyDataset(string fullName, DatasetDestroyFlags destroyFlags)
        {
            var pc = BuildCommand(DatasetCommands.DestroyDataset(fullName, destroyFlags));

            var response = pc.LoadResponse(true);
            
            if (!destroyFlags.HasFlag(DatasetDestroyFlags.DryRun))
            {
                return new DatasetDestroyResponse { Flags = destroyFlags };
            }

            return new DatasetDestroyResponse { Flags = destroyFlags, DryRun = response.StdOut };
        }
    }
}
