using System.Collections.Generic;
using System.Linq;
using System.Text;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Public.Arguments
{
    /// <summary>
    /// Represents all arguments to zpool attach
    /// </summary>
    public class ZpoolAttachArgs
    {
        /// <summary>
        /// Name of the pool to attach a device to
        /// </summary>
        public string PoolName { get; set; }

        /// <summary>
        /// device to attach <see cref="NewDevice"/> to
        /// </summary>
        public string OldDevice { get; set; }

        /// <summary>
        /// name of the new device to attach to give pool/vdev
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

            if (string.IsNullOrWhiteSpace(NewDevice))
            {
                errors = new List<string>();
                errors.Add("Please specify a new device");
            }

            return errors == null;
        }

        /// <summary>
        /// Returns a string representation that can be passed directly onto zpool attach
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
            sb.Append($" {NewDevice}");
            return sb.ToString();
        }
    }
}
