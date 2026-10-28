using System;
using Microsoft.Extensions.DependencyInjection;
using ROOT.Shared.Utils.OS;
using ROOT.Zfs.Core.Commands;
using ROOT.Zfs.Public;

namespace ROOT.Zfs.Core;

/// <summary>
/// Microsoft.Extensions.DependencyInjection integration for the ZFS library.
/// </summary>
public static class ZfsServiceCollectionExtensions
{
    /// <summary>
    /// Registers the ZFS services, command builders, and <see cref="IZfs"/> implementation.
    /// </summary>
    /// <param name="services">The service collection to add the ZFS services to.</param>
    /// <param name="remoteConnection">The remote connection to use. If null, the local machine is used.</param>
    /// <param name="configureBinaries">Optional callback used to override the default binary locations.</param>
    public static IServiceCollection AddZfs(
        this IServiceCollection services,
        IProcessCall remoteConnection = null,
        Action<IZfsBinaries> configureBinaries = null)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        services.AddProcessCall(remoteConnection);

        services.AddSingleton<IZfsBinaries>(sp =>
        {
            var binaries = new ZfsBinaries(sp.GetRequiredService<IProcessCall>());
            configureBinaries?.Invoke(binaries);
            return binaries;
        });
        services.AddSingleton<ISystemCommands, SystemCommands>();
        services.AddSingleton<IDatasetCommands, DatasetCommands>();
        services.AddSingleton<IPropertyCommands, PropertyCommands>();
        services.AddSingleton<ISnapshotCommands, SnapshotCommands>();
        services.AddSingleton<IZdbCommands, ZdbCommands>();
        services.AddSingleton<IZfsCommands, ZfsCommands>();
        services.AddSingleton<IZpoolCommands, ZpoolCommands>();

        services.AddSingleton<ISnapshots, Snapshots>();
        services.AddSingleton<IDatasets, Datasets>();
        services.AddSingleton<IProperties, Properties>();
        services.AddSingleton<IZPool, ZPool>();

        services.AddSingleton<IZfs, Zfs>();

        return services;
    }
}