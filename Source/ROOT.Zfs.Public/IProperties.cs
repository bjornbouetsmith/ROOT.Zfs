using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ROOT.Zfs.Public.Arguments;
using ROOT.Zfs.Public.Arguments.Properties;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Public;

/// <summary>
/// Base properties interface with common functionality
/// </summary>
public interface IProperties : IBasicZfs
{
    /// <summary>
    /// Gets all available properties that can be set for the given type of root
    /// </summary>
    /// <param name="targetType">The type of root to get the available properties for.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
    /// <returns>A task whose result contains a read-only list of the properties that can be set.</returns>
    Task<IReadOnlyList<Property>> GetAvailableAsync(PropertyTarget targetType, CancellationToken cancellationToken = default);
        
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