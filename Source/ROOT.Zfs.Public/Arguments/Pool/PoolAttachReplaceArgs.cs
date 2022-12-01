using System.Collections.Generic;
using System.Linq;
using System.Text;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Public.Arguments.Pool
{
    /// <summary>
    /// Represents all arguments to zpool attach or replace
    /// </summary>
    public class PoolAttachReplaceArgs : Args
    {
        /// <summary>
        /// Creates an instance of the attach/replace args class with the given command
        /// </summary>
        protected PoolAttachReplaceArgs(string command) : base(command)
        {
            if (command.Equals("replace", System.StringComparison.OrdinalIgnoreCase))
            {
                IsReplace = true;
            }
        }

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
        public bool IsReplace { get; protected set; }

        /// <inheritdoc />
        public override bool Validate(out IList<string> errors)
        {
            errors = null;
            ValidateString(PoolName, false, ref errors);
            ValidateString(OldDevice, false, ref errors);
            // Only replace allows not setting new device, to cater for the special scenario where a device has been removed and a new disk has been inserted and got the same device name
            ValidateString(NewDevice, IsReplace, ref errors);

            return errors == null;
        }

        /// <inheritdoc />
        protected override string BuildArgs(string command)
        {
            var args = new StringBuilder();
            args.Append(command);

            if (Force)
            {
                args.Append(" -f");
            }

            if (RestoreSequentially)
            {
                args.Append(" -s");
            }

            var ashift = PropertyValues?.FirstOrDefault(p => p.Property.Equals("ashift", System.StringComparison.OrdinalIgnoreCase));

            if (ashift != null)
            {
                args.Append($" -o ashift={ashift.Value}");
            }

            args.Append($" {Decode(PoolName)}");
            args.Append($" {Decode(OldDevice)}");

            var newDev = Decode(NewDevice);
            if (!string.IsNullOrWhiteSpace(newDev))
            {
                args.Append($" {newDev}");
            }

            return args.ToString();
        }
    }
}
