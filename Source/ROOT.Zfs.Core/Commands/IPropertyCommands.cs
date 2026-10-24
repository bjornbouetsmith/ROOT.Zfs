using ROOT.Shared.Utils.OS;
using ROOT.Zfs.Public.Arguments.Properties;

namespace ROOT.Zfs.Core.Commands;

/// <summary>
/// Contains all commands that relate to properties.
/// </summary>
public interface IPropertyCommands
{
    /// <summary>
    /// Returns a command to get a single, many or all properties.
    /// </summary>
    IProcessCall Get(GetPropertyArgs args);

    /// <summary>
    /// Returns a command that resets a property to inherited.
    /// </summary>
    IProcessCall Inherit(InheritPropertyArgs args);

    /// <summary>
    /// Returns a command to set a single property value.
    /// </summary>
    IProcessCall Set(SetPropertyArgs args);
}