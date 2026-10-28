using ROOT.Shared.Utils.OS;

namespace ROOT.Zfs.Core.Commands;

internal sealed class ZdbCommands : CommandBuilderBase, IZdbCommands
{
    public ZdbCommands(IZfsBinaries binaries) : base(binaries)
    {
    }

    /// <summary>
    /// Returns a command that will output from zdb
    /// </summary>
    public IProcessCall GetRawZdbOutput()
    {
        return new ProcessCall(Binaries.Zdb);
    }
}