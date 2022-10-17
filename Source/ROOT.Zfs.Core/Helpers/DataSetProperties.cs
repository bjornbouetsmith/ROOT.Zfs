using System;
using System.Collections.Generic;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Core.Helpers
{
    internal static class DatasetProperties
    {
        internal static IEnumerable<PropertyValue> FromStdOutput(string stdOutput)
        {
            foreach (var line in stdOutput.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                yield return PropertyValueHelper.FromString(line);
            }
        }
    }
}