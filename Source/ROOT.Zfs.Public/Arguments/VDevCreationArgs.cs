using System.Collections.Generic;
using ROOT.Zfs.Public.Data;
using ROOT.Zfs.Public.Data.Pools;

namespace ROOT.Zfs.Public.Arguments
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
        /// <param name="errors">The error messages if any</param>
        /// <returns>true if valid;false otherwise</returns>
        public virtual bool Validate(out IList<string> errors)
        {
            errors = null;
            if ((Devices?.Count ?? 0) == 0)
            {
                errors = new List<string>();
                errors.Add("Please provide the proper amount of minimum devices");
                return false;
            }

            if (Type == VDevCreationType.Mirror
                && Devices.Count < 2)
            {
                errors = new List<string>();
                errors.Add("Please provide at least two devices when creating a mirror");
            }

            return errors == null;
        }
    }
}