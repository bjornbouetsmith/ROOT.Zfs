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

        public IEnumerable<Dataset> GetDatasets(string fullName, DatasetType datasetType, bool includeChildren)
        {
            var pc = BuildCommand(DatasetCommands.ZfsList(datasetType, fullName, false));

            var response = pc.LoadResponse(true);

            foreach (var line in response.StdOut.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                yield return DatasetHelper.ParseStdOut(line);
            }
        }

        public IEnumerable<Dataset> GetDatasets(DatasetType datasetType)
        {
            return GetDatasets(null, datasetType, false);
        }

        public Dataset CreateDataset(string dataSetName, PropertyValue[] properties)
        {
            var pc = BuildCommand(DatasetCommands.CreateDataset(dataSetName, properties));

            pc.LoadResponse(true);

            return GetDatasets(dataSetName, DatasetType.NotSet, false).FirstOrDefault();
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
