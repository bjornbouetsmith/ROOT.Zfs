using System;
using System.Linq;

namespace ROOT.Zfs.Core.Helpers;

/// <summary>
/// Parses the output of the different virtualization detection commands.
/// </summary>
internal static class VirtualizationHelper
{
    private static readonly string[] VirtualizationVendors =
    [
        "QEMU",
        "KVM",
        "VMware",
        "VirtualBox",
        "innotek",
        "Xen",
        "Virtual Machine",
        "Bochs",
        "Parallels",
        "BHYVE",
        "Nutanix",
        "oVirt",
        "RHV",
        "Red Hat",
        "OpenStack",
        "Amazon EC2",
        "Google Compute Engine",
        "DigitalOcean",
        "Alibaba Cloud",
        "Hetzner",
        "Scaleway",
        "UpCloud",
        "Virtuozzo",
        "Oracle VM"
    ];

    /// <summary>
    /// Interprets the output of <c>systemd-detect-virt --vm</c>.
    /// </summary>
    /// <param name="stdOut">The standard output from the command.</param>
    /// <returns><c>true</c> when a virtualization type other than <c>none</c> was reported; otherwise <c>false</c>.</returns>
    internal static bool IsVirtualMachine(string stdOut)
    {
        if (string.IsNullOrWhiteSpace(stdOut))
        {
            return false;
        }

        return !string.Equals(stdOut.Trim(), "none", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Interprets the output of <c>virt-what</c>.
    /// </summary>
    /// <param name="stdOut">The standard output from the command.</param>
    /// <returns><c>true</c> when any virtualization type was reported; otherwise <c>false</c>.</returns>
    internal static bool IsVirtualizedByVirtWhat(string stdOut)
    {
        return !string.IsNullOrWhiteSpace(stdOut);
    }

    /// <summary>
    /// Interprets the output of <c>hostnamectl status</c>.
    /// </summary>
    /// <param name="stdOut">The standard output from the command.</param>
    /// <returns><c>true</c> when a Virtualization entry with a value other than <c>none</c> was reported; otherwise <c>false</c>.</returns>
    internal static bool HasVirtualizationEntry(string stdOut)
    {
        if (string.IsNullOrWhiteSpace(stdOut))
        {
            return false;
        }

        const string entry = "Virtualization:";
        foreach (var line in stdOut.Split(Constants.LinebreakCharacters, StringSplitOptions.RemoveEmptyEntries))
        {
            var trimmed = line.Trim();
            if (!trimmed.StartsWith(entry, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var value = trimmed[entry.Length..].Trim();
            return value.Length > 0 && !string.Equals(value, "none", StringComparison.OrdinalIgnoreCase);
        }

        return false;
    }

    /// <summary>
    /// Interprets the DMI system vendor and product name.
    /// </summary>
    /// <param name="stdOut">The combined system vendor and product name.</param>
    /// <returns><c>true</c> when the DMI data identifies a known hypervisor; otherwise <c>false</c>.</returns>
    internal static bool IsVirtualVendor(string stdOut)
    {
        if (string.IsNullOrWhiteSpace(stdOut))
        {
            return false;
        }

        return VirtualizationVendors.Any(vendor => stdOut.Contains(vendor, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Interprets <c>/proc/cpuinfo</c> and looks for the x86 hypervisor CPU flag.
    /// </summary>
    /// <param name="stdOut">The contents of <c>/proc/cpuinfo</c>.</param>
    /// <returns><c>true</c> when the hypervisor flag is present; otherwise <c>false</c>.</returns>
    internal static bool HasHypervisorFlag(string stdOut)
    {
        if (string.IsNullOrWhiteSpace(stdOut))
        {
            return false;
        }

        foreach (var line in stdOut.Split(Constants.LinebreakCharacters, StringSplitOptions.RemoveEmptyEntries))
        {
            if (!line.TrimStart().StartsWith("flags", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var separator = line.IndexOf(':');
            if (separator < 0)
            {
                continue;
            }

            var flags = line[(separator + 1)..].Split(Constants.WhitespaceCharacters, StringSplitOptions.RemoveEmptyEntries);
            if (flags.Any(flag => string.Equals(flag, "hypervisor", StringComparison.OrdinalIgnoreCase)))
            {
                return true;
            }
        }

        return false;
    }
}
