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
    /// Setting this to true exercises the continuation path of callers awaiting the command.
    /// Defaults to true, which is the most common case. Set to false to simulate a command that completes synchronously.
    /// </summary>
    public bool CompleteAsyncResponses { get; set; } = true;

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