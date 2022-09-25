﻿using ROOT.Shared.Utils.OS;
using ROOT.Zfs.Core.Helpers;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Core.Commands
{
    public class PropertyCommands
    {
        public static class ProcessCalls
        {
            public static ProcessCall GetProperties(string dataset)
            {
                dataset = DataSetHelper.Decode(dataset);
                return new ProcessCall("/sbin/zfs", $"get all {dataset} -H");
            }

            public static ProcessCall GetDataSetProperties()
            {
                return new ProcessCall("/sbin/zfs", "get -H");
            }

            public static ProcessCall GetPoolProperties()
            {
                return new ProcessCall("/sbin/zpool", "get  -H");
            }

            public static ProcessCall ResetPropertyToInherited(string dataset, Property property)
            {
                dataset = DataSetHelper.Decode(dataset);
                return new ProcessCall("/sbin/zfs", $"inherit -rS {property.Name} {dataset}");
            }

            public static ProcessCall SetProperty(string dataset, Property property, string value)
            {
                dataset = DataSetHelper.Decode(dataset);
                return new ProcessCall("/sbin/zfs", $"set {property.Name}={value} {dataset}");
            }

            public static ProcessCall GetProperty(string dataset, Property property)
            {
                dataset = DataSetHelper.Decode(dataset);
                return new ProcessCall("/sbin/zfs", $"get {property.Name} {dataset} -H");
            }

            
        }
    }
}