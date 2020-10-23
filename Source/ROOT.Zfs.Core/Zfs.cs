using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using ROOT.Shared.Utils.OS;
using ROOT.Zfs.Core.Info;

namespace ROOT.Zfs.Core
{
    public class Zfs
    {
        public static class ProcessCalls
        {
            public static ProcessCall GetVersion()
            {
                return new ProcessCall("/sbin/zfs", "--version");
            }
        }

        public IEnumerable<Snapshot> LoadSnapshots(string dataset)
        {
            ProcessCall pc = new ProcessCall("/sbin/zfs", $"list -H -t snapshot -p -o creation,name,used -d 1 -r {dataset}");
            var response = pc.LoadResponse();
            if (response.Success)
            {
                Console.WriteLine($"Command: {pc.FullCommandLine} success");
                foreach (var line in response.StdOut.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    yield return Snapshot.FromString(line);
                }
            }
            else
            {
                throw response.ToException();
            }
        }

        private static readonly Regex NameAllow = new Regex("[0-9]|[a-z]|[A-Z]|_|-");

        public void DestroySnapshot(string dataset, string snapName)
        {
            if (NameAllow.Matches(snapName).Count != snapName.Length)
            {
                throw new ArgumentException($"{snapName} is not a valid snapshot name - valid characters are [0-9]|[a-z]|[A-Z]|_|-", nameof(snapName));
            }

            ProcessCall pc = new ProcessCall("/sbin/zfs", $"destroy {dataset}@{snapName}");
            var response = pc.LoadResponse();
            if (response.Success)
            {
                Console.WriteLine($"Command: {pc.FullCommandLine} success");
            }
            else
            {
                throw response.ToException();
            }

        }

        public void CreateSnapshot(string dataset)
        {
            CreateSnapshot(dataset, DateTime.UtcNow.ToString("yyyyMMddhhmmss"));
        }

        public void CreateSnapshot(string dataset, string snapName)
        {
            if (NameAllow.Matches(snapName).Count != snapName.Length)
            {
                throw new ArgumentException($"{snapName} is not a valid snapshot name - valid characters are [0-9]|[a-z]|[A-Z]|_|-", nameof(snapName));
            }

            ProcessCall pc = new ProcessCall("/sbin/zfs", $"snap {dataset}@{snapName}");
            var response = pc.LoadResponse();
            if (response.Success)
            {
                Console.WriteLine($"Command: {pc.FullCommandLine} success");
            }
            else
            {
                throw response.ToException();
            }
        }
    }
}
