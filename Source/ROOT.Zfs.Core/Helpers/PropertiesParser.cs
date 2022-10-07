using System;
using System.Collections.Generic;
using System.Linq;

using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Core.Helpers
{
    internal static class PropertiesParser
    {
        internal static ICollection<Property> FromStdOutput(string stdOutput, int expectedColumns)
        {
            bool startParsing = false;
            var list = new List<Property>();
            foreach (var line in stdOutput.Split(new[] { '\r', '\n' }, StringSplitOptions.None)) // Do not remove blank lines, since it will ruin the logic below
            {
                // Skip blank lines
                if (line.Trim().Length == 0)
                {
                    continue;
                }

                // IF we are at "PROPERTY" - signal that lines should be parsed
                if (line.Trim().StartsWith("PROPERTY"))
                {
                    startParsing = true;
                    continue;
                }

                // If parsing is not to be started, just continue with next line
                if (!startParsing)
                {
                    continue;
                }

                // This line is after last
                if (line.Trim().StartsWith("The feature@ properties"))
                {
                    //no more data
                    break;
                }
                // data in formmat
                // whitespace [PROPERTY]whitespace [EDIT]whitespace[INHERIT]whitespace[VALUES]
                var parts = line.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length < expectedColumns)
                {
                    Console.Write("Error parsing line:{0}", line);
                    continue;
                }

                var property = parts[0];
                var editable = parts[1];

                if (!editable.Equals("YES", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var enumerable = expectedColumns==4 ? parts.Skip(3) : parts.Skip(2);
                var content = string.Join(' ', enumerable);
                var validValues = content.Split('|').Select(v => v.Trim()).ToArray();
                var prop = new Property(property, true, validValues);

                list.Add(prop);

            }
            return list;
        }

    }
}
