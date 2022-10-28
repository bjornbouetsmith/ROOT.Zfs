using System.Collections.Generic;

namespace ROOT.Zfs.Public.Arguments
{
    /// <summary>
    /// Contains arguments for zpool upgrade
    /// </summary>
    public class ZpoolUpgradeArgs
    {
        /// <summary>
        /// Get or set the name for the pool that should be upgraded
        /// </summary>
        public string PoolName { get; set; }

        /// <summary>
        /// Whether or not all pools should be upgraded to latest features
        /// </summary>
        public bool AllPools { get; set; }

        /// <summary>
        /// Validates the arguments and returns whether or not they are valid
        /// </summary>
        /// <param name="errors">Any errors</param>
        /// <returns>true if valid;false otherwise</returns>
        public bool Validate(out IList<string> errors)
        {
            errors = null;
            if (!AllPools && string.IsNullOrWhiteSpace(PoolName))
            {
                errors = new List<string>();
                errors.Add("Please specify either a PoolName or set AllPools to true");
            }

            return errors == null;
        }

        /// <summary>
        /// Returns a string representation of the arguments, that shoul be passed onto zpool upgrade
        /// </summary>
        public override string ToString()
        {
            return AllPools ? " -a" : $" {PoolName}";
        }
    }
}
