using System;
using System.Collections.Generic;
using System.Linq;
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

    public IEnumerable<CommandHistory> History(PoolHistoryArgs args)
    {
        var pc = BuildCommand(_zpoolCommands.History(args));

        var response = pc.LoadResponse(true);

        return CommandHistoryHelper.FromStdOut(response.StdOut, args.SkipLines, args.AfterDate);
    }

    /// <inheritdoc />
    public PoolStatus Status(PoolStatusArgs args)
    {
        var pc = BuildCommand(_zpoolCommands.GetStatus(args));
        var response = pc.LoadResponse(true);

        return ZPoolStatusParser.Parse(response.StdOut);
    }

    /// <inheritdoc />
    public IList<PoolInfo> List()
    {
        var pc = BuildCommand(_zpoolCommands.GetAllPoolInfos());
        var response = pc.LoadResponse(true);

        var list = ZPoolInfoParser.ParseFromStdOut(response.StdOut);
        var versionCommand = BuildCommand(_zdbCommands.GetRawZdbOutput());
        response = versionCommand.LoadResponse(true);
        var versions = ZdbHelper.ParsePoolVersions(response.StdOut);

        foreach (var info in list)
        {
            // Should be safe to do a .First, since zfs should contain data about the pool if the pool exist
            var versionInfo = versions.First(v => v.PoolName.Equals(info.PoolName, StringComparison.OrdinalIgnoreCase));
            info.Version = versionInfo.Version;
        }

        return list;
    }

    /// <inheritdoc />
    public PoolInfo List(PoolListArgs args)
    {
        var pc = BuildCommand(_zpoolCommands.GetPoolInfo(args));
        var response = pc.LoadResponse(true);

        var info = ZPoolInfoParser.ParseLine(response.StdOut);

        var versionCommand = BuildCommand(_zdbCommands.GetRawZdbOutput());
        response = versionCommand.LoadResponse(true);

        var versions = ZdbHelper.ParsePoolVersions(response.StdOut);
        // Should be safe to do a .First, since zfs should contain data about the pool if the pool exist
        var versionInfo = versions.First(v=>v.PoolName.Equals(args.PoolName, StringComparison.OrdinalIgnoreCase));
        info.Version = versionInfo.Version;

        return info;
    }

    /// <inheritdoc />
    public PoolStatus Create(PoolCreateArgs args)
    {
        var pc = BuildCommand(_zpoolCommands.Create(args));
        pc.LoadResponse(true);
        var statusArg = new PoolStatusArgs { PoolName = args.PoolName };

        return Status(statusArg);
    }

    /// <inheritdoc />
    public void Destroy(PoolDestroyArgs args)
    {
        var pc = BuildCommand(_zpoolCommands.Destroy(args));
        pc.LoadResponse(true);
    }

    /// <inheritdoc />
    public void Offline(PoolOfflineArgs args)
    {
        var pc = BuildCommand(_zpoolCommands.Offline(args));
        pc.LoadResponse(true);
    }

    /// <inheritdoc />
    public void Online(PoolOnlineArgs args)
    {
        var pc = BuildCommand(_zpoolCommands.Online(args));
        pc.LoadResponse(true);
    }

    /// <inheritdoc />
    public void Clear(PoolClearArgs args)
    {
        var pc = BuildCommand(_zpoolCommands.Clear(args));
        pc.LoadResponse(true);
    }

    /// <inheritdoc />
    public IOStats IOStats(PoolIOStatsArgs args)
    {
        var pc = BuildCommand(_zpoolCommands.IoStats(args));
        var response = pc.LoadResponse(true);
        return ZPoolIOStatParser.ParseStdOut(args.PoolName, response.StdOut);
    }

    /// <inheritdoc />
    public void Resilver(PoolResilverArgs args)
    {
        var pc = BuildCommand(_zpoolCommands.Resilver(args));
        pc.LoadResponse(true);
    }

    /// <inheritdoc />
    public void Scrub(PoolScrubArgs args)
    {
        var pc = BuildCommand(_zpoolCommands.Scrub(args));
        pc.LoadResponse(true);
    }

    /// <inheritdoc />
    public void Trim(PoolTrimArgs args)
    {
        var pc = BuildCommand(_zpoolCommands.Trim(args));
        pc.LoadResponse(true);
    }

    /// <inheritdoc />
    public IList<UpgradeablePool> GetUpgradeablePools()
    {
        var pc = BuildCommand(_zpoolCommands.GetUpgradeablePools());
        var response = pc.LoadResponse(true);
        return UpgradeablePoolsParser.ParseStdOut(response.StdOut);
    }

    /// <inheritdoc />
    public void Upgrade(PoolUpgradeArgs args)
    {
        var pc = BuildCommand(_zpoolCommands.Upgrade(args));
        pc.LoadResponse(true);
    }

    public void Detach(PoolDetachArgs args)
    {
        var pc = BuildCommand(_zpoolCommands.Detach(args));
        pc.LoadResponse(true);
    }

    /// <inheritdoc />
    public void Attach(PoolAttachArgs args)
    {
        var pc = BuildCommand(_zpoolCommands.Attach(args));
        pc.LoadResponse(true);
    }

    /// <inheritdoc />
    public void Replace(PoolReplaceArgs args)
    {
        var pc = BuildCommand(_zpoolCommands.Replace(args));
        pc.LoadResponse(true);
    }
    /// <inheritdoc />
    public void Add(PoolAddArgs args)
    {
        var pc = BuildCommand(_zpoolCommands.Add(args));
        pc.LoadResponse(true);
    }

    public void Remove(PoolRemoveArgs args)
    {
        var pc = BuildCommand(_zpoolCommands.Remove(args));
        pc.LoadResponse(true);
    }
}