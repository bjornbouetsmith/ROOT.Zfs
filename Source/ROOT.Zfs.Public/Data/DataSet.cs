using System;

namespace ROOT.Zfs.Public.Data
{
    public class DataSet
    {
        public string Name { get; set; }
        public string Used { get; set; }
        public string Available { get; set; }
        public string Refer { get; set; }
        public string Mountpoint { get; set; }

        public static DataSet FromString(string line)
        {
            var parts = line.Split(new[] { '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 5)
            {
                throw new ArgumentException($"{line} could not be parsed, expected 4 parts, got: {parts.Length} - requires an output of NAME,USED,AVAIL,REFER,MOUNTPINT from command 'zfs list'", nameof(line));
            }


            var dataset = new DataSet
            {
                Name = parts[0],
                Used = parts[1],
                Available = parts[2],
                Refer = parts[3],
                Mountpoint = parts[4]
            };

            return dataset;
        }
    }
}
