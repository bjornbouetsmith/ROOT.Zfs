using System;
using ROOT.Shared.Utils.OS;
using ROOT.Zfs.Core;
using ROOT.Zfs.Core.Commands;
using ZfsType = ROOT.Zfs.Core.Zfs;

namespace ROOT.Zfs.Tests;

/// <summary>
/// Wires up the command builders for tests that construct the internal managers directly.
/// </summary>
internal static class ZfsTestFactory
{
    internal static Datasets CreateDatasets(IProcessCall remoteConnection)
    {
        var binaries = new ZfsBinaries(remoteConnection);
        return new Datasets(remoteConnection, new DatasetCommands(binaries));
    }

    internal static Properties CreateProperties(IProcessCall remoteConnection)
    {
        var binaries = new ZfsBinaries(remoteConnection);
        return new Properties(remoteConnection, new PropertyCommands(binaries));
    }

    internal static Snapshots CreateSnapshots(IProcessCall remoteConnection)
    {
        var binaries = new ZfsBinaries(remoteConnection);
        var datasetCommands = new DatasetCommands(binaries);
        return new Snapshots(remoteConnection, new SnapshotCommands(binaries, datasetCommands));
    }

    internal static ZPool CreateZpool(IProcessCall remoteConnection)
    {
        var binaries = new ZfsBinaries(remoteConnection);
        return new ZPool(remoteConnection, new ZpoolCommands(binaries), new ZdbCommands(binaries));
    }

    internal static ZfsType CreateZfs(IProcessCall remoteConnection, Action<IZfsBinaries> configureBinaries = null)
    {
        var binaries = new ZfsBinaries(remoteConnection);
        configureBinaries?.Invoke(binaries);

        var datasetCommands = new DatasetCommands(binaries);

        return new ZfsType(
            remoteConnection,
            binaries,
            new ZfsCommands(binaries),
            new SystemCommands(binaries),
            new Datasets(remoteConnection, datasetCommands),
            new Properties(remoteConnection,new PropertyCommands(binaries)),
            new Snapshots(remoteConnection, new SnapshotCommands(binaries, datasetCommands)),
            new ZPool(remoteConnection,new ZpoolCommands(binaries),new ZdbCommands(binaries))
        );
    }
}