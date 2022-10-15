using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace ROOT.Zfs.Public.Data
{
    /// <summary>
    /// Represents a percentage
    /// </summary>
    public struct Part
    {
        /// <summary>
        /// The underluing floating point value of the percentage
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// Default ctor for serialization purposes
        /// </summary>
        [ExcludeFromCodeCoverage]
        public Part()
        {
            Value = 0;
        }

        /// <summary>
        /// Creates a <see cref="Part"/> based on the string representation of a percentage.
        /// If the passed value cannot be parsed into a valid floating point, the value gets set to 0
        /// </summary>
        public Part(string percentage)
        {
            if (!double.TryParse(percentage, NumberStyles.Any, CultureInfo.InvariantCulture, out var value))
            {
                Trace.WriteLine($"Could not parse '{percentage}' into a double");
            }
            Value = value;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0:N2}%", Value);
        }
    }
}