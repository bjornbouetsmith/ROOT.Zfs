namespace ROOT.Zfs.Tests.VersionResponses
{
    internal class VersionResponse2_1_5_2 : VersionAgnosticResponse
    {
        public VersionResponse2_1_5_2() : base("2.1.5-2")
        {

        }
        public override (string StdOut, string StdError) LoadResponse(string commandLine)
        {
            switch (commandLine)
            {
                default:
                    return base.LoadResponse(commandLine);
            }
        }

    }
}