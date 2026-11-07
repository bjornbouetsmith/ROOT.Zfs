using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
    public async Task<VersionInfo> GetVersionInfoAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var pc = BuildCommand(_zfsCommands.GetVersion());
        var response = await pc.LoadResponseAsync(true, cancellationToken: cancellationToken).ConfigureAwait(false);

        return new VersionInfo { Lines = response.StdOut.Split(Constants.LinebreakCharacters, StringSplitOptions.RemoveEmptyEntries) };
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<DiskInfo>> ListDisksAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var disksCommand = BuildCommand(_systemCommands.ListBlockDevices());
        var disksResponse = await disksCommand.LoadResponseAsync(true, cancellationToken: cancellationToken).ConfigureAwait(false);

        var pc = BuildCommand(_systemCommands.ListDisks());
        var response = await pc.LoadResponseAsync(true, cancellationToken: cancellationToken).ConfigureAwait(false);

        return ParseDisks(disksResponse.StdOut, response.StdOut);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<SmartInfo>> GetSmartInfosAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var smartInfos = new List<SmartInfo>();
        var disks = await ListDisksAsync(cancellationToken).ConfigureAwait(false);
        foreach (var id in disks.Where(d => d.Type == DeviceType.Disk).Select(disk => disk.Id))
        {
            var command = BuildCommand(_systemCommands.GetSmartInfo(id));
            var response = await command.LoadResponseAsync(false, cancellationToken: cancellationToken).ConfigureAwait(false);
            var info = SmartInfoParser.ParseStdOut(id, response.StdOut);
            smartInfos.Add(info);
        }

        return smartInfos;
    }

    /// <inheritdoc />
    public async Task<SmartInfo> GetSmartInfoAsync(string name, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var command = BuildCommand(_systemCommands.GetSmartInfo(name));
        var response = await command.LoadResponseAsync(false, cancellationToken: cancellationToken).ConfigureAwait(false);
        return SmartInfoParser.ParseStdOut(name, response.StdOut);
    }

    private static List<DiskInfo> ParseDisks(string blockDeviceOutput, string disksOutput)
    {
        var blockDevices = DiskHelper.BlockDevicesFromStdOutput(blockDeviceOutput);

        return disksOutput
            .Split(Constants.LinebreakCharacters, StringSplitOptions.RemoveEmptyEntries)
            .Select(line => DiskHelper.FromString(line, blockDevices))
            .ToList();
    }
}