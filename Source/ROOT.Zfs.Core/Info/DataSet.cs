using System;
using System.Collections.Generic;
using System.Text;

namespace ROOT.Zfs.Core.Info
{
    public class DataSet
    {
        public string Name { get; }

        public DataSet(string name, List<Property> properties)
        {
            Name = name;
            Properties = properties;
        }

        public List<Property> Properties { get; set; }
    }
}
