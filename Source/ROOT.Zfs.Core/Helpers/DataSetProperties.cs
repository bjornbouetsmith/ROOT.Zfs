using System;
using System.Collections.Generic;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Core.Helpers
{
    internal static class DatasetProperties
    {
        internal static IList<PropertyValue> FromStdOutput(string stdOutput)
        {
            var list = new List<PropertyValue>();
            foreach (var line in stdOutput.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                list.Add(PropertyValueHelper.FromString(line));
            }

            return list;
        }
    }
}