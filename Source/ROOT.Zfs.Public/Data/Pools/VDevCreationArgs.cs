using System.Collections.Generic;

namespace ROOT.Zfs.Public.Data.Pools
{
    /// <summary>
    /// Arguments for specifying a vdev when creating a pool
    /// </summary>
    public class VDevCreationArgs
    {
        /// <summary>
        /// The type of vdev to create
        /// </summary>
        public VDevCreationType Type { get; set; }
        
        /// <summary>
        /// The list of devices that should be part of this vdev
        /// </summary>
        public IList<string> Devices { get; set; }

        /// <summary>
        /// Converts this instance into what is required for zpool create
        /// </summary>
        public override string ToString()
        {
            return $"{Type.AsString()} {string.Join(' ', Devices)}";
        }

        /// <summary>
        /// Validates that the vdev arguments are correct and will not cause runtime errors when passed onto to zpool
        /// </summary>
        /// <param name="errorMessage">The error message if any</param>
        /// <returns>true if valid;false otherwise</returns>
        public virtual bool Validate(out string errorMessage)
        {
            //TODO: Write validation logic, i.e. mirror needs at least two devices etc.

            if (Type == VDevCreationType.Mirror && Devices?.Count < 2)
            {
                errorMessage = "Please provide at least two devices when creating a mirror";
                return false;
            }


            errorMessage = null;
            return true;
        }
    }
}