using System.Diagnostics;
using System.Globalization;

namespace ROOT.Zfs.Public.Data.Pools
{
    public struct Ratio
    {
        public double Value { get; set; }

        public Ratio()
        {
            Value = 0;
        }

        public Ratio(string ratio)
        {
            if (double.TryParse(ratio, NumberStyles.Any, CultureInfo.InvariantCulture, out var value))
            {
                Value = value;
            }
            else
            {
                Trace.WriteLine($"Could not parse '{ratio}' into a double");
                Value = 0;
            }
        }
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0:N2}x", Value);
        }
    }

    public struct Part
    {
        public double Value { get; set; }

        public Part()
        {
            Value = 0;
        }

        public Part(string percentage)
        {
            if (double.TryParse(percentage, NumberStyles.Any, CultureInfo.InvariantCulture, out var value))
            {
                Value = value;
            }
            else
            {
                Trace.WriteLine($"Could not parse '{percentage}' into a double");
                Value = 0;
            }
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0:N1}%", Value);
        }
    }
}