using System;
using ROOT.Shared.Utils.OS;
using ROOT.Zfs.Public.Arguments;
using ROOT.Zfs.Public.Arguments.Properties;

namespace ROOT.Zfs.Core.Commands;

/// <summary>
/// Contains all the commands that relates to properties
/// </summary>
internal sealed class PropertyCommands : CommandBuilderBase, IPropertyCommands
{
    public PropertyCommands(IZfsBinaries binaries) : base(binaries)
    {
    }

    /// <summary>
    /// Returns a command to get a single, many or all properties depending on the values on the args instance
    /// </summary>
    /// <exception cref="ArgumentException">If arguments are not valid</exception>
    public IProcessCall Get(GetPropertyArgs args)
    {
        if (!args.Validate(out var errors))
        {
            throw ToArgumentException(errors, args);
        }
        var binary = args.PropertyTarget == PropertyTarget.Pool ? Binaries.Zpool : Binaries.Zfs;
        return new ProcessCall(binary, args.ToString());
    }

    /// <summary>
    /// Returns a command that will reset a property to inherited
    /// </summary>
    /// <exception cref="ArgumentException">If arguments are not valid</exception>
    public IProcessCall Inherit(InheritPropertyArgs args)
    {
        if (!args.Validate(out var errors))
        {
            throw ToArgumentException(errors, args);
        }

        return new ProcessCall(Binaries.Zfs, args.ToString());
    }

    /// <summary>
    /// Returns a command to set a single property value
    /// </summary>
    /// <exception cref="ArgumentException">If arguments are not valid</exception>
    public IProcessCall Set(SetPropertyArgs args)
    {
        if (!args.Validate(out var errors))
        {
            throw ToArgumentException(errors, args);
        }
        var binary = args.PropertyTarget == PropertyTarget.Pool ? Binaries.Zpool : Binaries.Zfs;
        return new ProcessCall(binary, args.ToString());
    }
}