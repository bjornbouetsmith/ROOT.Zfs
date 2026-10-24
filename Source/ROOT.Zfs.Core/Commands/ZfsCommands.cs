using ROOT.Shared.Utils.OS;

namespace ROOT.Zfs.Core.Commands;

internal sealed class ZfsCommands : CommandBuilderBase, IZfsCommands
{
    public ZfsCommands(IZfsBinaries binaries) : base(binaries)
    {
    }

    /// <inheritdoc />
    public IProcessCall GetVersion()
    {
        return new ProcessCall(Binaries.Zfs, "--version");
    }
}