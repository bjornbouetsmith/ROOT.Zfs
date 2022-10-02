using System;
using System.IO;
using ROOT.Shared.Utils.OS;
using ROOT.Zfs.Tests.VersionResponses;

namespace ROOT.Zfs.Tests
{
    internal class FakeRemoteConnection : IProcessCall
    {
        private readonly string _zfsVersion;
        private readonly IVersionResponse _responses;

        public FakeRemoteConnection(string zfsVersion)
        {
            _zfsVersion = zfsVersion;
            _responses = ZfsResponses.GetVersionResponse(zfsVersion);
        }

        public ProcessCallResult LoadResponse(params string[] arguments)
        {
            return GetResponse();
        }

        public ProcessCallResult LoadResponse(Stream inputStream, params string[] arguments)
        {
            return GetResponse();
        }

        private ProcessCallResult GetResponse()
        {
            var fakes = _responses.LoadResponse(FullCommandLine);
            var stdOut = (StdOutput ?? fakes.StdOut);
            var stdError = (StdError ?? fakes.StdError);
            return new ProcessCallResult
            {
                CommandLine = FullCommandLine,
                ExitCode = Success ? 0 : 1,
                StdError = stdError,
                StdOut = stdOut
            };
        }

        public IProcessCall Pipe(IProcessCall other)
        {
            FullCommandLine = other.FullCommandLine;
            return this;
        }

        public string BinPath => "FAKE";
        public string Arguments => "FAKE";
        public string FullCommandLine { get; set; }
        public string Shell { get; set; } = "/bin/false";
        public bool UseShell { get; set; } = false;
        public TimeSpan Timeout { get; set; } = TimeSpan.MaxValue;
        public bool Started => false;
        public bool Success { get; set; } = true;

        /// <summary>
        /// Set this if you want to override default response from mocks
        /// </summary>
        public string StdOutput { get; set; }
        /// <summary>
        /// Set this if you want to override default response from mocks
        /// </summary>
        public string StdError { get; set; }
    }
}
