namespace ROOT.Zfs.Tests.VersionResponses
{
    internal interface IVersionResponse
    {
        string LoadResponse(string commandLine);
    }
}