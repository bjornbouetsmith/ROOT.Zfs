
using System.Globalization;
using System.Text;

namespace ROOT.Zfs.Public.Arguments.Dataset;

/// <summary>
/// Contains required and optional arguments to 'zfs promote' command
/// </summary>
public class PromoteArgs : DatasetNameArgs
{
    /// <inheritdoc />
    public PromoteArgs() : base("promote")
    {
    }

    /// <inheritdoc />
    protected override string BuildArgs(string command)
    {
        var args = new StringBuilder();
        args.Append(command);
        args.Append(CultureInfo.InvariantCulture, $" {Decode(Name)}");

        return args.ToString();
    }
}