using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace ROOT.Zfs.Public.Data.Statistics
{
    /// <summary>
    /// Represents a ratio - similar to <see cref="Part"/> - but different in the sense that valid values are probably never above 10
    /// Used to represent the dedup ratio in a zpool
    /// </summary>
    public struct Ratio
    {
        /// <summary>
        /// The underlying raw value of the ratio
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// Default ctor for serialization purposes
        /// </summary>
        [ExcludeFromCodeCoverage]
        public Ratio()
        {
            Value = 0;
        }

        /// <summary>
        /// Creates a new <see cref="Ratio"/> based on the string representation of a ratio represented a a floating point value.
        /// If the passed string cannot be parsed into a valid floating point value - 0 is assigned to the <see cref="Value"/> property
        /// </summary>
        public Ratio(string ratio)
        {
            if (!double.TryParse(ratio, NumberStyles.Any, CultureInfo.InvariantCulture, out var value))
            {
                Trace.TraceWarning($"Could not parse '{ratio}' into a double");
            }
            Value = value;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0:N2}x", Value);
        }
    }
}