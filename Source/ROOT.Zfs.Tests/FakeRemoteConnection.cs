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

        public ProcessCallResult LoadResponse(bool throwOnFailure, params string[] arguments)
        {
            return GetResponse(throwOnFailure);
        }

        public ProcessCallResult LoadResponse(bool throwOnFailure, Stream inputStream, params string[] arguments)
        {
            return GetResponse(throwOnFailure);
        }

        private ProcessCallResult GetResponse(bool throwOnFailure)
        {
            (string StdOut, string StdError) fakes = (null, null);
            if (StdError == null 
                && StdOutput == null)
            {
                fakes = _responses.LoadResponse(FullCommandLine);
            }

            var stdOut = (StdOutput ?? fakes.StdOut);
            var stdError = (StdError ?? fakes.StdError);
            var result =  new ProcessCallResult
            {
                CommandLine = FullCommandLine,
                ExitCode = stdError != null ? 1 : 0,
                StdError = stdError,
                StdOut = stdOut
            };

            if (throwOnFailure && !result.Success)
            {
                throw result.ToException();
            }

            return result;
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
