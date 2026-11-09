using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ROOT.Shared.Utils.OS;
using ROOT.Zfs.Core.Commands;
using ROOT.Zfs.Core.Helpers;
using ROOT.Zfs.Public;
using ROOT.Zfs.Public.Arguments;
using ROOT.Zfs.Public.Arguments.Properties;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Core;

internal sealed class Properties : ZfsBase, IProperties
{
    private readonly IPropertyCommands _commands;

    public Properties(IProcessCall remoteConnection, IPropertyCommands commands) : base(remoteConnection)
    {
        _commands = commands;
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<Property>> GetAvailableAsync(PropertyTarget targetType, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var args = new GetPropertyArgs { PropertyTarget = targetType };
        return GetAvailableAsync(args, cancellationToken);
    }

    private async Task<IReadOnlyList<Property>> GetAvailableAsync(GetPropertyArgs args, CancellationToken cancellationToken)
    {
        // create new intance, just in case someone re-uses an existing GetPropertyArgs with extra values set that can cause us to call with wrong arguments
        var newArgs = new GetPropertyArgs { PropertyTarget = args.PropertyTarget };

        var cmd = BuildCommand(_commands.Get(newArgs));
        // get -H returns an error, so to get available we have to create bogus command, which will return on std error
        var response = await cmd.LoadResponseAsync(false, cancellationToken: cancellationToken).ConfigureAwait(false);
        var expectedColumns = args.PropertyTarget == PropertyTarget.Pool ? 3 : 4;
        return PropertiesParser.FromStdOutput(response.StdError, expectedColumns);
    }

    /// <inheritdoc />
    public IList<PropertyValue> Get(GetPropertyArgs args)
    {
        var pc = BuildCommand(_commands.Get(args));

        var response = pc.LoadResponse(true);
        return DatasetProperties.FromStdOutput(response.StdOut);
    }

    /// <inheritdoc />
    public PropertyValue Set(SetPropertyArgs args)
    {
        var pc = BuildCommand(_commands.Set(args));

        pc.LoadResponse(true);
        var getArgs = new GetPropertyArgs { PropertyTarget = args.PropertyTarget, Property = args.Property, Target = args.Target };

        // Should be safe - if not, then the set above would have failed - if we could not find a property to set
        return Get(getArgs)[0];
    }

    /// <inheritdoc />
    public void Reset(InheritPropertyArgs args)
    {
        var pc = BuildCommand(_commands.Inherit(args));

        pc.LoadResponse(true);
    }
}