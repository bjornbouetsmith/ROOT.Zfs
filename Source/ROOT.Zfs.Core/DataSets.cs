﻿using System;
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
    /// <inheritdoc cref="IZfs" />
    internal class Datasets : ZfsBase, IDatasets
    {
        public Datasets(IProcessCall remoteConnection) : base(remoteConnection)
        {
        }

        /// <inheritdoc />
        public IEnumerable<Dataset> GetDatasets(string fullName, DatasetType datasetType, bool includeChildren)
        {
            var pc = BuildCommand(DatasetCommands.ZfsList(datasetType, fullName, false));

            var response = pc.LoadResponse(true);

            foreach (var line in response.StdOut.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                yield return DatasetHelper.ParseStdOut(line);
            }
        }

        /// <inheritdoc />
        public IEnumerable<Dataset> GetDatasets(DatasetType datasetType)
        {
            return GetDatasets(null, datasetType, false);
        }

        /// <inheritdoc />
        public Dataset CreateDataset(DatasetCreationArgs arguments)
        {
            if (!arguments.Validate(out var errors))
            {
                throw new ArgumentException(nameof(arguments),string.Join(Environment.NewLine, errors));
            }

            var pc = BuildCommand(DatasetCommands.CreateDataset(arguments));

            pc.LoadResponse(true);

            return GetDatasets(arguments.DataSetName, arguments.Type, false).FirstOrDefault();
        }

        /// <inheritdoc />
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
