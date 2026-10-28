using System;
using System.Collections.Generic;
using System.Linq;
using ROOT.Shared.Utils.OS;
using ROOT.Zfs.Core.Commands;
using ROOT.Zfs.Core.Helpers;
using ROOT.Zfs.Public;
using ROOT.Zfs.Public.Data;
using ROOT.Zfs.Public.Data.Smart;

namespace ROOT.Zfs.Core;

/// <inheritdoc cref="IZfs" />
public sealed class Zfs : ZfsBase, IZfs
{
    private readonly IZfsBinaries _binaries;
    private readonly IZfsCommands _zfsCommands;
    private readonly ISystemCommands _systemCommands;

    /// <summary>
    /// Creates an instance of the Zfs class.
    /// </summary>
    /// <param name="remoteConnection">Any remote connection that should be used. This parameter is optional - if null is passed then the local machine will be used.</param>
    /// <param name="binaries">The binary locations to use for commands.</param>
    /// <param name="zfsCommands">Command builder for the zfs binary.</param>
    /// <param name="systemCommands">Command builder for system/diagnostic commands.</param>
    /// <param name="datasets">datasets instance.</param>
    /// <param name="properties">properties instance.</param>
    /// <param name="snapshots">Snapshots instance</param>
    /// <param name="zpool">zpool instance.</param>
    public Zfs(
        IProcessCall remoteConnection,
        IZfsBinaries binaries,
        IZfsCommands zfsCommands,
        ISystemCommands systemCommands,
        IDatasets datasets,
        IProperties properties,
        ISnapshots snapshots,
        IZPool zpool)
        : base(remoteConnection)
    {
        _binaries = binaries;
        _zfsCommands = zfsCommands;
        _systemCommands = systemCommands;

        Snapshots = snapshots;
        Datasets = datasets;
        Properties = properties;
        Pool = zpool;
    }

    /// <inheritdoc />
    public ISnapshots Snapshots { get; }

    /// <inheritdoc />
    public IDatasets Datasets { get; }

    /// <inheritdoc />
    public IProperties Properties { get; }

    /// <inheritdoc />
    public IZPool Pool { get; }

    /// <summary>
    /// The binary configuration used by this instance.
    /// </summary>
    internal IZfsBinaries Binaries => _binaries;

    /// <inheritdoc />
    public VersionInfo GetVersionInfo()
    {
        var pc = BuildCommand(_zfsCommands.GetVersion());
        var response = pc.LoadResponse(true);

        return new VersionInfo { Lines = response.StdOut.Split(Constants.LinebreakCharacters, StringSplitOptions.RemoveEmptyEntries) };
    }

    /// <inheritdoc />
    public IEnumerable<DiskInfo> ListDisks()
    {
        var disksCommand = BuildCommand(_systemCommands.ListBlockDevices());
        var disksReponse = disksCommand.LoadResponse(true);

        var blockDevices = DiskHelper.BlockDevicesFromStdOutput(disksReponse.StdOut);

        var pc = BuildCommand(_systemCommands.ListDisks());
        var response = pc.LoadResponse(true);

        foreach (var line in response.StdOut.Split(Constants.LinebreakCharacters, StringSplitOptions.RemoveEmptyEntries))
        {
            yield return DiskHelper.FromString(line, blockDevices);
        }
    }

    /// <inheritdoc />
    public IEnumerable<SmartInfo> GetSmartInfos()
    {
        List<SmartInfo> smartInfos = new List<SmartInfo>();
        foreach (var id in ListDisks().Where(d => d.Type == DeviceType.Disk).Select(disk => disk.Id))
        {
            var command = BuildCommand(_systemCommands.GetSmartInfo(id));
            var response = command.LoadResponse(true);
            var info = SmartInfoParser.ParseStdOut(id, response.StdOut);
            smartInfos.Add(info);
        }

        return smartInfos;
    }

    /// <inheritdoc />
    public SmartInfo GetSmartInfo(string name)
    {
        var command = BuildCommand(_systemCommands.GetSmartInfo(name));
        var response = command.LoadResponse(true);
        return SmartInfoParser.ParseStdOut(name, response.StdOut);
    }
}