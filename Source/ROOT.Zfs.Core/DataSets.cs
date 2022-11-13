using System.Collections.Generic;
using System.Linq;
using ROOT.Shared.Utils.OS;
using ROOT.Zfs.Core.Commands;
using ROOT.Zfs.Core.Helpers;
using ROOT.Zfs.Public;
using ROOT.Zfs.Public.Arguments.Dataset;
using ROOT.Zfs.Public.Data.Datasets;

namespace ROOT.Zfs.Core
{
    /// <inheritdoc cref="IZfs" />
    internal class Datasets : ZfsBase, IDatasets
    {
        public Datasets(IProcessCall remoteConnection) : base(remoteConnection)
        {
        }

        public IList<Dataset> List(DatasetListArgs args)
        {
            var pc = BuildCommand(Commands.Commands.ZfsList(args));
            var response = pc.LoadResponse(true);
            return DatasetHelper.ParseStdOut(response.StdOut);
        }

        /// <inheritdoc />
        public Dataset Create(DatasetCreationArgs arguments)
        {
            var pc = BuildCommand(DatasetCommands.Create(arguments));

            pc.LoadResponse(true);
            var listArgs = new DatasetListArgs { Root = arguments.DatasetName, IncludeChildren = false, DatasetTypes = arguments.Type };
            return List(listArgs).FirstOrDefault();
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
        public void Promote(PromoteArgs args)
        {
            var pc = BuildCommand(DatasetCommands.Promote(args));
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
