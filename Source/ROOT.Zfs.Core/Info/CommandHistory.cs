using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ROOT.Zfs.Core.Info
{
    public class CommandHistory
    {
        public DateTime Time { get; set; }
        public string Command { get; set; }
        public string Caller { get; set; }

        internal static IEnumerable<CommandHistory> FromStdOut(string stdOut, int skipLines = 0, DateTime afterDate = default)
        {
            foreach (var line in stdOut.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Skip(1 + skipLines))
            {
                var dateEnd = line.IndexOf(' ');
                if (dateEnd > 0)
                {
                    var command = string.Empty;
                    var caller = string.Empty;
                    var datePart = line.Substring(0, dateEnd);
                    var date = DateTime.ParseExact(datePart, "yyyy-MM-dd.HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal);
                    if (date <= afterDate)
                    {
                        continue;
                    }

                    var indexOfBracket = line.IndexOf('[', dateEnd);
                    if (indexOfBracket > 0)
                    {
                        var commandStart = dateEnd + 1;
                        var commandEnd = indexOfBracket - 1; // Remove whitespace
                        command = line[commandStart..commandEnd];
                        var indexOfEndBracket = line.IndexOf(']', dateEnd);
                        var callerStart = indexOfBracket + 1;
                        caller = line[callerStart..indexOfEndBracket];
                    }
                    
                    yield return new CommandHistory { Time = date, Command = command, Caller = caller };
                }

            }
        }
    }
}
