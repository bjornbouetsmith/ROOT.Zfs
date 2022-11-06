using System.Collections.Generic;
using ROOT.Zfs.Public.Arguments;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Public
{
    /// <summary>
    /// Base properties interface with common functionality
    /// </summary>
    public interface IProperties : IBasicZfs
    {
        /// <summary>
        /// Gets all available properties that can be set for the given type of root
        /// </summary>
        IList<Property> GetAvailable(PropertyTarget targetType);
        
        /// <summary>
        /// Gets property information
        /// Based on the values in the args instance, this can either be one property, many properties, all properties
        /// </summary>
        IList<PropertyValue> Get(GetPropertyArgs args);
        
        /// <summary>
        /// Sets a property to the given value
        /// </summary>
        PropertyValue Set(SetPropertyArgs args);
        
        /// <summary>
        /// Resets a dataset property to its inherited value.
        /// Does not work for a zpool, since a pool cannot have a parent
        /// </summary>
        void Reset(InheritPropertyArgs args);
    }
}