using System.Collections.Generic;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Public
{
    public interface IProperties
    {
        IEnumerable<PropertyValue> GetProperties(string dataset);
        PropertyValue GetProperty(string dataset, string property);
        PropertyValue SetProperty(string dataset, string property, string value);
        ICollection<Property> GetAvailableDataSetProperties();
        void ResetPropertyToInherited(string dataset, string property);
    }
}