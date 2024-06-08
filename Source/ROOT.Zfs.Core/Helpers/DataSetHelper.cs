using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web;
using ROOT.Zfs.Public.Data;
using ROOT.Zfs.Public.Data.Datasets;

namespace ROOT.Zfs.Core.Helpers
{
    internal static class DatasetHelper
    {
        public static string Decode(string dataset)
        {
            if (dataset.Contains('%'))
            {
                return HttpUtility.UrlDecode(dataset);
            }
            return dataset;
        }

        public static string CreateDatasetName(string parent, string dataSet)
        {
            parent = Decode(parent);
            dataSet = Decode(dataSet);
            return $"{parent}/{dataSet}";
        }

        public static IList<Dataset> ParseStdOut(string stdOut)
        {
            var list = new List<Dataset>();
            foreach (var line in stdOut.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                list.Add(ParseLine(line));
            }

            return list;
        }

        /// <summary>
        /// Parses a single line and returns a Dataset object.
        /// Only supports the format that is returned from
        /// 'zfs list -Hpr -o type,creation,name,used,refer,avail,mountpoint,origin'
        /// </summary>
        /// <param name="line">The single line to parse</param>
        /// <returns>A dataset object</returns>
        /// <exception cref="ArgumentException">If the line is not in the correct format</exception>
        public static Dataset ParseLine(string line)
        {
            var parts = line.Split(new[] { '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 8)
            {
                throw new FormatException($"{line} could not be parsed, expected 8 parts, got: {parts.Length} - requires an output of type,creation,name,used,refer,avail,mountpoint,origin from command 'zfs list'");
            }

            if (!Enum.TryParse<DatasetTypes>(parts[0], true, out var datasetType))
            {
                Trace.WriteLine($"Could not parse:'{parts[0]}' into a valid dataset type - this is probably a bug - or a new unsupported dataset type has been added");
            }

            var dataset = new Dataset
            {
                Type = datasetType,
                DatasetName = parts[2],
                Used = new Size(parts[3]),
                Available = new Size(parts[4]),
                Refer = new Size(parts[5]),
                Mountpoint = parts[6],
                IsClone = parts[7].Equals("-", StringComparison.InvariantCultureIgnoreCase) == false,
            };

            return dataset;
        }
    }
}