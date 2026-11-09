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
    /// <param name="args">The arguments describing which properties to get.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
    /// <returns>A task whose result contains a read-only list of property values.</returns>
    Task<IReadOnlyList<PropertyValue>> GetAsync(GetPropertyArgs args, CancellationToken cancellationToken = default);
        
    /// <summary>
    /// Sets a property to the given value
    /// </summary>
    /// <param name="args">The arguments describing the property to set.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
    /// <returns>A task whose result contains the updated property value.</returns>
    Task<PropertyValue> SetAsync(SetPropertyArgs args, CancellationToken cancellationToken = default);
        
    /// <summary>
    /// Resets a dataset property to its inherited value.
    /// Does not work for a zpool, since a pool cannot have a parent
    /// </summary>
    /// <param name="args">The arguments describing the property to reset.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
    /// <returns>A task that completes when the property has been reset.</returns>
    Task ResetAsync(InheritPropertyArgs args, CancellationToken cancellationToken = default);
}