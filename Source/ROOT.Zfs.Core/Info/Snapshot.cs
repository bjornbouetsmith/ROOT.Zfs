using System;
using ROOT.Shared.Utils.Date;

namespace ROOT.Zfs.Core.Info
{
    public class Snapshot
    {
        public string Name { get; set; }
        public DateTime CreationDate { get; set; }
        public long Size { get; set; }

        public Snapshot()
        {
        }

        public Snapshot(string name, long size, DateTime creationDate)
        {
            Name = name;
            Size = size;
            CreationDate = creationDate;
        }

        /// <summary>
        /// Expects one line of output from the zfs list -t snapshot command with output of only creation,name,used
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static Snapshot FromString(string line)
        {
            var parts = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 3)
            {
                throw new ArgumentException($"{line} could not be parsed, expected 3 parts, got: {parts.Length} - requires an output of creation,name,used to be used for snapshot list", nameof(line));
            }

            var snapshot = new Snapshot();

            snapshot.Name = parts[1];
            if (long.TryParse(parts[0], out var secs))
            {
                snapshot.CreationDate = DateUtils.ToDateTime(secs);
            }

            long size;
            if (long.TryParse(parts[2], out size))
            {
                snapshot.Size = size;
            }

            return snapshot;
        }

    }
}
