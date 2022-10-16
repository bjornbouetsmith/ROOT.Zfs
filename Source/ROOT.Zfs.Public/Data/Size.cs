using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace ROOT.Zfs.Public.Data
{
    /// <summary>
    /// Represents a size of something.
    /// A pool, device, disk etc.
    /// </summary>
    public struct Size
    {
        /// <summary>
        /// Creates a size with 0 bytes - intended for serialization purposes
        /// </summary>
        [ExcludeFromCodeCoverage]
        public Size()
        {
            Bytes = 0;
        }

        /// <summary>
        /// Creates a size with the given bytes
        /// </summary>
        public Size(long bytes)
        {
            Bytes = bytes;
        }

        /// <summary>
        /// Creates a size with the given bytes represented as a string.
        /// If the string cannot be parsed into a number, the size gets set to 0;
        /// </summary>
        public Size(string bytes)
        {
            if (!long.TryParse(bytes, out var b))
            {
                Trace.WriteLine($"Cannnot parse {bytes} into a number");
            }
            Bytes = b;
        }
        /// <summary>
        /// Gets or sets the size in bytes
        /// </summary>
        public long Bytes { get; set; }

        /// <summary>
        /// Gets the size converted to KiB
        /// </summary>
        public double KiB => Bytes / 1024d;

        /// <summary>
        /// Gets the size converted to MiB
        /// </summary>
        public double MiB => KiB / 1024d;

        /// <summary>
        /// Gets the size converted to GiB
        /// </summary>
        public double GiB => MiB / 1024d;

        /// <summary>
        /// Gets the size converted to TiB
        /// </summary>
        public double TiB => GiB / 1024d;
        
        /// <summary>
        /// Returns a string representation that most represents what people would represent the raw number of bytes as
        /// </summary>
        public override string ToString()
        {
            if (TiB >= 1.0d)
            {
                return TiB.ToString("##.#T", CultureInfo.InvariantCulture);
            }
            if (GiB >= 1.0d)
            {
                return GiB.ToString("##.#G", CultureInfo.InvariantCulture);
            }
            if (MiB >= 1.0d)
            {
                return MiB.ToString("##.#M",CultureInfo.InvariantCulture);
            }
            if (KiB >= 1.0d)
            {
                return KiB.ToString("##.#K", CultureInfo.InvariantCulture);
            }
            return Bytes.ToString(CultureInfo.InvariantCulture);
        }
    }
}