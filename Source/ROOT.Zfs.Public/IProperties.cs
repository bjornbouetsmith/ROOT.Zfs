using System.Collections.Generic;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Public
{
    /// <summary>
    /// Base properties interface with common functionality
    /// </summary>
    public interface IProperties
    {
        /// <summary>
        /// Gets all available properties that can be set for the given type of root
        /// </summary>
        ICollection<Property> GetAvailableProperties(PropertyTarget targetType);

        /// <summary>
        /// Gets the given property from the target
        /// </summary>
        /// <param name="targetType">The type of target, either Pool or Dataset</param>
        /// <param name="target">The target - this can either be a pool, dataset, filesystem, volume or snapshot</param>
        /// <param name="property">The name of the property to get</param>
        /// <returns></returns>
        PropertyValue GetProperty(PropertyTarget targetType,string target, string property);

        /// <summary>
        /// Sets a property to the given value
        /// </summary>
        /// <param name="targetType">The type of target, either Pool or Dataset</param>
        /// <param name="target">The target - this can either be a pool, dataset, filesystem, volume or snapshot</param>
        /// <param name="property">The name of the property to set</param>
        /// <param name="value">The value of the property</param>
        /// <returns></returns>
        PropertyValue SetProperty(PropertyTarget targetType, string target, string property, string value);

        /// <summary>
        /// Gets all properties for the given target
        /// </summary>
        /// <param name="targetType">The type of target, either Pool or Dataset. If target is a pool, this has to be <see cref="PropertyTarget.Pool"/></param>
        /// <param name="target">The target - this can either be a pool, dataset, filesystem, volume or snapshot</param>
        IEnumerable<PropertyValue> GetProperties(PropertyTarget targetType, string target);

        /// <summary>
        /// Resets a dataset property to its inherited value.
        /// Does not work for a zpool, since a pool cannot have a parent
        /// </summary>
        void ResetPropertyToInherited(string dataset, string property);
    }
}