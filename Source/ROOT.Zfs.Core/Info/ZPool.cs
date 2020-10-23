using System.Collections.Generic;

namespace ROOT.Zfs.Core.Info
{
    public class ZPool
    {
        public string Name { get; }

        public ZPool(string name, List<Property> properties)
        {
            Name = name;
            Properties = properties;
        }

        public List<Property> Properties { get; set; }
    
    }
}
