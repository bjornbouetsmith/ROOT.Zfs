using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using ROOT.Shared.Utils.OS;
using ROOT.Zfs.Core.Commands;

namespace ROOT.Zfs.Core.Helpers;

/// <summary>
/// Detects whether the machine behind a connection is a virtual machine.
/// </summary>
internal sealed class VirtualMachineDetector
{
    private readonly IProcessCall _remoteConnection;
    private readonly IVirtualizationCommands _commands;

    /// <summary>
    /// Creates a new detector for the given remote connection.
    /// </summary>
    /// <param name="remoteConnection">The remote connection to run commands through. If null, the local machine is used.</param>
    /// <param name="commands">The command builder used for the detection probes.</param>
    public VirtualMachineDetector(IProcessCall remoteConnection, IVirtualizationCommands commands)
    {
        _remoteConnection = remoteConnection;
        _commands = commands ?? throw new ArgumentNullException(nameof(commands));
    }

    /// <summary>
    /// The timeout applied to each detection command. Defaults to 30 seconds.
    /// </summary>
    public TimeSpan CommandTimeout { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Whether the detection commands should be run with sudo.
    /// </summary>
    public bool RequiresSudo { get; set; }

    /// <summary>
    /// Detects whether the machine running the commands is a virtual machine.
    /// The detection uses <c>systemd-detect-virt</c> when available and falls back to
    /// <c>virt-what</c>, <c>hostnamectl</c>, DMI data, and the CPU hypervisor flag for systems without systemd.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
    /// <returns><c>true</c> when the machine appears to be virtualized; otherwise <c>false</c>.</returns>
    internal async Task<bool> IsVirtualMachineAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var systemdVirt = await ProbeVirtualizationAsync(
            _commands.DetectVirtualization(),
            response => response.ExitCode == 0 && VirtualizationHelper.IsVirtualMachine(response.StdOut),
            cancellationToken).ConfigureAwait(false);

        if (systemdVirt)
        {
            return true;
        }

        var virtWhat = await ProbeVirtualizationAsync(
            _commands.DetectVirtualizationWithVirtWhat(),
            response => response.ExitCode == 0 && VirtualizationHelper.IsVirtualizedByVirtWhat(response.StdOut),
            cancellationToken).ConfigureAwait(false);

        if (virtWhat)
        {
            return true;
        }

        var hostnamectl = await ProbeVirtualizationAsync(
            _commands.GetHostnamectlStatus(),
            response => response.ExitCode == 0 && VirtualizationHelper.HasVirtualizationEntry(response.StdOut),
            cancellationToken).ConfigureAwait(false);

        if (hostnamectl)
        {
            return true;
        }

        var dmi = await ProbeVirtualizationAsync(
            _commands.GetDmiInfo(),
            response => VirtualizationHelper.IsVirtualVendor(response.StdOut),
            cancellationToken).ConfigureAwait(false);

        if (dmi)
        {
            return true;
        }

        return await ProbeVirtualizationAsync(
            _commands.GetCpuInfo(),
            response => VirtualizationHelper.HasHypervisorFlag(response.StdOut),
            cancellationToken).ConfigureAwait(false);
    }

    private async Task<bool> ProbeVirtualizationAsync(IProcessCall current, Func<ProcessCallResult, bool> isVirtualMachine, CancellationToken cancellationToken)
    {
        var processCall = BuildCommand(current);
        try
        {
            var response = await processCall.LoadResponseAsync(false, cancellationToken: cancellationToken).ConfigureAwait(false);
            return isVirtualMachine(response);
        }
        catch (Exception e) when (e is Win32Exception or TimeoutException)
        {
            // The probe binary is not installed or did not respond. Try the next detection method.
            return false;
        }
    }

    private IProcessCall BuildCommand(IProcessCall current)
    {
        current.RequiresSudo = RequiresSudo;

        var command = ProcessCallExtensions.Pipe(_remoteConnection, current);
        command.Timeout = CommandTimeout;
        return command;
    }
}
