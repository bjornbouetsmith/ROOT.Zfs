using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Core.Helpers
{
    internal static class CommandHistoryHelper
    {
        /// <summary>
        /// Returns command history based on the std output from zpool history command
        /// </summary>
        /// <param name="stdOut">The std output to parse and convert into objects</param>
        /// <param name="skipLines">Lines to skip, i.e. for paging purposes</param>
        /// <param name="afterDate">Require that history be after the given date</param>
        internal static IEnumerable<CommandHistory> FromStdOut(string stdOut, int skipLines = 0, DateTime afterDate = default)
        {
            foreach (var line in stdOut.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Skip(1 + skipLines))
            {
                var dateEnd = line.IndexOf(' ');

                var datePart = line.Substring(0, dateEnd);
                var date = DateTime.ParseExact(datePart, "yyyy-MM-dd.HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal);
                if (date <= afterDate)
                {
                    continue;
                }

                var indexOfBracket = line.IndexOf('[', dateEnd);

                var commandStart = dateEnd + 1;
                var commandEnd = indexOfBracket - 1; // Remove whitespace
                var command = line[commandStart..commandEnd];
                var indexOfEndBracket = line.IndexOf(']', dateEnd);
                var callerStart = indexOfBracket + 1;
                var caller = line[callerStart..indexOfEndBracket];


                yield return new CommandHistory { Time = date, Command = command, Caller = caller };
            }
        }
    }
}
