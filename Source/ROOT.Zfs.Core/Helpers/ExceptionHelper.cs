using System;

namespace ROOT.Zfs.Core.Helpers
{
    internal static class ExceptionHelper
    {
        internal static FormatException FormatException(int index, string contents)
        {
            return new FormatException($@"Failed to parse trimmed input:
Look to index:{index} when discarding empty linies,
Content:
{contents}");
        }
    }
}
