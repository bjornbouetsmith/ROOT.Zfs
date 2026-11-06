using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ROOT.Shared.Utils.OS;
using ROOT.Zfs.Tests.VersionResponses;

namespace ROOT.Zfs.Tests;

internal class FakeRemoteConnection : IProcessCall
{
    private readonly string _zfsVersion;
    private readonly IVersionResponse _responses;
    private readonly Dictionary<string, (string StdOut, string StdError)> _commandResponses = [];
    private readonly Dictionary<string, Exception> _commandExceptions = [];
    private readonly List<string> _commandsInvoked = [];
    private readonly List<CancellationToken> _asyncCancellationTokens = [];
    private int _syncLoadResponseCalls;
    private int _asyncLoadResponseCalls;

    public FakeRemoteConnection(string zfsVersion)
    {
        _zfsVersion = zfsVersion;
        _responses = ZfsResponses.GetVersionResponse(zfsVersion);
    }

    public List<string> GetCommandsInvoked()
    {
        return _commandsInvoked;
    }

    /// <summary>
    /// Gets the number of synchronous command executions.
    /// </summary>
    public int SyncLoadResponseCalls => _syncLoadResponseCalls;

    /// <summary>
    /// Gets the number of asynchronous command executions.
    /// </summary>
    public int AsyncLoadResponseCalls => _asyncLoadResponseCalls;

    /// <summary>
    /// Gets the cancellation tokens passed to asynchronous command executions.
    /// </summary>
    public IReadOnlyList<CancellationToken> AsyncCancellationTokens => _asyncCancellationTokens;

    /// <summary>
    /// Gets or sets whether async command responses should complete asynchronously.
    /// Setting this to true forces callers through their async continuation instead of completing synchronously.
    /// Defaults to false to keep existing tests synchronous unless they explicitly opt in.
    /// </summary>
    public bool CompleteAsyncResponses { get; set; }

    /// <summary>
    /// Gets command-specific responses keyed by the full command line.
    /// Responses registered here take precedence over the version response and the global <see cref="StdOutput"/> / <see cref="StdError"/> overrides.
    /// </summary>
    public Dictionary<string, (string StdOut, string StdError)> CommandResponses => _commandResponses;

    /// <summary>
    /// Gets exceptions keyed by the full command line. A matching command throws the configured exception instead of returning a response.
    /// </summary>
    public Dictionary<string, Exception> CommandExceptions => _commandExceptions;

    public ProcessCallResult LoadResponse(bool throwOnFailure, params string[] arguments)
    {
        _syncLoadResponseCalls++;
        return GetResponse(throwOnFailure);
    }

    public ProcessCallResult LoadResponse(bool throwOnFailure, Stream inputStream, params string[] arguments)
    {
        _syncLoadResponseCalls++;
        return GetResponse(throwOnFailure);
    }

    public async Task<ProcessCallResult> LoadResponseAsync(bool throwOnFailure, ProcessCallArguments arguments = default, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _asyncLoadResponseCalls++;
        _asyncCancellationTokens.Add(cancellationToken);

        if (CompleteAsyncResponses)
        {
            await Task.Yield();
        }

        return GetResponse(throwOnFailure);
    }

    public async Task<ProcessCallResult> LoadResponseAsync(bool throwOnFailure, Stream inputStream, ProcessCallArguments arguments = default, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _asyncLoadResponseCalls++;
        _asyncCancellationTokens.Add(cancellationToken);

        if (CompleteAsyncResponses)
        {
            await Task.Yield();
        }

        return GetResponse(throwOnFailure);
    }

    private ProcessCallResult GetResponse(bool throwOnFailure)
    {
        _commandsInvoked.Add(FullCommandLine);
        if (_commandExceptions.TryGetValue(FullCommandLine, out var commandException))
        {
            throw commandException;
        }

        (string StdOut, string StdError) response;
        if (_commandResponses.TryGetValue(FullCommandLine, out var commandResponse))
        {
            response = commandResponse;
        }
        else
        {
            (string StdOut, string StdError) fakes = (null, null);
            if (StdError == null 
                && StdOutput == null)
            {
                fakes = _responses.LoadResponse(FullCommandLine);
            }

            response = (StdOutput ?? fakes.StdOut, StdError ?? fakes.StdError);
        }

        var result =  new ProcessCallResult
        {
            CommandLine = FullCommandLine,
            ExitCode = response.StdError != null ? 1 : 0,
            StdError = response.StdError,
            StdOut = response.StdOut
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
    public bool RequiresSudo { get; set; }

    /// <summary>
    /// Set this if you want to override default response from mocks
    /// </summary>
    public string StdOutput { get; set; }
    /// <summary>
    /// Set this if you want to override default response from mocks
    /// </summary>
    public string StdError { get; set; }
}