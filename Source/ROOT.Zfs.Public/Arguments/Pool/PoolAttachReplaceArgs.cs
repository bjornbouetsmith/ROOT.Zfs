using System.Collections.Generic;
using System.Linq;
using System.Text;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Public.Arguments.Pool
{
    /// <summary>
    /// Represents all arguments to zpool attach or replace
    /// </summary>
    public class PoolAttachReplaceArgs
    {
        /// <summary>
        /// Name of the pool to attach or replace a device to
        /// </summary>
        public string PoolName { get; set; }

        /// <summary>
        /// device to either attach <see cref="NewDevice"/> to or replace with
        /// </summary>
        public string OldDevice { get; set; }

        /// <summary>
        /// name of the new device to attach/replace to given pool/device
        /// </summary>
        public string NewDevice { get; set; }

        /// <summary>
        /// Forces use of <see cref="NewDevice"/>, even if it appears to be in use. Not all devices can be overridden in this manner.
        /// </summary>
        public bool Force { get; set; }

        /// <summary>
        /// The <see cref="NewDevice"/> is reconstructed sequentially to restore redundancy as quickly as possible.
        /// Checksums are not verified during sequential reconstruction so a scrub is started when the resilver completes.
        /// Sequential reconstruction is not supported for raidz configurations.
        /// </summary>
        public bool RestoreSequentially { get; set; }

        /// <summary>
        /// Sets the given pool properties.
        /// The only property supported at the moment is ashift.
        /// </summary>
        public PropertyValue[] PropertyValues { get; set; }

        /// <summary>
        /// Gets or set whether or not this is a replace or attach.
        /// This controls the validation of whether or not NewDevice is required
        /// </summary>
        public bool IsReplace { get; set; }

        /// <summary>
        /// Validates the arguments and returns errors if not valid
        /// If no errors are found, the list <paramref name="errors"/> will be null
        /// </summary>
        public bool Validate(out IList<string> errors)
        {
            errors = null;
            if (string.IsNullOrWhiteSpace(PoolName))
            {
                errors = new List<string>();
                errors.Add("Please specify a pool name");
            }

            if (string.IsNullOrWhiteSpace(OldDevice))
            {
                errors = new List<string>();
                errors.Add("Please specify an old device");
            }

            if (string.IsNullOrWhiteSpace(NewDevice) && !IsReplace)
            {
                errors = new List<string>();
                errors.Add("Please specify a new device");
            }

            return errors == null;
        }

        /// <summary>
        /// Returns a string representation that can be passed directly onto zpool attach or replace
        /// </summary>
        public override string ToString()
        {
            var sb = new StringBuilder();
            if (Force)
            {
                sb.Append(" -f");
            }

            if (RestoreSequentially)
            {
                sb.Append(" -s");
            }

            var ashift = PropertyValues?.FirstOrDefault(p => p.Property.Equals("ashift", System.StringComparison.OrdinalIgnoreCase));

            if (ashift != null)
            {
                sb.Append($" -o ashift={ashift.Value}");
            }

            sb.Append($" {PoolName}");
            sb.Append($" {OldDevice}");
            
            if (!string.IsNullOrWhiteSpace(NewDevice))
            {
                sb.Append($" {NewDevice}");
            }

            return sb.ToString();
        }
    }
}
