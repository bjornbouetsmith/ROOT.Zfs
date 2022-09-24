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

        internal static IEnumerable<CommandHistory> FromStdOut(string stdOut, int skipLines = 0)
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
                    var indexOfBracket = line.IndexOf('[', dateEnd);
                    if (indexOfBracket > 0)
                    {
                        command = line.Substring(dateEnd + 1, indexOfBracket - (dateEnd + 1));
                        var indexOfEndBracket = line.IndexOf(']', dateEnd);
                        caller = line.Substring(indexOfBracket + 1, indexOfEndBracket - (indexOfBracket + 1));
                    }

                    yield return new CommandHistory { Time = date, Command = command, Caller = caller };
                }

            }
        }
    }
}
