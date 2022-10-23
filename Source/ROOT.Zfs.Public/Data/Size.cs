using System;
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
        public Size(ulong bytes)
        {
            Bytes = bytes;
        }

        /// <summary>
        /// Creates a size with the given bytes represented as a string.
        /// If the string cannot be parsed into a number, the size gets set to 0;
        /// </summary>
        public Size(string bytes)
        {
            if (!ulong.TryParse(bytes, out var b))
            {
                Trace.WriteLine($"Cannnot parse {bytes} into a number");
            }
            Bytes = b;
        }
        /// <summary>
        /// Gets or sets the size in bytes
        /// </summary>
        public ulong Bytes { get; set; }

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
                return MiB.ToString("##.#M", CultureInfo.InvariantCulture);
            }
            if (KiB >= 1.0d)
            {
                return KiB.ToString("##.#K", CultureInfo.InvariantCulture);
            }

            if (Bytes == 0)
            {
                return "0";
            }

            return Bytes.ToString("###B", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Implicitly converts a string into a size if possible.
        /// Valid formats follow zfs rules, 1K, 1G and so forth
        /// </summary>
        public static implicit operator Size(string sizeString)
        {
            return FromSizeString(sizeString);
        }
        /// <summary>
        /// Implicitly converts a ulong into a size.
        /// </summary>
        public static implicit operator Size(ulong size)
        {
            return new Size(size);
        }

        /// <summary>
        /// Creates a size from the
        /// </summary>
        /// <param name="sizeString"></param>
        /// <returns></returns>
        public static Size FromSizeString(string sizeString)
        {
            if (sizeString == "0")
            {
                return new Size();
            }

            if (!sizeString.EndsWith("B")
                && !sizeString.EndsWith("K")
                && !sizeString.EndsWith("M")
                && !sizeString.EndsWith("G")
                && !sizeString.EndsWith("T"))
            {
                throw new ArgumentException("size must be a string that ends in either B, K, M, G, T");
            }

            var last = sizeString[^1];
            var first = sizeString[..^1];
            if (!double.TryParse(first, NumberStyles.Any, CultureInfo.InvariantCulture, out var size))
            {
                throw new ArgumentException("Size must start with a number, possibly floating point using a period as decimal separator");
            }

            switch (last)
            {
                case 'B':
                    return new Size((ulong)size);
                case 'K':
                    return new Size((ulong)(size * 1024));
                case 'M':
                    return new Size((ulong)(size * 1024 * 1024));
                case 'G':
                    return new Size((ulong)(size * 1024 * 1024 * 1024));
                // Should only be T, since validation above only allow these values to pass
                default:
                    return new Size((ulong)(size * 1024 * 1024 * 1024 * 1024));
            }
        }
    }
}