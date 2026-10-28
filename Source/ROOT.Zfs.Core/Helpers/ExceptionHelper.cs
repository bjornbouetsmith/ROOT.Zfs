using System;

namespace ROOT.Zfs.Core.Helpers;

internal static class ExceptionHelper
{
    internal static FormatException FormatException(int index, string contents, int partsFound)
    {
        return new FormatException($@"Failed to parse trimmed input:
Split into {partsFound} parts
Look to index:{index} when discarding empty linies,
Content:
{contents}");
    }
}