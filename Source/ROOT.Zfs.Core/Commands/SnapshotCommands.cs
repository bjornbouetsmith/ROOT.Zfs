﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ROOT.Shared.Utils.OS;
using ROOT.Zfs.Core.Helpers;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Core.Commands
{
    public class SnapshotCommands
    {
        public static class ProcessCalls
        {
            private static readonly Regex NameAllow = new Regex("[0-9]|[a-z]|[A-Z]|_|-");

            public static ProcessCall ListSnapshots(string dataset)
            {
                dataset = DataSetHelper.Decode(dataset);
                return new ProcessCall("/sbin/zfs", $"list -H -t snapshot -p -o creation,name,used -d 1 -r {dataset}");
            }

            public static ProcessCall DestroySnapshot(string dataset, string snapName)
            {
                dataset = DataSetHelper.Decode(dataset);
                var rawSnapName = snapName;
                if (snapName.StartsWith(dataset, StringComparison.OrdinalIgnoreCase))
                {
                    rawSnapName = snapName[(dataset.Length + 1)..];
                }

                return new ProcessCall("/sbin/zfs", $"destroy {dataset}@{rawSnapName}");
            }

            public static ProcessCall CreateSnapshot(string dataset, string snapName)
            {
                dataset = DataSetHelper.Decode(dataset);
                if (NameAllow.Matches(snapName).Count != snapName.Length)
                {
                    throw new ArgumentException($"{snapName} is not a valid snapshot name - valid characters are [0-9]|[a-z]|[A-Z]|_|-", nameof(snapName));
                }

                return new ProcessCall("/sbin/zfs", $"snap {dataset}@{snapName}");
            }

            public static string CreateSnapshotName(DateTime dateTime) => dateTime.ToString("yyyyMMddHHmmss");

            /// <summary>
            /// Creates a snapshot of the dataset using the format: yyyyMMddHHmmss
            /// </summary>
            /// <param name="dataset"></param>
            /// <returns></returns>
            public static ProcessCall CreateSnapshot(string dataset)
            {
                dataset = DataSetHelper.Decode(dataset);
                return CreateSnapshot(dataset, CreateSnapshotName(DateTime.UtcNow));
            }
        }

        public IEnumerable<Snapshot> LoadSnapshots(string dataset)
        {
            ProcessCall pc = ProcessCalls.ListSnapshots(dataset);
            var response = pc.LoadResponse();
            if (response.Success)
            {
                Console.WriteLine($"Command: {pc.FullCommandLine} success");
                return SnapshotParser.Parse(response.StdOut);
            }

            throw response.ToException();
        }



    }
}