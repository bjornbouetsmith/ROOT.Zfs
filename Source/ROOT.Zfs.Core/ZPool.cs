using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ROOT.Shared.Utils.OS;
using ROOT.Zfs.Core.Commands;
using ROOT.Zfs.Core.Helpers;
using ROOT.Zfs.Public;
using ROOT.Zfs.Public.Arguments.Pool;
using ROOT.Zfs.Public.Data;
using ROOT.Zfs.Public.Data.Pools;
using ROOT.Zfs.Public.Data.Statistics;

namespace ROOT.Zfs.Core;

/// <inheritdoc cref="IZPool" />
internal sealed class ZPool : ZfsBase, IZPool
{
    private readonly IZpoolCommands _zpoolCommands;
    private readonly IZdbCommands _zdbCommands;

    public ZPool(IProcessCall remoteConnection, IZpoolCommands zpoolCommands, IZdbCommands zdbCommands) : base(remoteConnection)
    {
        _zpoolCommands = zpoolCommands;
        _zdbCommands = zdbCommands;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<CommandHistory>> HistoryAsync(PoolHistoryArgs args, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var pc = BuildCommand(_zpoolCommands.History(args));

        var response = await pc.LoadResponseAsync(true, cancellationToken: cancellationToken).ConfigureAwait(false);

        return CommandHistoryHelper.FromStdOut(response.StdOut, args.SkipLines, args.AfterDate);
    }

    /// <inheritdoc />
    public async Task<PoolStatus> StatusAsync(PoolStatusArgs args, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var pc = BuildCommand(_zpoolCommands.GetStatus(args));
        var response = await pc.LoadResponseAsync(true, cancellationToken: cancellationToken).ConfigureAwait(false);

        return ZPoolStatusParser.Parse(response.StdOut);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<PoolInfo>> ListAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var pc = BuildCommand(_zpoolCommands.GetAllPoolInfos());
        var response = await pc.LoadResponseAsync(true, cancellationToken: cancellationToken).ConfigureAwait(false);

        var list = ZPoolInfoParser.ParseFromStdOut(response.StdOut);
        var versionCommand = BuildCommand(_zdbCommands.GetRawZdbOutput());
        response = await versionCommand.LoadResponseAsync(true, cancellationToken: cancellationToken).ConfigureAwait(false);
        var versions = ZdbHelper.ParsePoolVersions(response.StdOut);

        foreach (var info in list)
        {
            // Should be safe to do a .First, since zfs should contain data about the pool if the pool exist
            var versionInfo = versions.First(v => v.PoolName.Equals(info.PoolName, StringComparison.OrdinalIgnoreCase));
            info.Version = versionInfo.Version;
        }

        return list.ToList();
    }

    /// <inheritdoc />
    public async Task<PoolInfo> ListAsync(PoolListArgs args, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var pc = BuildCommand(_zpoolCommands.GetPoolInfo(args));
        var response = await pc.LoadResponseAsync(true, cancellationToken: cancellationToken).ConfigureAwait(false);

        var info = ZPoolInfoParser.ParseLine(response.StdOut);

        var versionCommand = BuildCommand(_zdbCommands.GetRawZdbOutput());
        response = await versionCommand.LoadResponseAsync(true, cancellationToken: cancellationToken).ConfigureAwait(false);

        var versions = ZdbHelper.ParsePoolVersions(response.StdOut);
        // Should be safe to do a .First, since zfs should contain data about the pool if the pool exist
        var versionInfo = versions.First(v => v.PoolName.Equals(args.PoolName, StringComparison.OrdinalIgnoreCase));
        info.Version = versionInfo.Version;

        return info;
    }

    /// <inheritdoc />
    public async Task<PoolStatus> CreateAsync(PoolCreateArgs args, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var pc = BuildCommand(_zpoolCommands.Create(args));
        await pc.LoadResponseAsync(true, cancellationToken: cancellationToken).ConfigureAwait(false);
        var statusArg = new PoolStatusArgs { PoolName = args.PoolName };

        return await StatusAsync(statusArg, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task DestroyAsync(PoolDestroyArgs args, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var pc = BuildCommand(_zpoolCommands.Destroy(args));
        await pc.LoadResponseAsync(true, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task OfflineAsync(PoolOfflineArgs args, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var pc = BuildCommand(_zpoolCommands.Offline(args));
        await pc.LoadResponseAsync(true, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task OnlineAsync(PoolOnlineArgs args, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var pc = BuildCommand(_zpoolCommands.Online(args));
        await pc.LoadResponseAsync(true, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task ClearAsync(PoolClearArgs args, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var pc = BuildCommand(_zpoolCommands.Clear(args));
        await pc.LoadResponseAsync(true, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<IOStats> IOStatsAsync(PoolIOStatsArgs args, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var pc = BuildCommand(_zpoolCommands.IoStats(args));
        var response = await pc.LoadResponseAsync(true, cancellationToken: cancellationToken).ConfigureAwait(false);
        return ZPoolIOStatParser.ParseStdOut(args.PoolName, response.StdOut);
    }

    /// <inheritdoc />
    public async Task ResilverAsync(PoolResilverArgs args, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var pc = BuildCommand(_zpoolCommands.Resilver(args));
        await pc.LoadResponseAsync(true, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task ScrubAsync(PoolScrubArgs args, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var pc = BuildCommand(_zpoolCommands.Scrub(args));
        await pc.LoadResponseAsync(true, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task TrimAsync(PoolTrimArgs args, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var pc = BuildCommand(_zpoolCommands.Trim(args));
        await pc.LoadResponseAsync(true, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<UpgradeablePool>> GetUpgradeablePoolsAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var pc = BuildCommand(_zpoolCommands.GetUpgradeablePools());
        var response = await pc.LoadResponseAsync(true, cancellationToken: cancellationToken).ConfigureAwait(false);
        return UpgradeablePoolsParser.ParseStdOut(response.StdOut).ToList();
    }

    /// <inheritdoc />
    public async Task UpgradeAsync(PoolUpgradeArgs args, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var pc = BuildCommand(_zpoolCommands.Upgrade(args));
        await pc.LoadResponseAsync(true, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task DetachAsync(PoolDetachArgs args, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var pc = BuildCommand(_zpoolCommands.Detach(args));
        await pc.LoadResponseAsync(true, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task AttachAsync(PoolAttachArgs args, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var pc = BuildCommand(_zpoolCommands.Attach(args));
        await pc.LoadResponseAsync(true, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task ReplaceAsync(PoolReplaceArgs args, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var pc = BuildCommand(_zpoolCommands.Replace(args));
        await pc.LoadResponseAsync(true, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task AddAsync(PoolAddArgs args, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var pc = BuildCommand(_zpoolCommands.Add(args));
        await pc.LoadResponseAsync(true, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task RemoveAsync(PoolRemoveArgs args, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var pc = BuildCommand(_zpoolCommands.Remove(args));
        await pc.LoadResponseAsync(true, cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}
