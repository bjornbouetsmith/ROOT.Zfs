using System;
using System.Web;
using ROOT.Zfs.Public.Data;
using ROOT.Zfs.Public.Data.DataSets;

namespace ROOT.Zfs.Core.Helpers
{
    public static class DataSetHelper
    {
        public static string Decode(string dataset) 
        {
            if (dataset.Contains('%'))
            {
                return HttpUtility.UrlDecode(dataset);
            }
            return dataset;
        }

        public static string CreateDataSetName(string parent, string dataSet)
        {
            parent = Decode(parent);
            dataSet= Decode(dataSet);
            return $"{parent}/{dataSet}";
        }

        /// <summary>
        /// Parses a single line and returns a Dataset object.
        /// Only supports the format that is returned from
        /// 'zfs list -Hpr -o type,creation,name,used,refer,avail,mountpoint'
        /// </summary>
        /// <param name="line">The single line to parse</param>
        /// <returns>A dataset object</returns>
        /// <exception cref="ArgumentException">If the line is not in the correct format</exception>
        public static DataSet ParseStdOut(string line)
        {
            var parts = line.Split(new[] { '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 7)
            {
                throw new ArgumentException($"{line} could not be parsed, expected 7 parts, got: {parts.Length} - requires an output of type,creation,name,used,refer,avail,mountpoint from command 'zfs list'", nameof(line));
            }
            
            var dataset = new DataSet
            {
                Name = parts[2],
                Used = new Size(parts[3]),
                Available = new Size(parts[4]),
                Refer = new Size(parts[5]),
                Mountpoint = parts[6]
            };

            return dataset;
        }
    }
}