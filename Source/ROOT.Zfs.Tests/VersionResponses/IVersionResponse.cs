namespace ROOT.Zfs.Tests.VersionResponses
{
    internal interface IVersionResponse
    {
        (string StdOut, string StdError) LoadResponse(string commandLine);
    }
}