using System;
using System.Collections.Generic;
using System.Linq;
using ROOT.Shared.Utils.OS;
using ROOT.Zfs.Core.Commands;
using ROOT.Zfs.Core.Helpers;
using ROOT.Zfs.Public;
using ROOT.Zfs.Public.Arguments;
using ROOT.Zfs.Public.Arguments.Dataset;
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
        public IEnumerable<Dataset> List(string fullName, DatasetTypes datasetType, bool includeChildren)
        {
            var pc = BuildCommand(DatasetCommands.ZfsList(datasetType, fullName, false));

            var response = pc.LoadResponse(true);

            foreach (var line in response.StdOut.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                yield return DatasetHelper.ParseStdOut(line);
            }
        }

        /// <inheritdoc />
        public IEnumerable<Dataset> GetDatasets(DatasetTypes datasetType)
        {
            return List(null, datasetType, false);
        }

        /// <inheritdoc />
        public Dataset Create(DatasetCreationArgs arguments)
        {
            if (!arguments.Validate(out var errors))
            {
                throw new ArgumentException(string.Join(Environment.NewLine, errors), nameof(arguments));
            }

            var pc = BuildCommand(DatasetCommands.CreateDataset(arguments));

            pc.LoadResponse(true);

            return List(arguments.DataSetName, arguments.Type, false).FirstOrDefault();
        }

        /// <inheritdoc />
        public DatasetDestroyResponse Destroy(string fullName, DatasetDestroyFlags destroyFlags)
        {
            var pc = BuildCommand(DatasetCommands.DestroyDataset(fullName, destroyFlags));

            var response = pc.LoadResponse(true);

            if (!destroyFlags.HasFlag(DatasetDestroyFlags.DryRun))
            {
                return new DatasetDestroyResponse { Flags = destroyFlags };
            }

            return new DatasetDestroyResponse { Flags = destroyFlags, DryRun = response.StdOut };
        }

        /// <inheritdoc />
        public void Promote(string dataset)
        {
            var pc = BuildCommand(DatasetCommands.Promote(dataset));
            pc.LoadResponse(true);
        }

        /// <inheritdoc />
        public void Mount(MountArgs mountArgs)
        {
            var pc = BuildCommand(DatasetCommands.Mount(mountArgs));
            pc.LoadResponse(true);
        }

        /// <inheritdoc />
        public void Unmount(UnmountArgs unmountArgs)
        {
            var pc = BuildCommand(DatasetCommands.Unmount(unmountArgs));
            pc.LoadResponse(true);
        }
    }
}
